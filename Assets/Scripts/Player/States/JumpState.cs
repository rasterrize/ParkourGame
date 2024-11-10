﻿using System;
using UnityEngine;

namespace Player.States
{
    public class JumpState : MovementState
    {
        public JumpState(PlayerMovement movement, MovementStateFactory factory)
            : base(movement, factory)
        {
        }

        public override void OnEnter()
        {
            Jump();
            Movement.OnLandEvent += OnLand;
        }

        public override void OnUpdate()
        {
            Move();
        }

        public override void OnExit()
        {
            Movement.OnLandEvent -= OnLand;
        }

        private void Jump()
        {
            // Jump the player using jumpForce
            Movement.SetYVelocity(Movement.GetJumpForce());
            
            Movement.OnJump();
        }

        private void Move()
        {
            var inputs = Movement.GetMoveInputs();
            var speed = Movement.IsHoldingSprintKey ? Movement.GetRunSpeed() : Movement.GetWalkSpeed();
            var controller = Movement.Controller;
            var moveVector = controller.transform.forward * inputs.y + controller.transform.right * inputs.x;
            
            Movement.Controller.Move(speed * Time.deltaTime * moveVector);

            Movement.OnMove();
        }

        private void OnLand(object sender, EventArgs e)
        {
            Movement.SwitchState(Factory.NewInputBasedGroundState());
        }
    }
}