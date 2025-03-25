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

        private Dictionary<Direction, MTexture> directionTextures = new();

        private Direction movementDirection = Direction.Down;

        private float[] idleTimeSet = { 0.0f, 1.0f, 2.0f, 3.0f };

        private CreatureMovement creatureMovement;

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

            BodyImage.Centered = true;
            BodyImage.Offset = new Vector2(0, directionTextureHeight / 2 - Engine.TILE_SIZE / 2);
            BodyImage.LocalPosition = new Vector2(Engine.TILE_SIZE / 2, Engine.TILE_SIZE / 2);

            AddChild(creatureMovement = new CreatureMovement(speed));
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
                        if(creatureMovement.State == MovementState.Success)
                        {
                            animalState = AnimalState.Idle;
                            idleTime = Calc.Random.Choose(idleTimeSet);
                        }
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
                Vector2 targetTile = GetRandomTargetTile();

                if (CurrentLocation.IsTilePassable((int)targetTile.X, (int)targetTile.Y))
                {
                    Vector2 currentTile = GetTilePosition();

                    creatureMovement.SetPath(CurrentLocation.FindPath(currentTile, targetTile, false));

                    // Set the direction of the animal
                    if (targetTile.X < currentTile.X)
                    {
                        movementDirection = Direction.Left;
                    }
                    else if (targetTile.X > currentTile.X)
                    {
                        movementDirection = Direction.Right;
                    }
                    else if (targetTile.Y < currentTile.Y)
                    {
                        movementDirection = Direction.Up;
                    }
                    else if (targetTile.Y > currentTile.Y)
                    {
                        movementDirection = Direction.Down;
                    }

                    animalState = AnimalState.Moving;
                }
                else
                {
                    idleTime = Calc.Random.Choose(idleTimeSet);
                }
            }
        }

        public override void SetTilePosition(Vector2 tile)
        {
            PathNode pathNode = CurrentLocation.GetPathNode((int)tile.X, (int)tile.Y);
            creatureMovement.TeleportTo(pathNode);
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
