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

        public override bool CanInteract(Item item)
        {
            return item is ToolItem toolItem && toolItem.ToolType == resourceItem.RequiredToolType;
        }

        public override void Interact(Item item)
        {
            if (CanInteract(item))
            {
                ToolItem toolItem = (ToolItem)item;

                currentStrength -= toolItem.Efficiency;

                ItemContainer itemContainer = new ItemContainer();
                itemContainer.Item = Engine.ItemsDatabase.GetItemByName<Item>(resourceItem.ItemName);
                itemContainer.Quantity = resourceItem.ItemAmount;

                gameLocation.AddItem(OccupiedTiles[0, 0] * Engine.TILE_SIZE, itemContainer);

                if (currentStrength <= 0)
                {
                    gameLocation.RemoveBuilding(this);
                }
            }
        }
    
        
    }
}
