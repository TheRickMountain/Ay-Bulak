using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace palmesneo_village
{
    public class Chunk
    {
        private MTileset tileset;
        public MTileset Tileset 
        {
            get => tileset;
            set
            {
                tileset = value;

                isDirty = true;
            }
        }

        private int chunkSize;
        private int tileSize;

        private int[,] tiles;
        private Color[,] colors;

        private RenderTarget2D renderTarget;

        private int renderTargetSize;

        private bool isDirty;

        private Rectangle rect;

        public Chunk(int x, int y, int chunkSize, int tileSize)
        {
            this.chunkSize = chunkSize;
            this.tileSize = tileSize;

            tiles = new int[chunkSize, chunkSize];
            colors = new Color[chunkSize, chunkSize];

            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    tiles[i, j] = -1;
                    colors[i, j] = Color.White;
                }
            }

            renderTargetSize = tileSize * chunkSize;

            GraphicsDevice graphicsDevice = Engine.Instance.GraphicsDevice;

            renderTarget = new RenderTarget2D(
                graphicsDevice, 
                renderTargetSize, 
                renderTargetSize, 
                false,
                graphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.None);

            rect.X = x; 
            rect.Y = y;
            rect.Width = renderTargetSize;
            rect.Height = renderTargetSize;

            isDirty = true;
        }

        public void Update()
        {
            if (isDirty == false) return;

            DrawSceneToTexture(renderTarget);

            isDirty = false;
        }

        public void Render()
        {
            RenderManager.SpriteBatch.Draw(renderTarget, rect, Color.White);
        }

        private void DrawSceneToTexture(RenderTarget2D renderTarget)
        {
            // Set the render target
            Engine.Instance.GraphicsDevice.SetRenderTarget(renderTarget);

            // Draw the scene
            Engine.Instance.GraphicsDevice.Clear(Color.Transparent);

            RenderManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);

            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    int tileId = tiles[x, y];

                    if (tileId == -1)
                        continue;

                    Tileset?[tileId].Draw(new Vector2(x * tileSize, y * tileSize), Vector2.Zero, 0, Vector2.One, colors[x, y], SpriteEffects.None);
                }
            }

            RenderManager.SpriteBatch.End();

            // Drop the render target
            Engine.Instance.GraphicsDevice.SetRenderTarget(null);
        }

        public void SetCell(int x, int y, int tile, Color color)
        {
            if (tiles[x, y] == tile && colors[x, y] == color)
                return;

            tiles[x, y] = tile;
            colors[x, y] = color;

            isDirty = true;
        }

        public int GetCell(int x, int y)
        {
            return tiles[x, y];
        }

    }
}
