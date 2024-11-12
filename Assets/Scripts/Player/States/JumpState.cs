using System;
using UnityEngine;

namespace Player.States
{
    public class JumpState : MovementState
    {
        public JumpState(PlayerMovement movement, MovementStateFactory factory)
            : base(movement, factory)
        {
        }

        public override void OnEnter()
        {
            Jump();
            Movement.OnLandEvent += OnLand;
        }

        public override void OnUpdate()
        {
            Move();
        }

        public override void OnExit()
        {
            Movement.OnLandEvent -= OnLand;
        }

        private void Jump()
        {
            // Jump the player using jumpForce
            Movement.SetYVelocity(Movement.GetJumpForce());

            Movement.OnJump();
        }

        private void Move()
        {
            Vector2 inputs = Movement.GetMoveInputs();
            float speed = Movement.IsHoldingSprintKey ? Movement.GetRunSpeed() : Movement.GetWalkSpeed();
            CharacterController controller = Movement.CharController;
            Vector3 moveVector = controller.transform.forward * inputs.y + controller.transform.right * inputs.x;

            Movement.CharController.Move(speed * Time.deltaTime * moveVector);

            Movement.OnMove();
        }

        private void OnLand(object sender, EventArgs e)
        {
            Movement.SwitchState(Factory.NewInputBasedGroundState());
        }
    }
}