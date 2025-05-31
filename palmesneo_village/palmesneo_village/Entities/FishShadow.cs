using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Tweening;
using System;
using System.Collections.Generic;

namespace palmesneo_village
{
    public class FishShadow : SpriteEntity
    {
        // TODO: получать данные лута  из json файла

        private enum FishState
        {
            Idle,
            RotatingAndSwimming,
            AttractedRotating,
            AttractedSwimming,
            Scared
        }

        private FishState currentState = FishState.Idle;

        public bool IsAttracted { get; private set; }
        public bool IsHooked { get; private set;}

        private GameLocation currentLocation;

        private readonly Range<float> idleTimeRange = new(2.0f, 5.0f);
        private float idleTimer;

        private Vector2 targetPosition;
        private float rotationSpeed = 4f;
        private float defaultSpeed = 10f;
        private float scaredSpeed = 40f;

        private Color originalColor = Color.White * 0.4f;

        private float previousDistance = 0;

        private Tweener tweener;

        public FishShadow()
        {
            MTileset tileset = new MTileset(ResourcesManager.GetTexture("Sprites", "fish_shadow"), 16, 16);

            AddAnimation("swimming", new Animation([tileset[0], tileset[1], tileset[2], tileset[3], tileset[4]], 0, 5));
            Play("swimming");

            Offset = new Vector2(16 - 16 / 3, 16 / 2);
            SelfColor = originalColor;

            tweener = new Tweener();

            TransitionToState(FishState.Idle);
        }

        public void SetGameLocation(GameLocation location)
        {
            currentLocation = location;
        }

        public override void Update()
        {
            base.Update();

            tweener.Update(Engine.GameDeltaTime);

            switch (currentState)
            {
                case FishState.Idle:
                    UpdateIdle();
                    break;
                case FishState.AttractedRotating:
                    UpdateAttractedRotating();
                    break;
                case FishState.AttractedSwimming:
                    UpdateAttractedSwimming();
                    break;
                case FishState.Scared:
                    UpdateScared();
                    break;
            }
        }

        public Item GetLoot()
        {
            List<Item> loot =
            [
                Engine.ItemsDatabase.GetItemByName("prussian_carp"),
                Engine.ItemsDatabase.GetItemByName("common_bream"),
            ];
            
            return Calc.Random.Choose(loot);
        }

        private void UpdateIdle()
        {
            idleTimer -= Engine.GameDeltaTime;
            if (idleTimer <= 0f)
            {
                TransitionToState(FishState.RotatingAndSwimming);
            }
        }

        private void UpdateAttractedRotating()
        {
            Vector2 direction = targetPosition - LocalPosition;
            direction.Normalize();

            float targetAngle = Calc.Angle(direction);
            LocalRotation = Calc.RotateTo(LocalRotation, targetAngle, rotationSpeed * Engine.GameDeltaTime);

            float angleDiff = MathF.Abs(MathHelper.WrapAngle(LocalRotation - targetAngle));
            if (angleDiff < 0.05f)
            {
                TransitionToState(FishState.AttractedSwimming);
            }
        }

        private void UpdateAttractedSwimming()
        {
            float distance = Vector2.Distance(LocalPosition, targetPosition);

            if (distance > 4f)
            {
                Vector2 direction = targetPosition - LocalPosition;
                direction.Normalize();

                LocalPosition += direction * defaultSpeed * Engine.GameDeltaTime;
            }
            else
            {
                IsHooked = true;
            }
        }

        private void UpdateScared()
        {
            Vector2 direction = targetPosition - LocalPosition;
            direction.Normalize();

            LocalPosition += direction * scaredSpeed * Engine.GameDeltaTime;

            float distance = Vector2.Distance(LocalPosition, targetPosition);

            SelfColor = Color.Lerp(originalColor, Color.Transparent, 1.0f - (distance / previousDistance));

            if (distance < 1f)
            {
                currentLocation.RemoveFish(this);
            }
        }

