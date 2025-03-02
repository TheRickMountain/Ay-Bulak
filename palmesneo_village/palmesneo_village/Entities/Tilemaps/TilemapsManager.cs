using System;
using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class TilemapsManager : Entity
    {
        private Tilemap groundTilemap;

        private Building[,] buildings;

        public TilemapsManager(Tilemap groundTilemap)
        {
            this.groundTilemap = groundTilemap;

            buildings = new Building[groundTilemap.TileColumns, groundTilemap.TileRows];
        }

        #region Building

        public void SetBuilding(Vector2 tile, Building building)
        {
            if (buildings[(int)tile.X, (int)tile.Y] != null)
            {
                throw new Exception("The tile is already occupied!");
            }

            buildings[(int)tile.X, (int)tile.Y] = building;
        }

        public void ClearBuilding(Vector2 tile)
        {
            buildings[(int)tile.X, (int)tile.Y] = null;
        }

        public Building GetBuilding(Vector2 tile)
        {
            return buildings[(int)tile.X, (int)tile.Y];
        }

        #endregion
    }
}
