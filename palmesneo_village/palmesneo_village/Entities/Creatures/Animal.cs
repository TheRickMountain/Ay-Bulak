using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
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

        private Dictionary<Direction, MTexture> directionTextures = new();

        private Direction movementDirection = Direction.Down;

        private float[] idleTimeSet = new float[] { 0.0f, 1.0f, 2.0f, 3.0f };

        public Animal(string name, MTexture texture, float speed) 
            : base(name, texture, speed)
        {
            int directionTextureWidth = texture.Width / 4;
            int directionTextureHeight = texture.Height;

            foreach(Direction direction in Enum.GetValues<Direction>())
            {
                MTexture directionTexture = new MTexture(texture, new Rectangle(
                    directionTextureWidth * (int)direction, 
                    0, 
                    directionTextureWidth, 
                    directionTextureHeight));

                directionTextures.Add(direction, directionTexture);
            }

            BodyImage.Texture = directionTextures[movementDirection];
        }

        public override void Update()
        {
            BodyImage.Texture = directionTextures[movementDirection];

            switch (animalState)
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
                    idleTime = Calc.Random.Choose(idleTimeSet);
                }
            }
        }

        private void UpdateMovement()
        {
            Vector2 movement = targetTile - GetTilePosition();
            if (movement == Vector2.Zero)
            {
                animalState = AnimalState.Idle;
                idleTime = Calc.Random.Choose(idleTimeSet);
            }
            else
            {
                movement.Normalize();

                LocalPosition = LocalPosition + movement * Speed * Engine.GameDeltaTime;
            }

            if (movement.X > 0)
            {
                movementDirection = Direction.Right;
            }
            else if (movement.X < 0)
            {
                movementDirection = Direction.Left;
            }
            else if (movement.Y > 0)
            {
                movementDirection = Direction.Down;
            }
            else if (movement.Y < 0)
            {
                movementDirection = Direction.Up;
            }
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
