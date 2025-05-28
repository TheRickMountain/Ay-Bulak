using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class RainSplashEmitter : ParticlesEntity
    {

        public RainSplashEmitter() : base(20, () => new MParticle() 
        {
            Texture = null,
            Lifetime = 0.4f,
            Color = Color.White * 0.75f,
        })
        {
            SetSpawnShape(new MRectangleShape(Vector2.Zero));

            MTileset tileset = new MTileset(ResourcesManager.GetTexture("Sprites", "rain_splash"), 16, 16);

            AddModifier(new AnimatedParticleModifier(tileset, 0, 3));
        }

    }
}
