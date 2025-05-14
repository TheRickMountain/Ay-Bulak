using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public class CrafterInteractionData : InteractionData
    {
        public CraftingRecipe CraftingRecipe { get; private set; }

        public CrafterInteractionData(CraftingRecipe craftingRecipe, string information, bool isActive) 
            : base(InteractionType.Craft, craftingRecipe.Result.Item.Icon, information, isActive)
        {
            CraftingRecipe = craftingRecipe;
        }

    }
}
