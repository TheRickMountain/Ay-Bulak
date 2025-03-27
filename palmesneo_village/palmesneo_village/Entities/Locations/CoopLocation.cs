using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace palmesneo_village
{
    public class CoopLocation : AnimalHouseLocation
    {
        public CoopLocation(string id, Teleport exitTeleport) : base(6, id, 32, 16)
        {
            CreateTeleport(20, 10, exitTeleport);

            CreateCoopFirstLayer();

            for (int i = 0; i < Capacity; i++)
            {
                TryBuild(Engine.ItemsDatabase.GetItemByName<BuildingItem>("animal_feeder"), 13 + i, 5, Direction.Down);
            }

            TryBuild(Engine.ItemsDatabase.GetItemByName<BuildingItem>("bird_nest"), 10, 5, Direction.Down);
            TryBuild(Engine.ItemsDatabase.GetItemByName<BuildingItem>("bird_nest"), 10, 6, Direction.Down);
            TryBuild(Engine.ItemsDatabase.GetItemByName<BuildingItem>("bird_nest"), 10, 7, Direction.Down);

            TryBuild(Engine.ItemsDatabase.GetItemByName<BuildingItem>("bird_nest"), 21, 5, Direction.Down);
            TryBuild(Engine.ItemsDatabase.GetItemByName<BuildingItem>("bird_nest"), 21, 6, Direction.Down);
            TryBuild(Engine.ItemsDatabase.GetItemByName<BuildingItem>("bird_nest"), 21, 7, Direction.Down);
        }

        private void CreateCoopFirstLayer()
        {
            string rawMap = "********************************\n" +
                         "********************************\n" +
                         "********************************\n" +
                         "**********777777777777**********\n" +
                         "**********777777777777**********\n" +
                         "**********666666666666**********\n" +
                         "**********666666666666**********\n" +
                         "**********666666666666**********\n" +
                         "**********666666666666**********\n" +
                         "********************6***********\n" +
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

        public override void StartNextDay(TimeOfDayManager timeOfDayManager)
        {
            base.StartNextDay(timeOfDayManager);

            int animalsAmount = GetAnimalsAmount();

            if (animalsAmount > 0)
            {
                foreach (Building building in GetBuildings())
                {
                    if (building is BirdNestBuilding birdNestBuilding)
                    {
                        birdNestBuilding.Upgrade();

                        animalsAmount--;

                        if (animalsAmount == 0)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}
