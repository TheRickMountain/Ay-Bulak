using Cyotek.Drawing.BitmapFont;
using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class AlphaFadeParticleModifier : IParticleModifier
    {
        public AlphaFadeParticleModifier()
        {
            
        }

        public void Update(MParticle particle, float deltaTime)
        {
            particle.Alpha = 1f - (particle.Age / particle.Lifetime);
        }
    }
}
