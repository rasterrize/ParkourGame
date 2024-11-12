using System;
using UnityEngine;

namespace Player.States
{
    public class SlideState : MovementState
    {
        private FirstPersonController firstPersonController;
        private Camera playerCamera;
        private CharacterController charController;

        private float currentSlideVelocity;
        
        public SlideState(PlayerMovement movement, MovementStateFactory factory)
            : base(movement, factory)
        {
        }

        public override void OnEnter()
        {
            firstPersonController = Movement.GetFirstPersonController();
            playerCamera = firstPersonController.GetCamera();
            charController = Movement.Controller;

            Movement.rhythmController.rhythmBarActivated(100f, 100, 3f);

            Movement.OnSlideActionReleaseEvent += OnSlideActionReleased;
            
            if (Movement.GetSlideStartSpeed() - Movement.GetCurrentSlidePenalty() > 0.0f)
                BeginSlide(Movement.GetSlideStartSpeed(), Movement.GetCurrentSlidePenalty());
            else 
                Movement.SwitchState(Factory.NewInputBasedGroundState());
        }

        public override void OnUpdate()
        {
            var inputs = Movement.GetMoveInputs();
        
            // Stop sliding if the player isn't holding the forward key or the slide velocity has run out.
            if (inputs.y <= 0.0f || currentSlideVelocity <= 0.0f)
            {
                Movement.SwitchState(Factory.NewInputBasedGroundState());
                return;
            }

            var controller = Movement.Controller;
            
            var groundCheck = Movement.GetGroundCheckObject();
            var forwardDirection = controller.transform.forward;
            
            // Get the direction of the ground so we can decide to speed up or slow down the slide.
            if (Physics.Raycast(groundCheck.transform.position, Vector3.down, out var hit, 1.0f, Physics.AllLayers))
                forwardDirection = Vector3.Cross(controller.transform.right, hit.normal);
            
            // if the slope goes downhill, increase the velocity
            if (forwardDirection.y < -0.1f)
                currentSlideVelocity += 15.0f * Time.deltaTime;
                        
            charController.Move(forwardDirection * (currentSlideVelocity * Time.deltaTime));
            
            // Reduce slide velocity to simulate friction
            currentSlideVelocity -= 10.0f * Time.deltaTime;
            
            Movement.OnMove();
        }

        public override void OnExit()
        {
            EndSlide();
        }

        private void BeginSlide(float startVelocity, float currentPenalty)
        {
            currentSlideVelocity = startVelocity - currentPenalty;
            
            // Increase penalty
            Movement.SetCurrentSlidePenalty(Movement.GetCurrentSlidePenalty() + Movement.GetSlidePenaltyMultiplier());
            
            // Move camera down to emulate character sliding
            playerCamera.transform.localPosition = new Vector3(0, playerCamera.transform.localPosition.y - Movement.GetSlideCameraYDistance(), 0);
            
            // Shrink player collider
            charController.center = new Vector3(0, -0.5f, 0);
            charController.height = 1;
            
            // Lock player camera rotation
            firstPersonController.XRotationMaxClamp = 60.0f;
            
            Movement.OnSlideBegin();
        }

        private void EndSlide()
        {
            currentSlideVelocity = 0.0f;
        
            // Reset camera
            playerCamera.transform.localPosition = new Vector3(0, 0.5f, 0);
        
            // Reset player collider
            charController.center = new Vector3(0, 0, 0);
            charController.height = 2;
            
            // Reset player camera rotation
            firstPersonController.ResetXRotationClamps();
            
            Movement.OnSlideEnd();
        }

        private void OnSlideActionReleased(object sender, EventArgs e)
        {
            // End slide when slide key is released
            EndSlide();
            Movement.SwitchState(Factory.NewInputBasedGroundState());
        }
    }
}
