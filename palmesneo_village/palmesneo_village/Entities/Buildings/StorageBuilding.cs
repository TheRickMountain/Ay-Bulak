using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class StorageBuilding : Building
    {
        private StorageItem storageItem;

        private Inventory storageInventory;

        public StorageBuilding(GameLocation gameLocation, StorageItem item, Direction direction, Vector2[,] occupiedTiles)
            : base(gameLocation, item, direction, occupiedTiles)
        {
            storageItem = item;

            storageInventory = new Inventory(10, 3, 3);
        }

        public override void Interact(InteractionData interactionData, Inventory inventory)
        {
            switch(interactionData.InteractionType)
            {
                case InteractionType.Open:
                    {
                        ((GameplayScene)Engine.CurrentScene).OpenStorageInventoryUI(storageInventory);

                        ResourcesManager.GetSoundEffect(storageItem.OpenSoundEffect)?.Play();
                    }
                    break;
            }
        }

        public override IEnumerable<InteractionData> GetAvailableInteractions(Inventory inventory)
        {
            yield return new InteractionData(InteractionType.Open,
                ResourcesManager.GetTexture("Sprites", "chest_icon"),
                LocalizationManager.GetText("open_x", LocalizationManager.GetText(storageItem.Name)),
                true);
        }
    }
}
