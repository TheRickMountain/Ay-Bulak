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
    public class RainEffectEntity : Entity
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

        public RainEffectEntity()
        {
            MTexture texture = ResourcesManager.GetTexture("Sprites", "rain_drop");
            Texture2DRegion textureRegion = new Texture2DRegion(texture.Texture, texture.ClipRect);

            particleEffect = new ParticleEffect();

            particleEffect.Emitters = new List<ParticleEmitter>
            {
                new ParticleEmitter(textureRegion, 1000, TimeSpan.FromSeconds(3), Profile.Line(new Vector2(1, 0), 100))
                {
                    AutoTriggerFrequency = 0.05f,
                    Parameters = new ParticleReleaseParameters
                    {
                        Quantity = 10,
                        Scale = new Range<float>(1.0f, 1.0f),
                        Rotation = new Range<float>(0, 0)
                    },
                    Modifiers =
                    {
                        new AgeModifier
                        {
                            Interpolators =
                            {
                                new ColorInterpolator
                                {
                                    StartValue = HslColor.FromRgb(new Color(1.0f, 1.0f, 1.0f) * 0.75f),
                                    EndValue = HslColor.FromRgb(new Color(1.0f, 1.0f, 1.0f))
                                }
                            }
                        },
                        new LinearGravityModifier {Direction = new Vector2(-0.2f, 1), Strength = 1000f}
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
