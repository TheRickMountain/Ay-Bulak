using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
        public bool IsFed { get; set; } = false;

        private AnimalItem animalItem;

        private AnimalState animalState = AnimalState.Idle;

        private float idleTime = 0;

        private Direction movementDirection = Direction.Down;

        private float[] idleTimeSet = { 0.0f, 1.0f, 2.0f, 3.0f };

        private CreatureMovement creatureMovement;

        private SoundEffect[] soundEffects;

        private float soundEffectTimer = 0;

        private Range<float> soundEffectTimeRange = new Range<float>(5.0f, 20.0f);

        public Animal(AnimalItem animalItem) 
            : base(animalItem.Name, null, animalItem.MovementSpeed)
        {
            this.animalItem = animalItem;

            // TODO: заменить на анимированный спрайт
            BodyImage.Texture = animalItem.DirectionTexture[movementDirection];

            BodyImage.Centered = true;
            BodyImage.Offset = new Vector2(0, animalItem.DirectionTexture[movementDirection].Height / 2 - Engine.TILE_SIZE / 2);
            BodyImage.LocalPosition = new Vector2(Engine.TILE_SIZE / 2, Engine.TILE_SIZE / 2);

            AddChild(creatureMovement = new CreatureMovement(animalItem.MovementSpeed));

            // TODO: Каждое животное должно иметь свою звуквовое сопровождение
            soundEffects = new SoundEffect[5];
            soundEffects[0] = ResourcesManager.GetSoundEffect("SoundEffects", "Chicken", "chicken_1");
            soundEffects[1] = ResourcesManager.GetSoundEffect("SoundEffects", "Chicken", "chicken_2");
            soundEffects[2] = ResourcesManager.GetSoundEffect("SoundEffects", "Chicken", "chicken_3");
            soundEffects[3] = ResourcesManager.GetSoundEffect("SoundEffects", "Chicken", "chicken_4");
            soundEffects[4] = ResourcesManager.GetSoundEffect("SoundEffects", "Chicken", "chicken_5");

            soundEffectTimer = Calc.Random.Range(soundEffectTimeRange);
        }

        public override void Update()
        {
            BodyImage.Texture = animalItem.DirectionTexture[movementDirection];

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

            soundEffectTimer -= Engine.GameDeltaTime;

            if(soundEffectTimer <= 0)
            {
                soundEffects[Calc.Random.Next(soundEffects.Length)].Play();

                soundEffectTimer = Calc.Random.Range(soundEffectTimeRange);
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

                    movementDirection = GetMovementDirection(currentTile, targetTile);

                    creatureMovement.SetPath(CurrentLocation.FindPath(currentTile, targetTile, false));

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

        private Direction GetMovementDirection(Vector2 startTile, Vector2 finishTile)
        {
            if (finishTile.X < startTile.X)
            {
                return Direction.Left;
            }
            
            if (finishTile.X > startTile.X)
            {
                return Direction.Right;
            }
            
            if (finishTile.Y < startTile.Y)
            {
                return Direction.Up;
            }
            
            if (finishTile.Y > startTile.Y)
            {
                return Direction.Down;
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
