using System;

namespace Player.States
{
    public class IdleState : MovementState
    {
        public IdleState(PlayerMovement movement, MovementStateFactory factory)
            : base(movement, factory)
        {
        }

        public override void OnEnter()
        {
            Movement.OnMoveActionEvent += OnMoveAction;
            //Movement.OnJumpActionEvent += OnJumpAction;
        }

        public override void OnUpdate()
        {
        }

        public override void OnExit()
        {
            Movement.OnMoveActionEvent -= OnMoveAction;
            //Movement.OnJumpActionEvent -= OnJumpAction;
        }

        private void OnMoveAction(object sender, EventArgs e)
        {
            Movement.SwitchState(Movement.IsHoldingSprintKey ? Factory.NewRunState() : Factory.NewWalkState());
        }

        // private void OnJumpAction(object sender, EventArgs e)
        // {
        //     Movement.SwitchState(Factory.NewJumpState());
        // }
    }
}