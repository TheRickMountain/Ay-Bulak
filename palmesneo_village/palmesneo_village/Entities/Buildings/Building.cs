using Microsoft.Xna.Framework;
using System;

namespace palmesneo_village
{
    public class Building : Entity
    {
        private BuildingItem buildingItem;
        private GameLocation gameLocation;
        private Direction direction;
        private Vector2[,] occupiedTiles;
        private SpriteEntity sprite;

        public Building(BuildingItem buildingItem, GameLocation gameLocation, Direction direction, Vector2[,] occupiedTiles)
        {
            this.buildingItem = buildingItem;
            this.gameLocation = gameLocation;
            this.direction = direction;
            this.occupiedTiles = occupiedTiles;

            sprite = new SpriteEntity();
            sprite.Texture = buildingItem.DirectionIcon[direction];
            AddChild(sprite);
        }

        public BuildingItem BuildingItem => buildingItem;
        public Direction Direction => direction;
        public Vector2[,] OccupiedTiles => occupiedTiles;

    }
}