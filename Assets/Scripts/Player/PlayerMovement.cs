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
        private const float DEFAULT_WALK_SPEED = 4f;
        private const float DEFAULT_RUN_SPEED = 8f;
        private const float DEFAULT_JUMP_FORCE = 10f;
        private const float DEFAULT_SLIDE_START_SPEED = 12f;
        private const float DEFAULT_SLIDE_MAX_SPEED = 20f;
        private const float DEFAULT_SLIDE_CAMERA_Y_DISTANCE = 0.5f;
        private const float DEFAULT_WALL_RUN_SPEED = 8f;

        [SerializeField] private Camera playerCamera;
        [SerializeField] private GameObject groundCheckObject;
        [SerializeField] private float groundCheckRadius = 0.4f;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask parkourWallLayer;
        [SerializeField] private GameObject wallCheckLeft;
        [SerializeField] private GameObject wallCheckRight;

        public bool IsGrounded { get; private set; }
        public CharacterController Controller { get; private set; } // the character controller of the player

        [Tooltip("The gravity of the player. This defaults to earth's gravity.")] 
        [SerializeField] private float gravity = DEFAULT_GRAVITY;
        
        [Tooltip("The walking speed of the player.")]
        [SerializeField] private float walkSpeed = DEFAULT_WALK_SPEED;
        
        [Tooltip("The running speed of the player.")]
        [SerializeField] private float runSpeed = DEFAULT_RUN_SPEED;
        
        [Tooltip("The amount of force to apply when the player jumps")]
        [SerializeField] private float jumpForce = DEFAULT_JUMP_FORCE;
        
        [Tooltip("The speed at which sliding will start at.")]
        [SerializeField] private float slideStartSpeed = DEFAULT_SLIDE_START_SPEED;
        
        [Tooltip("The maximum speed the player can slide. Usually only reached when sliding downhill.")]
        [SerializeField] private float maxSlideSpeed = DEFAULT_SLIDE_MAX_SPEED;

        [Tooltip("The distance to move the camera down on the y axis when the player slides.")]
        [SerializeField] private float slideCameraYDistance = DEFAULT_SLIDE_CAMERA_Y_DISTANCE;
        
        [Tooltip("The speed at which the player will run on walls")]
        [SerializeField] private float wallRunSpeed = DEFAULT_WALL_RUN_SPEED;
        
        private float currentSlidePenalty;
        private float currentSlideVelocity;
        private bool isHoldingSprintKey;
        private Vector3 velocity;
        private Vector2 moveInputs;

        private WallRunController wallRunController;

        private bool isWallRunning;

        private MovementType currentMovementState;

        private PlayerRhythmController rhythmController;

        // --------
        // Events
        // --------
        
        /// <summary>
        /// Occurs when the movement keys are pressed or the joystick is moved. Avoid expensive code when using this.
        /// </summary>
        public event EventHandler OnMoveActionEvent;
        
        /// <summary>
        /// Occurs when the player moves. Avoid expensive code when using this.
        /// </summary>
        public event EventHandler OnMoveEvent;
        
        /// <summary>
        /// Occurs when the player looks. Avoid expensive code when using this.
        /// </summary>
        public event EventHandler OnLookEvent;
        
        /// <summary>
        /// Occurs when the jump button is pressed.
        /// </summary>
        public event EventHandler OnJumpActionEvent;
        
        /// <summary>
        /// Occurs when the player jumps.
        /// </summary>
        public event EventHandler OnJumpEvent;
        
        /// <summary>
        /// Occurs when the player lands on the ground.
        /// </summary>
        public event EventHandler OnLandEvent;
        
        /// <summary>
        /// Occurs when the slide key is pressed.
        /// </summary>
        public event EventHandler OnSlideActionEvent;
        
        /// <summary>
        /// Occurs when the player begins sliding.
        /// </summary>
        public event EventHandler OnSlideBeginEvent;
        
        /// <summary>
        /// Occurs when the player stops sliding.
        /// </summary>
        public event EventHandler OnSlideEndEvent;
        
        /// <summary>
        /// Occurs when the player presses the roll key.
        /// </summary>
        public event EventHandler OnRollActionEvent;
        
        /// <summary>
        /// Occurs when the player begins rolling.
        /// </summary>
        public event EventHandler OnRollBeginEvent;
        
        /// <summary>
        /// Occurs when the player stops rolling.
        /// </summary>
        public event EventHandler OnRollEndEvent;
        
        public MovementType GetMovementState() => currentMovementState;
        public void SetMovementState(MovementType movement) => currentMovementState = movement;
        
        public float GetWalkSpeed() => walkSpeed;
        public void SetWalkSpeed(float speed) => walkSpeed = speed;
        
        public float GetRunSpeed() => runSpeed;
        public void SetRunSpeed(float speed) => runSpeed = speed;
        
        public float GetJumpForce() => jumpForce;
        public void SetJumpForce(float force) => jumpForce = force;
        
        public float GetSlideStartSpeed() => slideStartSpeed;
        public void SetSlideStartSpeed(float speed) => slideStartSpeed = speed;
        
        public float GetSlideMaxSpeed() => maxSlideSpeed;
        public void SetSlideMaxSpeed(float speed) => maxSlideSpeed = speed;
        
        public float GetWallRunSpeed() => wallRunSpeed;
        public void SetWallRunSpeed(float speed) => wallRunSpeed = speed;

        private void Start()
        {
            Controller = GetComponent<CharacterController>();
            wallRunController = new WallRunController(this, wallCheckLeft, wallCheckRight, parkourWallLayer, wallRunSpeed);
            rhythmController = GetComponent<PlayerRhythmController>();
        }

        private void Update()
        {

            velocity.y -= gravity * -2f * Time.deltaTime;
            var oldIsGrounded = IsGrounded;
            IsGrounded = Physics.CheckSphere(groundCheckObject.transform.position, groundCheckRadius, groundLayer);
            
            if (IsGrounded && velocity.y < 0)
            {
                // Force the player to the ground
                velocity.y = -2f;
            }
            
            if (oldIsGrounded == false && IsGrounded)
            {
                OnLand();
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
            var currentSpeed = isHoldingSprintKey ? runSpeed : walkSpeed;
            Controller.Move(currentSpeed * Time.deltaTime * moveVector);
            
            OnMoveEvent?.Invoke(this, EventArgs.Empty);
        }
        public void OnLookAction(InputAction.CallbackContext context)
        {
            OnLookEvent?.Invoke(this, EventArgs.Empty);
        }

        public void Jump()
        {
            if (currentMovementState == MovementType.Sliding)
                EndSlide();
            
            if (currentMovementState == MovementType.WallRunning)
                wallRunController.EndWallRun();
        
            currentMovementState = MovementType.Jumping;
            velocity.y = jumpForce;
            
            OnJumpEvent?.Invoke(this, EventArgs.Empty);
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
            
            OnSlideBeginEvent?.Invoke(this, EventArgs.Empty);
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
            
            OnSlideEndEvent?.Invoke(this, EventArgs.Empty);
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
        
        public void OnMoveAction(InputAction.CallbackContext context)
        {
            // Update move values (x = left/right, y = forward/backward)
            moveInputs = context.ReadValue<Vector2>();
            
            OnMoveActionEvent?.Invoke(this, EventArgs.Empty);
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
            if (currentMovementState is MovementType.Idle or MovementType.Walking or MovementType.Running or MovementType.Sliding && IsGrounded)
            {
                Jump();
            }

            if (currentMovementState == MovementType.WallRunning)
            {
                wallRunController.EndWallRun();
                Jump();
            }
            
            OnJumpActionEvent?.Invoke(this, EventArgs.Empty);
        }

        public void OnRollAction(InputAction.CallbackContext context)
        {
            // if y velocity > certain value
            // check distance to the ground
            // roll if the distance is between a certain threshold
            
            OnRollActionEvent?.Invoke(this, EventArgs.Empty);
        }
        public void OnSlideAction(InputAction.CallbackContext context)
        {
            if (currentMovementState is not MovementType.Running or MovementType.Sliding)
                return;
            
            if (context.canceled)
            {
                if (currentMovementState == MovementType.Sliding)
                    EndSlide();
            }
            
            if (context.started && IsGrounded && currentMovementState == MovementType.Running)
            {
                BeginSlide();
                rhythmController.rhythmBarActivated(100f, 100, 3f);
            }
            
            OnSlideActionEvent?.Invoke(this, EventArgs.Empty);
        }

        private void OnLand()
        {
            OnLandEvent?.Invoke(this, EventArgs.Empty);
        }
        public void OnTrickAction(InputAction.CallbackContext context)
        {
            if (context.started) { rhythmController.rhythmBarActivated(300f, 100, 2f); }
            
            //OnLandEvent?.Invoke(this, EventArgs.Empty);
            
        }
    }
}