        public void ScareAway()
        {
            if (currentState == FishState.Scared) return;

            TransitionToState(FishState.Scared);
        }

        private void TransitionToState(FishState newState)
        {
            currentState = newState;

            switch (newState)
            {
                case FishState.RotatingAndSwimming:
                    {
                        Vector2 offset = new Vector2(Calc.Random.Range(0, Engine.TILE_SIZE), Calc.Random.Range(0, Engine.TILE_SIZE));
                        targetPosition = GetRandomNeighbourWaterTile() * Engine.TILE_SIZE + offset;

                        Vector2 direction = targetPosition - LocalPosition;
                        direction.Normalize();

                        float targetAngle = Calc.Angle(direction);

                        var tween = tweener.TweenTo(
                            target: this,
                            expression: sprite => LocalRotation,
                            toValue: targetAngle,
                            duration: 1.0f)
                        .Easing(EasingFunctions.CubicOut);

                        tweener.TweenTo(
                            target: this,
                            expression: sprite => LocalPosition,
                            toValue: targetPosition,
                            duration: 2.0f)
                        .Easing(EasingFunctions.QuadraticOut)
                        .OnEnd((tween) =>
                        {
                            idleTimer = Calc.Random.Range(idleTimeRange);
                            TransitionToState(FishState.Idle);
                        });
                    }
                    break;
                case FishState.AttractedRotating:
                    {
                        tweener.CancelAll();
                    }
                    break;
                case FishState.Scared:
                    {
                        Vector2 offset = new Vector2(Calc.Random.Range(0, Engine.TILE_SIZE), Calc.Random.Range(0, Engine.TILE_SIZE));
                        targetPosition = GetRandomNeighbourWaterTile() * Engine.TILE_SIZE + offset;

                        Vector2 direction = targetPosition - LocalPosition;
                        direction.Normalize();

                        LocalRotation = Calc.Angle(direction);

                        previousDistance = Vector2.Distance(LocalPosition, targetPosition);
                    }
                    break;
            }
        }

        public void TryToAttract(Vector2 bobberPosition, float attractionRadius)
        {
            if (IsAttracted) return;

            if (currentState == FishState.Scared) return;

            float distance = Vector2.Distance(LocalPosition, bobberPosition);

            if (distance <= attractionRadius)
            {
                // Проверяем, смотрит ли рыба в сторону приманки (конус обзора)
                Vector2 directionToBobber = (bobberPosition - LocalPosition);
                directionToBobber.Normalize();

                Vector2 fishDirection = new Vector2(MathF.Cos(LocalRotation), MathF.Sin(LocalRotation));

                float dotProduct = Vector2.Dot(fishDirection, directionToBobber);
                float viewAngle = MathF.Acos(MathHelper.Clamp(dotProduct, -1f, 1f));

                // Рыба видит приманку в конусе 120 градусов
                if (viewAngle <= MathHelper.ToRadians(60f))
                {
                    targetPosition = bobberPosition;

                    IsAttracted = true;

                    TransitionToState(FishState.AttractedRotating);
                }
            }
        }

        private Vector2 GetRandomNeighbourWaterTile()
        {
            var neighbors = new List<Vector2>();

            Vector2 fishTile = currentLocation.WorldToMap(LocalPosition);
            int tileX = (int)fishTile.X;
            int tileY = (int)fishTile.Y;

            void TryAdd(int x, int y)
            {
                if (currentLocation.GetGroundTile(x, y) == GroundTile.Water)
                    neighbors.Add(new Vector2(x, y));
            }

            TryAdd(tileX, tileY);
            TryAdd(tileX - 1, tileY);
            TryAdd(tileX + 1, tileY);
            TryAdd(tileX, tileY - 1);
            TryAdd(tileX, tileY + 1);

            return Calc.Random.Choose(neighbors);
        }
    }

}
