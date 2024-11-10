using UnityEngine;

namespace Player.States
{
    public class WalkState : MovementState
    {
        public WalkState(PlayerMovement movement, MovementStateFactory factory)
            : base(movement, factory)
        {
        }

        public override void OnEnter()
        {
        }

        public override void OnUpdate()
        {
            if (Movement.GetMoveInputs() == Vector2.zero)
                Movement.SwitchState(Factory.NewIdleState());
            
            if (Movement.IsHoldingSprintKey)
                Movement.SwitchState(Factory.NewRunState());
            
            Move();
        }

        public override void OnExit()
        {
        }

        private void Move()
        {
            var inputs = Movement.GetMoveInputs();
            var speed = Movement.GetWalkSpeed();
            var controller = Movement.Controller;
            var moveVector = controller.transform.forward * inputs.y + controller.transform.right * inputs.x;
            
            Movement.Controller.Move(speed * Time.deltaTime * moveVector);

            Movement.OnMove();
        }
    }
}