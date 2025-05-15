using System.Collections.Generic;

namespace palmesneo_village
{
    public struct Ingredient
    {
        public Item Item { get; private set; }
        public int Amount { get; private set; }
        public Ingredient(Item item, int amount)
        {
            Item = item;
            Amount = amount;
        }
    }

    public class CraftingRecipe
    {
        public Ingredient Result { get; private set; }
        public float CraftingTimeInHours { get; private set; } = 0;

        private List<Ingredient> requiredIngredients;
        
        public CraftingRecipe(Ingredient result, List<Ingredient> requiredIngredients, float craftingTimeInHours)
        {
            Result = result;
            this.requiredIngredients = requiredIngredients;
            CraftingTimeInHours = craftingTimeInHours;
        }

        public IReadOnlyList<Ingredient> RequiredIngredients => requiredIngredients;
    }
}
