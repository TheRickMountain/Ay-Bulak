using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class SprinklerBuilding : Building
    {
        private SprinklerItem sprinklerItem;

        public SprinklerBuilding(GameLocation gameLocation, SprinklerItem item, Direction direction, Vector2[,] occupiedTiles) 
            : base(gameLocation, item, direction, occupiedTiles)
        {
            sprinklerItem = item;
        }

        public override void OnDayChanged()
        {
            if(sprinklerItem.Range == 1)
            {
                foreach(var tile in GameLocation.GetNeighbourTiles(OccupiedTiles[0, 0], true))
                {
                    GroundTile groundTile = GameLocation.GetGroundTile((int)tile.X, (int)tile.Y);

                    if (groundTile != GroundTile.FarmPlot) continue;

                    GameLocation.SetGroundTopTile((int)tile.X, (int)tile.Y,  GroundTopTile.Moisture);
                }
            }
        }

    }
}
