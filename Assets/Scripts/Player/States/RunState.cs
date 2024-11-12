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
            CharacterController controller = Movement.CharController;
            Vector2 inputs = Movement.GetMoveInputs();
            float speed = Movement.GetRunSpeed();

            GameObject groundCheck = Movement.GetGroundCheckObject();
            Vector3 forwardDirection = controller.transform.forward;
            Vector3 rightDirection = controller.transform.right;

            // Get the direction of the ground to account for slopes when moving.
            if (Physics.Raycast(groundCheck.transform.position, Vector3.down, out RaycastHit hit, 1.0f,
                    Physics.AllLayers))
            {
                forwardDirection = Vector3.Cross(controller.transform.right, hit.normal);
                rightDirection = Vector3.Cross(-controller.transform.forward, hit.normal);
            }

            Vector3 moveVector = forwardDirection * inputs.y + rightDirection * inputs.x;

            Movement.CharController.Move(speed * Time.deltaTime * moveVector);

            Movement.OnMove();
        }
    }
}