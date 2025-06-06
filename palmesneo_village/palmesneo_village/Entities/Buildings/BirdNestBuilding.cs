﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace palmesneo_village
{
    public class BirdNestBuilding : Building
    {
        private BirdNestItem birdNestItem;

        private int currentStage;

        public BirdNestBuilding(GameLocation gameLocation, BirdNestItem item, Direction direction, Vector2[,] occupiedTiles) 
            : base(gameLocation, item, direction, occupiedTiles)
        {
            birdNestItem = item;
        }

        public override void Interact(Inventory inventory, int activeSlotIndex, PlayerEnergyManager playerEnergyManager)
        {
            Gather();

            Reset();
        }

        public void Upgrade()
        {
            currentStage++;
            
            if (currentStage >= birdNestItem.Stages)
            {
                currentStage = birdNestItem.Stages - 1;
            }

            Sprite.Texture = birdNestItem.GetStageTexture(currentStage);
        }

        private void Gather()
        {
            if(currentStage == 0)
            {
                return;
            }

            ItemContainer itemContainer = new ItemContainer();
            itemContainer.Item = Engine.ItemsDatabase.GetItemByName<Item>("chicken_egg");
            itemContainer.Quantity = currentStage;
            GameLocation.AddItem(OccupiedTiles[0, 0] * Engine.TILE_SIZE, itemContainer);
        }

        private void Reset()
        {
            currentStage = 0;
            Sprite.Texture = birdNestItem.GetStageTexture(currentStage);
        }
    }
}
