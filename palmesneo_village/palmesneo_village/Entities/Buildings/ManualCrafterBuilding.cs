using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace palmesneo_village
{
    public class ManualCrafterBuilding : Building
    {
        private List<CraftingRecipe> craftingRecipes = new();

        public ManualCrafterBuilding(GameLocation gameLocation, ManualCrafterItem item, Direction direction, Vector2[,] occupiedTiles) 
            : base(gameLocation, item, direction, occupiedTiles)
        {
            foreach (JsonCraftingRecipe jsonCraftingRecipe in item.CraftingRecipes)
            {
                CraftingRecipe craftingRecipe = Engine.CraftingRecipesDatabase.ConvertJsonCraftingRecipe(jsonCraftingRecipe);
                craftingRecipes.Add(craftingRecipe);
            }
        }

        public IEnumerable<CraftingRecipe> CraftingRecipes => craftingRecipes;

        public override void InteractAlternatively(Item item, PlayerEnergyManager playerEnergyManager)
        {
            ((GameplayScene)Engine.CurrentScene).OpenPlayerInventoryUI(CraftingRecipes);
        }
    }
}
