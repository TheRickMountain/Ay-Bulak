using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class HouseLocation : GameLocation
    {

        public HouseLocation(string id, Teleport exitTeleport, TimeOfDayManager timeOfDayManager) 
            : base(id, 32, 16, false, timeOfDayManager)
        {
            CreateHouseFirstLayer();

            BuildingItem singleBed = Engine.ItemsDatabase.GetItemByName<BuildingItem>("single_bed");
            TryBuild(singleBed, 6, 5, Direction.Down);

            BuildingItem stove = Engine.ItemsDatabase.GetItemByName<BuildingItem>("stove");
            TryBuild(stove, 20, 5, Direction.Down);

            BuildingItem washBasin = Engine.ItemsDatabase.GetItemByName<BuildingItem>("wash_basin");
            TryBuild(washBasin, 23, 5, Direction.Down);

            BuildingItem gasStove = Engine.ItemsDatabase.GetItemByName<BuildingItem>("gas_stove");
            TryBuild(gasStove, 24, 5, Direction.Down);

            BuildingItem fridge = Engine.ItemsDatabase.GetItemByName<BuildingItem>("fridge");
            TryBuild(fridge, 25, 5, Direction.Down);

            TryBuild(Engine.ItemsDatabase.GetItemByName<BuildingItem>("window"), 9, 2, Direction.Down);
            TryBuild(Engine.ItemsDatabase.GetItemByName<BuildingItem>("window"), 14, 2, Direction.Down);
            TryBuild(Engine.ItemsDatabase.GetItemByName<BuildingItem>("window"), 23, 2, Direction.Down);

            TryBuild(Engine.ItemsDatabase.GetItemByName<BuildingItem>("table"), 24, 7, Direction.Down);

            CreateTeleport(23, 10, exitTeleport);
        }

        private void CreateHouseFirstLayer()
        {
            string rawMap = "********************************\n" +
                         "********************************\n" +
                         "******555555555555**555555******\n" +
                         "******555555555555**555555******\n" +
                         "******55555555555555555555******\n" +
                         "******44444444444455444444******\n" +
                         "******44444444444455444444******\n" +
                         "******44444444444444444444******\n" +
                         "******44444444444444444444******\n" +
                         "******444444444444**444444******\n" +
                         "***********************4********\n" +
                         "********************************\n" +
                         "********************************\n" +
                         "********************************\n" +
                         "********************************\n" +
                         "********************************\n";

            char[,] map = new char[MapWidth, MapHeight];

            // Разделяем строку rawMap на отдельные строки
            string[] rows = rawMap.Split('\n');

            // Заполняем двумерный массив
            for (int y = 0; y < MapHeight; y++)
            {
                for (int x = 0; x < MapWidth; x++)
                {
                    map[x, y] = rows[y][x];
                }
            }

            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    char pattern = map[x, y];

                    if (pattern == '*') continue;

                    SetGroundTile(x, y, (GroundTile)int.Parse(pattern + " "));
                }
            }
        }

    }
}
