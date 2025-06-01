using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class AnimalFeederBuilding : Building
    {
        public bool IsFull { get; private set; } = false;

        private AnimalFeederItem animalFeederItem;

        public AnimalFeederBuilding(GameLocation gameLocation, AnimalFeederItem item, Direction direction, Vector2[,] occupiedTiles) 
            : base(gameLocation, item, direction, occupiedTiles)
        {
            animalFeederItem = item;
        }

        public override void Interact(Inventory inventory, int activeSlotIndex)
        {
            Item handItem = inventory.GetSlotItem(activeSlotIndex);

            if (handItem == null) return;

            if (handItem != Engine.ItemsDatabase.GetItemByName("hay")) return;

            if (IsFull) return;

            IsFull = true;
            Sprite.Texture = animalFeederItem.FullTexture;
            
            inventory.RemoveItem(handItem, 1, activeSlotIndex);
        }

        public void Empty()
        {
            IsFull = false;
            Sprite.Texture = animalFeederItem.EmptyTexture;
        }

    }
}
