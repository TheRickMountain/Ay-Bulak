using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace palmesneo_village
{
    public class MParticle
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float Lifetime;
        public float Age;
        public Color Color;
        public float Scale;
        public float Rotation;
        public float RotationSpeed;
        public MTexture Texture;

        public bool IsAlive => Age < Lifetime;

        public void Update(float deltaTime)
        {
            Age += deltaTime;
            Position += Velocity * deltaTime;
            Rotation += RotationSpeed * deltaTime;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsAlive == false) return;
            
            float alpha = 1f - (Age / Lifetime);

            Texture?.Draw(Position, Texture.Size / 2, Rotation, new Vector2(Scale), Color * alpha, SpriteEffects.None);
        }
    }
}
