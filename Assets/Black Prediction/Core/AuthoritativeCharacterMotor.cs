using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Black.Utility;

namespace Black.ClientSidePrediction
{
    [DisallowMultipleComponent]
    public abstract class AuthoritativeCharacterMotor : NetworkBehaviour
    {
        [SerializeField] private byte defaultBuffer = 2;

        private ulong currentFrame;
        private ClientInput currentInput;
        private ServerResult currentResult;
        private List<ClientInput> inputs = new List<ClientInput>();

        protected abstract ClientInput GetInput();
        public abstract void SetInput(ClientInput input);
        public abstract ServerResult GetResult();
        protected abstract void SetResult(ServerResult result);
        public abstract void ApplyPhysics();

        protected virtual void Start()
        {
            if (NetworkServer.active && AuthoritativeCharacterSystem.Instance != null)
            {
                AuthoritativeCharacterSystem.Instance.AddMotor(connectionToClient, this);
            }
        }

        protected virtual void OnDestroy()
        {
            if (NetworkServer.active && AuthoritativeCharacterSystem.Instance != null)
            {
                AuthoritativeCharacterSystem.Instance.RemoveMotor(this);
            }
        }

        protected virtual void FixedUpdate()
        {
            if (!hasAuthority || AuthoritativeCharacterSystem.Instance == null)
            {
                return;
            }

            currentFrame++;

            BufferInput();
            CreateInput();
            ReconciliateState();
            PredictMovement();

            AuthoritativeCharacterSystem.Instance.SendInputToServer(currentInput);
        }

        [TargetRpc]
        public void SendResultToClient(ServerResult result)
        {
            currentResult = result;
        }

        private void BufferInput()
        {
            if (isServer)
            {
                return;
            }

            float updateRate = AuthoritativeCharacterSystem.Instance.UpdateRate;
            byte pingBuffer = (byte)(NetworkTime.rtt / 2 * updateRate);
            byte targetBuffer = (byte)(defaultBuffer + pingBuffer);

            if (currentResult.Buffer > targetBuffer)
            {
                BlackUtility.ApplyFixedTimestep(updateRate - 10);
            }
            else if (currentResult.Buffer < targetBuffer)
            {
                BlackUtility.ApplyFixedTimestep(updateRate + 10);
            }
            else
            {
                BlackUtility.ApplyFixedTimestep(updateRate);
            }
        }

        private void CreateInput()
        {
            currentInput = GetInput();
            currentInput.Frame = currentFrame;
        }

        private void ReconciliateState()
        {
            if (isServer)
            {
                return;
            }

            SetResult(currentResult);
        }

        private void PredictMovement()
        {
            if (isServer)
            {
                return;
            }

            inputs.RemoveAll(IsObsoleteInput);
            inputs.Add(currentInput);

            for (int i = 0; i < inputs.Count; i++)
            {
                SetInput(inputs[i]);
                ApplyPhysics();
            }
        }

        private bool IsObsoleteInput(ClientInput input)
        {
            return input.Frame <= currentResult.Frame;
        }
    }
}