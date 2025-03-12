using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace palmesneo_village
{
    public class NineSlicedImageUI : EntityUI
    {
        private MTexture texture;
        public MTexture Texture 
        {
            get => texture;
            set
            {
                if(texture == value)
                {
                    return;
                }

                texture = value;

                RecutPieces();

                RefreshTexture();
            }
        }

        public override Vector2 Size
        {
            get => base.Size;
            set 
            {
                Vector2 oldSize = base.Size;

                base.Size = value;

                if (oldSize != base.Size)
                {
                    Console.WriteLine("Nine sliced size changed");

                    RefreshTexture();
                }
            }
        }

        private Rectangle sliceCenter;

        public Rectangle SliceCenter
        {
            get => sliceCenter; 
            set
            {
                sliceCenter = value;

                RecutPieces();

                RefreshTexture();
            }
        }

        public MTexture FinalTexture { get; private set; }

        private MTexture[] pieces = new MTexture[9];

        public NineSlicedImageUI()
        {
            MinSize = Vector2.One;
        }

        private void RecutPieces()
        {
            if(Texture == null)
            {
                return;
            }

            pieces[0] = new MTexture(Texture, new Rectangle(0, 0, sliceCenter.X, sliceCenter.Y));
            pieces[1] = new MTexture(Texture, new Rectangle(sliceCenter.X, 0, sliceCenter.Width, sliceCenter.Y));
            pieces[2] = new MTexture(Texture, new Rectangle(sliceCenter.Right, 0, sliceCenter.X, sliceCenter.Y));

            pieces[3] = new MTexture(Texture, new Rectangle(0, sliceCenter.Y, sliceCenter.X, sliceCenter.Height));
            pieces[4] = new MTexture(Texture, sliceCenter);
            pieces[5] = new MTexture(Texture, new Rectangle(sliceCenter.Right, sliceCenter.Y, sliceCenter.X, sliceCenter.Height));

            pieces[6] = new MTexture(Texture, new Rectangle(0, sliceCenter.Bottom, sliceCenter.X, sliceCenter.Y));
            pieces[7] = new MTexture(Texture, new Rectangle(sliceCenter.X, sliceCenter.Bottom, sliceCenter.Width, sliceCenter.Y));
            pieces[8] = new MTexture(Texture, new Rectangle(sliceCenter.Right, sliceCenter.Bottom, sliceCenter.X, sliceCenter.Y));
        }

        private void RefreshTexture()
        {
            if(Texture == null)
            {
                return;
            }

            GraphicsDevice gfx = Engine.Instance.GraphicsDevice;
            SpriteBatch sb = RenderManager.SpriteBatch;

            var renderTarget = new RenderTarget2D(gfx, (int)Size.X, (int)Size.Y);
            gfx.SetRenderTarget(renderTarget);

            gfx.Clear(Color.Transparent);

            sb.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);

            int centerWidth = (int)Size.X - (Texture.Width - sliceCenter.Width);
            int centerHeight = (int)Size.Y - (Texture.Height - sliceCenter.Height);

            pieces[0].Draw(new Rectangle(0, 0, sliceCenter.X, sliceCenter.Y), Vector2.Zero, 0, Vector2.One, Color.White, SpriteEffects.None);
            pieces[1].Draw(new Rectangle(sliceCenter.X, 0, centerWidth, sliceCenter.Y), Vector2.Zero, 0, Vector2.One, Color.White, SpriteEffects.None);
            pieces[2].Draw(new Rectangle(sliceCenter.X + centerWidth, 0, sliceCenter.X, sliceCenter.Y), Vector2.Zero, 0, Vector2.One, Color.White, SpriteEffects.None);

            pieces[3].Draw(new Rectangle(0, sliceCenter.Y, sliceCenter.X, centerHeight), Vector2.Zero, 0, Vector2.One, Color.White, SpriteEffects.None);
            pieces[4].Draw(new Rectangle(sliceCenter.X, sliceCenter.Y, centerWidth, centerHeight), Vector2.Zero, 0, Vector2.One, Color.White, SpriteEffects.None);
            pieces[5].Draw(new Rectangle(sliceCenter.X + centerWidth, sliceCenter.Y, sliceCenter.X, centerHeight), Vector2.Zero, 0, Vector2.One, Color.White, SpriteEffects.None);

            pieces[6].Draw(new Rectangle(0, sliceCenter.Y + centerHeight, sliceCenter.X, sliceCenter.Y), Vector2.Zero, 0, Vector2.One, Color.White, SpriteEffects.None);
            pieces[7].Draw(new Rectangle(sliceCenter.X, sliceCenter.Y + centerHeight, centerWidth, sliceCenter.X), Vector2.Zero, 0, Vector2.One, Color.White, SpriteEffects.None);
            pieces[8].Draw(new Rectangle(sliceCenter.X + centerWidth, sliceCenter.Y + centerHeight, sliceCenter.X, sliceCenter.Y), Vector2.Zero, 0, Vector2.One, Color.White, SpriteEffects.None);

            sb.End();

            gfx.SetRenderTarget(null);

            FinalTexture?.Unload();

            FinalTexture = new MTexture(renderTarget);
        }

        public override void Render()
        {
            FinalTexture?.Draw(GlobalPosition, Origin, GlobalRotation, GlobalScale, SelfColor, SpriteEffects.None);

            base.Render();
        }

    }
}
