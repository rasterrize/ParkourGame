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
        }

        public override void OnUpdate()
        {
        }

        public override void OnExit()
        {
            Movement.OnMoveActionEvent -= OnMoveAction;
        }

        private void OnMoveAction(object sender, EventArgs e)
        {
            Movement.SwitchState(Movement.IsHoldingSprintKey ? Factory.NewRunState() : Factory.NewWalkState());
        }
    }
}