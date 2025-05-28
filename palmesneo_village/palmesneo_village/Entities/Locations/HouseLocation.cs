using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class HouseLocation : GameLocation
    {

        public HouseLocation(string id, Teleport exitTeleport, TimeOfDayManager timeOfDayManager) 
            : base(id, 16, 16, false, timeOfDayManager)
        {
            CreateHouseFirstLayer();

            TryBuild(Engine.ItemsDatabase.GetItemByName<BuildingItem>("window"), 6, 2, Direction.Down);
            TryBuild(Engine.ItemsDatabase.GetItemByName<BuildingItem>("window"), 9, 2, Direction.Down);

            CreateTeleport(9, 13, exitTeleport);
        }

        private void CreateHouseFirstLayer()
        {
            string rawMap = "****************\n" +
                            "****************\n" +
                            "****55555555****\n" +
                            "****55555555****\n" +
                            "****55555555****\n" +
                            "****44444444****\n" +
                            "****44444444****\n" +
                            "****44444444****\n" +
                            "****44444444****\n" +
                            "****44444444****\n" +
                            "****44444444****\n" +
                            "****44444444****\n" +
                            "****44444444****\n" +
                            "*********4******\n" +
                            "****************\n" +
                            "****************\n";

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
