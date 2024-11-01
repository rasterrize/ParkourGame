using System;
using Player;

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
        
        public override void OnJumpActionEvent(object sender, EventArgs e)
        {
            if (!jumpAvailable || playerMovement.IsGrounded)
                return;
            
            playerMovement.Jump();
            jumpAvailable = false;
        }

        public override void OnLand(object sender, EventArgs e)
        {
            jumpAvailable = true;
        }
    } 
}