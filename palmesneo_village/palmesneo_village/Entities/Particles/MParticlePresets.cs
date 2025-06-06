﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using System;

namespace palmesneo_village
{
    public static class MParticlePresets
    {
        public static Func<MParticle> SnowParticle(MTileset tileset)
        {
            return () => new MParticle
            {
                Texture = tileset[Calc.Random.Range(0, tileset.Columns), Calc.Random.Range(0, tileset.Rows)],
                Velocity = new Vector2(0, 5 + Calc.Random.Range(0f, 10f)),
                Lifetime = 10f,
                Color = Color.White,
                Scale = new Vector2(1.0f, 1.0f),
                Rotation = 0,
                RotationSpeed = 0
            };
        }

        public static Func<MParticle> RainParticle(MTexture texture)
        {
            return () => new MParticle
            {
                Texture = texture,
                Velocity = new Vector2(-50, 200 + Calc.Random.Range(0f, 50f)),
                Lifetime = 1.5f,
                Color = Color.White,
                Scale = new Vector2(1f, 1f),
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
                Color = Color.White,
                Scale = new Vector2(Calc.Random.Range(0.8f, 1.0f)),
                Rotation = Calc.Random.Range(0f, MathF.Tau),
                RotationSpeed = Calc.Random.Range(-0.2f, 0.2f)
            };
        }
    }

}
