using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Black.ClientSidePrediction
{
    [DisallowMultipleComponent]
    public abstract class AuthoritativeCharacterMotor : NetworkBehaviour
    {
        private ulong inputFrame;
        private bool isMismatch;

        private ClientInput currentInput;
        private ServerResult currentResult;

        private List<ClientInput> inputs = new List<ClientInput>();
        private List<ServerResult> results = new List<ServerResult>();

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

            inputFrame++;

            CreateInput();
            Reconciliate();
            Predict();
            CreateResult();

            AuthoritativeCharacterSystem.Instance.SendInputToServer(currentInput);
        }

        [TargetRpc]
        public void SendResultToClient(ServerResult result)
        {
            currentResult = result;
        }

        private void CreateInput()
        {
            currentInput = GetInput();
            currentInput.TimeFrame = inputFrame;
        }

        private void CreateResult()
        {
            if (isServer)
            {
                return;
            }

            ServerResult result = GetResult();
            result.TimeFrame = inputFrame;

            results.RemoveAll(IsObsoleteResult);
            results.Add(result);
        }

        private void Reconciliate()
        {
            if (isServer)
            {
                return;
            }

            ServerResult sameResult = results.Find(IsSameTimeFrame);
            isMismatch = !sameResult.Equals(currentResult);

            if (isMismatch)
            {
                SetResult(currentResult);
            }
        }

        private void Predict()
        {
            if (isServer)
            {
                return;
            }

            inputs.RemoveAll(IsObsoleteInput);
            inputs.Add(currentInput);

            if (isMismatch)
            {
                for (int i = 0; i < inputs.Count; i++)
                {
                    SetInput(inputs[i]);
                    ApplyPhysics();
                }
            }
            else
            {
                SetInput(currentInput);
                ApplyPhysics();
            }
        }

        private bool IsObsoleteInput(ClientInput input)
        {
            return input.TimeFrame <= currentResult.TimeFrame;
        }

        private bool IsObsoleteResult(ServerResult result)
        {
            return result.TimeFrame < currentResult.TimeFrame;
        }

        private bool IsSameTimeFrame(ServerResult result)
        {
            return result.TimeFrame == currentResult.TimeFrame;
        }
    }
}