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

        public PlantBuilding(PlantItem plantItem, Direction direction, Vector2[,] occupiedTiles) 
            : base(plantItem, direction, occupiedTiles)
        {
            this.plantItem = plantItem;

            Sprite.Texture = plantItem.GrowthStagesTextures[0];
        }

        public override void OnDayChanged(int day)
        {
            if (IsRipe) return;

            float progressPerDay = 1.0f / plantItem.GrowthRateInDays;

            growthProgress = MathHelper.Clamp(growthProgress + progressPerDay, 0.0f, 1.0f);

            currentGrowthStage = (int)(growthProgress * ((plantItem.GrowthStages - 1) / 1.0f));

            Sprite.Texture = plantItem.GrowthStagesTextures[currentGrowthStage];
        }

    }
}
