using Microsoft.Xna.Framework;
using System;

namespace palmesneo_village
{
    public class FishingBobberEntity : SpriteEntity
    {
        public enum BobberState { None, Casting, WaterEntering, Biting, Floating, Completing, Completed };

        public BobberState CurrentState { get; private set; } = BobberState.None;

        private Vector2 startPosition;
        private Vector2 middlePosition;
        private Vector2 endPosition;
        private float t = 0f;
        private float castDuration = 1f;
        private float bobTime = 0f;
        private float bobAmplitude = 1f;
        private float bobFrequency = 2.5f;

        public FishingBobberEntity()
        {
            MTileset tileset = new MTileset(ResourcesManager.GetTexture("Sprites", "fishing_bobber"), 16, 16);

            AddAnimation("casting", new Animation([tileset[0]], 0, 0));
            AddAnimation("floating", new Animation([tileset[1]], 0, 0));
            AddAnimation("biting", new Animation([tileset[2], tileset[3], tileset[4], tileset[5]], 0, 6) { Loop = false });

            Centered = true;
        }

        public void BeginCast(Vector2 from, Vector2 to, float height)
        {
            startPosition = from;
            endPosition = to;
            middlePosition = new Vector2((from.X + to.X) / 2, from.Y - height);

            LocalPosition = from;
            t = 0f;

            SetState(BobberState.Casting);
        }

        public void Complete(Vector2 from, Vector2 to, float height)
        {
            startPosition = from;
            endPosition = to;
            middlePosition = new Vector2((from.X + to.X) / 2, from.Y - height);

            LocalPosition = from;
            t = 0f;

            SetState(BobberState.Completing);
        }

        public void Bite()
        {
            SetState(BobberState.Biting);
        }

        public override void Update()
        {
            base.Update();

            switch (CurrentState)
            {
                case BobberState.Completing:
                case BobberState.Casting:
                    {
                        t += Engine.GameDeltaTime / castDuration;
                        if (t >= 1f)
                        {
                            t = 1f;

                            LocalRotation = 0;
                            LocalPosition = endPosition;

                            if (CurrentState == BobberState.Casting)
                            {
                                SetState(BobberState.WaterEntering);
                            }
                            else if(CurrentState == BobberState.Completing)
                            {
                                SetState(BobberState.Completed);
                            }
                            return;
                        }

                        Vector2 previousPosition = LocalPosition;

                        LocalPosition =
                            (1 - t) * (1 - t) * startPosition +
                            2 * (1 - t) * t * middlePosition +
                            t * t * endPosition;

                        Vector2 velocity = LocalPosition - previousPosition;

                        if (velocity.LengthSquared() > 0.0001f)
                            LocalRotation = (float)Math.Atan2(velocity.Y, velocity.X) - MathHelper.ToRadians(90);
                    }
                    break;
                case BobberState.Biting:
                case BobberState.WaterEntering:
                    {
                        if(CurrentAnimation.IsFinished)
                        {
                            SetState(BobberState.Floating);
                        }
                    }
                    break;
                case BobberState.Floating:
                    {
                        bobTime += Engine.GameDeltaTime;
                        float offsetY = (float)MathF.Sin(bobTime * bobFrequency) * bobAmplitude;
                        LocalPosition = endPosition + new Vector2(0, offsetY);
                    }
                    break;
            }
        }

        private void SetState(BobberState newState)
        {
            CurrentState = newState;

            switch (CurrentState)
            {
                case BobberState.Casting:
                case BobberState.Completing:
                    {
                        Play("casting");
                    }
                    break;
                case BobberState.Biting:
                case BobberState.WaterEntering:
                    {
                        GetAnimation("biting").Reset();
                        Play("biting");
                    }
                    break;
                case BobberState.Floating:
                    {
                        Play("floating");
                    }
                    break;
            }
        }
    }
}
