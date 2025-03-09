using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public class HouseLocation : GameLocation
    {

        public HouseLocation(string id) : base(id, 32, 16)
        {
            CreateHouseFirstLayer();
            CreateHouseSecondLayer();
        }

        private void CreateHouseFirstLayer()
        {
            string rawMap = "********************************\n" +
                         "********************************\n" +
                         "*****6666666666666666666666*****\n" +
                         "*****6555555555555665555556*****\n" +
                         "*****6555555555555665555556*****\n" +
                         "*****6444444444444554444446*****\n" +
                         "*****6444444444444554444446*****\n" +
                         "*****6444444444444444444446*****\n" +
                         "*****6444444444444444444446*****\n" +
                         "*****6444444444444664444446*****\n" +
                         "*****6666666666666666666666*****\n" +
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

        private void CreateHouseSecondLayer()
        {
            string rawMap = "********************************\n" +
                         "********************************\n" +
                         "*****1000000000000410000004*****\n" +
                         "*****2************52******5*****\n" +
                         "*****2************67******5*****\n" +
                         "*****2********************5*****\n" +
                         "*****2********************5*****\n" +
                         "*****2********************5*****\n" +
                         "*****2************9!******5*****\n" +
                         "*****3000000000000830000008*****\n" +
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


                    if(pattern == '!')
                    {
                        SetBuildingTopTile(x, y, 10);
                    }
                    else
                    {
                        SetBuildingTopTile(x, y, int.Parse(pattern + " "));
                    }
                }
            }
        }

    }
}
