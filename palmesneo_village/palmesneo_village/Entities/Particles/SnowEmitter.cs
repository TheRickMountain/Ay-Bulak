using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class SnowEmitter : ParticlesEntity
    {

        public SnowEmitter() : base(100, MParticlePresets.SnowParticle(
            new MTileset(ResourcesManager.GetTexture("Sprites", "snow_flakes"), 16, 16)))
        {
            SetSpawnShape(new MRectangleShape(Vector2.Zero));
            AddModifier(new AlphaFadeParticleModifier());
        }

    }
}
