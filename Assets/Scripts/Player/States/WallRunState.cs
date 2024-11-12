using System;
using UnityEngine;

namespace Player.States
{
    public enum WallRunSide
    {
        Left,
        Right
    }

    public class WallRunState : MovementState
    {
        private RaycastHit startHit;
        private WallRunSide startSide;

        private Vector3 wallRunDirectionNormal;

        public WallRunState(PlayerMovement movement, MovementStateFactory factory, RaycastHit wallHit,
            WallRunSide wallRunSide)
            : base(movement, factory)
        {
            startHit = wallHit;
            startSide = wallRunSide;
        }

        public override void OnEnter()
        {
            Movement.ShouldHandleGravity = false;

            Movement.OnJumpActionEvent += OnJumpAction;

            CalculateWallRunDirection(startSide, startHit);
        }

        public override void OnUpdate()
        {
            Vector2 inputs = Movement.GetMoveInputs();

            if (!Movement.RunWallChecks() || inputs.y <= 0.0f)
                Movement.SwitchState(Factory.NewFallState());

            // Move player in the direction of the wall run
            Movement.CharController.Move(wallRunDirectionNormal * (Movement.GetWallRunSpeed() * Time.deltaTime));
        }

        public override void OnExit()
        {
            Movement.ShouldHandleGravity = true;

            Movement.OnJumpActionEvent -= OnJumpAction;
        }

        private void OnJumpAction(object sender, EventArgs e)
        {
            // Manually make the player jump since the PlayerMovement version checks for IsGrounded
            Movement.SwitchState(Factory.NewJumpState());

            // Tell PlayerMovement to wait until we are far from the wall before wall running again
            Movement.WaitUntilFarFromWall = true;
        }

        private void CalculateWallRunDirection(WallRunSide side, RaycastHit wallHit)
        {
            switch (side)
            {
                case WallRunSide.Left:
                    wallRunDirectionNormal = Vector3.Cross(wallHit.normal, Vector3.up);
                    break;
                case WallRunSide.Right:
                    wallRunDirectionNormal = Vector3.Cross(wallHit.normal, Vector3.down);
                    break;
            }
        }
    }
}