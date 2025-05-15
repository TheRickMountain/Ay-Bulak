using Microsoft.Xna.Framework;
using palmesneo_village;
using System;
using System.Collections.Generic;

namespace palmesneo_village
{
    public abstract class InteractableEntity : Entity
    {
        public InteractableEntity()
        {

        }

        public abstract void Interact(InteractionData interactionData, Inventory inventory);

        public abstract IEnumerable<InteractionData> GetAvailableInteractions(Inventory inventory);
    }
}
