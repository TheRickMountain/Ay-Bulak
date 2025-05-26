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
        public Vector2 Scale;
        public float Rotation;
        public float RotationSpeed;
        public MTexture Texture;
        public float Alpha;

        public bool IsAlive => Age < Lifetime;

        public void Update(float deltaTime)
        {
            Age += deltaTime;
            Position += Velocity * deltaTime;
            Rotation += RotationSpeed * deltaTime;
        }
    }
}
