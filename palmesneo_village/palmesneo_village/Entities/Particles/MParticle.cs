using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace palmesneo_village
{
    public class MParticle
    {
        public Vector2 Position = Vector2.Zero;
        public Vector2 Velocity = Vector2.Zero;
        public float Lifetime = 1.0f;
        public float Age;
        public Color Color = Color.White;
        public Vector2 Scale = Vector2.One;
        public float Rotation = 0;
        public float RotationSpeed = 0;
        public MTexture Texture;
        public float Alpha = 1.0f;

        public bool IsAlive => Age < Lifetime;

        public void Update(float deltaTime)
        {
            Age += deltaTime;
            Position += Velocity * deltaTime;
            Rotation += RotationSpeed * deltaTime;
        }
    }
}
