using Microsoft.Xna.Framework;
using System;

namespace palmesneo_village
{
    public class PlantBuilding : Building
    {
        public bool IsRipe
        {
            get => currentGrowthStage == plantItem.GrowthStages - 1;
        }

        private PlantItem plantItem;

        private float growthProgress = 0.0f;

        private int currentGrowthStage = 0;

        private bool tileWasWatered = false;

        public PlantBuilding(GameLocation gameLocation, PlantItem plantItem, Direction direction, Vector2[,] occupiedTiles) 
            : base(gameLocation, plantItem, direction, occupiedTiles)
        {
            this.plantItem = plantItem;

            Sprite.Texture = plantItem.GrowthStagesTextures[0];
        }

        public override void OnBeforeDayChanged()
        {
            Vector2 plantTile = OccupiedTiles[0, 0];
            GroundTopTile groundTopTile = GameLocation.GetGroundTopTile((int)plantTile.X, (int)plantTile.Y);

            tileWasWatered = groundTopTile == GroundTopTile.Moisture;
        }

        public override void OnAfterDayChanged()
        {
            if (IsRipe) return;

            if (tileWasWatered == false) return;

            float progressPerDay = 1.0f / plantItem.GrowthRateInDays;

            SetGrowthProgress(growthProgress + progressPerDay);
        }

        public void SetGrowthProgress(float value)
        {
            growthProgress = MathHelper.Clamp(value, 0.0f, 1.0f);

            currentGrowthStage = (int)(value * ((plantItem.GrowthStages - 1) / 1.0f));

            Sprite.Texture = plantItem.GrowthStagesTextures[currentGrowthStage];
        }

        public override void Interact(Item item, PlayerEnergyManager playerEnergyManager)
        {
            if (IsRipe)
            {
                Harvest();
            }
            else if(item is ToolItem toolItem)
            {
                if(toolItem.ToolType == ToolType.Axe || toolItem.ToolType == ToolType.Pickaxe)
                {
                    playerEnergyManager.ConsumeEnergy(1);
                    
                    GameLocation.RemoveBuilding(this);
                }
            }
        }

        public override void InteractAlternatively(Item item, PlayerEnergyManager playerEnergyManager)
        {
            if(IsRipe)
            {
                Harvest();
            }
        }

        private void Harvest()
        {
            ItemContainer itemContainer = new ItemContainer();
            itemContainer.Item = Engine.ItemsDatabase.GetItemByName<Item>(plantItem.HarvestItem);
            itemContainer.Quantity = plantItem.HarvestAmount;

            GameLocation.AddItem(OccupiedTiles[0, 0] * Engine.TILE_SIZE, itemContainer);

            if (plantItem.RemoveAfterHarvest)
            {
                GameLocation.RemoveBuilding(this);
            }
            else
            {
                int penultimateGrowthStage = plantItem.GrowthStages - 1;

                float newGrowthProgress = penultimateGrowthStage / (float)plantItem.GrowthStages;

                SetGrowthProgress(newGrowthProgress);
            }
        }
    }
}
