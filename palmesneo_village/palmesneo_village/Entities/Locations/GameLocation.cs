using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace palmesneo_village
{
    public class GameLocation : Entity
    {

        private Tilemap groundTilemap;
        private Tilemap groundTopTilemap;
        private bool[,] collisionMap;
        private Building[,] buildingMap;

        private Entity itemList;

        public GameLocation(int mapWidth, int mapHeight)
        {
            groundTilemap = new Tilemap(TilesetConnection.SidesAndCorners, 16, 16, mapWidth, mapHeight);
            groundTopTilemap = new Tilemap(TilesetConnection.SidesAndCorners, 16, 16, mapWidth, mapHeight);

            groundTilemap.Tileset = new MTileset(ResourcesManager.GetTexture("Tilesets", "ground_summer_tileset"), 16, 16);
            groundTopTilemap.Tileset = new MTileset(ResourcesManager.GetTexture("Tilesets", "ground_top_tileset"), 16, 16);

            AddChild(groundTilemap);
            AddChild(groundTopTilemap);

            collisionMap = new bool[mapWidth, mapHeight];

            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    groundTilemap.SetCell(x, y, 0);

                    collisionMap[x,y] = true;
                }
            }

            buildingMap = new Building[mapWidth, mapHeight];

            groundTilemap.SetCell(10, 10, 2);
            collisionMap[10, 10] = false;

            groundTilemap.SetCell(12, 10, 2);
            collisionMap[12, 10] = false;

            groundTilemap.SetCell(13, 10, 2);
            collisionMap[13, 10] = false;

            itemList = new Entity();
            itemList.IsDepthSortEnabled = true;
            AddChild(itemList);
        }

        public Rectangle GetBoundaries()
        {
            return new Rectangle(0, 0, groundTilemap.TileColumns * Engine.TILE_SIZE, groundTilemap.TileRows * Engine.TILE_SIZE);
        }

        public bool CanInteractWithTile(int x, int y, Item handItem)
        {
            int groundTileId = groundTilemap.GetCell(x, y);
            int groundTopTileId = groundTopTilemap.GetCell(x, y);

            if (handItem is ToolItem toolItem)
            {
                switch (toolItem.ToolType)
                {
                    case ToolType.Hoe:
                        {
                            if (groundTileId == 0 || groundTileId == 1) return true;
                        }
                        break;
                    case ToolType.WateringCan:
                        {
                            if (groundTileId == 3 && groundTopTileId != 0) return true;
                        }
                        break;
                }
            }

            return false;
        }

        public void InteractWithTile(int x, int y, Item handItem)
        {
            if (handItem is ToolItem toolItem)
            {
                switch (toolItem.ToolType)
                {
                    case ToolType.Hoe:
                        {
                            groundTilemap.SetCell(x, y, 3);
                        }
                        break;
                    case ToolType.WateringCan:
                        {
                            groundTopTilemap.SetCell(x, y, 0);
                        }
                        break;
                }
            }
        }

        public bool IsTilePassable(int x, int y)
        {
            return collisionMap[x, y];
        }

        public Vector2 WorldToMap(Vector2 vector)
        {
            return groundTilemap.WorldToMap(vector);
        }

        public Vector2 MapToWorld(Vector2 vector)
        {
            return groundTilemap.MapToWorld(vector);
        }

        #region Buildings

        public void BuildBuilding(int x, int y, BuildingItem buildingItem, Direction direction)
        {
            // TODO: complete the code
        }

        public bool CanBuildBuilding(int x, int y, BuildingItem buildingItem, Direction direction)
        {
            // TODO: complete the code

            return true;
        }

        #endregion

        #region Items

        public void AddItem(Vector2 position, Item item, int quantity)
        {
            LocationItem locationItem = new LocationItem(item, quantity);
            locationItem.LocalPosition = position;
            locationItem.Depth = (int)position.Y;
            itemList.AddChild(locationItem);
        }

        public void RemoveItem(LocationItem item)
        {
            itemList.RemoveChild(item);
        }

        public IEnumerable<LocationItem> GetLocationItems(Vector2 position)
        {
            List<LocationItem> items = itemList.GetChildren<LocationItem>().ToList();

            foreach (LocationItem item in items)
            {
                yield return item;
            }
        }

        #endregion
    }
}
