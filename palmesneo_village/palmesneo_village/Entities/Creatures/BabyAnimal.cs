using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace palmesneo_village
{
    public class BabyAnimal : Animal
    {

        private BabyAnimalItem babyAnimalItem;

        private int currentAge = 0;

        public BabyAnimal(BabyAnimalItem item) : base(item)
        {
            babyAnimalItem = item;
        }

        public void IncrementAge()
        {
            currentAge++;

            if(currentAge >= babyAnimalItem.DaysUntilAging)
            {
                CurrentLocation.RemoveAnimal(this);
                
                string adultAnimalName = babyAnimalItem.AdultAnimalName;
                
                AdultAnimalItem adultAnimalItem = Engine.ItemsDatabase.GetItemByName<AdultAnimalItem>(adultAnimalName);

                Vector2 tilePosition = GetTilePosition();

                CurrentLocation.TrySpawnAnimal(adultAnimalItem, (int)tilePosition.X, (int)tilePosition.Y);
            }
        }

        public override IEnumerable<InteractionData> GetAvailableInteractions(Inventory inventory)
        {
            yield break;
        }

        public override void Interact(InteractionData interactionData, Inventory inventory)
        {

        }

    }
}
