using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Black.ClientSidePrediction
{
    [DisallowMultipleComponent]
    public sealed class AuthoritativeCharacterSystem : NetworkBehaviour
    {
        public static AuthoritativeCharacterSystem Instance { get; private set; }

        private List<AuthoritativeCharacterMotor> motors = new List<AuthoritativeCharacterMotor>();
        private Dictionary<NetworkConnection, List<ClientInput>> clientInputs = new Dictionary<NetworkConnection, List<ClientInput>>();

        private void Awake()
        {
            Instance = this;
        }

        [ServerCallback]
        private void FixedUpdate()
        {
            SimulateClients();
        }

        [Command(requiresAuthority = false)]
        public void SendInputToServer(ClientInput input, NetworkConnectionToClient conn = null)
        {
            if (clientInputs.ContainsKey(conn))
            {
                clientInputs[conn].Add(input);
            }
        }

        public void AddMotor(NetworkConnection conn, AuthoritativeCharacterMotor motor)
        {
            if (!motors.Contains(motor))
            {
                motors.Add(motor);
            }

            if (!clientInputs.ContainsKey(conn))
            {
                clientInputs.Add(conn, new List<ClientInput>());
            }
        }

        public void RemoveMotor(AuthoritativeCharacterMotor motor)
        {
            if (motors.Contains(motor))
            {
                motors.Remove(motor);
            }
        }

        private void SimulateClients()
        {
            for (int i = 0; i < motors.Count; i++)
            {
                ApplyInput(motors[i]);
                motors[i].ApplyPhysics();
                ApplyResult(motors[i]);
            }
        }

        private void ApplyInput(AuthoritativeCharacterMotor motor)
        {
            if (clientInputs.ContainsKey(motor.connectionToClient) && clientInputs[motor.connectionToClient].Count > 0)
            {
                motor.SetInput(clientInputs[motor.connectionToClient][0]);
            }
        }

        private void ApplyResult(AuthoritativeCharacterMotor motor)
        {
            if (clientInputs.ContainsKey(motor.connectionToClient) && clientInputs[motor.connectionToClient].Count > 0)
            {
                ServerResult result = motor.GetResult();
                result.TimeFrame = clientInputs[motor.connectionToClient][0].TimeFrame;

                clientInputs[motor.connectionToClient].RemoveAt(0);
                motor.SendResultToClient(result);
            }
        }
    }
}