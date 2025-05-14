using Microsoft.Xna.Framework;
using MonoGame.Extended.Tweening;
using System;

namespace palmesneo_village
{
    public class TreeBuilding : Building
    {
        public bool IsRipe
        {
            get => currentGrowthStage == treeItem.GrowthStagesData.Length - 1;
        }

        private TreeItem treeItem;
        private int currentStrength;

        private float growthProgress = 0.0f;

        private int currentGrowthStage = 0;

        private ImageEntity trunkImage;
        private ImageEntity shadowImage;

        private Tweener trunkAnimationTweener;
        private Tweener fadeTweener;

        private bool isFalling = false;

        public TreeBuilding(GameLocation gameLocation, TreeItem treeItem, Direction direction, Vector2[,] occupiedTiles)
            : base(gameLocation, treeItem, direction, occupiedTiles)
        {
            this.treeItem = treeItem;

            Sprite.Texture = treeItem.GrowthStagesTextures[0];

            trunkImage = new ImageEntity();
            trunkImage.Texture = treeItem.TrunkTexture;
            trunkImage.Centered = true;
            // TODO: продумать как это вычислить автоматически
            trunkImage.Offset = new Vector2(0, 30);
            trunkImage.LocalPosition = new Vector2(8, 14);

            shadowImage = new ImageEntity();
            shadowImage.Texture = ResourcesManager.GetTexture("Sprites", "big_tree_shadow");
            shadowImage.Centered = true;
            // TODO: продумать как это вычислить автоматически
            shadowImage.Offset = new Vector2(0, 30);
            shadowImage.LocalPosition = new Vector2(8, 14);

            trunkAnimationTweener = new Tweener();
            fadeTweener = new Tweener();
        }

        public override void Update()
        {
            trunkAnimationTweener.Update(Engine.GameDeltaTime);
            fadeTweener.Update(Engine.GameDeltaTime);

            base.Update();
        }

        public override void OnAfterDayChanged()
        {
            if (IsRipe) return;

            float progressPerDay = 1.0f / treeItem.GrowthRateInDays;

            SetGrowthProgress(growthProgress + progressPerDay);
        }

        public void SetGrowthProgress(float value)
        {
            growthProgress = MathHelper.Clamp(value, 0.0f, 1.0f);

            currentGrowthStage = (int)(growthProgress * ((treeItem.GrowthStagesData.Length - 1) / 1.0f));

            Sprite.Texture = treeItem.GrowthStagesTextures[currentGrowthStage];

            currentStrength = treeItem.GrowthStagesData[currentGrowthStage].Strength;

            if (IsRipe)
            {
                AddChild(shadowImage);
                AddChild(trunkImage);
            }
        }

        public override void Interact(Inventory inventory, int activeSlotIndex, PlayerEnergyManager playerEnergyManager)
        {
            Item item = inventory.GetSlotItem(activeSlotIndex);

            if (isFalling) return;

            if(item is ToolItem toolItem)
            {
                if (toolItem.ToolType == ToolType.Axe)
                {
                    toolItem.PlaySoundEffect();

                    playerEnergyManager.ConsumeEnergy(1);

                    currentStrength -= toolItem.Efficiency;

                    // Ствол срублен
                    if (currentStrength <= 0)
                    {
                        if (IsRipe)
                        {
                            ResourcesManager.GetSoundEffect("SoundEffects", "tree_falling").Play();

                            PlayTrunkFallingAnimation((x) =>
                            {
                                SpawnLootForStage(currentGrowthStage);

                                GameLocation.RemoveBuilding(this);

                                BuildTreeStump();
                            });

                            isFalling = true;
                        }
                        else
                        {
                            SpawnLootForStage(currentGrowthStage);

                            GameLocation.RemoveBuilding(this);
                        }
                    }
                    else
                    {
                        // Только зрелое дерево может покачиваться
                        if (IsRipe)
                        {
                            ResourcesManager.GetSoundEffect("SoundEffects", "tree_shaking").Play();

                            PlayTrunkShakeAnimation();
                        }
                    }
                }
            }
        }

        private void PlayTrunkShakeAnimation()
        {
            trunkAnimationTweener.TweenTo(
                    target: trunkImage,
                    expression: trunk => trunkImage.LocalRotation,
                    toValue: 0.04f,
                    duration: 0.06f)
                    .Easing(EasingFunctions.CubicOut)
                    .OnEnd(tween1 =>
                    {
                        trunkAnimationTweener.TweenTo(
                            target: trunkImage,
                            expression: trunk => trunkImage.LocalRotation,
                            toValue: -0.03f,
                            duration: 0.08f)
                            .Easing(EasingFunctions.Linear)
                            .OnEnd(tween1 =>
                            {
                                trunkAnimationTweener.TweenTo(
                                    target: trunkImage,
                                    expression: trunk => trunkImage.LocalRotation,
                                    toValue: 0,
                                    duration: 0.08f)
                                    .Easing(EasingFunctions.Linear);
                            });
                    });
        }

        private void PlayTrunkFallingAnimation(Action<Tween> onEnd)
        {
            trunkAnimationTweener.CancelAndCompleteAll();

            trunkAnimationTweener.TweenTo(
                    target: trunkImage,
                    expression: trunk => trunkImage.LocalRotation,
                    toValue: 1.5f,
                    duration: 2)
                    .Easing(EasingFunctions.CubicIn)
                    .OnEnd(onEnd);

            trunkAnimationTweener.TweenTo(
                target: shadowImage,
                expression: shadow => shadowImage.SelfColor,
                toValue: Color.Transparent,
                duration: 1.5f)
                .Easing(EasingFunctions.CubicIn);

            fadeTweener.TweenTo(
                target: trunkImage,
                expression: trunk => trunkImage.SelfColor,
                toValue: Color.White * 0,
                duration: 0.5f,
                delay: 1.5f)
                .Easing(EasingFunctions.Linear);
        }

        private void SpawnLootForStage(int growthStage)
        {
            foreach (var kvp in treeItem.GrowthStagesData[growthStage].Loot)
            {
                ItemContainer itemContainer = new ItemContainer();
                itemContainer.Item = Engine.ItemsDatabase.GetItemByName(kvp.Key);
                itemContainer.Quantity = kvp.Value;

                GameLocation.AddItem(OccupiedTiles[0, 0] * Engine.TILE_SIZE, itemContainer);
            }
        }
    
        private void BuildTreeStump()
        {
            Vector2 position = OccupiedTiles[0, 0];

            ResourceItem stumpItem = Engine.ItemsDatabase.GetItemByName<ResourceItem>(treeItem.StumpName);

            GameLocation.TryBuild(stumpItem, (int)position.X, (int)position.Y, Direction.Down);
        }
    }
}
