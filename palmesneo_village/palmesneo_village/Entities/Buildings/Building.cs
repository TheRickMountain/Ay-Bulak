using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace palmesneo_village
{
    public class Building : InteractableEntity
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
            Sprite.Texture = buildingItem.DirectionTexture[direction];

            int buildingWidthInPixels = buildingItem.Width * Engine.TILE_SIZE;
            int buildingTextureWidth = buildingItem.DirectionTexture[direction].Width;

            int buildingHeightInPixels = buildingItem.Height * Engine.TILE_SIZE;
            int buildingTextureHeight = buildingItem.DirectionTexture[direction].Height;

            Sprite.Offset = new Vector2(-(buildingWidthInPixels / 2 - buildingTextureWidth / 2), 
                buildingTextureHeight - buildingHeightInPixels);

            AddChild(Sprite);

            if (buildingItem.SmokeSpawnData != null)
            {
                SmokeEffectEntity smokeEffect = new SmokeEffectEntity();
                smokeEffect.LocalPosition = buildingItem.SmokeSpawnData.GetPosition();
                AddChild(smokeEffect);
            }
        }

        public BuildingItem BuildingItem => buildingItem;
        public Direction Direction => direction;

        public virtual void OnBeforeDayChanged()
        {

        }

        public virtual void OnAfterDayChanged()
        {

        }

        public virtual void Interact(Inventory inventory, int activeSlotIndex, PlayerEnergyManager playerEnergyManager)
        {
        }

        public override void Interact(InteractionData interactionData, Inventory inventory)
        {
            
        }

        public override IEnumerable<InteractionData> GetAvailableInteractions(Inventory inventory)
        {
            yield break;
        }
    }
}