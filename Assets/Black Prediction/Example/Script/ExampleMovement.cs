using UnityEngine;
using Mirror;

namespace Black.ClientSidePrediction.Example
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    public sealed class ExampleMovement : MovementEntity
    {
        [SerializeField] private CharacterController cc;
        [SerializeField] private ExampleCamera cam;
        [SerializeField] private GameObject body;

        [SerializeField] private float moveSpeed = 10;
        [SerializeField] private float jumpSpeed = 10;
        [SerializeField] private float gravity = -30;

        [SerializeField] private float groundDistance = 0.5f;
        [SerializeField] private LayerMask groundLayer;

        private Vector3 move;
        private float yaw;
        private bool pressedJump;

        private Vector3 velocity;
        private bool isGrounded;

        public override void OnStartAuthority()
        {
            cam.gameObject.SetActive(true);
            body.SetActive(false);

            if (!isServer)
            {
                GetComponent<NetworkTransform>().enabled = false;
            }
        }

        protected override ClientInput GetInput()
        {
            var input = new ClientInput
            {
                Horizontal = Input.GetAxisRaw("Horizontal"),
                Vertical = Input.GetAxisRaw("Vertical"),
                Yaw = cam.Yaw,
                Jump = Input.GetKey(KeyCode.Space)
            };

            return input;
        }

        public override void SetInput(ClientInput input)
        {
            move = input.Horizontal * transform.right + input.Vertical * transform.forward;
            yaw = input.Yaw;
            pressedJump = input.Jump;
        }

        public override ServerResult GetResult()
        {
            var state = new ServerResult
            {
                Position = transform.localPosition,
                Velocity = velocity,
                IsGrounded = isGrounded
            };

            return state;
        }

        protected override void SetResult(ServerResult result)
        {
            cc.enabled = false;
            transform.localPosition = result.Position;
            cc.enabled = true;

            velocity = result.Velocity;
            isGrounded = result.IsGrounded;
        }

        public override void ApplyMovement()
        {
            transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, yaw, transform.localEulerAngles.z);

            isGrounded = Physics.CheckSphere(transform.localPosition, groundDistance, groundLayer);

            if (pressedJump && isGrounded)
            {
                velocity.y = jumpSpeed;
            }

            velocity.y += gravity * Time.fixedDeltaTime;
            velocity = new Vector3(move.x * moveSpeed, velocity.y, move.z * moveSpeed);

            cc.Move(velocity * Time.fixedDeltaTime);
        }
    }
}