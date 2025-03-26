using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace palmesneo_village
{
    public class CoopLocation : GameLocation
    {

        public CoopLocation(string id, Teleport exitTeleport) : base(id, 32, 16)
        {
            CreateTeleport(20, 10, exitTeleport);

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
                         "********************6***********\n" +
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

        public override void StartNextDay(TimeOfDayManager timeOfDayManager)
        {
            base.StartNextDay(timeOfDayManager);

            int animalsAmount = 0;

            foreach(Animal animal in GetAnimals())
            {
                animalsAmount++;
            }

            for (int i = 0; i < animalsAmount; i++)
            {
                Vector2 spawnTile = GetTileForResourceSpawning();

                ResourceItem resourceItem = Engine.ItemsDatabase.GetItemByName<ResourceItem>("chicken_egg_resource");

                TryBuild(resourceItem, (int)spawnTile.X, (int)spawnTile.Y, Direction.Down);
            }
        }

        public Vector2 GetTileForResourceSpawning()
        {
            List<Vector2> freeTiles = new List<Vector2>();

            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    if(GetGroundTile(x, y) != GroundTile.CoopHouseFloor) continue;

                    if (GetBuilding(x, y) != null) continue;

                    freeTiles.Add(new Vector2(x, y));
                }
            }

            return Calc.Random.Choose(freeTiles);
        }
    }
}
