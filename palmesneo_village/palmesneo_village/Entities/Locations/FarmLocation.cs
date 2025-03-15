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

            for (int x = 0; x < MapWidth; x++)
            {
                SetGroundTopTile(x, 0, GroundTopTile.Gate);
            }

            for (int x = 0; x < MapWidth; x++)
            {
                SetGroundTopTile(x, 48, GroundTopTile.Gate);
            }

            for (int y = 1; y <= 48; y++)
            {
                SetGroundTopTile(0, y, GroundTopTile.Gate);
            }

            for (int y = 1; y <= 48; y++)
            {
                SetGroundTopTile(MapWidth - 1, y, GroundTopTile.Gate);
            }

            for (int x = 0; x < MapWidth; x++)
            {
                SetAirTile(x, 50, 11);
            }

            BuildingItem toilet = Engine.ItemsDatabase.GetItemByName<BuildingItem>("toilet");
            TryBuild(toilet, 34, 24, Direction.Down);

            BuildingItem playerHouse = Engine.ItemsDatabase.GetItemByName<BuildingItem>("player_house");
            TryBuild(playerHouse, 23, 38, Direction.Down);

            int gasPipePolesAmount = 6;

            BuildingItem gasPipePoleItem = Engine.ItemsDatabase.GetItemByName<BuildingItem>("gas_pipe_pole");

            for (int i = 0; i <= gasPipePolesAmount; i++)
            {
                int step = MapWidth / gasPipePolesAmount;

                TryBuild(gasPipePoleItem, i * step, 53, Direction.Down);
            }

            BuildingItem electricPoleItem = Engine.ItemsDatabase.GetItemByName<BuildingItem>("electric_pole");

            TryBuild(electricPoleItem, 24, 55, Direction.Down);
            TryBuild(electricPoleItem, 59, 55, Direction.Down);

            for (int x = 0; x < MapWidth; x++)
            {
                if (x == 24 || x == 59) continue;

                SetAirTile(x, 51, 12);
                SetAirTile(x, 52, 13);
            }
        }

    }
}
