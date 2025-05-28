using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tweening;
using System;
using System.Collections.Generic;
using System.Resources;

namespace palmesneo_village
{
    public class GrassBuilding : Building
    {
        private const float SHAKE_DURATION = 0.3f;
        private const float SHAKE_ANGLE = 30f;
        private Range<int> grassSpawnAmountRange = new Range<int>(1, 4);

        private GrassItem grassItem;

        private List<ImageEntity> grassSprites = new();

        private Tweener tweener;

        private bool isShaking = false;
        private bool childrenWasAdded = false;

        public GrassBuilding(GameLocation gameLocation, GrassItem item, Direction direction, Vector2[,] occupiedTiles)
            : base(gameLocation, item, direction, occupiedTiles)
        {
            grassItem = item;

            Sprite.IsVisible = false;

            tweener = new Tweener();

            CreateGrassSprites();
        }

        private void CreateGrassSprites()
        {
            for (int i = 0; i < Calc.Random.Range(grassSpawnAmountRange); i++)
            {
                ImageEntity grassSprite = new ImageEntity();
                grassSprite.Texture = grassItem.GetRandomVariationTexture();
                grassSprite.Offset = new Vector2(grassSprite.Texture.Size.X / 2, grassSprite.Texture.Size.Y - 2);
                grassSprite.LocalPosition = new Vector2(Calc.Random.Range(2, 14), Calc.Random.Range(2, 14));
                grassSprites.Add(grassSprite);
            }
        }

        public override void Update()
        {
            base.Update();

            if(Parent != null && childrenWasAdded == false)
            {
                foreach (var grassSprite in grassSprites)
                {
                    grassSprite.LocalPosition += LocalPosition;
                    grassSprite.Depth = (int)grassSprite.LocalPosition.Y;
                    Parent.AddChild(grassSprite);
                }

                childrenWasAdded = true;
            }

            if (isShaking)
            {
                tweener.Update(Engine.GameDeltaTime);

                foreach (var sprite in grassSprites)
                {
                    sprite.LocalRotation = LocalRotation;
                }
            }
        }

        public override void Interact(Inventory inventory, int activeSlotIndex, PlayerEnergyManager playerEnergyManager)
        {
            Item item = inventory.GetSlotItem(activeSlotIndex);

            if (item is ToolItem toolItem && toolItem.ToolType == ToolType.Scythe)
            {
                if (childrenWasAdded)
                {
                    foreach (var sprite in grassSprites)
                    {
                        Parent.RemoveChild(sprite);
                    }
                }

                SpawnLoot();

                toolItem.PlaySoundEffect();

                GameLocation.RemoveBuilding(this);
            }
        }

        private void SpawnLoot()
        {
            ItemContainer itemContainer = new ItemContainer();
            itemContainer.Item = Engine.ItemsDatabase.GetItemByName<Item>("hay");
            itemContainer.Quantity = 1;
            GameLocation.AddItem(OccupiedTiles[0, 0] * Engine.TILE_SIZE, itemContainer);
        }

        public void Shake(Direction shakeDirection)
        {
            if (isShaking) return;

            isShaking = true;

            float angle = 0;

            if (shakeDirection == Direction.Left)
            {
                angle = MathHelper.ToRadians(-SHAKE_ANGLE);
            }
            else if (shakeDirection == Direction.Right)
            {
                angle = MathHelper.ToRadians(SHAKE_ANGLE);
            }
            else if (shakeDirection == Direction.Up || shakeDirection == Direction.Down)
            {
                angle = MathHelper.ToRadians(Calc.Random.Range(-SHAKE_ANGLE, SHAKE_ANGLE));
            }

            tweener.TweenTo(
                        target: this,
                        expression: trunk => LocalRotation,
                        toValue: angle,
                        duration: SHAKE_DURATION)
                        .Easing(EasingFunctions.Linear)
                        .OnEnd((x) =>
                        {
                            tweener.TweenTo(
                                target: this,
                                expression: trunk => LocalRotation,
                                toValue: 0,
                                duration: SHAKE_DURATION)
                                .Easing(EasingFunctions.Linear)
                                .OnEnd((x) =>
                                {
                                    isShaking = false;
                                });
                        });
        }
    }
}
