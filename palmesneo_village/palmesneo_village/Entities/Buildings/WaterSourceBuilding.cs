using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class WaterSourceBuilding : Building
    {
        public WaterSourceBuilding(GameLocation gameLocation, WaterSourceItem item, Direction direction, Vector2[,] occupiedTiles)
            : base(gameLocation, item, direction, occupiedTiles)
        {
        }
    }
}
