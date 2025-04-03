using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public class AnimalHouseLocation : GameLocation
    {
        public int Capacity { get; private set; }

        public AnimalHouseLocation(int capacity, string locationId, int mapWidth, int mapHeight) 
            : base(locationId, mapWidth, mapHeight, false)
        {
            Capacity = capacity;
        }

        public override void StartNextDay(TimeOfDayManager timeOfDayManager)
        {
            base.StartNextDay(timeOfDayManager);

            EmptyAnimalFeeders();
        }

        private void EmptyAnimalFeeders()
        {
            int animalsAmount = GetAnimalsAmount();

            foreach (Building building in GetBuildings())
            {
                if (building is AnimalFeederBuilding animalFeeder)
                {
                    animalFeeder.Empty();

                    animalsAmount--;

                    if (animalsAmount == 0)
                    {
                        break;
                    }
                }
            }
        }

        public int GetAnimalsAmount()
        {
            int animalsAmount = 0;

            foreach (Animal animal in GetAnimals())
            {
                animalsAmount++;
            }

            return animalsAmount;
        }
    }
}
