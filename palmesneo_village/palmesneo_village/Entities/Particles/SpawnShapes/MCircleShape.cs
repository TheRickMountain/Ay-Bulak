using Microsoft.Xna.Framework;
using System;

namespace palmesneo_village
{
    public class MCircleShape : ISpawnShape
    {
        public float Radius;

        public MCircleShape(float radius)
        {
            Radius = radius;
        }

        public Vector2 GetSpawnPosition(Vector2 emitterPosition)
        {
            float angle = Calc.Random.Range(0, MathF.Tau);
            float r = Calc.Random.Range(0f, Radius);
            return emitterPosition + new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * r;
        }
    }
}
