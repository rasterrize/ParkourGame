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
            Movement.OnSlideActionPressEvent += OnSlideActionPress;
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
            Movement.OnSlideActionPressEvent -= OnSlideActionPress;
        }

        private void OnSlideActionPress(object sender, EventArgs e)
        {
            Movement.SwitchState(Factory.NewSlideState());
        }

        private void Move()
        {
            var controller = Movement.Controller;
            var inputs = Movement.GetMoveInputs();
            var speed = Movement.GetRunSpeed();
            
            var groundCheck = Movement.GetGroundCheckObject();
            var forwardDirection = controller.transform.forward;
            var rightDirection = controller.transform.right;
            
            // Get the direction of the ground to account for slopes when moving.
            if (Physics.Raycast(groundCheck.transform.position, Vector3.down, out var hit, 1.0f, Physics.AllLayers))
            {
                forwardDirection = Vector3.Cross(controller.transform.right, hit.normal);
                rightDirection = Vector3.Cross(-controller.transform.forward, hit.normal);
            }
            
            var moveVector = forwardDirection * inputs.y + rightDirection * inputs.x;
            
            Movement.Controller.Move(speed * Time.deltaTime * moveVector);
            
            Movement.OnMove();
        }
    }
}