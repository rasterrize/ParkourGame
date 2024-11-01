using System;

namespace Charms
{
     public enum CharmCategory
     {
          Movement,
          Rhythm,
          Combo,
          Stat,
     }
     
     public abstract class Charm
     {
          private const string DEFAULT_DESCRIPTION = "Default description.";
          
          public CharmCategory Category { get; protected set; }
          public string Name { get; protected set; }
          public string Description { get; protected set; }

          protected Charm(CharmCategory category, string name, string description = DEFAULT_DESCRIPTION)
          {
               Category = category;
               Name = name;
               Description = description;
          }

          // Enable the charm's ability
          public abstract void Activate();

          // Disable the charm's ability
          public abstract void Deactivate();

          public abstract void OnJumpActionEvent(object sender, EventArgs e);

          public abstract void OnLand(object sender, EventArgs e);
     }
}
