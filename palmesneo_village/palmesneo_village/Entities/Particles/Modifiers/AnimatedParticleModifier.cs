using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace palmesneo_village
{
    public class AnimatedParticleModifier : IParticleModifier
    {
        private readonly MTexture[] frames;

        public AnimatedParticleModifier(MTileset tileset, int startFrameIndex, int endFrameIndex)
        {
            int frameCount = (endFrameIndex - startFrameIndex) + 1;
            frames = new MTexture[frameCount];

            for (int i = 0; i < frameCount; i++)
            {
                frames[i] = tileset[startFrameIndex + i];
            }
        }

        public void Update(MParticle particle, float deltaTime)
        {
            float percent = particle.Age / particle.Lifetime;
            int frameIndex = (int)(percent * frames.Length);

            frameIndex = MathHelper.Clamp(frameIndex, 0, frames.Length - 1);

            particle.Texture = frames[frameIndex];
        }
    }
}
