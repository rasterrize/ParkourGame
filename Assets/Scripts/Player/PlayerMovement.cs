using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public enum MovementType
    {
        Idle,
        Walking,
        Running,
        Jumping,
        Rolling,
        Sliding,
        Vaulting,
    }

    public class PlayerMovement : MonoBehaviour
    {
        private const float DEFAULT_GRAVITY = -9.81f;

        [SerializeField] private Camera playerCamera;
        [SerializeField] private GameObject groundCheckObject;
        [SerializeField] private float groundCheckRadius = 0.4f;
        [SerializeField] private LayerMask groundLayer;
        
        private bool isGrounded;
        private CharacterController controller; // the character controller of the player

        [Tooltip("The gravity of the player. This defaults to earth's gravity.")] 
        [SerializeField] private float gravity = DEFAULT_GRAVITY;
        
        [Tooltip("The walking speed of the player.")]
        [SerializeField] private float walkSpeed;
        
        [Tooltip("The running speed of the player.")]
        [SerializeField] private float runSpeed;
        
        [Tooltip("The amount of force to apply when the player jumps")]
        [SerializeField] private float jumpForce;
        
        [Tooltip("The speed at which sliding will start at.")]
        [SerializeField] private float slideStartSpeed;
        
        [Tooltip("The maximum speed the player can slide. Usually only reached when sliding downhill.")]
        [SerializeField] private float maxSlideSpeed;

        [Tooltip("The distance to move the camera down on the y axis when the player slides.")]
        [SerializeField] private float slideCameraYDistance;
        
        private float currentSlidePenalty;
        private float currentSlideVelocity;
        private bool isHoldingSprintKey;
        private Vector3 velocity;
        private Vector2 moveInputValues;

        private MovementType currentMovementState;

        private void Start()
        {
            controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            velocity.y -= gravity * -2f * Time.deltaTime;
            isGrounded = Physics.CheckSphere(groundCheckObject.transform.position, groundCheckRadius, groundLayer);
            
            if (isGrounded && velocity.y < 0)
            {
                // Force the player to the ground
                velocity.y = -2f;
            }
        
            // Check if the player is trying to move or not
            if (moveInputValues == Vector2.zero)
                currentMovementState = MovementType.Idle;
            
            // Update based on movement state
            switch (currentMovementState)
            {
                case MovementType.Idle:
                    if (moveInputValues != Vector2.zero)
                        Move(moveInputValues);
                    break;
                case MovementType.Walking:
                    Move(moveInputValues);
                    break;
                case MovementType.Running:
                    Move(moveInputValues);
                    break;
                case MovementType.Jumping:
                    Move(moveInputValues);
                    break;
                case MovementType.Sliding:
                    UpdateSlide();
                    break;
                case MovementType.Rolling:
                    break;
                case MovementType.Vaulting:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            // Move player using gravity
            controller.Move(velocity * Time.deltaTime);
        }

        private void Move(Vector2 inputs)
        {
            // Check if the player is holding sprint
            currentMovementState =  isHoldingSprintKey ? MovementType.Running : MovementType.Walking;
        
            // Move the player in the direction they are facing
            var moveVector = controller.transform.forward * inputs.y + controller.transform.right * inputs.x;
            var currentSpeed = isHoldingSprintKey ? runSpeed : walkSpeed;
            controller.Move(currentSpeed * Time.deltaTime * moveVector);
        }

        private void Jump()
        {
            if (currentMovementState == MovementType.Sliding)
                EndSlide();
        
            currentMovementState = MovementType.Jumping;
            velocity.y = jumpForce;
        }

        private void BeginSlide()
        {
            currentMovementState = MovementType.Sliding;
            currentSlideVelocity = slideStartSpeed;
                
            // Move camera down to emulate character sliding
            playerCamera.transform.localPosition = new Vector3(0, playerCamera.transform.localPosition.y - slideCameraYDistance, 0);
            
            // Shrink player collider
            controller.center = new Vector3(0, -0.5f, 0);
            controller.height = 1;
        }

        private void EndSlide()
        {
            currentSlideVelocity = 0.0f;
            currentMovementState = MovementType.Idle;
        
            // Reset camera
            playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, playerCamera.transform.localPosition.y + 0.5f, playerCamera.transform.localPosition.z);
            
            // Reset player collider
            controller.center = new Vector3(0, 0, 0);
            controller.height = 2;
        }

        private void UpdateSlide()
        {
            if (currentSlideVelocity <= 0.0f)
            {
                EndSlide();
                return;
            }
        
            currentSlideVelocity -= 10.0f * Time.deltaTime;
            controller.Move(controller.transform.forward * (currentSlideVelocity * Time.deltaTime));
        }

        public void OnMoveAction(InputAction.CallbackContext context)
        {
            // Update move values (x = left/right, y = forward/backward)
            moveInputValues = context.ReadValue<Vector2>();
        }

        public void OnSprintAction(InputAction.CallbackContext context)
        {
            isHoldingSprintKey = context.performed;
        }

        public void OnJumpAction(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            
            // Check if in correct state to jump
            if (currentMovementState is MovementType.Idle or MovementType.Walking or MovementType.Running or MovementType.Sliding && isGrounded)
            {
                Jump();
            }
        }

        public void OnRollAction(InputAction.CallbackContext context)
        {
            // if y velocity > certain value
            // check distance to the ground
            // roll if the distance is between a certain threshold
        }

        public void OnSlideAction(InputAction.CallbackContext context)
        {
            if (currentMovementState is MovementType.Idle or MovementType.Walking)
                return;
            
            if (context.canceled)
            {
                if (currentMovementState == MovementType.Sliding)
                    EndSlide();
            }
            
            if (context.started && isGrounded && currentMovementState == MovementType.Running)
            {
                BeginSlide();
            }
        }
    }
}