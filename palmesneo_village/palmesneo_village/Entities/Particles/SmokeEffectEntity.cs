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
    public class SmokeEffectEntity : Entity
    {

        public bool Emitting { get; set; } = true;
        private ParticleEffect particleEffect;
        public SmokeEffectEntity()
        {
            MTexture texture = ResourcesManager.GetTexture("Sprites", "smoke");
            Texture2DRegion textureRegion = new Texture2DRegion(texture.Texture, texture.ClipRect);
            particleEffect = new ParticleEffect();
            particleEffect.Emitters = new List<ParticleEmitter>
            {
                new ParticleEmitter(textureRegion, 1000, TimeSpan.FromSeconds(3f), Profile.Point())
                {
                    AutoTriggerFrequency = 0.5f,
                    Parameters = new ParticleReleaseParameters
                    {
                        Quantity = 1,
                        Scale = new Range<float>(1.0f, 1.0f),
                        Rotation = new Range<float>(MathHelper.ToRadians(-180), MathHelper.ToRadians(180))
                    },
                    Modifiers =
                    {
                        new AgeModifier
                        {
                            Interpolators =
                            {
                                new OpacityInterpolator
                                {
                                    StartValue = 1.0f,
                                    EndValue = 0.0f
                                },
                                new ScaleInterpolator
                                {
                                    StartValue = new Vector2(0.5f, 0.5f),
                                    EndValue = new Vector2(1.7f, 1.7f)
                                },
                                new RotationInterpolator
                                {
                                    StartValue = 0,
                                    EndValue = MathHelper.ToRadians(180)
                                }
                            }
                        },
                        new LinearGravityModifier {Direction = new Vector2(0, -1), Strength = 20}
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

            particleEffect.Position = GlobalPosition;

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
