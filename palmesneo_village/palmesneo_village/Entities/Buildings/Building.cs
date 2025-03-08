using Microsoft.Xna.Framework;
using System;

namespace palmesneo_village
{
    public class Building : Entity
    {
        private BuildingItem buildingItem;
        private Direction direction;
        private Vector2[,] occupiedTiles;
        protected SpriteEntity Sprite { get; private set; }

        public Building(BuildingItem buildingItem, Direction direction, Vector2[,] occupiedTiles)
        {
            this.buildingItem = buildingItem;
            this.direction = direction;
            this.occupiedTiles = occupiedTiles;

            Sprite = new SpriteEntity();
            Sprite.Texture = buildingItem.DirectionIcon[direction];

            int buildingHeightInPixels = buildingItem.Height * Engine.TILE_SIZE;
            int buildingTextureHeight = buildingItem.DirectionIcon[direction].Height;

            Sprite.Offset = new Vector2(0, buildingTextureHeight - buildingHeightInPixels);

            AddChild(Sprite);
        }

        public BuildingItem BuildingItem => buildingItem;
        public Direction Direction => direction;
        public Vector2[,] OccupiedTiles => occupiedTiles;

        public virtual void OnDayChanged()
        {

        }
    }
}