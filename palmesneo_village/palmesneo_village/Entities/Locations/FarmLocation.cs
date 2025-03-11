using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public class FarmLocation : GameLocation
    {

        public FarmLocation(string id) : base(id, 64, 64)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    SetGroundTile(x, y, GroundTile.Grass);
                }
            }

            SetGroundTopTile(10, 10, GroundTopTile.Stone);

            SetGroundTopTile(14, 11, GroundTopTile.Stone);

            SetGroundTopTile(10, 15, GroundTopTile.Stone);

            SetGroundTopTile(13, 8, GroundTopTile.Wood);

            SetGroundTopTile(5, 4, GroundTopTile.Wood);

            SetGroundTopTile(17, 6, GroundTopTile.Wood);
        }

    }
}
