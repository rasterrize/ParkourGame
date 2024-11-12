using UnityEngine;

namespace Player.States
{
    public class MovementStateFactory
    {
        private readonly PlayerMovement movement;

        public MovementStateFactory(PlayerMovement playerMovement)
        {
            movement = playerMovement;
        }

        public MovementState NewIdleState() { return new IdleState(movement, this); }
        public MovementState NewWalkState() { return new WalkState(movement, this); }
        public MovementState NewRunState() { return new RunState(movement, this); }
        public MovementState NewJumpState() { return new JumpState(movement, this); }

        public MovementState NewWallRunState(RaycastHit wallHit, WallRunSide wallRunSide)
        {
            return new WallRunState(movement, this, wallHit, wallRunSide);
        }

        public MovementState NewFallState() { return new FallState(movement, this); }
        public MovementState NewSlideState() { return new SlideState(movement, this); }

        /// <summary>
        /// Returns a new ground movement state based on the players inputs.
        /// Will either be <see cref="IdleState"/>, <see cref="WalkState"/>, or <see cref="RunState"/>
        /// </summary>
        /// <returns>The new state</returns>
        public MovementState NewInputBasedGroundState()
        {
            Vector2 inputs = movement.GetMoveInputs();
            bool isHoldingSprintKey = movement.IsHoldingSprintKey;

            if (inputs != Vector2.zero)
                return isHoldingSprintKey ? NewRunState() : NewWalkState();

            return NewIdleState();
        }
    }
}