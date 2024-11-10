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
            
            Movement.OnSlideActionReleaseEvent += OnSlideActionReleased;
            
            if (Movement.GetSlideStartSpeed() - Movement.GetCurrentSlidePenalty() > 0.0f)
                BeginSlide(Movement.GetSlideStartSpeed(), Movement.GetCurrentSlidePenalty());
            else 
                Movement.SwitchState(Factory.NewInputBasedGroundState());
        }

        public override void OnUpdate()
        {
            var inputs = Movement.GetMoveInputs();
        
            if (inputs.y <= 0.0f || currentSlideVelocity <= 0.0f)
            {
                Movement.SwitchState(Factory.NewInputBasedGroundState());
                return;
            }

            var inclineVelocityIncrease = Math.Max(0.0f, -Movement.GetYVelocity());
            
            currentSlideVelocity -= 10.0f * Time.deltaTime;
            charController.Move(charController.transform.forward * (currentSlideVelocity * Time.deltaTime));
            
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
