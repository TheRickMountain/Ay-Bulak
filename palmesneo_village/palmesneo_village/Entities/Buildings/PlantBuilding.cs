using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Tweening;
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

        private Tweener tweener;

        private bool isRemoved = false;

        public PlantBuilding(GameLocation gameLocation, PlantItem plantItem, Direction direction, Vector2[,] occupiedTiles) 
            : base(gameLocation, plantItem, direction, occupiedTiles)
        {
            this.plantItem = plantItem;

            Sprite.Texture = plantItem.GrowthStagesTextures[0];
            Sprite.Centered = true;
            Sprite.LocalPosition = new Vector2(Engine.TILE_SIZE / 2, Engine.TILE_SIZE / 2);

            tweener = new Tweener();
        }

        public override void Update()
        {
            base.Update();

            tweener.Update(Engine.GameDeltaTime);
        }

        public override void OnBeforeDayChanged()
        {
            Vector2 plantTile = OccupiedTiles[0, 0];
            GroundTopTile groundTopTile = GameLocation.GetGroundTopTile((int)plantTile.X, (int)plantTile.Y);

            tileWasWatered = groundTopTile == GroundTopTile.Moisture;
        }

        public override void OnAfterDayChanged(TimeOfDayManager timeOfDayManager)
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

        public override void Interact(Inventory inventory, int activeSlotIndex)
        {
            Item item = inventory.GetSlotItem(activeSlotIndex);

            if (IsRipe)
            {
                Harvest();

                GameplayScene.QuestManager.OnHarvestPlant(plantItem.Name);
            }
            else if(item is ToolItem toolItem)
            {
                if(toolItem.ToolType == ToolType.Axe)
                {   
                    GameLocation.RemoveBuilding(this);
                }
            }
        }

        private void Harvest()
        {
            if (isRemoved) return;

            ItemContainer itemContainer = new ItemContainer();
            itemContainer.Item = Engine.ItemsDatabase.GetItemByName<Item>(plantItem.HarvestItem);
            itemContainer.Quantity = plantItem.HarvestAmount;

            GameLocation.AddItem(OccupiedTiles[0, 0] * Engine.TILE_SIZE, itemContainer);

            if (plantItem.RemoveAfterHarvest)
            {
                isRemoved = true;

                tweener.TweenTo(
                    target: Sprite,
                    expression: sprite => Sprite.LocalScale,
                    toValue: new Vector2(0.7f, 1.2f),
                    duration: 0.2f)
                    .Easing(EasingFunctions.Linear)
                    .OnEnd(tween => { GameLocation.RemoveBuilding(this); });

                tweener.TweenTo(
                    target: Sprite,
                    expression: sprite => Sprite.LocalPosition,
                    toValue: Sprite.LocalPosition + new Vector2(0, -10),
                    duration: 0.2f)
                    .Easing(EasingFunctions.CubicIn);
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
