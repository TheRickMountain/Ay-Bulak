using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class BedBuilding : Building
    {
        public BedBuilding(GameLocation gameLocation, BedItem bedItem, Direction direction, Vector2[,] occupiedTiles) 
            : base(gameLocation, bedItem, direction, occupiedTiles)
        {
            
        }

        public override void Interact(InteractionData interactionData, Inventory inventory)
        {
            ((GameplayScene)Engine.CurrentScene).StartNextDay();
        }

        public override IEnumerable<InteractionData> GetAvailableInteractions(Inventory inventory)
        {
            yield return new InteractionData(
                InteractionType.Sleep, 
                ResourcesManager.GetTexture("Sprites", "sleep_icon"), 
                LocalizationManager.GetText("sleep"), 
                true);
        }
    }
}
