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

            SetGroundTile(56, 32, GroundTile.Water);
            SetGroundTile(57, 32, GroundTile.Water);
            SetGroundTile(55, 33, GroundTile.Water);
            SetGroundTile(56, 33, GroundTile.Water);
            SetGroundTile(57, 33, GroundTile.Water);
            SetGroundTile(58, 33, GroundTile.Water);
            SetGroundTile(55, 34, GroundTile.Water);
            SetGroundTile(56, 34, GroundTile.Water);
            SetGroundTile(57, 34, GroundTile.Water);
            SetGroundTile(58, 34, GroundTile.Water);
            SetGroundTile(55, 35, GroundTile.Water);
            SetGroundTile(56, 35, GroundTile.Water);
            SetGroundTile(57, 35, GroundTile.Water);
            SetGroundTile(58, 35, GroundTile.Water);

            for (int x = 0; x < MapWidth; x++)
            {
                SetGroundTopTile(x, 48, GroundTopTile.Gate);
            }

            for (int x = 0; x < MapWidth; x++)
            {
                SetAirTile(x, 50, 11);
            }

            BuildingItem toilet = Engine.ItemsDatabase.GetItemByName<BuildingItem>("toilet");
            TryBuild(toilet, 34, 24, Direction.Down);

            BuildingItem playerHouse = Engine.ItemsDatabase.GetItemByName<BuildingItem>("player_house");
            TryBuild(playerHouse, 23, 38, Direction.Down);

            // Выделяем участок, в пределах которого можно генерировать строения
            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < 48; y++)
                {
                    int generationType = Calc.Random.Range(0, 3);

                    if (Calc.Random.Chance(0.10f))
                    {
                        if (generationType == 0)
                        {
                            TreeItem birchTree = Engine.ItemsDatabase.GetItemByName<TreeItem>("birch_tree");

                            TreeBuilding treeBuilding = TryBuild(birchTree, x, y, Direction.Down) as TreeBuilding;

                            treeBuilding?.SetGrowthProgress(Calc.Random.Choose(0.35f, 0.5f, 0.75f, 1.0f));
                        }
                        else if(generationType == 1)
                        {
                            ResourceItem resourceItem = Engine.ItemsDatabase.GetItemByName<ResourceItem>("stone_resource");

                            TryBuild(resourceItem, x, y, Direction.Down);
                        }
                        else if (generationType == 2)
                        {
                            ResourceItem resourceItem = Engine.ItemsDatabase.GetItemByName<ResourceItem>("wood_resource");

                            TryBuild(resourceItem, x, y, Direction.Down);
                        }
                    }
                }
            }

            // Газовая труба
            int gasPipePolesAmount = 6;

            BuildingItem gasPipePoleItem = Engine.ItemsDatabase.GetItemByName<BuildingItem>("gas_pipe_pole");

            for (int i = 0; i <= gasPipePolesAmount; i++)
            {
                int step = MapWidth / gasPipePolesAmount;

                TryBuild(gasPipePoleItem, i * step, 53, Direction.Down);
            }

            // Линия электропередач
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
