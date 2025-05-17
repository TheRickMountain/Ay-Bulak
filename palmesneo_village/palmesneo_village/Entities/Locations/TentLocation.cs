using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class TentLocation : GameLocation
    {

        public TentLocation(string id, Teleport exitTeleport, TimeOfDayManager timeOfDayManager) 
            : base(id, 16, 16, false, timeOfDayManager)
        {
            CreateHouseFirstLayer();

            CreateTeleport(8, 11, exitTeleport);
        }

        private void CreateHouseFirstLayer()
        {
            string rawMap = "****************\n" +
                            "****************\n" +
                            "********9*******\n" +
                            "*******999******\n" +
                            "******99999*****\n" +
                            "******99999*****\n" +
                            "******88888*****\n" +
                            "******88888*****\n" +
                            "******88888*****\n" +
                            "******88888*****\n" +
                            "******88888*****\n" +
                            "********8*******\n" +
                            "****************\n" +
                            "****************\n" +
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
