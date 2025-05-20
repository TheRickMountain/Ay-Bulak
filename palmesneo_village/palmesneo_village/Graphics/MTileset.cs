namespace palmesneo_village
{
    public class MTileset
    {
        public int Columns { get; private set; }
        public int Rows { get; private set; }

        private MTexture[,] tiles;

        public MTileset(MTexture texture, int tileWidth, int tileHeight)
        {
            Texture = texture;
            TileWidth = tileWidth;
            TileHeight = tileHeight;

            Columns = Texture.Width / TileWidth;
            Rows = Texture.Height / TileHeight;

            tiles = new MTexture[texture.Width / tileWidth, texture.Height / tileHeight];
            for (int x = 0; x < texture.Width / tileWidth; x++)
                for (int y = 0; y < texture.Height / tileHeight; y++)
                    tiles[x, y] = new MTexture(texture, x * tileWidth, y * tileHeight, tileWidth, tileHeight);
        }

        public MTexture Texture
        {
            get; private set;
        }

        public int TileWidth
        {
            get; private set;
        }

        public int TileHeight
        {
            get; private set;
        }

        public MTexture this[int x, int y]
        {
            get
            {
                return tiles[x, y];
            }
        }

        public MTexture this[int index]
        {
            get
            {
                if (index < 0)
                    return null;
                else
                    return tiles[index % tiles.GetLength(0), index / tiles.GetLength(0)];
            }
        }
    }
}
