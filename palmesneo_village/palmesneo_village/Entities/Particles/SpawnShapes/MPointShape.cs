using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class MPointShape : ISpawnShape
    {
        public Vector2 GetSpawnPosition(Vector2 emitterPosition) => emitterPosition;
    }
}
