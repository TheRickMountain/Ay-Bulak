using Microsoft.Win32;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace palmesneo_village
{
    public enum GroundTile
    {
        Grass = 0,
        Ground = 1,
        Water = 2,
        FarmPlot = 3,
        HouseFloor = 4,
        HouseWall = 5,
        Plinth = 6
    }

    public class GameLocation : Entity
    {
        public string Id { get; private set; }
        public Vector2 MouseTile { get; private set; }

        public int MapWidth { get; private set; }
        public int MapHeight { get; private set; }
        private TimeOfDayManager timeOfDayManager;

        private CameraMovement cameraMovement;

        private Tilemap groundTilemap;
        private Tilemap groundTopTilemap;
        private Tilemap buildingTopTilemap;
        private Building[,] buildingsMap;
        private bool[,] collisionMap;

        private Entity buildingsList;
        private Entity itemsList;
        private Entity creaturesList;

        private Player _player;

        public GameLocation(string id, int mapWidth, int mapHeight)
        {
            Id = id;
            MapWidth = mapWidth;
            MapHeight = mapHeight;

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

            buildingsList = new Entity();
            buildingsList.IsDepthSortEnabled = true;
            AddChild(buildingsList);

            itemsList = new Entity();
            itemsList.IsDepthSortEnabled = true;
            AddChild(itemsList);

            creaturesList = new Entity();
            creaturesList.IsDepthSortEnabled = true;
            AddChild(creaturesList);

            buildingTopTilemap = new Tilemap(TilesetConnection.Individual, 16, 16, mapWidth, mapHeight);
            buildingTopTilemap.Tileset = new MTileset(ResourcesManager.GetTexture("Tilesets", "building_top_tileset"), 16, 16);
            AddChild(buildingTopTilemap);
        }

        public override void Update()
        {
            base.Update();

            MouseTile = Vector2.Clamp(WorldToMap(MInput.Mouse.GlobalPosition), Vector2.Zero, new Vector2(MapWidth - 1, MapHeight - 1));
        }

        public void SetGroundTile(int x, int y, GroundTile groundTile)
        {
            groundTilemap.SetCell(x, y, (int)groundTile);

            switch(groundTile)
            {
                case GroundTile.Water:
                case GroundTile.HouseWall:
                case GroundTile.Plinth:
                    collisionMap[x, y] = false;
                    break;
                default:
                    collisionMap[x, y] = true;
                    break;
            }
        }

        public void SetBuildingTopTile(int x, int y, int terrainId)
        {
            buildingTopTilemap.SetCell(x, y, terrainId);
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
            return new Rectangle(0, 0, MapWidth * Engine.TILE_SIZE, MapHeight * Engine.TILE_SIZE);
        }

        public bool CanInteractWithTile(int x, int y, Item handItem)
        {
            int groundTileId = groundTilemap.GetCell(x, y);
            int groundTopTileId = groundTopTilemap.GetCell(x, y);

            Building building = buildingsMap[x, y];

            if (building != null)
            {
                if(building is PlantBuilding plantBuilding)
                {
                    if(plantBuilding.IsRipe)
                    {
                        return true;
                    }
                }
            }

            if (handItem is ToolItem toolItem)
            {
                if (handItem is ShowelItem showelItem)
                {
                    if (buildingsMap[x, y] != null) return false;

                    if (groundTileId == 0 || groundTileId == 1) return true;
                }
                else if (handItem is WateringCanItem wateringCanItem)
                {
                    if (groundTileId == 3 && groundTopTileId != 0) return true;
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
            Building building = buildingsMap[x, y];

            if (building != null)
            {
                if(building is PlantBuilding plantBuilding)
                {
                    if(plantBuilding.IsRipe)
                    {
                        plantBuilding.Harvest();
                        return;
                    }
                }
            }

            if (handItem is ToolItem toolItem)
            {
                if (handItem is ShowelItem showelItem)
                {
                    groundTilemap.SetCell(x, y, 3);
                }
                else if (handItem is WateringCanItem wateringCanItem)
                {
                    groundTopTilemap.SetCell(x, y, 0);
                }
            }
            else if(handItem is SeedItem seedItem)
            {
                PlantItem plantBuilding = Engine.ItemsDatabase.GetItemByName<PlantItem>(seedItem.PlantName);

                Vector2[,] tiles = new Vector2[1, 1];
                tiles[0, 0] = new Vector2(x, y);

                Build(plantBuilding, tiles, Direction.Down);
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

        public void StartNextDay()
        {
            foreach (Entity entity in buildingsList.GetChildren())
            {
                if (entity is Building building)
                {
                    building.OnDayChanged();
                }
            }
        }

        #region Buildings

        public void Build(BuildingItem buildingItem, Vector2[,] tiles, Direction direction)
        {
            Building building;

            if(buildingItem is PlantItem plantItem)
            {
                building = new PlantBuilding(this, plantItem, direction, tiles);
            }
            else if(buildingItem is WaterSourceItem waterSourceItem)
            {
                building = new WaterSourceBuilding(this, waterSourceItem, direction, tiles);
            }
            else
            {
                building = new Building(this, buildingItem, direction, tiles);
            }

            building.LocalPosition = tiles[0, 0] * Engine.TILE_SIZE;
            buildingsList.AddChild(building);

            RegisterBuildingTiles(building, tiles);
        }

        public void RemoveBuilding(Building building)
        {
            buildingsList.RemoveChild(building);

            Vector2[,] tiles = building.OccupiedTiles;

            ClearTilesFromBuilding(tiles);
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

        private void ClearTilesFromBuilding(Vector2[,] tiles)
        {
            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    int tileX = (int)tiles[i, j].X;
                    int tileY = (int)tiles[i, j].Y;
                    buildingsMap[tileX, tileY] = null;
                }
            }
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

        public void AddItem(Vector2 position, ItemContainer itemContainer)
        {
            LocationItem locationItem = new LocationItem(itemContainer);
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
