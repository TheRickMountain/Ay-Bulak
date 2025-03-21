using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public class ManualCrafterItem : BuildingItem
    {
        public JsonCraftingRecipe[] CraftingRecipes { get; init; }

        public override void Initialize(MTileset sourceTileset)
        {
            base.Initialize(sourceTileset);
        }
    }
}
