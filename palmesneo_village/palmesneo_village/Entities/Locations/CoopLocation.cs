using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public class CoopLocation : GameLocation
    {

        public CoopLocation(string id, Teleport exitTeleport) : base(id, 32, 16)
        {
            CreateTeleport(21, 9, exitTeleport);

            CreateCoopFirstLayer();
        }

        private void CreateCoopFirstLayer()
        {
            string rawMap = "********************************\n" +
                         "********************************\n" +
                         "********************************\n" +
                         "*********77777777777777*********\n" +
                         "*********77777777777777*********\n" +
                         "*********66666666666666*********\n" +
                         "*********66666666666666*********\n" +
                         "*********66666666666666*********\n" +
                         "*********66666666666666*********\n" +
                         "*********66666666666666*********\n" +
                         "********************************\n" +
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
