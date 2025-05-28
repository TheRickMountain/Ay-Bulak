using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Microsoft.Xna.Framework.Graphics;

namespace palmesneo_village
{
    public class StaticParticleRenderer : IParticleRenderer
    {
        public void Draw(MParticle particle)
        {
            if(particle.IsAlive == false) return;

            particle.Texture?.Draw(
                particle.Position, 
                particle.Texture.Size / 2, 
                particle.Rotation, 
                particle.Scale, 
                particle.Color * particle.Alpha, 
                SpriteEffects.None);
        }
    }
}
