using System;
using UnityEngine;

namespace Player.States
{
    public class RunState : MovementState
    {
        public RunState(PlayerMovement movement, MovementStateFactory factory)
            : base(movement, factory)
        {
        }

        public override void OnEnter()
        {
            Movement.OnSlideActionEvent += OnSlideAction;
        }

        public override void OnUpdate()
        {
            if (Movement.GetMoveInputs() == Vector2.zero)
                Movement.SwitchState(Factory.NewIdleState());
            
            if (!Movement.IsHoldingSprintKey)
                Movement.SwitchState(Factory.NewWalkState());
            
            Move();
        }

        public override void OnExit()
        {
            Movement.OnSlideActionEvent -= OnSlideAction;
        }

        private void OnSlideAction(object sender, EventArgs e)
        {
            Movement.SwitchState(Factory.NewSlideState());
        }

        private void Move()
        {
            var inputs = Movement.GetMoveInputs();
            var speed = Movement.GetRunSpeed();
            var controller = Movement.Controller;
            var moveVector = controller.transform.forward * inputs.y + controller.transform.right * inputs.x;
            
            Movement.Controller.Move(speed * Time.deltaTime * moveVector);
            
            Movement.OnMove();
        }
    }
}