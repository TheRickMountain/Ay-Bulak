using Microsoft.Xna.Framework;
using System;

namespace palmesneo_village
{

    public abstract class Creature : InteractableEntity
    {
        protected float Speed { get; private set; }
        protected CreatureBodyVisual BodyVisual { get; private set; }

        public GameLocation CurrentLocation { get; private set; }

        public Creature(string name, MTexture bodySprite, MTexture shadowSprite, float speed)
        {
            Name = name;
            Speed = speed;

            ImageEntity shadow = new ImageEntity();
            shadow.Texture = shadowSprite;
            AddChild(shadow);

            BodyVisual = new CreatureBodyVisual(bodySprite);
            AddChild(BodyVisual);

            shadow.Offset = BodyVisual.GetBodyOffset();
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
