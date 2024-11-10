using UnityEngine;

namespace Player.States
{
    public class SlideState : MovementState
    {
        private float currentSlideVelocity;
        private Camera playerCamera;
        private CharacterController charController;
        
        public SlideState(PlayerMovement movement, MovementStateFactory factory)
            : base(movement, factory)
        {
        }

        public override void OnEnter()
        {
            currentSlideVelocity = Movement.GetSlideStartSpeed();
        
            playerCamera = Movement.GetPlayerCamera();
            charController = Movement.Controller;
        
            // Move camera down to emulate character sliding
            playerCamera.transform.localPosition = new Vector3(0, playerCamera.transform.localPosition.y - Movement.GetSlideCameraYDistance(), 0);
            
            // Shrink player collider
            charController.center = new Vector3(0, -0.5f, 0);
            charController.height = 1;
            
            Movement.OnSlideBegin();
        }

        public override void OnUpdate()
        {
            var inputs = Movement.GetMoveInputs();
        
            if (inputs.y <= 0.0f || currentSlideVelocity <= 0.0f)
            {
                Movement.SwitchState(Factory.NewInputBasedGroundState());
                return;
            }
        
            currentSlideVelocity -= 10.0f * Time.deltaTime;
            charController.Move(charController.transform.forward * (currentSlideVelocity * Time.deltaTime));
            
            Movement.OnMove();
        }

        public override void OnExit()
        {
            currentSlideVelocity = 0.0f;
        
            // Reset camera
            playerCamera.transform.localPosition = new Vector3(0, 0.5f, 0);
        
            // Reset player collider
            charController.center = new Vector3(0, 0, 0);
            charController.height = 2;
            
            Movement.OnSlideEnd();
        }
    }
}
