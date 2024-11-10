namespace Charms
{
    /// <summary>
    /// A player-wearable keychain that stores a limited number of <see cref="Charm"/>'s.
    /// Charm slots are indexed from 1 to <see cref="charmSlotCount"/>
    /// </summary>
    public class Keychain
    {
        private const uint DEFAULT_CHARM_LIMIT = 3;
        private Charm[] charms;
        private uint charmSlotCount = DEFAULT_CHARM_LIMIT;
        
        /// <summary>
        /// Create a keychain with the default charm limit: <see cref="DEFAULT_CHARM_LIMIT"/>
        /// </summary>
        public Keychain()
        {
            charms = new Charm[charmSlotCount];
        }
        
        /// <summary>
        /// Create a keychain with a specific number of slots
        /// </summary>
        /// <param name="numCharmSlots">The number of slots</param>
        public Keychain(uint numCharmSlots)
        {
            charms = new Charm[numCharmSlots];
            charmSlotCount = numCharmSlots;
        }
        
        /// <summary>
        /// Create a keychain using an existing array of charms
        /// </summary>
        /// <param name="charms">The charms for this keychain to hold</param>
        public Keychain(Charm[] charms)
        {
            this.charms = charms;
        }

        /// <summary>
        /// Activates all charms on this keychain
        /// </summary>
        public void Activate()
        {
            foreach (var charm in charms)
                charm?.Activate();
        }
        
        /// <summary>
        /// Deactivates all charms on this keychain
        /// </summary>
        public void Deactivate()
        {
            foreach (var charm in charms)
                charm?.Deactivate();
        }
    
        /// <summary>
        /// Get all the charms on this keychain
        /// </summary>
        /// <returns>The array of charms</returns>
        public Charm[] GetCharms()
        {
            return charms;
        }
        
        /// <summary>
        /// Get a charm from this keychain.
        /// </summary>
        /// <param name="slot">The slot to retrieve from</param>
        /// <returns>The charm if it exists, otherwise null.</returns>
        public Charm GetCharm(uint slot)
        {
            return charms[slot];
        }
        
        /// <summary>
        /// Sets a charm for this keychain.
        /// </summary>
        /// <param name="slot">The slot to use in this keychain. Must be from 1 to <see cref="charmSlotCount"/></param>
        /// <param name="charm">The charm to set.</param>
        /// <returns>The existing charm in the slot if it exists. returns null otherwise.</returns>
        public Charm SetCharm(uint slot, Charm charm)
        {
            // Decrement slot number to match array
            slot--;
            
            var oldCharm = charms[slot];
            charms[slot] = charm;
            return oldCharm;
        }

        /// <summary>
        /// Add a charm to the next available slot on this keychain if available.
        /// </summary>
        /// <param name="charm">The charm to add</param>
        /// <returns>Whether adding the charm succeeded, returns false if there are no available slots</returns>
        private bool AddCharm(Charm charm)
        {
            for (uint slot = 0; slot < charmSlotCount; slot++)
            {
                if (charms[slot] == null)
                {
                    SetCharm(slot, charm);
                    return true;
                }
            }

            return false;
        }
        
        public uint GetCharmSlotCount() => charmSlotCount;
        
        public uint GetSlotsTakenCount()
        {
            uint slotsTaken = 0;

            foreach (var charm in charms)
            {
                if (charm != null)
                    slotsTaken++;
            }
            
            return slotsTaken;
        }
    }
}