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

        private List<Ingredient> requiredIngredients;

        public CraftingRecipe(Ingredient result, List<Ingredient> requiredIngredients)
        {
            Result = result;
            this.requiredIngredients = requiredIngredients;
        }

        public IEnumerable<Ingredient> GetRequiredIngredients()
        {
            return requiredIngredients;
        }

        public override string ToString()
        {
            string info = Result.Item.GetTooltipInfo();
            
            foreach(Ingredient ingredient in requiredIngredients)
            {
                info += "\n" + LocalizationManager.GetText(ingredient.Item.Name) + " x" + ingredient.Amount;
            }

            return info;
        }

    }
}
