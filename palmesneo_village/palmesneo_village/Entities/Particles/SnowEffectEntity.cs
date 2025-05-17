using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;
using System;
using System.Collections.Generic;

namespace palmesneo_village
{
    public class SnowEffectEntity : Entity
    {
        public bool Emitting { get; set; } = true;

        public float EmitterLineLength
        {
            get => ((LineProfile)particleEffect.Emitters[0].Profile).Length;
            set
            {
                ((LineProfile)particleEffect.Emitters[0].Profile).Length = value;
            }
        }

        private ParticleEffect particleEffect;

        public SnowEffectEntity()
        {
            MTexture texture = ResourcesManager.GetTexture("Sprites", "snow_flake");
            Texture2DRegion textureRegion = new Texture2DRegion(texture.Texture, texture.ClipRect);

            particleEffect = new ParticleEffect();

            particleEffect.Emitters = new List<ParticleEmitter>
            {
                new ParticleEmitter(textureRegion, 1000, TimeSpan.FromSeconds(6), Profile.Line(new Vector2(1, 0), 100))
                {
                    AutoTriggerFrequency = 0.05f,
                    Parameters = new ParticleReleaseParameters
                    {
                        Quantity = 10,
                        Scale = new Range<float>(1.0f, 1.0f),
                        Rotation = new Range<float>(0, 0),
                        Color = HslColor.FromRgb(new Color(1.0f, 1.0f, 1.0f))
                    },
                    Modifiers =
                    {
                        new AgeModifier
                        {
                            Interpolators =
                            {
                                new ColorInterpolator
                                {
                                    StartValue = HslColor.FromRgb(new Color(1.0f, 1.0f, 1.0f)),
                                    EndValue = HslColor.FromRgb(new Color(1.0f, 1.0f, 1.0f))
                                }
                            }
                        },
                        new LinearGravityModifier {Direction = new Vector2(0, 1), Strength = 10f}
                    }
                }
            };
        }

        public override void Update()
        {
            if (Emitting)
            {
                particleEffect.Update(Engine.GameDeltaTime);
            }

            particleEffect.Position = LocalPosition;

            base.Update();
        }

        public override void Render()
        {
            if (Emitting)
            {
                RenderManager.SpriteBatch.Draw(particleEffect);
            }

            base.Render();
        }
    }
}
