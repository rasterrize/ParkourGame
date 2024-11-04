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
        WallRunning,
    }

    public class PlayerMovement : MonoBehaviour
    {
        private const float DEFAULT_GRAVITY = -9.81f;

        [SerializeField] private Camera playerCamera;
        [SerializeField] private GameObject groundCheckObject;
        [SerializeField] private float groundCheckRadius = 0.4f;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask parkourWallLayer;
        [SerializeField] private GameObject wallCheckLeft;
        [SerializeField] private GameObject wallCheckRight;

        public bool IsGrounded { get; private set; }
        public CharacterController Controller { get; private set; } // the character controller of the player
        
        private FirstPersonController firstPersonController;

        [Tooltip("The gravity of the player. This defaults to earth's gravity.")] 
        [SerializeField] private float gravity = DEFAULT_GRAVITY;
        
        [Tooltip("The base walking speed of the player.")]
        [SerializeField] private float baseWalkSpeed;
        
        [Tooltip("The base running speed of the player.")]
        [SerializeField] private float baseRunSpeed;
        
        [Tooltip("The amount of force to apply when the player jumps")]
        [SerializeField] private float jumpForce;
        
        [Tooltip("The speed at which sliding will start at.")]
        [SerializeField] private float slideStartSpeed;
        
        [Tooltip("The maximum speed the player can slide. Usually only reached when sliding downhill.")]
        [SerializeField] private float maxSlideSpeed;

        [Tooltip("The distance to move the camera down on the y axis when the player slides.")]
        [SerializeField] private float slideCameraYDistance;
        
        [Tooltip("The speed at which the player will run on walls")]
        [SerializeField] private float wallRunSpeed = 8.0f;
        
        [Tooltip("The y velocity the player must be when landing to consider a high fall.")]
        [SerializeField] private float highFallThreshold = -10.0f;
        
        [Tooltip("The time (in seconds) it takes to return to normal speed after hard landing.")]
        [SerializeField] private float staggerTime = 1.0f;
        
        [Tooltip("The percentage of walk/run speed to change when the player is staggering.")]
        [SerializeField] private float staggerMovePercentage;
        
        [Tooltip("The speed at which the player rolls.")]
        [SerializeField] private float rollSpeed = 0.5f;
        
        private float currentSlidePenalty;
        private float currentSlideVelocity;
        private bool isHoldingSprintKey;
        private bool isHoldingRollKey;
        private Vector3 velocity;
        private Vector2 moveInputs;

        private WallRunController wallRunController;

        private bool isWallRunning;

        private MovementType currentMovementState;
        
        private bool staggering;
        private float remainingStaggerTime;
        
        private float cameraRollRotation;

        private void Start()
        {
            Controller = GetComponent<CharacterController>();
            firstPersonController = GetComponent<FirstPersonController>();
            wallRunController = new WallRunController(this, wallCheckLeft, wallCheckRight, parkourWallLayer, wallRunSpeed);
        }

        private void Update()
        {
            velocity.y -= gravity * -2f * Time.deltaTime;
            
            // Check if the player is grounded
            var oldIsGrounded = IsGrounded;
            IsGrounded = Physics.CheckSphere(groundCheckObject.transform.position, groundCheckRadius, groundLayer);
            
            // Check if the player is landing
            if (oldIsGrounded == false && IsGrounded)
            {
                OnLand();
            }
            
            if (IsGrounded && velocity.y < 0)
            {
                // Force the player to the ground
                velocity.y = -2f;
            }

            wallRunController.RunWallChecks();
        
            // Check if the player is trying to move or not
            if (moveInputs == Vector2.zero)
                currentMovementState = MovementType.Idle;
            
            // Update based on movement state
            switch (currentMovementState)
            {
                case MovementType.Idle:
                    if (moveInputs != Vector2.zero)
                        Move(moveInputs);
                    break;
                case MovementType.Walking:
                    Move(moveInputs);
                    break;
                case MovementType.Running:
                    Move(moveInputs);
                    break;
                case MovementType.Jumping:
                    Move(moveInputs);
                    break;
                case MovementType.Sliding:
                    UpdateSlide();
                    break;
                case MovementType.Rolling:
                    UpdateRoll();
                    break;
                case MovementType.Vaulting:
                    break;
                case MovementType.WallRunning:
                    wallRunController.Update(moveInputs);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            // Move player using gravity
            if (currentMovementState != MovementType.WallRunning)
                Controller.Move(velocity * Time.deltaTime);
        }

        private void Move(Vector2 inputs)
        {
            // Check if the player is holding sprint
            currentMovementState =  isHoldingSprintKey ? MovementType.Running : MovementType.Walking;
        
            // Move the player in the direction they are facing
            var moveVector = Controller.transform.forward * inputs.y + Controller.transform.right * inputs.x;
            var currentSpeed = isHoldingSprintKey ? baseRunSpeed : baseWalkSpeed;
            
            // Lower movement speed when the player is staggering
            if (staggering)
            {
                remainingStaggerTime -= Time.deltaTime;
                if (remainingStaggerTime > 0)
                {
                    currentSpeed = (1.0f - staggerMovePercentage / 100.0f) * currentSpeed;
                }
                else
                {
                    staggering = false;
                    remainingStaggerTime = 0;
                }
            }
            
            Controller.Move(currentSpeed * Time.deltaTime * moveVector);
        }

        private void Jump()
        {
            if (currentMovementState == MovementType.Sliding)
                EndSlide();
            
            if (currentMovementState == MovementType.WallRunning)
                wallRunController.EndWallRun();
        
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
            Controller.center = new Vector3(0, -0.5f, 0);
            Controller.height = 1;
        }

        private void EndSlide()
        {
            currentSlideVelocity = 0.0f;
            currentMovementState = MovementType.Idle;
        
            // Reset camera
            playerCamera.transform.localPosition = new Vector3(0, 0.5f, 0);
            
            // Reset player collider
            Controller.center = new Vector3(0, 0, 0);
            Controller.height = 2;
        }

        private void UpdateSlide()
        {
            if (moveInputs.y <= 0.0f)
            {
                EndSlide();
                return;
            }
            
            if (currentSlideVelocity <= 0.0f)
            {
                EndSlide();
                return;
            }
        
            currentSlideVelocity -= 10.0f * Time.deltaTime;
            Controller.Move(Controller.transform.forward * (currentSlideVelocity * Time.deltaTime));
        }

        private void Roll()
        {
            currentMovementState = MovementType.Rolling;
            cameraRollRotation = 0.0f;

            firstPersonController.AllowLooking = false;
        }

        private void UpdateRoll()
        {
            // Roll camera 360 degrees over the course of rollTime
            cameraRollRotation += rollSpeed * Time.deltaTime;
            playerCamera.transform.localRotation = Quaternion.Euler(cameraRollRotation, 0.0f, 0.0f);
            
            Controller.Move(baseRunSpeed * Time.deltaTime * Controller.transform.forward);

            // end roll if time as run out.
            if (cameraRollRotation > 360.0f)
            {
                cameraRollRotation = 0.0f;
                firstPersonController.AllowLooking = true;
                currentMovementState = MovementType.Running;
            }
        }

        private void HardLand()
        {
            if (staggering)
                return;
            
            staggering = true;   
            remainingStaggerTime = staggerTime;
        }
        
        public void OnMoveAction(InputAction.CallbackContext context)
        {
            // Update move values (x = left/right, y = forward/backward)
            moveInputs = context.ReadValue<Vector2>();
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
            if (currentMovementState is MovementType.Idle or MovementType.Walking or MovementType.Running or MovementType.Sliding && IsGrounded && !staggering)
            {
                Jump();
            }

            if (currentMovementState == MovementType.WallRunning)
            {
                wallRunController.EndWallRun();
                Jump();
            }
        }

        public void OnRollAction(InputAction.CallbackContext context)
        {
            isHoldingRollKey = context.performed;
        }

        public void OnSlideAction(InputAction.CallbackContext context)
        {
            if (currentMovementState is not MovementType.Running or MovementType.Sliding || staggering)
                return;
            
            if (context.canceled)
            {
                if (currentMovementState == MovementType.Sliding)
                    EndSlide();
            }
            
            if (context.started && IsGrounded && currentMovementState == MovementType.Running)
            {
                BeginSlide();
            }
        }

        private void OnLand()
        {
            if (velocity.y <= highFallThreshold)
            {
                if (isHoldingRollKey)
                    Roll();
                else
                    HardLand();
            }
        }
        
        public MovementType GetMovementState() => currentMovementState;
        
        public void SetMovementState(MovementType movement) => currentMovementState = movement;
    }
}