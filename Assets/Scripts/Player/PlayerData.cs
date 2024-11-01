using Charms;
using UnityEngine;

namespace Player
{
    public class PlayerData : MonoBehaviour
    {
        private string name;
        private float experience;
        private uint cosmicCubes;
        
        private Charm testCharm;
        
        private PlayerMovement movement;
        
        private void Start()
        {
            movement = GetComponent<PlayerMovement>();
            
            testCharm = new DoubleJumpCharm(movement);
            testCharm.Activate();
        }

        public Charm GetCharm()
        {
            return testCharm;
        }
    }
}
