using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Black.ClientSidePrediction
{
    [DisallowMultipleComponent]
    public sealed class MovementSimulation : NetworkBehaviour
    {
        public static MovementSimulation Instance { get; private set; }

        [SerializeField] private byte updateRate = 60;
        public byte UpdateRate => updateRate;

        private List<MovementEntity> entities = new List<MovementEntity>();
        private Dictionary<NetworkConnection, List<ClientInput>> clientInputs = new Dictionary<NetworkConnection, List<ClientInput>>();

        private void Awake()
        {
            Instance = this;

            Time.fixedDeltaTime = 1.0f / updateRate;
        }

        [ServerCallback]
        private void FixedUpdate()
        {
            SimulateEveryEntity();
        }

        [Command(requiresAuthority = false)]
        public void SendInputToServer(ClientInput input, NetworkConnectionToClient conn = null)
        {
            if (clientInputs.ContainsKey(conn))
            {
                clientInputs[conn].Add(input);
            }
        }

        public void AddEntity(NetworkConnection conn, MovementEntity entity)
        {
            if (!entities.Contains(entity))
            {
                entities.Add(entity);
            }

            if (!clientInputs.ContainsKey(conn))
            {
                clientInputs.Add(conn, new List<ClientInput>());
            }
        }

        public void RemoveEntity(MovementEntity entity)
        {
            if (entities.Contains(entity))
            {
                entities.Remove(entity);
            }
        }

        private void SimulateEveryEntity()
        {
            for (int i = 0; i < entities.Count; i++)
            {
                MovementEntity entity = entities[i];
                NetworkConnection connection = entity.connectionToClient;

                if (!clientInputs.ContainsKey(connection))
                {
                    continue;
                }

                List<ClientInput> inputs = clientInputs[connection];

                ApplyInput(entity, inputs);
                entity.ApplyMovement();
                ApplyResult(entity, inputs);
            }
        }

        private void ApplyInput(MovementEntity entity, List<ClientInput> inputs)
        {
            if (inputs.Count <= 0)
            {
                return;
            }

            entity.SetInput(inputs[0]);
        }

        private void ApplyResult(MovementEntity entity, List<ClientInput> inputs)
        {
            if (inputs.Count <= 0)
            {
                return;
            }

            ServerResult result = entity.GetResult();
            result.Frame = inputs[0].Frame;
            result.Buffer = (byte)inputs.Count;

            inputs.RemoveAt(0);
            entity.SendResultToClient(result);
        }
    }
}