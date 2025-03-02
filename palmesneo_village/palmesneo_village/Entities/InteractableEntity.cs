using Microsoft.Xna.Framework;
using palmesneo_village;
using System;
using System.Collections.Generic;

namespace palmesneo_village
{
    public abstract class InteractableEntity : Entity
    {
        public event Action Destroyed;

        private bool isDestroyed;

        public InteractableEntity()
        {

        }

        public void Destroy()
        {
            if (isDestroyed)
            {
                return;
            }

            Parent.RemoveChild(this);

            isDestroyed = true;

            Destroyed?.Invoke();
        }

        public abstract Rectangle GetSelectorBounds();
    }
}
