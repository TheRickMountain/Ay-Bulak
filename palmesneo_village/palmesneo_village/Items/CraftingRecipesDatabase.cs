using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public class CraftingRecipesDatabase
    {

        public JsonCraftingRecipe[] CraftingRecipes { get; init; }

        private ItemsDatabase itemsDatabase;

        private CraftingRecipe[] convertedCraftingRecipes;

        public void Initialize(ItemsDatabase itemsDatabase)
        {
            this.itemsDatabase = itemsDatabase;

            convertedCraftingRecipes = new CraftingRecipe[CraftingRecipes.Length];

            for (int i = 0; i < CraftingRecipes.Length; i++)
            {
                CraftingRecipe craftingRecipe = ConvertJsonCraftingRecipe(CraftingRecipes[i]);

                convertedCraftingRecipes[i] = craftingRecipe;
            }
        }

        public IEnumerable<CraftingRecipe> GetCraftingRecipes()
        {
            return convertedCraftingRecipes;
        }

        public Ingredient ConvertJsonIngredient(JsonIngredient jsonIngredient)
        {
            Item item = itemsDatabase.GetItemByName(jsonIngredient.Item);

            if (item == null)
            {
                throw new Exception($"Item with name {jsonIngredient.Item} not found in the database.");
            }

            return new Ingredient(item, jsonIngredient.Amount);
        }

        public CraftingRecipe ConvertJsonCraftingRecipe(JsonCraftingRecipe jsonCraftingRecipe)
        {
            Ingredient resultIngredient = ConvertJsonIngredient(jsonCraftingRecipe.Result);

            List<Ingredient> requiredIngredients = new();

            foreach (JsonIngredient jsonIngredient in jsonCraftingRecipe.RequiredIngredients)
            {
                requiredIngredients.Add(ConvertJsonIngredient(jsonIngredient));
            }

            return new CraftingRecipe(resultIngredient, requiredIngredients);
        }

    }
}
