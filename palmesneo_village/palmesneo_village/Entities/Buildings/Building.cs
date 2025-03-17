using Microsoft.Xna.Framework;
using System;

namespace palmesneo_village
{
    public class Building : Entity
    {
        public bool IsPassable => buildingItem.IsPassable;
        public GameLocation gameLocation { get; private set; }
        private BuildingItem buildingItem;
        private Direction direction;
        private Vector2[,] occupiedTiles;
        protected SpriteEntity Sprite { get; private set; }

        public Building(GameLocation gameLocation, BuildingItem buildingItem, Direction direction, Vector2[,] occupiedTiles)
        {
            this.gameLocation = gameLocation;
            this.buildingItem = buildingItem;
            this.direction = direction;
            this.occupiedTiles = occupiedTiles;

            Sprite = new SpriteEntity();
            Sprite.Texture = buildingItem.DirectionIcon[direction];

            int buildingWidthInPixels = buildingItem.Width * Engine.TILE_SIZE;
            int buildingTextureWidth = buildingItem.DirectionIcon[direction].Width;

            int buildingHeightInPixels = buildingItem.Height * Engine.TILE_SIZE;
            int buildingTextureHeight = buildingItem.DirectionIcon[direction].Height;

            Sprite.Offset = new Vector2(-(buildingWidthInPixels / 2 - buildingTextureWidth / 2), 
                buildingTextureHeight - buildingHeightInPixels);

            AddChild(Sprite);
        }

        public BuildingItem BuildingItem => buildingItem;
        public Direction Direction => direction;
        public Vector2[,] OccupiedTiles => occupiedTiles;

        public virtual void OnDayChanged()
        {

        }

        public virtual void Interact(Item item)
        {

        }

        public virtual bool CanInteract(Item item)
        {
            return false;
        }
    }
}