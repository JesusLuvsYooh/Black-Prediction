using UnityEngine;

namespace Black.ClientSidePrediction.Example
{
    [DisallowMultipleComponent]
    public sealed class ExampleCamera : MonoBehaviour
    {
        [SerializeField] private float minPitch = -90.0f;
        [SerializeField] private float maxPitch = 90.0f;
        [SerializeField] private float sensitivity = 5.0f;

        private float pitch;
        public float Yaw { get; private set; }

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void FixedUpdate()
        {
            pitch -= Input.GetAxisRaw("Mouse Y") * sensitivity;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            Yaw += Input.GetAxisRaw("Mouse X") * sensitivity;

            transform.localRotation = Quaternion.Euler(pitch, 0.0f, 0.0f);
        }
    }
}