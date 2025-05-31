using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Tweening;
using System;
using System.Collections.Generic;

namespace palmesneo_village
{
    public class FishShadow : SpriteEntity
    {
        public enum FishState
        {
            None,
            Spawning,
            Idle,
            SwimmingForward,
            Playing,
            SwimmingBackward,
            Biting,
            Ate,
            Dispawning
        }

        public Action<FishState> StateChanged { get; set; }

        private FishState currentState = FishState.None;

        private float spawnDistance = 8;
        private readonly Range<float> idleTimeRange = new(1.0f, 2.5f);
        private readonly Range<int> playingAmountRange = new(0, 3);
        private float bitingWindow = 1.0f;

        private float timer = 0.0f;
        private int playingCounter = 0;
        
        private Vector2 spawnPosition;
        private Vector2 dispawnPosition;
        private Vector2 bobberPosition;

        private Tweener tweener;

        public FishShadow()
        {
            MTileset tileset = new MTileset(ResourcesManager.GetTexture("Sprites", "fish_shadow"), 16, 16);

            AddAnimation("swimming", new Animation([tileset[0], tileset[1], tileset[2], tileset[3], tileset[4]], 0, 5));
            Play("swimming");

            Offset = new Vector2(16 - 16 / 3, 16 / 2);
            SelfColor = Color.White * 0.4f;

            tweener = new Tweener();
        }

        public override void Update()
        {
            base.Update();

            tweener.Update(Engine.GameDeltaTime);

            switch (currentState)
            {
                case FishState.Idle:
                    {
                        timer -= Engine.GameDeltaTime;

                        if (timer < 0.0f)
                        {
                            SetState(FishState.SwimmingForward);
                        }
                    }
                    break;
                case FishState.Biting:
                    {
                        timer -= Engine.GameDeltaTime;

                        if(timer < 0.0f)
                        {
                            SetState(FishState.Ate);
                        }
                    }
                    break;
            }
        }

        public void SetBobberPosition(Vector2 bobberPosition)
        {
            this.bobberPosition = bobberPosition;

            Vector2 positionOffset;
            Calc.Random.NextUnitVector(out positionOffset);
            positionOffset *= spawnDistance;
            spawnPosition = bobberPosition + positionOffset;
            dispawnPosition = bobberPosition - positionOffset;

            LocalPosition = spawnPosition;

            Vector2 direction = bobberPosition - LocalPosition;
            direction.Normalize();

            LocalRotation = Calc.Angle(direction);

            SetState(FishState.Spawning);
        }

        public void SetState(FishState newState)
        {
            currentState = newState;

            switch (currentState)
            {
                case FishState.Spawning:
                    {
                        playingCounter = Calc.Random.Range(playingAmountRange);

                        SelfColor = Color.White * 0.0f;

                        tweener.TweenTo(
                            target: this,
                            expression: sprite => SelfColor,
                            toValue: Color.White * 0.4f,
                            duration: 0.5f)
                        .Easing(EasingFunctions.Linear)
                        .OnEnd(tween =>
                        {
                            SetState(FishState.Idle);
                        });
                    }
                    break;
                case FishState.Idle:
                    {
                        timer = Calc.Random.Range(idleTimeRange);
                    }
                    break;
                case FishState.SwimmingForward:
                    {
                        tweener.TweenTo(
                            target: this,
                            expression: sprite => LocalPosition,
                            toValue: bobberPosition,
                            duration: 0.5f)
                        .Easing(EasingFunctions.Linear)
                        .OnEnd(tween =>
                        {
                            if (playingCounter == 0)
                            {
                                SetState(FishState.Biting);
                            }
                            else
                            {
                                playingCounter--;

                                SetState(FishState.Playing);
                            }
                        });
                    }
                    break;
                case FishState.Playing:
                    {
                        SetState(FishState.SwimmingBackward);
                    }
                    break;
                case FishState.SwimmingBackward:
                    {
                        tweener.TweenTo(
                            target: this,
                            expression: sprite => LocalPosition,
                            toValue: spawnPosition,
                            duration: 1.0f)
                        .Easing(EasingFunctions.QuadraticOut)
                        .OnEnd(tween =>
                        {
                            SetState(FishState.Idle);
                        });
                    }
                    break;
                case FishState.Biting:
                    {
                        timer = bitingWindow;
                    }
                    break;
                case FishState.Ate:
                    {
                        SetState(FishState.Dispawning);
                    }
                    break;
                case FishState.Dispawning:
                    {
                        Vector2 direction = dispawnPosition - LocalPosition;
                        direction.Normalize();

                        LocalRotation = Calc.Angle(direction);

                        tweener.TweenTo(
                            target: this,
                            expression: sprite => LocalPosition,
                            toValue: dispawnPosition,
                            duration: 0.5f)
                        .Easing(EasingFunctions.QuadraticOut)
                        .OnEnd(tween =>
                        {
                            Parent.RemoveChild(this);
                        });

                        tweener.TweenTo(
                            target: this,
                            expression: sprite => SelfColor,
                            toValue: Color.White * 0.0f,
                            duration: 0.4f)
                        .Easing(EasingFunctions.QuadraticOut);
                    }
                    break;
            }

            StateChanged?.Invoke(newState);
        }

        public void ScareAway()
        {
            SetState(FishState.Dispawning);
        }
    }

}
