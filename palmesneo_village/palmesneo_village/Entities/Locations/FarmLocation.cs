using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public class FarmLocation : GameLocation
    {

        public FarmLocation(string id) : base(id, 64, 64, true)
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
            
            BuildingItem toilet = Engine.ItemsDatabase.GetItemByName<BuildingItem>("toilet");
            TryBuild(toilet, 34, 24, Direction.Down);

            BuildingItem playerHouse = Engine.ItemsDatabase.GetItemByName<BuildingItem>("player_house");
            TryBuild(playerHouse, 23, 38, Direction.Down);

            BuildingItem coop = Engine.ItemsDatabase.GetItemByName<BuildingItem>("coop");
            TryBuild(coop, 40, 28, Direction.Down);

            BuildingItem well = Engine.ItemsDatabase.GetItemByName<BuildingItem>("well");
            TryBuild(well, 40, 42, Direction.Down);

            // Выделяем участок, в пределах которого можно генерировать строения
            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < 48; y++)
                {
                    int generationType = Calc.Random.Range(0, 4);

                    if (Calc.Random.Chance(0.10f))
                    {
                        if (generationType == 0)
                        {
                            TreeItem birchTree = Engine.ItemsDatabase.GetItemByName<TreeItem>("birch_tree");

                            TryBuild(birchTree, x, y, Direction.Down);

                            TreeBuilding treeBuilding = GetBuilding(x, y) as TreeBuilding;

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
                        else if(generationType == 3)
                        {
                            ResourceItem resourceItem = Engine.ItemsDatabase.GetItemByName<ResourceItem>("grass_resource");

                            TryBuild(resourceItem, x, y, Direction.Down);
                        }
                    }
                }
            }
        }

    }
}
