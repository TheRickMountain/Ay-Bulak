using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace palmesneo_village
{
    public class MTexture
    {

        public Texture2D Texture { get; private set; }
        public Rectangle ClipRect { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Vector2 Size { get; private set; }
        public Vector2 Center { get; private set; }

        public MTexture(Texture2D texture)
        {
            Texture = texture;
            ClipRect = new Rectangle(0, 0, texture.Width, texture.Height);
            Width = ClipRect.Width;
            Height = ClipRect.Height;
            Size = new Vector2(Width, Height);
            SetUtil();
        }

        public MTexture(MTexture parent, int x, int y, int width, int height)
        {
            Texture = parent.Texture;
            ClipRect = parent.GetRelativeRect(x, y, width, height);
            Width = width;
            Height = height;
            Size = new Vector2(Width, Height);
            SetUtil();
        }

        public MTexture(MTexture parent, Rectangle clipRect)
        {
            Texture = parent.Texture;
            ClipRect = parent.GetRelativeRect(clipRect.X, clipRect.Y, clipRect.Width, clipRect.Height);
            Width = clipRect.Width;
            Height = clipRect.Height;
            Size = new Vector2(Width, Height);
            SetUtil();
        }

        public MTexture(int width, int height, Color color)
        {
            Texture = new Texture2D(Engine.Instance.GraphicsDevice, width, height);
            var colors = new Color[width * height];
            for (int i = 0; i < width * height; i++)
            {
                colors[i] = color;
            }
            Texture.SetData(colors);

            ClipRect = new Rectangle(0, 0, width, height);
            Width = width;
            Height = height;
            Size = new Vector2(Width, Height);
            SetUtil();
        }

        private void SetUtil()
        {
            Center = new Vector2(Width, Height) * 0.5f;
        }

        private Rectangle GetRelativeRect(int x, int y, int width, int height)
        {
            int atX = ClipRect.X + x;
            int atY = ClipRect.Y + y;

            int rX = MathHelper.Clamp(atX, ClipRect.Left, ClipRect.Right);
            int rY = MathHelper.Clamp(atY, ClipRect.Top, ClipRect.Bottom);
            int rW = Math.Max(0, Math.Min(atX + width, ClipRect.Right) - rX);
            int rH = Math.Max(0, Math.Min(atY + height, ClipRect.Bottom) - rY);

            return new Rectangle(rX, rY, rW, rH);
        }

        public void Draw(Vector2 position, Vector2 origin, float rotation, Vector2 scale, Color color, SpriteEffects effects)
        {
            RenderManager.SpriteBatch.Draw(Texture, position, ClipRect, color, rotation, origin, scale, effects, 0);
        }

        public void Draw(Rectangle destRectangle, Vector2 origin, float rotation, Vector2 scale, Color color, SpriteEffects effects)
        {
            RenderManager.SpriteBatch.Draw(Texture, destRectangle, ClipRect, color, rotation, origin, effects, 0);
        }

        public void Unload()
        {
            Console.WriteLine("Texture was unloaded");

            Texture.Dispose();
            Texture = null;
        }

    }
}
