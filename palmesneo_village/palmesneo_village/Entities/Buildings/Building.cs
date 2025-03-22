using Microsoft.Xna.Framework;
using System;

namespace palmesneo_village
{
    public class Building : Entity
    {
        public bool IsPassable => buildingItem.IsPassable;
        public Vector2[,] OccupiedTiles { get; private set; }

        protected GameLocation GameLocation { get; private set; }
        protected SpriteEntity Sprite { get; private set; }
        
        private BuildingItem buildingItem;
        private Direction direction;
        
        public Building(GameLocation gameLocation, BuildingItem buildingItem, Direction direction, Vector2[,] occupiedTiles)
        {
            GameLocation = gameLocation;
            OccupiedTiles = occupiedTiles;

            this.buildingItem = buildingItem;
            this.direction = direction;
            
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