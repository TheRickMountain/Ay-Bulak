using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class Building : Entity
    {
        private BuildingItem buildingItem;
        private GameLocation gameLocation;
        private Direction direction;
        private Vector2[,] tiles;

        private SpriteEntity sprite;

        public Building(BuildingItem buildingItem, GameLocation gameLocation, Direction direction, Vector2[,] tiles)
        {
            this.buildingItem = buildingItem;
            this.gameLocation = gameLocation;
            this.direction = direction;
            this.tiles = tiles;

            sprite = new SpriteEntity();
            sprite.Centered = true;
            AddChild(sprite);
        }

    }
}
