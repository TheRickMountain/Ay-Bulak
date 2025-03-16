using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public class TreeBuilding : Building
    {
        public bool IsRipe
        {
            get => currentGrowthStage == treeItem.GrowthStages - 1;
        }

        private TreeItem treeItem;

        private float growthProgress = 0.0f;

        private int currentGrowthStage = 0;

        public TreeBuilding(GameLocation gameLocation, TreeItem treeItem, Direction direction, Vector2[,] occupiedTiles)
            : base(gameLocation, treeItem, direction, occupiedTiles)
        {
            this.treeItem = treeItem;

            Sprite.Texture = treeItem.GrowthStagesTextures[0];
        }

        public override void OnDayChanged()
        {
            if (IsRipe) return;

            float progressPerDay = 1.0f / treeItem.GrowthRateInDays;

            growthProgress = MathHelper.Clamp(growthProgress + progressPerDay, 0.0f, 1.0f);

            currentGrowthStage = (int)(growthProgress * ((treeItem.GrowthStages - 1) / 1.0f));

            Sprite.Texture = treeItem.GrowthStagesTextures[currentGrowthStage];
        }
    }
}
