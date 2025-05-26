using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class RainEmitter : ParticlesEntity
    {

        public RainEmitter() : base(100, MParticlePresets.RainParticle(ResourcesManager.GetTexture("Sprites", "rain_drop")))
        {
            SetSpawnShape(new MRectangleShape(Vector2.Zero));
            AddModifier(new AlphaFadeParticleModifier());
        }

    }
}
