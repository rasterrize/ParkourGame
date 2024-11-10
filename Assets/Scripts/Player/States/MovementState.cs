namespace Player.States
{
    public abstract class MovementState
    {
        protected PlayerMovement Movement;
        protected MovementStateFactory Factory;
    
        protected MovementState(PlayerMovement movement, MovementStateFactory factory)
        {
            Movement = movement;
            Factory = factory;
        }
        
        /// <summary>
        /// Called when the state begins
        /// </summary>
        public abstract void OnEnter();
        
        /// <summary>
        /// Called every frame
        /// </summary>
        public abstract void OnUpdate();
        
        /// <summary>
        /// Called when the state is exiting
        /// </summary>
        public abstract void OnExit();
    }
}
