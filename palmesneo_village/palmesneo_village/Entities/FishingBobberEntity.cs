using Microsoft.Xna.Framework;
using System;

namespace palmesneo_village
{
    public class FishingBobberEntity : SpriteEntity
    {
        public enum BobberState { None, Casting, WaterEntering, Floating };

        public Vector2 StartPosition { get; private set; }
        public BobberState CurrentState { get; private set; } = BobberState.None;

        private Vector2 middle;
        private Vector2 end;
        private float t = 0f;
        private float castDuration = 1f;
        private float bobTime = 0f;
        private float bobAmplitude = 1f;   // высота качания
        private float bobFrequency = 2.5f; // скорость качания

        private MTexture castingTexture;
        private MTexture floatingTexture;

        public FishingBobberEntity()
        {
            MTileset tileset = new MTileset(ResourcesManager.GetTexture("Sprites", "fishing_bobber"), 16, 16);

            AddAnimation("casting", new Animation([tileset[0]], 0, 0));
            AddAnimation("floating", new Animation([tileset[1]], 0, 0));
            AddAnimation("water_entering", new Animation([tileset[2], tileset[3], tileset[4], tileset[5]], 0, 5) { Loop = false });

            castingTexture = tileset[0];
            floatingTexture = tileset[1];

            Centered = true;
        }

        public void BeginCast(Vector2 from, Vector2 to, float height)
        {
            StartPosition = from;
            end = to;
            middle = new Vector2((from.X + to.X) / 2, from.Y - height);

            LocalPosition = from;
            t = 0f;

            SetState(BobberState.Casting);
        }

        public override void Update()
        {
            base.Update();

            switch (CurrentState)
            {
                case BobberState.Casting:
                    {
                        Texture = castingTexture;

                        t += Engine.GameDeltaTime / castDuration;
                        if (t >= 1f)
                        {
                            t = 1f;

                            LocalRotation = 0;
                            LocalPosition = end;

                            SetState(BobberState.WaterEntering);
                            return;
                        }

                        Vector2 previousPosition = LocalPosition;

                        LocalPosition =
                            (1 - t) * (1 - t) * StartPosition +
                            2 * (1 - t) * t * middle +
                            t * t * end;

                        Vector2 velocity = LocalPosition - previousPosition;

                        if (velocity.LengthSquared() > 0.0001f)
                            LocalRotation = (float)Math.Atan2(velocity.Y, velocity.X) - MathHelper.ToRadians(90);
                    }
                    break;
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
                        Texture = floatingTexture;

                        bobTime += Engine.GameDeltaTime;
                        float offsetY = (float)MathF.Sin(bobTime * bobFrequency) * bobAmplitude;
                        LocalPosition = end + new Vector2(0, offsetY);
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
                    {
                        Play("casting");
                    }
                    break;
                case BobberState.WaterEntering:
                    {
                        GetAnimation("water_entering").Reset();
                        Play("water_entering");

                        ResourcesManager.GetSoundEffect("SoundEffects", "bobber_splash")
                        .Play(1.0f, Calc.Random.Range(0.0f, 0.5f), 0.0f);
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
