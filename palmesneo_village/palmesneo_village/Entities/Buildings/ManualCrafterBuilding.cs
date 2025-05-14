using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace palmesneo_village
{
    public class ManualCrafterBuilding : CrafterBuilding
    {
        public ManualCrafterBuilding(GameLocation gameLocation, ManualCrafterItem item, Direction direction, Vector2[,] occupiedTiles) 
            : base(gameLocation, item, direction, occupiedTiles)
        {
            
        }

        public override void InteractAlternatively(Item item, PlayerEnergyManager playerEnergyManager)
        {
            ((GameplayScene)Engine.CurrentScene).OpenCrafterUI(CraftingRecipes);
        }
    }
}
