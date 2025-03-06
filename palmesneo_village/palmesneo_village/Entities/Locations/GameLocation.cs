using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace palmesneo_village
{
    public class GameLocation : Entity
    {
        private int mapWidth;
        private int mapHeight;

        private CameraMovement cameraMovement;

        private Tilemap groundTilemap;
        private Tilemap groundTopTilemap;
        private bool[,] collisionMap;

        private Entity itemsList;
        private Entity creaturesList;

        private Player _player;

        public GameLocation(int mapWidth, int mapHeight)
        {
            this.mapWidth = mapWidth;
            this.mapHeight = mapHeight;

            cameraMovement = new CameraMovement();
            cameraMovement.Bounds = GetBoundaries();
            AddChild(cameraMovement);

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

            itemsList = new Entity();
            itemsList.IsDepthSortEnabled = true;
            AddChild(itemsList);

            creaturesList = new Entity();
            creaturesList.IsDepthSortEnabled = true;
            AddChild(creaturesList);

            groundTilemap.SetCell(10, 10, 2);
            collisionMap[10, 10] = false;

            groundTilemap.SetCell(12, 10, 2);
            collisionMap[12, 10] = false;

            groundTilemap.SetCell(13, 10, 2);
            collisionMap[13, 10] = false;
        }

        public void SetPlayer(Player player)
        {
            if(_player != null)
            {
                throw new Exception("Player is already set!");
            }

            _player = player;

            creaturesList.AddChild(player);
            cameraMovement.Target = player;
        }

        public void RemovePlayer(Player player)
        {
            if (_player != player)
            {
                throw new Exception("Incrorrect player!");
            }

            creaturesList.RemoveChild(_player);
            cameraMovement.Target = null;

            _player = null;
        }

        public Rectangle GetBoundaries()
        {
            return new Rectangle(0, 0, mapWidth * Engine.TILE_SIZE, mapHeight * Engine.TILE_SIZE);
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

        public bool TryBuild(int x, int y, BuildingItem buildingItem, Direction direction, string[,] groundPattern)
        {
            // TODO: complete the code

            return true;
        }

        public bool CheckGroundPattern(int x, int y, string groundPatternId)
        {
            // TODO: if tile there is any building - return false

            switch(groundPatternId)
            {
                case "A":
                    {
                        int groundTileId = groundTilemap.GetCell(x, y);
                        if (groundTileId == 0 || groundTileId == 1) return true;
                    }
                    break;
                case "B":
                    {
                        int groundTileId = groundTilemap.GetCell(x, y);
                        if (groundTileId == 3) return true;
                    }
                    break;
            }

            return false;
        }

        #endregion

        #region Items

        public void AddItem(Vector2 position, Item item, int quantity)
        {
            LocationItem locationItem = new LocationItem(item, quantity);
            locationItem.LocalPosition = position;
            locationItem.Depth = (int)position.Y;
            itemsList.AddChild(locationItem);
        }

        public void RemoveItem(LocationItem item)
        {
            itemsList.RemoveChild(item);
        }

        public IEnumerable<LocationItem> GetLocationItems(Vector2 position)
        {
            List<LocationItem> items = itemsList.GetChildren<LocationItem>().ToList();

            foreach (LocationItem item in items)
            {
                yield return item;
            }
        }

        #endregion
    }
}
