using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Black.Utility;

namespace Black.ClientSidePrediction
{
    [DisallowMultipleComponent]
    public sealed class AuthoritativeCharacterSystem : NetworkBehaviour
    {
        public static AuthoritativeCharacterSystem Instance { get; private set; }

        [SerializeField] private byte updateRate = 60;
        public byte UpdateRate => updateRate;

        private List<AuthoritativeCharacterMotor> motors = new List<AuthoritativeCharacterMotor>();
        private Dictionary<NetworkConnection, List<ClientInput>> clientInputs = new Dictionary<NetworkConnection, List<ClientInput>>();

        private void Awake()
        {
            Instance = this;

            BlackUtility.ApplyFixedTimestep(updateRate);
        }

        [ServerCallback]
        private void FixedUpdate()
        {
            SimulateEveryMotor();
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

        private void SimulateEveryMotor()
        {
            for (int i = 0; i < motors.Count; i++)
            {
                AuthoritativeCharacterMotor motor = motors[i];
                NetworkConnection connection = motor.connectionToClient;

                if (!clientInputs.ContainsKey(connection))
                {
                    continue;
                }

                List<ClientInput> inputs = clientInputs[connection];

                ApplyInput(motor, inputs);
                motor.ApplyMovement();
                ApplyResult(motor, inputs);
            }
        }

        private void ApplyInput(AuthoritativeCharacterMotor motor, List<ClientInput> inputs)
        {
            if (inputs.Count <= 0)
            {
                return;
            }

            motor.SetInput(inputs[0]);
        }

        private void ApplyResult(AuthoritativeCharacterMotor motor, List<ClientInput> inputs)
        {
            if (inputs.Count <= 0)
            {
                return;
            }

            ServerResult result = motor.GetResult();
            result.Frame = inputs[0].Frame;
            result.Buffer = (byte)inputs.Count;

            inputs.RemoveAt(0);
            motor.SendResultToClient(result);
        }
    }
}