using Microsoft.Xna.Framework;
using System;

namespace palmesneo_village
{
    public enum TilesetConnection
    {
        Individual,
        Sides,
        SidesAndCorners
    }

    public class Tilemap : Entity
    {
        private MTileset tileset;
        public MTileset Tileset 
        {
            get => tileset;
            set
            {
                if (tileset == value) return;

                tileset = value;

                isTilesetUpdated = true;
            }
        }
        public int TileColumns { get; }
        public int TileRows { get; }

        private TilesetConnection tilesetConnection = TilesetConnection.Individual;
        private bool connectTilesetWithTilemapBorders;
        private int tileSize;
        private int chunkSize;

        private int chunkColumns;
        private int chunkRows;

        private Chunk[,] chunks;

        private int[,] terrains;

        private bool isTilesetUpdated = false;

        public Tilemap(TilesetConnection tilesetConnection, bool connectTilesetWithTilemapBorders, 
            int tileSize, int chunkSize, int tileColumns, int tileRows)
        {
            this.tilesetConnection = tilesetConnection;
            this.connectTilesetWithTilemapBorders = connectTilesetWithTilemapBorders;
            this.tileSize = tileSize;
            this.chunkSize = chunkSize;
            TileColumns = tileColumns;
            TileRows = tileRows;

            CreateChunks();

            InitializeTerrains();
        }

        private void CreateChunks()
        {
            chunkColumns = TileColumns / chunkSize;
            chunkRows = TileRows / chunkSize;

            chunks = new Chunk[chunkColumns, chunkRows];

            for (int x = 0; x < chunkColumns; x++)
            {
                for (int y = 0; y < chunkRows; y++)
                {
                    int chunkX = x * chunkSize * tileSize;
                    int chunkY = y * chunkSize * tileSize;

                    chunks[x, y] = new Chunk(chunkX, chunkY, chunkSize, tileSize);
                }
            }
        }

        private void InitializeTerrains()
        {
            terrains = new int[TileColumns, TileRows];

            for (int x = 0; x < TileColumns; x++)
            {
                for (int y = 0; y < TileRows; y++)
                {
                    terrains[x, y] = -1;
                }
            }
        }

        public override void Update()
        {
            base.Update();

            if(isTilesetUpdated)
            {
                for (int x = 0; x < chunkColumns; x++)
                {
                    for (int y = 0; y < chunkRows; y++)
                    {
                        chunks[x, y].Tileset = Tileset;
                    }
                }

                isTilesetUpdated = false;
            }

            for (int x = 0; x < chunkColumns; x++)
            {
                for (int y = 0; y < chunkRows; y++)
                {
                    chunks[x, y].Update();
                }
            }
        }

        public override void Render()
        {
            for (int x = 0; x < chunkColumns; x++)
            {
                for (int y = 0; y < chunkRows; y++)
                {
                    chunks[x, y].Render();
                }
            }

            base.Render();
        }

        public void SetCell(int x, int y, int terrain)
        {
            if (IsWithinBounds(x, y) == false)
            {
                throw new Exception("Out of bounds!");
            }

            if (terrains[x, y] == terrain)
            {
                return;
            }

            terrains[x, y] = terrain;

            switch (tilesetConnection)
            {
                case TilesetConnection.Individual:
                    {
                        SetCellTileId(x, y, terrain);
                    }
                    break;
                case TilesetConnection.Sides:
                    {
                        UpdateCell(x, y);

                        UpdateCell(x - 1, y);
                        UpdateCell(x + 1, y);
                        UpdateCell(x, y - 1);
                        UpdateCell(x, y + 1);
                    }
                    break;
                case TilesetConnection.SidesAndCorners:
                    {
                        UpdateCell(x, y);

                        UpdateCell(x - 1, y);
                        UpdateCell(x + 1, y);
                        UpdateCell(x, y - 1);
                        UpdateCell(x, y + 1);

                        UpdateCell(x - 1, y - 1);
                        UpdateCell(x + 1, y - 1);
                        UpdateCell(x - 1, y + 1);
                        UpdateCell(x + 1, y + 1);
                    }
                    break;
            }
        }

