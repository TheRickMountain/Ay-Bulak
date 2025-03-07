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
        private TimeOfDayManager timeOfDayManager;

        private CameraMovement cameraMovement;

        private Tilemap groundTilemap;
        private Tilemap groundTopTilemap;
        private Building[,] buildingsMap;
        private bool[,] collisionMap;

        private Entity buildingsList;
        private Entity itemsList;
        private Entity creaturesList;

        private Player _player;

        public GameLocation(int mapWidth, int mapHeight, TimeOfDayManager timeOfDayManager)
        {
            this.mapWidth = mapWidth;
            this.mapHeight = mapHeight;
            this.timeOfDayManager = timeOfDayManager;

            timeOfDayManager.DayChanged += OnDayChanged;

            cameraMovement = new CameraMovement();
            cameraMovement.Bounds = GetBoundaries();
            AddChild(cameraMovement);

            groundTilemap = new Tilemap(TilesetConnection.SidesAndCorners, 16, 16, mapWidth, mapHeight);
            groundTopTilemap = new Tilemap(TilesetConnection.SidesAndCorners, 16, 16, mapWidth, mapHeight);

            groundTilemap.Tileset = new MTileset(ResourcesManager.GetTexture("Tilesets", "ground_summer_tileset"), 16, 16);
            groundTopTilemap.Tileset = new MTileset(ResourcesManager.GetTexture("Tilesets", "ground_top_tileset"), 16, 16);

            AddChild(groundTilemap);
            AddChild(groundTopTilemap);

            buildingsMap = new Building[mapWidth, mapHeight];

            collisionMap = new bool[mapWidth, mapHeight];

            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    groundTilemap.SetCell(x, y, 0);

                    collisionMap[x,y] = true;
                }
            }

            buildingsList = new Entity();
            buildingsList.IsDepthSortEnabled = true;
            AddChild(buildingsList);

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
                            if (buildingsMap[x, y] != null) return false;

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
            else if (handItem is SeedItem seedItem)
            {
                if (groundTileId == 3 && buildingsMap[x, y] == null) return true;
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
            else if(handItem is SeedItem seedItem)
            {
                PlantItem plantBuilding = Engine.ItemsDatabase.GetItemByName<PlantItem>(seedItem.PlantName);

                TryBuild(x, y, plantBuilding, Direction.Down, plantBuilding.GroundPattern);
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

        private void OnDayChanged(int day)
        {
            foreach(Entity entity in buildingsList.GetChildren())
            {
                if (entity is Building building)
                {
                    building.OnDayChanged(day);
                }
            }
        }

        #region Buildings

        public bool TryBuild(int x, int y, BuildingItem buildingItem, Direction direction, string[,] groundPattern)
        {
            Vector2[,] tiles = CalculateBuildingTiles(x, y, groundPattern);

            // Проверка, можно ли разместить здание
            if (ValidateBuildingPlacement(tiles, groundPattern) == false)
                return false;

            // Размещение здания
            Building building;

            if(buildingItem is PlantItem plantItem)
            {
                building = new PlantBuilding(plantItem, direction, tiles);
            }
            else
            {
                building = new Building(buildingItem, direction, tiles);
            }

            building.LocalPosition = new Vector2(x, y) * Engine.TILE_SIZE;
            buildingsList.AddChild(building);

            // Размещаем здание на карте
            RegisterBuildingTiles(building, tiles);

            return true;
        }

        private bool ValidateBuildingPlacement(Vector2[,] tiles, string[,] groundPattern)
        {
            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    int tileX = (int)tiles[i, j].X;
                    int tileY = (int)tiles[i, j].Y;

                    if (!IsWithinBounds(tileX, tileY))
                        return false;

                    string patternId = groundPattern[i, j];
                    if (CheckGroundPattern(tileX, tileY, patternId) == false)
                        return false;
                }
            }

            return true;
        }

        private Vector2[,] CalculateBuildingTiles(int x, int y, string[,] groundPattern)
        {
            int width = groundPattern.GetLength(0);
            int height = groundPattern.GetLength(1);
            Vector2[,] tiles = new Vector2[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    tiles[i, j] = new Vector2(x + i, y + j);
                }
            }
            return tiles;
        }

        private void RegisterBuildingTiles(Building building, Vector2[,] tiles)
        {
            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    int tileX = (int)tiles[i, j].X;
                    int tileY = (int)tiles[i, j].Y;
                    buildingsMap[tileX, tileY] = building;
                }
            }
        }

        private bool IsWithinBounds(int x, int y)
        {
            return x >= 0 && x < mapWidth && y >= 0 && y < mapHeight;
        }

        public bool CheckGroundPattern(int x, int y, string groundPatternId)
        {
            if (buildingsMap[x, y] != null) return false;

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
