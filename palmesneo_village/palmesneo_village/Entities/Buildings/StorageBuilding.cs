using System;
using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class StorageBuilding : Building
    {
        private StorageItem storageItem;

        private Inventory inventory;

        public StorageBuilding(GameLocation gameLocation, StorageItem item, Direction direction, Vector2[,] occupiedTiles)
            : base(gameLocation, item, direction, occupiedTiles)
        {
            storageItem = item;

            inventory = new Inventory(10, 3, 3);
        }

        public override void InteractAlternatively(Item item, PlayerEnergyManager playerEnergyManager)
        {
            ((GameplayScene)Engine.CurrentScene).OpenStorageInventoryUI(inventory);

            ResourcesManager.GetSoundEffect(storageItem.OpenSoundEffect)?.Play();
        }
    }
}
