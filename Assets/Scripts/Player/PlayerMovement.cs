using System;
using Player.States;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        private const float DEFAULT_GRAVITY = -9.81f;
        private const float DEFAULT_WALK_SPEED = 4f;
        private const float DEFAULT_RUN_SPEED = 8f;
        private const float DEFAULT_JUMP_FORCE = 10f;
        private const float DEFAULT_SLIDE_START_VELOCITY = 12f;
        private const float DEFAULT_SLIDE_END_VELOCITY = 0f;
        private const float DEFAULT_SLIDE_MAX_SPEED = 20f;
        private const float DEFAULT_SLIDE_CAMERA_Y_DISTANCE = 0.5f;
        private const float DEFAULT_SLIDE_PENALTY_MULTIPLIER = 5.0f;
        private const float DEFAULT_WALL_RUN_SPEED = 8f;
        private const float DEFAULT_WALL_CHECK_DISTANCE = 0.1f;

        [SerializeField] private Camera playerCamera;
        [SerializeField] private LayerMask parkourWallLayer;
        [SerializeField] private GameObject wallCheckLeft;
        [SerializeField] private GameObject wallCheckRight;

        public bool IsGrounded { get; private set; }
        
        // Custom ground check
        private bool useCustomGroundCheck;
        private CustomGroundCheck customGroundCheck;

        [SerializeField] private GameObject groundCheck;
        
        /// <summary>
        /// The character controller of the player
        /// </summary>
        public CharacterController Controller { get; private set; }
        
        private FirstPersonController firstPersonController;

        [Tooltip("The gravity of the player. This defaults to earth's gravity.")] 
        [SerializeField] private float gravity = DEFAULT_GRAVITY;
        
        [Tooltip("The walking speed of the player.")]
        [SerializeField] private float walkSpeed = DEFAULT_WALK_SPEED;
        
        [Tooltip("The running speed of the player.")]
        [SerializeField] private float runSpeed = DEFAULT_RUN_SPEED;
        
        [Tooltip("The amount of force to apply when the player jumps")]
        [SerializeField] private float jumpForce = DEFAULT_JUMP_FORCE;
        
        [Tooltip("The speed at which sliding will start at.")]
        [SerializeField] private float slideStartVelocity = DEFAULT_SLIDE_START_VELOCITY;
        
        [Tooltip("The speed at which sliding will end at automatically.")]
        [SerializeField] private float slideEndVelocity = DEFAULT_SLIDE_END_VELOCITY;
        
        [Tooltip("The maximum speed the player can slide. Usually only reached when sliding downhill.")]
        [SerializeField] private float maxSlideSpeed = DEFAULT_SLIDE_MAX_SPEED;

        [Tooltip("The distance to move the camera down on the y axis when the player slides.")]
        [SerializeField] private float slideCameraYDistance = DEFAULT_SLIDE_CAMERA_Y_DISTANCE;
        
        [Tooltip("The penalty multiplier for consecutive slides.")]
        [SerializeField] private float slidePenaltyMultiplier = DEFAULT_SLIDE_PENALTY_MULTIPLIER;
        
        [Tooltip("The speed at which the player will run on walls")]
        [SerializeField] private float wallRunSpeed = DEFAULT_WALL_RUN_SPEED;
        
        [Tooltip("The distance for player to wall checks to initiate mechanics such as wall running.")]
        [SerializeField] private float wallCheckDistance = DEFAULT_WALL_CHECK_DISTANCE;
        
        public bool IsHoldingSprintKey { get; private set; }
        private float yVelocity;
        private Vector2 moveInputs;
        public bool ShouldHandleGravity { get; set; } = true;

        private MovementStateFactory movementStateFactory;
        private MovementState currentMovementState;

        public bool WaitUntilFarFromWall { get; set; }

        [SerializeField] private string movementStateString;

        private PlayerRhythmController rhythmController;

        private float currentSlidePenalty;
        
        #region Events
        
        // --------
        // Events
        // --------
        
        /// <summary>
        /// Occurs when the movement keys are pressed or the joystick is moved. Avoid expensive code when using this.
        /// </summary>
        public event EventHandler OnMoveActionEvent;
        
        /// <summary>
        /// Occurs when the player moves horizontally. Avoid expensive code when using this.
        /// </summary>
        public event EventHandler OnMoveEvent;
        
        /// <summary>
        /// Occurs when the player presses the sprint key.
        /// </summary>
        public event EventHandler OnSprintActionEvent;
        
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
        public event EventHandler OnSlideActionPressEvent;
        
        /// <summary>
        /// Occurs when the slide key is released.
        /// </summary>
        public event EventHandler OnSlideActionReleaseEvent;
        
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
        
        #endregion

        public FirstPersonController GetFirstPersonController() => firstPersonController;
        
        public Vector2 GetMoveInputs() => moveInputs;

        public MovementStateFactory GetStateFactory() => movementStateFactory;
        
        public float GetYVelocity() => yVelocity;
        public void SetYVelocity(float velocity) => yVelocity = velocity;
        
        public float GetWalkSpeed() => walkSpeed;
        public void SetWalkSpeed(float speed) => walkSpeed = speed;
        
        public float GetRunSpeed() => runSpeed;
        public void SetRunSpeed(float speed) => runSpeed = speed;
        
        public float GetJumpForce() => jumpForce;
        public void SetJumpForce(float force) => jumpForce = force;
        
        public float GetSlideStartSpeed() => slideStartVelocity;
        public void SetSlideStartSpeed(float speed) => slideStartVelocity = speed;
        
        public float GetSlideMaxSpeed() => maxSlideSpeed;
        public void SetSlideMaxSpeed(float speed) => maxSlideSpeed = speed;
        
        public float GetSlideCameraYDistance() => slideCameraYDistance;

        public float GetSlidePenaltyMultiplier() => slidePenaltyMultiplier;
        public float GetCurrentSlidePenalty() => currentSlidePenalty;
        public float SetCurrentSlidePenalty(float penalty) => currentSlidePenalty = penalty;
        
        public float GetWallRunSpeed() => wallRunSpeed;
        public void SetWallRunSpeed(float speed) => wallRunSpeed = speed;

        private void Start()
        {
            Controller = GetComponent<CharacterController>();
            firstPersonController = GetComponent<FirstPersonController>();
            customGroundCheck = GetComponent<CustomGroundCheck>();

            movementStateFactory = new MovementStateFactory(this);
            currentMovementState = movementStateFactory.NewIdleState();
            currentMovementState.OnEnter();
            
            // Use custom ground check if it exists.
            if (customGroundCheck != null)
                useCustomGroundCheck = true;
            
            rhythmController = GetComponent<PlayerRhythmController>();
        }

        private void Update()
        {
            RunGroundedCheck();

            RunWallChecks();
            
            ReduceSlidePenalty();
            
            currentMovementState.OnUpdate();

            if (ShouldHandleGravity)
                HandleGravity();

            movementStateString = currentMovementState.GetType().ToString();
        }

        private void RunGroundedCheck()
        {
            // Check if we are currently grounded
            var oldIsGrounded = IsGrounded;
            IsGrounded = useCustomGroundCheck ? customGroundCheck.IsGrounded() : Controller.isGrounded;
            
            // Check if the player has just landed on the ground
            if (oldIsGrounded == false && IsGrounded)
                OnLand();
        }

        public bool RunWallChecks()
        {
            // Left Wall Check
            if (Physics.Raycast(wallCheckLeft.transform.position, -wallCheckLeft.transform.right, out var hit, wallCheckDistance, parkourWallLayer))
            {
                if (currentMovementState.GetType() != typeof(WallRunState) && !WaitUntilFarFromWall)
                    SwitchState(movementStateFactory.NewWallRunState(hit, WallRunSide.Left));

                return true;
            }
            
            // Right Wall Check
            if (Physics.Raycast(wallCheckRight.transform.position, wallCheckRight.transform.right, out var hit2, wallCheckDistance, parkourWallLayer))
            {
                if (currentMovementState.GetType() != typeof(WallRunState) && !WaitUntilFarFromWall)
                    SwitchState(movementStateFactory.NewWallRunState(hit2, WallRunSide.Right));
                
                return true;
            }
            
            WaitUntilFarFromWall = false;

            return false;
        }

        private void HandleGravity()
        {
            // Constantly apply gravity
            yVelocity -= gravity * -2f * Time.deltaTime;
            
            // Force the player to the ground
            if (IsGrounded && yVelocity < 0)
                yVelocity = -2f;
            
            // TODO: Automatically set player state to falling when the velocity is less than high fall threshold
            
            // Move the player using gravity
            Controller.Move(Vector3.up * (yVelocity * Time.deltaTime));
        }

        public void SwitchState(MovementState newState)
        {
            currentMovementState.OnExit();
            currentMovementState = newState;
            currentMovementState.OnEnter();
        }

        private void ReduceSlidePenalty()
        {
            currentSlidePenalty -= 1.0f * Time.deltaTime;
            currentSlidePenalty = Mathf.Max(0.0f, currentSlidePenalty);
        }
        
        public void OnMoveAction(InputAction.CallbackContext context)
        {
            // Update move values (x = left/right, y = forward/backward)
            moveInputs = context.ReadValue<Vector2>();
            
            OnMoveActionEvent?.Invoke(this, EventArgs.Empty);
        }

        public void OnSprintAction(InputAction.CallbackContext context)
        {
            IsHoldingSprintKey = context.performed;
            
            if (context.performed)
                OnSprintActionEvent?.Invoke(this, EventArgs.Empty);
        }

        public void OnJumpAction(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            
            // We jump regardless of the current movement state
            if (IsGrounded)
                SwitchState(movementStateFactory.NewJumpState());
            
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
            if (context.performed)
                OnSlideActionPressEvent?.Invoke(this, EventArgs.Empty);
            else if (context.canceled)
                OnSlideActionReleaseEvent?.Invoke(this, EventArgs.Empty);
            
            rhythmController?.rhythmBarActivated(100f, 100, 3f);
        }

        private void OnLand()
        {
            OnLandEvent?.Invoke(this, EventArgs.Empty);
        }
        
        public void OnTrickAction(InputAction.CallbackContext context)
        {
            if (context.started)
                rhythmController?.rhythmBarActivated(300f, 100, 2f);
        }

        public void OnJump()
        {
            OnJumpEvent?.Invoke(this, EventArgs.Empty);
        }

        public void OnSlideBegin()
        {
            OnSlideBeginEvent?.Invoke(this, EventArgs.Empty);
        }

        public void OnSlideEnd()
        {
            OnSlideEndEvent?.Invoke(this, EventArgs.Empty);   
        }

        public void OnMove()
        {
            OnMoveEvent?.Invoke(this, EventArgs.Empty);
        }

        public void OnLook()
        {
            OnLookEvent?.Invoke(this, EventArgs.Empty);
        }

        public GameObject GetGroundCheckObject()
        {
            return groundCheck;
        }
    }
}