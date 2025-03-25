using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System.Collections.Generic;
using System.Linq;

namespace palmesneo_village
{
    public enum AnimalState
    {
        Idle,
        Moving
    }

    public class Animal : Creature
    {
        private AnimalState animalState = AnimalState.Idle;

        private float idleTime = 0;

        private Vector2 targetTile;

        private Range<float> idleTimeRange = new Range<float>(2.0f, 4.0f);

        public Animal(string name, MTexture texture, float speed) 
            : base(name, texture, speed)
        {
            
        }

        public override void Update()
        {
            switch(animalState)
            {
                case AnimalState.Idle:
                    {
                        UpdateIdle();
                    }
                    break;
                case AnimalState.Moving:
                    {
                        UpdateMovement();
                    }
                    break;
            }

            base.Update();
        }

        private void UpdateIdle()
        {
            idleTime -= Engine.GameDeltaTime;

            if(idleTime <= 0)
            {
                Vector2 newTargetTile = GetRandomTargetTile();

                if (CurrentLocation.IsTilePassable((int)newTargetTile.X, (int)newTargetTile.Y))
                {
                    targetTile = newTargetTile;

                    animalState = AnimalState.Moving;
                }
                else
                {
                    idleTime = Calc.Random.Range(idleTimeRange);
                }
            }
        }

        protected override Direction UpdateMovement()
        {
            Vector2 movement = targetTile - GetTilePosition();
            if (movement == Vector2.Zero)
            {
                animalState = AnimalState.Idle;
                idleTime = Calc.Random.Range(idleTimeRange);
            }
            else
            {
                movement.Normalize();

                LocalPosition = LocalPosition + movement * Speed * Engine.GameDeltaTime;
            }

            return Direction.Down;
        }

        private Vector2 GetRandomTargetTile()
        {
            Vector2 animalTile = GetTilePosition();

            List<Vector2> neighbourTiles = CurrentLocation.GetNeighbourTiles(animalTile, false).ToList();

            if (neighbourTiles.Count == 0)
            {
                return animalTile;
            }

            return Calc.Random.Choose(neighbourTiles);
        }

    }
}
