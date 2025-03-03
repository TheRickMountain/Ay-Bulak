using Microsoft.Xna.Framework;

namespace palmesneo_village
{

    public abstract class Creature : Entity
    {
        protected float Speed { get; private set; }

        private ImageEntity bodyImage;

        public Creature(CreatureTemplate creatureTemplate)
        {
            Name = creatureTemplate.Name;
            Speed = creatureTemplate.MovementSpeed;

            bodyImage = new ImageEntity();
            bodyImage.Texture = creatureTemplate.Texture;
            bodyImage.Centered = true;
            bodyImage.Offset = new Vector2(0, creatureTemplate.Texture.Height / 2 - Engine.TILE_SIZE / 2);

            AddChild(bodyImage);
        }
    }
}
