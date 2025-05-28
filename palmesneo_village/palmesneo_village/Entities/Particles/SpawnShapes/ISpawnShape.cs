using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public interface ISpawnShape
    {
        Vector2 GetSpawnPosition(Vector2 emitterPosition);
    }
}
