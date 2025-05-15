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

        public override void Interact(InteractionData interactionData, Inventory inventory)
        {
            switch (interactionData.InteractionType)
            {
                case InteractionType.ManualCraft:
                    ((GameplayScene)Engine.CurrentScene).OpenCrafterUI(CraftingRecipes);
                    break;
            }
        }

        public override IEnumerable<InteractionData> GetAvailableInteractions(Inventory inventory)
        {
            yield return new InteractionData(
                InteractionType.ManualCraft,
                ResourcesManager.GetTexture("Sprites", "craft_icon"),
                LocalizationManager.GetText("create_item"),
                true);
        }
    }
}
