using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public class HouseLocation : GameLocation
    {

        public HouseLocation(string id) : base(id, 32, 32)
        {
            // Стена зала
            for (int x = 0; x < 12; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    SetGroundTile(x, y, GroundTile.HouseWall);
                }
            }

            // Зал
            for (int x = 0; x < 12; x++)
            {
                for (int y = 2; y < 10; y++)
                {
                    SetGroundTile(x, y, GroundTile.HouseFloor);
                }
            }

            // Переход между залом и кухней
            SetGroundTile(12, 4, GroundTile.HouseFloor);
            SetGroundTile(12, 5, GroundTile.HouseFloor);

            // Стена кухни
            for (int x = 13; x < 19; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    SetGroundTile(x, y, GroundTile.HouseWall);
                }
            }

            // Кухня
            for (int x = 13; x < 19; x++)
            {
                for (int y = 2; y < 10; y++)
                {
                    SetGroundTile(x, y, GroundTile.HouseFloor);
                }
            }

            // Вход
        }

    }
}
