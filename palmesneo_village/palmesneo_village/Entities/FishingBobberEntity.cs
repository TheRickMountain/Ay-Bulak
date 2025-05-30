using Microsoft.Xna.Framework;
using System;

namespace palmesneo_village
{
    public class FishingBobberEntity : ImageEntity
    {
        public enum BobberState { None, Casting, Floating };

        public Vector2 StartPosition { get; private set; }
        public BobberState State { get; private set; } = BobberState.None;

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
            
            State = BobberState.Casting;
        }

        public override void Update()
        {
            base.Update();

            switch (State)
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

                            State = BobberState.Floating;
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

    }
}
