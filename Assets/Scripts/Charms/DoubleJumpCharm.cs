using System;
using Player;
using Player.States;

namespace Charms
{
    public class DoubleJumpCharm : Charm
    {
        private bool jumpAvailable = true;
        private PlayerMovement playerMovement;

        public DoubleJumpCharm(PlayerMovement playerMovement)
            : base(CharmCategory.Movement, "Double Jump Charm", "A test description")
        {
            this.playerMovement = playerMovement;
        }

        public override void Activate()
        {
            playerMovement.OnJumpActionEvent += OnJumpActionEvent;
            playerMovement.OnLandEvent += OnLand;
        }

        public override void Deactivate()
        {
            playerMovement.OnJumpActionEvent -= OnJumpActionEvent;
            playerMovement.OnLandEvent -= OnLand;
        }
        
        private void OnJumpActionEvent(object sender, EventArgs e)
        {
            if (!jumpAvailable || playerMovement.IsGrounded)
                return;

            playerMovement.SwitchState(playerMovement.GetStateFactory().NewJumpState());
            jumpAvailable = false;
        }

        private void OnLand(object sender, EventArgs e)
        {
            jumpAvailable = true;
        }
    } 
}