        private void UpdateCell(int x, int y)
        {
            if (IsWithinBounds(x, y) == false)
            {
                return;
            }

            int terrain = terrains[x, y];

            if(terrain == -1)
            {
                SetCellTileId(x, y, -1);
                return;
            }

            switch(tilesetConnection)
            {
                case TilesetConnection.Sides:
                    {
                        bool top = GetCell(x, y - 1) == terrain;
                        bool left = GetCell(x - 1, y) == terrain;
                        bool right = GetCell(x + 1, y) == terrain;
                        bool bottom = GetCell(x, y + 1) == terrain;

                        if (connectTilesetWithTilemapBorders)
                        {
                            if(y - 1 < 0) top = true;

                            if(x - 1 < 0) left = true;

                            if(x + 1 >= TileColumns) right = true;

                            if(y + 1 >= TileRows) bottom = true;
                        }

                        int regionTileId = BitmaskGenerator.Get4BitBitmask(top, left, right, bottom);

                        int globalTileId = terrain * 16 + regionTileId;

                        SetCellTileId(x, y, globalTileId);
                    }
                    break;
                case TilesetConnection.SidesAndCorners:
                    {
                        bool top = GetCell(x, y - 1) == terrain;
                        bool left = GetCell(x - 1, y) == terrain;
                        bool right = GetCell(x + 1, y) == terrain;
                        bool bottom = GetCell(x, y + 1) == terrain;

                        bool topLeft = top && left ? GetCell(x - 1, y - 1) == terrain : false;
                        bool topRight = top && right ? GetCell(x + 1, y - 1) == terrain : false;
                        bool bottomLeft = bottom && left ? GetCell(x - 1, y + 1) == terrain : false;
                        bool bottomRight = bottom && right ? GetCell(x + 1, y + 1) == terrain : false;

                        if (connectTilesetWithTilemapBorders)
                        {
                            if (y - 1 < 0) top = true;

                            if (x - 1 < 0) left = true;

                            if (x + 1 >= TileColumns) right = true;

                            if (y + 1 >= TileRows) bottom = true;

                            if(y - 1 < 0)
                            {
                                topLeft = true;
                                topRight = true;
                            }

                            if(x - 1 < 0)
                            {
                                topLeft = true;
                                bottomLeft = true;
                            }

                            if (y + 1 >= TileColumns)
                            {
                                bottomLeft = true;
                                bottomRight = true;
                            }

                            if (x + 1 >= TileRows)
                            {
                                topRight = true;
                                bottomRight = true;
                            }
                        }

                        int bitmask = BitmaskGenerator.Get8BitBitmask(topLeft, top, topRight, left, 
                            right, bottomLeft, bottom, bottomRight);

                        int regionTileId = BitmaskGenerator.GetBitmask8BitTileId(bitmask);

                        int offsetX = (terrain % 4) * 8;
                        int offsetY = (terrain / 4) * 6;

                        int regionTileX = regionTileId % 8;
                        int regionTileY = regionTileId / 8;

                        int globalTileId = (regionTileX + offsetX) + (regionTileY + offsetY) * 32;

                        SetCellTileId(x, y, globalTileId);
                    }
                    break;
            }
        }

        public int GetCell(int x, int y)
        {
            if (IsWithinBounds(x, y) == false)
            {
                return -1;
            }

            return terrains[x, y];
        }

        private void SetCellTileId(int x, int y, int tileId)
        {
            if(IsWithinBounds(x, y) == false)
            {
                throw new Exception("Out of bounds!");
            }

            int chunkX = x / chunkSize;
            int chunkY = y / chunkSize;

            int chunkTileX = x - chunkX * chunkSize;
            int chunkTileY = y - chunkY * chunkSize;

            chunks[chunkX, chunkY].SetCell(chunkTileX, chunkTileY, tileId, Color.White);
        }

        public Vector2 WorldToMap(Vector2 vector)
        {
            return new Vector2((int)(vector.X / tileSize), (int)(vector.Y / tileSize));
        }

        public Vector2 MapToWorld(Vector2 vector)
        {
            return vector * tileSize;
        }

        private bool IsWithinBounds(int x, int y)
        {
            return x >= 0 && y >= 0 && x < TileColumns && y < TileRows;
        }
    }
}
