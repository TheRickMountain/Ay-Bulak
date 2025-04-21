using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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

            ResetAnimalsSatiety();

            FeedAnimals();
        }

        private void ResetAnimalsSatiety()
        {
            foreach(Animal animal in GetAnimals())
            {
                animal.IsFed = false;
            }
        }

        private void FeedAnimals()
        {
            List<Animal> animals = GetAnimals().ToList();
            List<AnimalFeederBuilding> fullFeeders = GetBuildings()
                .OfType<AnimalFeederBuilding>()
                .Where(f => f.IsFull)
                .ToList();

            int hayAmount = fullFeeders.Count;

            if (hayAmount == 0 || animals.Count == 0)
                return;

            // Выбираем животных, которых покормим
            List<Animal> animalsToFeed = animals
                //.OrderBy(_ => Guid.NewGuid()) // Раскомментировать, если нужно рандомно выбирать животных
                .Take(hayAmount)
                .ToList();

            foreach (var animal in animalsToFeed)
            {
                animal.IsFed = true;
            }

            // Очищаем ровно то количество кормушек, сколько сена было использовано
            for (int i = 0; i < animalsToFeed.Count && i < fullFeeders.Count; i++)
            {
                fullFeeders[i].Empty();
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
