using Charms;
using UnityEngine;

namespace Player
{
    public class PlayerData : MonoBehaviour
    {
        private string playerName;
        private float experience;
        private uint cosmicCubes;

        private Keychain keychain;
        private Charm testCharm;

        private PlayerMovement movement;

        private void Start()
        {
            movement = GetComponent<PlayerMovement>();

            testCharm = new DoubleJumpCharm(movement);
            keychain = new Keychain();
            keychain.SetCharm(1, testCharm);
            keychain.Activate();
        }
    }
}