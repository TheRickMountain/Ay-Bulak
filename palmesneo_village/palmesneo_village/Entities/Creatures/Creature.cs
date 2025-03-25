using Microsoft.Xna.Framework;

namespace palmesneo_village
{

    public abstract class Creature : Entity
    {
        protected float Speed { get; private set; }
        protected GameLocation CurrentLocation { get; private set; }

        protected ImageEntity BodyImage { get; private set; }

        public Creature(string name, MTexture texture, float speed)
        {
            Name = name;
            Speed = speed;

            BodyImage = new ImageEntity();
            BodyImage.Texture = texture;
            BodyImage.Centered = true;
            BodyImage.Offset = new Vector2(0, texture.Height / 2 - Engine.TILE_SIZE / 2);

            AddChild(BodyImage);
        }

        public override void Update()
        {
            Depth = (int)LocalPosition.Y;

            base.Update();
        }

        public void SetGameLocation(GameLocation gameLocation)
        {
            CurrentLocation = gameLocation;
        }

        public virtual void SetTilePosition(Vector2 tile)
        {
            LocalPosition = CurrentLocation.MapToWorld(tile) + new Vector2(Engine.TILE_SIZE / 2);
        }

        public Vector2 GetTilePosition()
        {
            return CurrentLocation.WorldToMap(LocalPosition);
        }
    }
}
