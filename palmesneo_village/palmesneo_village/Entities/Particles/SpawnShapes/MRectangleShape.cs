using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class MRectangleShape : ISpawnShape
    {

        public Vector2 Size;

        public MRectangleShape(Vector2 size)
        {
            Size = size;
        }

        public Vector2 GetSpawnPosition(Vector2 emitterPosition)
        {
            float x = Calc.Random.Range(-Size.X / 2f, Size.X / 2f);
            float y = Calc.Random.Range(-Size.Y / 2f, Size.Y / 2f);
            return emitterPosition + new Vector2(x, y);
        }

    }
}
