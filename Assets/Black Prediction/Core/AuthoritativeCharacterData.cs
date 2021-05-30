using UnityEngine;

namespace Black.ClientSidePrediction
{
    public struct ClientInput
    {
        public ulong TimeFrame;

        // Add your inputs here.
        public float Horizontal;
        public float Vertical;
        public bool Jump;
    }

    public struct ServerResult
    {
        public ulong TimeFrame;

        // Add your results here.
        public Vector3 Position;
        public Vector3 Velocity;
        public bool IsGrounded;
    }
}