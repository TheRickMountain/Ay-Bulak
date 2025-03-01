using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace palmesneo_village
{
    public class ImageEntity : Entity
    {
        private MTexture texture;
        public MTexture Texture 
        {
            get => texture; 
            set
            {
                texture = value;

                UpdateOrigin();
            }
        }

        private bool centered = false;

        public bool Centered
        {
            get => centered;
            set
            {
                centered = value;

                UpdateOrigin();
            }
        }

        private Vector2 offset;

        public Vector2 Offset
        {
            get { return offset; }
            set 
            { 
                offset = value;
                
                UpdateOrigin();
            }
        }

        private void UpdateOrigin()
        {
            if(Texture != null)
            {
                if (Centered)
                {
                    origin = new Vector2(Texture.Width / 2, Texture.Height / 2) + offset;
                }
                else
                {
                    origin = offset;
                }
            }
        }


        private Vector2 origin = Vector2.Zero;

        private SpriteEffects effects = SpriteEffects.None;

        public bool FlipX
        {
            get => (effects & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally;
            set => effects = value ? (effects | SpriteEffects.FlipHorizontally) : (effects & ~SpriteEffects.FlipHorizontally);
        }

        public bool FlipY
        {
            get => (effects & SpriteEffects.FlipVertically) == SpriteEffects.FlipVertically;
            set => effects = value ? (effects | SpriteEffects.FlipVertically) : (effects & ~SpriteEffects.FlipVertically);
        }

        public override void Render()
        {
            Texture?.Draw(GlobalPosition, origin, GlobalRotation, GlobalScale, SelfColor, effects);

            base.Render();
        }

        public override void DebugRender()
        {
            base.DebugRender();

            if(Texture != null)
            {
                Rectangle spriteRect = new Rectangle(
                (int)(GlobalPosition.X - origin.X * GlobalScale.X),
                (int)(GlobalPosition.Y - origin.Y * GlobalScale.Y),
                (int)(Texture.Width * GlobalScale.X),
                (int)(Texture.Height * GlobalScale.Y));

                RenderManager.HollowRect(spriteRect, Color.Blue * 0.5f);
            }
        }

        public bool Contains(Vector2 value)
        {
            if(Texture == null)
            {
                return false;
            }

            Rectangle spriteRect = new Rectangle(
                (int)(GlobalPosition.X - origin.X * GlobalScale.X),
                (int)(GlobalPosition.Y - origin.Y * GlobalScale.Y),
                (int)(Texture.Width * GlobalScale.X),
                (int)(Texture.Height * GlobalScale.Y)
            );

            return spriteRect.Contains(value);
        }

        public Rectangle GetRectangle()
        {
            return new Rectangle(
                (int)(GlobalPosition.X - origin.X * GlobalScale.X),
                (int)(GlobalPosition.Y - origin.Y * GlobalScale.Y),
                (int)(Texture.Width * GlobalScale.X),
                (int)(Texture.Height * GlobalScale.Y)
            );
        }

    }
}
