using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace palmesneo_village
{
    public static class MParticlePresets
    {
        public static Func<MParticle> SnowParticle(MTexture texture)
        {
            return () => new MParticle
            {
                Texture = texture,
                Velocity = new Vector2(0, 5 + Calc.Random.Range(0f, 10f)),
                Lifetime = 5f,
                Color = Color.White,
                Scale = Calc.Random.Range(0.3f, 0.6f),
                Rotation = 0,
                RotationSpeed = Calc.Random.Range(-0.5f, 0.5f)
            };
        }

        public static Func<MParticle> RainParticle(MTexture texture)
        {
            return () => new MParticle
            {
                Texture = texture,
                Velocity = new Vector2(-10, 200 + Calc.Random.Range(0f, 50f)),
                Lifetime = 1f,
                Color = Color.White,
                Scale = 0.5f,
                Rotation = 0,
                RotationSpeed = 0
            };
        }

        public static Func<MParticle> SmokeParticle(MTexture texture)
        {
            return () => new MParticle
            {
                Texture = texture,
                Velocity = new Vector2(Calc.Random.Range(-10f, 10f), Calc.Random.Range(-30f, -10f)),
                Lifetime = 2f,
                Color = Color.Gray,
                Scale = Calc.Random.Range(0.4f, 0.8f),
                Rotation = Calc.Random.Range(0f, MathF.Tau),
                RotationSpeed = Calc.Random.Range(-0.2f, 0.2f)
            };
        }
    }

}
