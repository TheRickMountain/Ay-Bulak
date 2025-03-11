using Microsoft.Xna.Framework;

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

        public PlantBuilding(GameLocation gameLocation, PlantItem plantItem, Direction direction, Vector2[,] occupiedTiles) 
            : base(gameLocation, plantItem, direction, occupiedTiles)
        {
            this.plantItem = plantItem;

            Sprite.Texture = plantItem.GrowthStagesTextures[0];
        }

        public override void OnDayChanged()
        {
            if (IsRipe) return;

            float progressPerDay = 1.0f / plantItem.GrowthRateInDays;

            growthProgress = MathHelper.Clamp(growthProgress + progressPerDay, 0.0f, 1.0f);

            currentGrowthStage = (int)(growthProgress * ((plantItem.GrowthStages - 1) / 1.0f));

            Sprite.Texture = plantItem.GrowthStagesTextures[currentGrowthStage];
        }

        public void Harvest()
        {
            if(IsRipe == false) return;

            Item harvestItem = Engine.ItemsDatabase.GetItemByName<Item>(plantItem.HarvestItem);

            ItemContainer itemContainer = new ItemContainer();
            itemContainer.Item = harvestItem;
            itemContainer.Quantity = plantItem.HarvestAmount;

            gameLocation.AddItem(OccupiedTiles[0, 0] * Engine.TILE_SIZE, itemContainer);

            if (plantItem.RemoveAfterHarvest)
            {
                gameLocation.RemoveBuilding(this);
            }
        }

    }
}
