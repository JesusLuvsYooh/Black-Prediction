﻿using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Black.Utility;

namespace Black.ClientSidePrediction
{
    [DisallowMultipleComponent]
    public abstract class MovementEntity : NetworkBehaviour
    {
        [SerializeField] private byte defaultBuffer = 2;
        [SerializeField] private byte bufferSpeed = 5;

        private ulong currentFrame;
        private ClientInput currentInput;
        private ServerResult currentResult;
        private List<ClientInput> inputs = new List<ClientInput>();

        protected abstract ClientInput GetInput();
        public abstract void SetInput(ClientInput input);
        public abstract ServerResult GetResult();
        protected abstract void SetResult(ServerResult result);
        public abstract void ApplyMovement();

        protected virtual void Start()
        {
            if (NetworkServer.active && MovementSimulation.Instance != null)
            {
                MovementSimulation.Instance.AddEntity(connectionToClient, this);
            }
        }

        protected virtual void OnDestroy()
        {
            if (NetworkServer.active && MovementSimulation.Instance != null)
            {
                MovementSimulation.Instance.RemoveEntity(this);
            }
        }

        protected virtual void FixedUpdate()
        {
            if (!hasAuthority || MovementSimulation.Instance == null)
            {
                return;
            }

            currentFrame++;

            BufferInput();
            CreateInput();
            ReconciliateState();
            PredictMovement();

            MovementSimulation.Instance.SendInputToServer(currentInput);
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

            byte updateRate = MovementSimulation.Instance.UpdateRate;
            var pingBuffer = (byte)(NetworkTime.rtt / 2 * updateRate);
            var targetBuffer = (byte)(defaultBuffer + pingBuffer);

            if (currentResult.Buffer > targetBuffer)
            {
                BlackUtility.ApplyFixedTimestep(updateRate - bufferSpeed);
            }
            else if (currentResult.Buffer < targetBuffer)
            {
                BlackUtility.ApplyFixedTimestep(updateRate + bufferSpeed);
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
                ApplyMovement();
            }
        }

        private bool IsObsoleteInput(ClientInput input)
        {
            return input.Frame <= currentResult.Frame;
        }
    }
}