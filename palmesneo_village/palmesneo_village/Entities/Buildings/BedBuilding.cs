using System;
using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class BedBuilding : Building
    {
        public BedBuilding(GameLocation gameLocation, BedItem bedItem, Direction direction, Vector2[,] occupiedTiles) 
            : base(gameLocation, bedItem, direction, occupiedTiles)
        {
            
        }
    }
}
