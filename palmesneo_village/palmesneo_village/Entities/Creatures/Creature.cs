using Microsoft.Xna.Framework;

namespace palmesneo_village
{

    public abstract class Creature : Entity
    {
        protected float Speed { get; private set; }
        protected GameLocation CurrentLocation { get; private set; }

        private ImageEntity bodyImage;

        public Creature(string name, MTexture texture, float speed)
        {
            Name = name;
            Speed = speed;

            bodyImage = new ImageEntity();
            bodyImage.Texture = texture;
            bodyImage.Centered = true;
            bodyImage.Offset = new Vector2(0, texture.Height / 2 - Engine.TILE_SIZE / 2);

            AddChild(bodyImage);
        }

        public void SetGameLocation(GameLocation gameLocation)
        {
            CurrentLocation = gameLocation;
        }

        public void SetTilePosition(Vector2 tile)
        {
            LocalPosition = CurrentLocation.MapToWorld(tile) + new Vector2(Engine.TILE_SIZE / 2);
        }

        public Vector2 GetTilePosition()
        {
            return CurrentLocation.WorldToMap(LocalPosition);
        }

        protected virtual Direction UpdateMovement()
        {
            return Direction.Down;
        }
    }
}
