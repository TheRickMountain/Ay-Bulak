using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace palmesneo_village
{
    public class FishShadow : ImageEntity
    {
        // TODO: рыба плывет к цели плавно, скользя по воде

        private enum FishState
        {
            Idle,
            Rotating,
            Swimming,
            Scared,
            Escaping
        }

        private FishState currentState = FishState.Idle;

        private GameLocation currentLocation;

        private readonly Range<float> idleTimeRange = new(2.0f, 5.0f);
        private float idleTimer;

        private Vector2 targetPosition;
        private float rotationSpeed = 4f; // radians per second
        private float defaultSpeed = 10f;
        private float scaredSpeed = 30f;

        private Color originalColor = Color.White * 0.4f;

        private float previousDistance = 0;

        public FishShadow()
        {
            Texture = ResourcesManager.GetTexture("Sprites", "fish_shadow");
            Centered = true;
            SelfColor = originalColor;
        }

        public void SetGameLocation(GameLocation location)
        {
            currentLocation = location;
        }

        public override void Update()
        {
            base.Update();

            switch (currentState)
            {
                case FishState.Idle:
                    UpdateIdle();
                    break;
                case FishState.Rotating:
                    UpdateRotating();
                    break;
                case FishState.Swimming:
                    UpdateSwimming();
                    break;
                case FishState.Scared:
                    UpdateScared();
                    break;
            }
        }

        public Item GetLoot()
        {
            // TODO: получать данные из json файла
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
                Vector2 offset = new Vector2(Calc.Random.Range(0, Engine.TILE_SIZE), Calc.Random.Range(0, Engine.TILE_SIZE));
                SetTarget(GetRandomNeighbourWaterTile() * Engine.TILE_SIZE + offset);
            }
        }

        private void UpdateRotating()
        {
            Vector2 direction = targetPosition - LocalPosition;
            direction.Normalize();

            float targetAngle = Calc.Angle(direction);
            LocalRotation = Calc.RotateTo(LocalRotation, targetAngle, rotationSpeed * Engine.GameDeltaTime);

            float angleDiff = MathF.Abs(MathHelper.WrapAngle(LocalRotation - targetAngle));
            if (angleDiff < 0.05f)
            {
                TransitionToState(FishState.Swimming);
            }
        }

        private void UpdateSwimming()
        {
            Vector2 direction = targetPosition - LocalPosition;
            direction.Normalize();

            LocalPosition += direction * defaultSpeed * Engine.GameDeltaTime;

            float distance = Vector2.Distance(LocalPosition, targetPosition);
            if (distance < 4f)
            {
                idleTimer = Calc.Random.Range(idleTimeRange);
                TransitionToState(FishState.Idle);
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

        public void SetTarget(Vector2 position)
        {
            targetPosition = position;
            TransitionToState(FishState.Rotating);
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
