using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public static class TooltipBuilder
    {

        public static string CreateTooltipText(CraftingRecipe craftingRecipe, Inventory inventory)
        {
            string tooltipText = craftingRecipe.Result.Item.GetTooltipInfo();

            foreach (Ingredient ingredient in craftingRecipe.RequiredIngredients)
            {
                int amountOfIngredientsInInventory = inventory.GetTotalItemQuantity(ingredient.Item);
                int requiredAmountOfIngredients = ingredient.Amount;

                if (amountOfIngredientsInInventory < requiredAmountOfIngredients)
                {
                    tooltipText += $"\n{LocalizationManager.GetText(ingredient.Item.Name)}   " +
                        $"/c[{ColorUtils.RED_HEX}]{amountOfIngredientsInInventory}/cd // {requiredAmountOfIngredients}";
                }
                else
                {
                    tooltipText += $"\n{LocalizationManager.GetText(ingredient.Item.Name)}   " +
                        $"{amountOfIngredientsInInventory} // {requiredAmountOfIngredients}";
                }
            }

            return tooltipText;
        }

    }
}
