using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace palmesneo_village
{
    public class ImageUI : EntityUI
    {

        public MTexture Texture { get; set; }

        public override void Render()
        {
            Vector2 sizeRatio = Size / Texture.Size;
            Texture?.Draw(GlobalPosition, Origin / sizeRatio, GlobalRotation, GlobalScale * sizeRatio, SelfColor, SpriteEffects.None);

            base.Render();
        }

    }
}
