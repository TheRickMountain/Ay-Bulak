using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class CrafterBuilding : Building
    {
        private List<CraftingRecipe> craftingRecipes = new();

        public CrafterBuilding(GameLocation gameLocation, CrafterItem item, Direction direction, Vector2[,] occupiedTiles)
            : base(gameLocation, item, direction, occupiedTiles)
        {
            foreach (JsonCraftingRecipe jsonCraftingRecipe in item.CraftingRecipes)
            {
                CraftingRecipe craftingRecipe = Engine.ItemsDatabase.ConvertJsonCraftingRecipe(jsonCraftingRecipe);
                craftingRecipes.Add(craftingRecipe);
            }
        }

        public IEnumerable<CraftingRecipe> CraftingRecipes => craftingRecipes;

        public override void InteractAlternatively(Item item, PlayerEnergyManager playerEnergyManager)
        {
            ((GameplayScene)Engine.CurrentScene).OpenCrafterUI(CraftingRecipes);
        }
    }
}
