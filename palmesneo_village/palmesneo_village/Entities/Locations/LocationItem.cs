using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class LocationItem : ImageEntity
    {
        public ItemContainer ItemContainer { get; }

        private float pickupRadius = 48f;
        private bool isBeingPickedUp;
        private Vector2 targetPosition;
        private float pickupSpeed = 300f;

        public LocationItem(ItemContainer itemContainer)
        {
            ItemContainer = itemContainer;

            Texture = ItemContainer.Item.Icon;
            Centered = true;
        }

        public bool CanBePickedUp(Vector2 playerPosition)
        {
            return !isBeingPickedUp && Vector2.DistanceSquared(GlobalPosition, playerPosition) <= pickupRadius * pickupRadius;
        }

        public void StartPickup(Vector2 target)
        {
            isBeingPickedUp = true;
            targetPosition = target;
        }

        public override void Update()
        {
            base.Update();

            if (isBeingPickedUp)
            {
                Vector2 direction = targetPosition - LocalPosition;
                float distance = direction.Length();

                if (distance > 5f) 
                {
                    direction.Normalize();
                    LocalPosition += direction * pickupSpeed * Engine.GameDeltaTime;
                    Depth = (int)LocalPosition.Y;
                }
                else
                {
                    IsActive = false;
                }
            }
        }

    }
}
