using System;
using UnityEngine;

namespace Player.States
{
    public class FallState : MovementState
    {
        public FallState(PlayerMovement movement, MovementStateFactory factory)
            : base(movement, factory)
        {
        }

        public override void OnEnter()
        {
            Movement.OnLandEvent += OnLand;
        }

        public override void OnUpdate()
        {
            Move();
        }

        public override void OnExit()
        {
            Movement.OnLandEvent += OnLand;
        }

        private void OnLand(object sender, EventArgs e)
        {
            Vector2 inputs = Movement.GetMoveInputs();
            if (inputs != Vector2.zero)
                Movement.SwitchState(Movement.IsHoldingSprintKey ? Factory.NewRunState() : Factory.NewWalkState());
            else
                Movement.SwitchState(Factory.NewIdleState());
        }

        private void Move()
        {
            Vector2 inputs = Movement.GetMoveInputs();
            float speed = Movement.IsHoldingSprintKey ? Movement.GetRunSpeed() : Movement.GetWalkSpeed();
            CharacterController controller = Movement.CharController;
            Vector3 moveVector = controller.transform.forward * inputs.y + controller.transform.right * inputs.x;

            Movement.CharController.Move(speed * Time.deltaTime * moveVector);
        }
    }
}