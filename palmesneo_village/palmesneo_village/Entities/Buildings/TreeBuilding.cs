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
            get => currentGrowthStage == treeItem.GrowthStagesData.Length - 1;
        }

        public int CurrentStrength { get; set; }

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

            currentGrowthStage = (int)(growthProgress * ((treeItem.GrowthStagesData.Length - 1) / 1.0f));

            Sprite.Texture = treeItem.GrowthStagesTextures[currentGrowthStage];

            CurrentStrength = treeItem.GrowthStagesData[currentGrowthStage].Strength;
        }

        public void Chop(AxeItem axeItem)
        {
            CurrentStrength -= axeItem.Efficiency;

            if(CurrentStrength <= 0)
            {
                foreach(var kvp in treeItem.GrowthStagesData[currentGrowthStage].Loot)
                {
                    ItemContainer itemContainer = new ItemContainer();
                    itemContainer.Item= Engine.ItemsDatabase.GetItemByName(kvp.Key);
                    itemContainer.Quantity = kvp.Value;

                    gameLocation.AddItem(OccupiedTiles[0, 0] * Engine.TILE_SIZE, itemContainer);
                }

                gameLocation.RemoveBuilding(this);
            }
        }
    }
}
