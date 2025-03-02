using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public abstract class Building : InteractableEntity
    {

        protected BuildingTemplate BuildingTemplate { get; private set; }
        protected Direction Direction { get; private set; }
        protected Vector2[,] Tiles { get; private set; }

        protected SpriteEntity Sprite { get; private set; }

        private TilemapsManager tilemapsManager;

        public Building(TilemapsManager tilemapsManager, BuildingTemplate buildingTemplate, Direction direction, Vector2[,] tiles)
        {
            this.tilemapsManager = tilemapsManager;
            BuildingTemplate = buildingTemplate;
            Direction = direction;
            Tiles = tiles;

            Sprite = new SpriteEntity();
            Sprite.Centered = true;
            Sprite.LocalPosition = buildingTemplate.TextureLocalPosition;
            Sprite.Offset = buildingTemplate.TextureOffset;
            AddChild(Sprite);

            AddToTilemapsManager();

            Destroyed += RemoveFromTilemapsManager;
        }

        private void AddToTilemapsManager()
        {
            for (int x = 0; x < Tiles.GetLength(0); x++)
            {
                for (int y = 0; y < Tiles.GetLength(1); y++)
                {
                    Vector2 tile = Tiles[x, y];
                    tilemapsManager.SetBuilding(tile, this);
                }
            }
        }

        private void RemoveFromTilemapsManager()
        {
            for (int x = 0; x < Tiles.GetLength(0); x++)
            {
                for (int y = 0; y < Tiles.GetLength(1); y++)
                {
                    Vector2 tile = Tiles[x, y];
                    tilemapsManager.ClearBuilding(tile);
                }
            }
        }

        public override Rectangle GetSelectorBounds()
        {
            return Sprite.GetRectangle();
        }
    }
}
