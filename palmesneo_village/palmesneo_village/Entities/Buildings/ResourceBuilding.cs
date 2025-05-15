using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class ResourceBuilding : Building
    {

        private ResourceItem resourceItem;

        private int currentStrength;

        public ResourceBuilding(GameLocation gameLocation, ResourceItem resourceItem, Direction direction, Vector2[,] occupiedTiles) 
            : base(gameLocation, resourceItem, direction, occupiedTiles)
        {
            this.resourceItem = resourceItem;

            currentStrength = resourceItem.Strength;
        }

        public override void Interact(Inventory inventory, int activeSlotIndex, PlayerEnergyManager playerEnergyManager)
        {
            Item item = inventory.GetSlotItem(activeSlotIndex);

            if (resourceItem.RequiredToolType == ToolType.None)
            {
                currentStrength--;

                if (currentStrength <= 0)
                {
                    GatherResource();
                }
            }
            else
            {
                if(item is ToolItem toolItem && toolItem.ToolType == resourceItem.RequiredToolType)
                {
                    toolItem.PlaySoundEffect();

                    playerEnergyManager.ConsumeEnergy(1);

                    currentStrength -= toolItem.Efficiency;

                    if (currentStrength <= 0)
                    {
                        GatherResource();
                    }
                }
            }
        }

        private void GatherResource()
        {
            ItemContainer itemContainer = new ItemContainer();
            itemContainer.Item = Engine.ItemsDatabase.GetItemByName<Item>(resourceItem.ItemName);
            itemContainer.Quantity = resourceItem.ItemAmount;
            GameLocation.AddItem(OccupiedTiles[0, 0] * Engine.TILE_SIZE, itemContainer);
            GameLocation.RemoveBuilding(this);
        }

    }
}
