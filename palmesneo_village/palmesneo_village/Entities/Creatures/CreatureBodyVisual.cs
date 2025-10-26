using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class CreatureBodyVisual : Entity
    {

        private ImageEntity _bodyImage;

        public CreatureBodyVisual(MTexture bodyTexture)
        {
            _bodyImage = new ImageEntity();
            _bodyImage.Texture = bodyTexture;

            _bodyImage.Offset = new Vector2(bodyTexture.Width / 2, bodyTexture.Height - Engine.TILE_SIZE / 2);

            AddChild(_bodyImage);
        }

        public void Flip()
        {
            _bodyImage.FlipX = true;
        }

        public void Unflip()
        {
            _bodyImage.FlipX = false;
        }

        // TODO: временное решение, так как родительское вращение не влияет на детей
        public void SetBodyRotation(float rotation)
        {
            _bodyImage.LocalRotation = rotation;
        }

        public Vector2 GetBodyOffset()
        {
            return _bodyImage.Offset;
        }
    }
}
