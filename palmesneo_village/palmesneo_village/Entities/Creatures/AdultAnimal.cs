using System.Collections.Generic;

namespace palmesneo_village
{
    public class AdultAnimal : Animal
    {

        public AdultAnimal(AdultAnimalItem item) : base(item)
        {
            
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
