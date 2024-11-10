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
            var inputs = Movement.GetMoveInputs();
            if (inputs != Vector2.zero)
                Movement.SwitchState(Movement.IsHoldingSprintKey ? Factory.NewRunState() : Factory.NewWalkState());
            else
                Movement.SwitchState(Factory.NewIdleState());
        }
        
        private void Move()
        {
            var inputs = Movement.GetMoveInputs();
            var speed = Movement.IsHoldingSprintKey ? Movement.GetRunSpeed() : Movement.GetWalkSpeed();
            var controller = Movement.Controller;
            var moveVector = controller.transform.forward * inputs.y + controller.transform.right * inputs.x;
            
            Movement.Controller.Move(speed * Time.deltaTime * moveVector);
        }
    }
}