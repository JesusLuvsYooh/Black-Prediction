using UnityEngine;
using Mirror;

namespace Black.ClientSidePrediction.Example
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    public sealed class ExampleCharacterController : AuthoritativeCharacterMotor
    {
        [SerializeField] private CharacterController controller;

        [SerializeField] private float speed = 10;
        [SerializeField] private float jump = 10;
        [SerializeField] private float gravity = -30;

        [SerializeField] private float groundDistance = 0.5f;
        [SerializeField] private LayerMask groundLayer;

        private Vector3 moveInput;
        private bool pressedJump;

        private Vector3 velocity;
        private bool isGrounded;

        public override void OnStartAuthority()
        {
            // Disable networktransform to prevent owner from getting sync replication from it's own transform. Normally this shouldn't happen even if it's enabled?
            GetComponent<NetworkTransform>().enabled = false;
        }

        protected override ClientInput GetInput()
        {
            var input = new ClientInput
            {
                Horizontal = Input.GetAxisRaw("Horizontal"),
                Vertical = Input.GetAxisRaw("Vertical"),
                Jump = Input.GetKey(KeyCode.Space)
            };

            return input;
        }

        public override void SetInput(ClientInput input)
        {
            moveInput =  transform.right * input.Horizontal + transform.forward * input.Vertical;
            pressedJump = input.Jump;
        }

        public override ServerResult GetResult()
        {
            var state = new ServerResult
            {
                Position = transform.position,
                Velocity = velocity,
                IsGrounded = isGrounded
            };

            return state;
        }

        protected override void SetResult(ServerResult result)
        {
            controller.enabled = false;
            transform.position = result.Position;
            controller.enabled = true;

            velocity = result.Velocity;
            isGrounded = result.IsGrounded;
        }

        public override void ApplyPhysics()
        {
            CheckGround();
            Jump();
            Gravity();
            Move();
        }

        private void CheckGround()
        {
            isGrounded = Physics.CheckSphere(transform.localPosition, groundDistance, groundLayer);
        }

        private void Jump()
        {
            if (pressedJump && isGrounded)
            {
                velocity.y = jump;
            }
        }

        private void Gravity()
        {
            velocity.y += gravity * Time.fixedDeltaTime;
        }

        private void Move()
        {
            velocity = new Vector3(moveInput.x * speed, velocity.y, moveInput.z * speed);

            controller.Move(velocity * Time.fixedDeltaTime);
        }
    }
}