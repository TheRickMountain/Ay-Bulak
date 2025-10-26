using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace palmesneo_village
{
    public enum AnimalState
    {
        Idle,
        Moving
    }

    public abstract class Animal : Creature
    {
        public bool IsFed { get; set; } = false;

        private AnimalItem animalItem;

        private AnimalState animalState = AnimalState.Idle;

        private float idleTime = 0;

        private float[] idleTimeSet = { 0.0f, 1.0f, 2.0f, 3.0f };

        private CreatureMovement _movement;

        private SoundEffect[] soundEffects;

        private float soundEffectTimer = 0;

        private Range<float> soundEffectTimeRange = new Range<float>(5.0f, 20.0f);

        public Animal(AnimalItem animalItem)
            : base(animalItem.Name, null, animalItem.MovementSpeed)
        {
            this.animalItem = animalItem;

            _movement = new CreatureMovement(animalItem.MovementSpeed);
            AddChild(_movement);

            ImageEntity shadow = new ImageEntity();
            shadow.Texture = animalItem.ShadowSprite;
            AddChild(shadow);

            CreatureBodyVisual visual = new CreatureBodyVisual(animalItem.BodySprite);
            AddChild(visual);

            CreatureAnimation animation = new CreatureAnimation(_movement, visual);
            AddChild(animation);

            // TODO: Удалить BodyImage в будущем, так как будет заменено CreatureVisual
            RemoveChild(BodyImage);

            InitializeSoundEffects();
        }

        private void InitializeSoundEffects()
        {
            soundEffects = new SoundEffect[animalItem.SoundEffects.Length];

            for (int i = 0; i < animalItem.SoundEffects.Length; i++)
            {
                string soundEffectPath = animalItem.SoundEffects[i];

                soundEffects[i] = ResourcesManager.GetSoundEffect(soundEffectPath);
            }

            soundEffectTimer = Calc.Random.Range(soundEffectTimeRange);
        }

        public override void Update()
        {
            switch (animalState)
            {
                case AnimalState.Idle:
                    {
                        UpdateIdle();
                    }
                    break;
                case AnimalState.Moving:
                    {
                        if(_movement.State == MovementState.Success)
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
                Calc.Random.ChooseOrDefault(null, soundEffects)?.Play();

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

                    _movement.SetPath(CurrentLocation.FindPath(currentTile, targetTile, false));

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
            _movement.TeleportTo(pathNode);
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
