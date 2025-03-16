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
        AnimalHouseFloor = 6,
        AnimalHouseWall = 7
    }

    public enum GroundTopTile
    {
        None = -1,
        Moisture = 0,
        Stone = 1,
        Wood = 2,
        Gate = 3
    }

    public class GameLocation : Entity
    {
        public string LocationId { get; private set; }
        public Vector2 MouseTile { get; private set; }

        public int MapWidth { get; private set; }
        public int MapHeight { get; private set; }

        private CameraMovement cameraMovement;

        private Tilemap groundTilemap;
        private Tilemap groundTopTilemap;
        private Tilemap airTopTilemap;
        private Building[,] buildingsMap;
        private bool[,] collisionMap;
        private Teleport[,] teleportsMap;

        private Dictionary<Building, List<Teleport>> buildingsTeleports = new();

        private Entity buildingsList;
        private Entity itemsList;
        private Entity creaturesList;

        private Player _player;

        public GameLocation(string locationId, int mapWidth, int mapHeight)
        {
            LocationId = locationId;
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

            teleportsMap = new Teleport[mapWidth, mapHeight];

            buildingsList = new Entity();
            buildingsList.IsDepthSortEnabled = true;
            AddChild(buildingsList);

            itemsList = new Entity();
            itemsList.IsDepthSortEnabled = true;
            AddChild(itemsList);

            creaturesList = new Entity();
            creaturesList.IsDepthSortEnabled = true;
            AddChild(creaturesList);

            airTopTilemap = new Tilemap(TilesetConnection.Individual, 16, 16, mapWidth, mapHeight);
            airTopTilemap.Tileset = new MTileset(ResourcesManager.GetTexture("Tilesets", "air_tileset"), 16, 16);
            AddChild(airTopTilemap);
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
                case GroundTile.AnimalHouseWall:
                    collisionMap[x, y] = false;
                    break;
                default:
                    collisionMap[x, y] = true;
                    break;
            }
        }

        public GroundTile GetGroundTile(int x, int y)
        {
            return (GroundTile)groundTilemap.GetCell(x, y);
        }

        public void SetGroundTopTile(int x, int y, GroundTopTile groundTopTile)
        {
            groundTopTilemap.SetCell(x, y, (int)groundTopTile);
        }

        public GroundTopTile GetGroundTopTile(int x, int y)
        {
            return (GroundTopTile)groundTopTilemap.GetCell(x, y);
        }

        public void SetAirTile(int x, int y, int terrainId)
        {
            airTopTilemap.SetCell(x, y, terrainId);
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
            GroundTile groundTile = GetGroundTile(x, y);
            int groundTopTileId = groundTopTilemap.GetCell(x, y);

            Building building = buildingsMap[x, y];

            if (teleportsMap[x, y] != null)
            {
                return true;
            }

            if (building is PlantBuilding plantBuilding)
            {
                if (plantBuilding.IsRipe)
                {
                    return true;
                }
            }
            else if (building is BedBuilding)
            {
                return true;
            }

            if (handItem is ToolItem toolItem)
            {
                if (handItem is ShowelItem showelItem)
                {
                    if (building != null) return false;

                    if (groundTile == GroundTile.Grass || groundTile == GroundTile.Ground) return true;
                }
                else if (handItem is WateringCanItem wateringCanItem)
                {
                    if (building is WaterSourceBuilding) return true;

                    if (groundTile == GroundTile.Water) return true;

                    if (groundTile == GroundTile.FarmPlot && groundTopTileId != 0) return true;
                }
                else if (handItem is PickaxeItem pickaxeItem)
                {
                    if (building != null) return false;

                    if (GetGroundTopTile(x, y) == GroundTopTile.Stone) return true;
                }
                else if (handItem is AxeItem axeItem)
                {
                    if (building != null) return false;

                    if (GetGroundTopTile(x, y) == GroundTopTile.Wood) return true;
                }
            }
            else if (handItem is SeedItem seedItem)
            {
                if (groundTile == GroundTile.FarmPlot && building == null) return true;
            }

            return false;
        }

        public void InteractWithTile(int x, int y, Inventory inventory, int slotIndex, PlayerEnergyManager playerEnergyManager,
            GameplayScene gameplayScene)
        {
            if (teleportsMap[x, y] != null)
            {
                Teleport teleport = teleportsMap[x, y];
                gameplayScene.GoToLocation(teleport.Location, teleport.Tile);
                return;
            }

            Building building = buildingsMap[x, y];

            if (building != null)
            {
                if (building is PlantBuilding plantBuilding)
                {
                    if (plantBuilding.IsRipe)
                    {
                        plantBuilding.Harvest();
                        return;
                    }
                }
                else if(building is BedBuilding)
                {
                    gameplayScene.StartNextDay();
                    return;
                }
            }

            Item handItem = inventory.GetSlotItem(slotIndex);

            if (handItem is ToolItem toolItem)
            {
                if (handItem is ShowelItem showelItem)
                {
                    groundTilemap.SetCell(x, y, 3);

                    playerEnergyManager.ConsumeEnergy(1);
                }
                else if (handItem is WateringCanItem wateringCanItem)
                {
                    if (GetGroundTile(x, y) == GroundTile.Water || building is WaterSourceBuilding)
                    {
                        inventory.AddSlotItemContentAmount(slotIndex, wateringCanItem.Capacity);
                    }
                    else
                    {
                        if (inventory.GetSlotContentAmount(slotIndex) > 0)
                        {
                            inventory.SubSlotItemContentAmount(slotIndex, 1);

                            SetGroundTopTile(x, y, GroundTopTile.Moisture);

                            playerEnergyManager.ConsumeEnergy(1);
                        }
                        else
                        {
                            //TODO: дать знать игроку, что лейка пуста
                        }
                    }
                }
                else if (handItem is PickaxeItem pickaxeItem)
                {
                    SetGroundTopTile(x, y, GroundTopTile.None);

                    Item spawnItem = Engine.ItemsDatabase.GetItemByName<Item>("stone");

                    AddItem(new Vector2(x, y) * Engine.TILE_SIZE, new ItemContainer()
                    {
                        Item = spawnItem,
                        Quantity = Calc.Random.Range(1, 2)
                    });

                    playerEnergyManager.ConsumeEnergy(1);
                }
                else if(handItem is AxeItem axeItem)
                {
                    SetGroundTopTile(x, y, GroundTopTile.None);

                    Item spawnItem = Engine.ItemsDatabase.GetItemByName<Item>("wood");

                    AddItem(new Vector2(x, y) * Engine.TILE_SIZE, new ItemContainer()
                    {
                        Item = spawnItem,
                        Quantity = Calc.Random.Range(1, 2)
                    });

                    playerEnergyManager.ConsumeEnergy(1);
                }
            }
            else if(handItem is SeedItem seedItem)
            {
                PlantItem plantBuilding = Engine.ItemsDatabase.GetItemByName<PlantItem>(seedItem.PlantName);

                Vector2[,] tiles = new Vector2[1, 1];
                tiles[0, 0] = new Vector2(x, y);

                Build(plantBuilding, tiles, Direction.Down);

                inventory.RemoveItem(handItem, 1, slotIndex);
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

        public void StartNextDay(TimeOfDayManager timeOfDayManager)
        {
            if (timeOfDayManager.CurrentWeather == Weather.Rainy)
            {
                for (int x = 0; x < MapWidth; x++)
                {
                    for (int y = 0; y < MapHeight; y++)
                    {
                        GroundTile groundTile = GetGroundTile(x, y);

                        if(groundTile == GroundTile.FarmPlot)
                        {
                            SetGroundTopTile(x, y, GroundTopTile.Moisture);
                        }
                    }
                }
            }
            else
            {
                for (int x = 0; x < MapWidth; x++)
                {
                    for (int y = 0; y < MapHeight; y++)
                    {
                        GroundTopTile groundTopTile = GetGroundTopTile(x, y);
                        if (groundTopTile == GroundTopTile.Moisture)
                        {
                            SetGroundTopTile(x, y, GroundTopTile.None);
                        }
                    }
                }
            }

            foreach (Entity entity in buildingsList.GetChildren())
            {
                if (entity is Building building)
                {
                    building.OnDayChanged();
                }
            }
        }

        #region Buildings

        public bool TryBuild(BuildingItem buildingItem, int x, int y, Direction direction)
        {
            string[,] groundPattern = Calc.RotateMatrix(buildingItem.GroundPattern, direction);

            Vector2[,] tiles = Calc.GetVector2DArray(new Vector2(x, y), groundPattern.GetLength(0), groundPattern.GetLength(1));

            if (CanBuildHere(tiles, groundPattern) == false) return false;

            Build(buildingItem, tiles, direction);

            return true;
        }

        public void RemoveBuilding(Building building)
        {
            buildingsList.RemoveChild(building);

            Vector2[,] tiles = building.OccupiedTiles;

            ClearTilesFromBuilding(tiles);

            RemoveBuildingTeleports(building);
        }

        private bool CanBuildHere(Vector2[,] tiles, string[,] groundPattern)
        {
            int tileX = (int)tiles[0, 0].X;
            int tileY = (int)tiles[0, 0].Y;

            foreach (var checkTile in tiles)
            {
                int offsetX = (int)checkTile.X - tileX;
                int offsetY = (int)checkTile.Y - tileY;

                if (offsetX < 0 || offsetY < 0 ||
                    offsetX >= groundPattern.GetLength(0) ||
                    offsetY >= groundPattern.GetLength(1))
                    continue;

                string groundPatternId = groundPattern[offsetX, offsetY];
                if (!CheckGroundPattern((int)checkTile.X, (int)checkTile.Y, groundPatternId))
                {
                    return false;
                }
            }

            return true;
        }

        private void Build(BuildingItem buildingItem, Vector2[,] tiles, Direction direction)
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
            else if(buildingItem is BedItem bedItem)
            {
                building = new BedBuilding(this, bedItem, direction, tiles);
            }
            else
            {
                building = new Building(this, buildingItem, direction, tiles);
            }

            building.LocalPosition = tiles[0, 0] * Engine.TILE_SIZE;
            buildingsList.AddChild(building);

            RegisterBuildingTiles(building, tiles);
        
            if(buildingItem.TeleportData != null)
            {
                TeleportData teleportData = buildingItem.TeleportData;

                int[,] teleportPattern = Calc.RotateMatrix(teleportData.TeleportPattern, direction);

                for (int x = 0; x < teleportPattern.GetLength(0); x++)
                {
                    for (int y = 0; y < teleportPattern.GetLength(1); y++)
                    {
                        if (teleportPattern[x, y] == 1)
                        {
                            Vector2 teleportEnterTile = tiles[x, y];

                            string enterLocationId = teleportData.Location + "_" + building.Id;
                            Vector2 enterLocationSpawnPosition = new Vector2(teleportData.X, teleportData.Y);

                            Teleport teleport = new Teleport(enterLocationId, enterLocationSpawnPosition);

                            if(buildingsTeleports.ContainsKey(building) == false)
                            {
                                buildingsTeleports.Add(building, new List<Teleport>());
                            }

                            buildingsTeleports[building].Add(teleport);

                            CreateTeleport((int)teleportEnterTile.X, (int)teleportEnterTile.Y, teleport);

                            // Создаем локацию для строения
                            if (teleportData.Location == "house")
                            {
                                HouseLocation location = new HouseLocation(
                                    enterLocationId, new Teleport(LocationId, teleportEnterTile));

                                ((GameplayScene)Engine.CurrentScene).RegisterLocation(location);
                            }
                            else if(teleportData.Location == "coop")
                            {
                                CoopLocation location = new CoopLocation(
                                    enterLocationId, new Teleport(LocationId, teleportEnterTile));

                                ((GameplayScene)Engine.CurrentScene).RegisterLocation(location);
                            }
                        }
                    }
                }
            }
        }

        private void RemoveBuildingTeleports(Building building)
        {
            if (buildingsTeleports.ContainsKey(building) == false)
            {
                return;
            }

            foreach (Teleport teleport in buildingsTeleports[building])
            {
                RemoveTeleport(teleport);
            }

            buildingsTeleports[building].Clear();
            buildingsTeleports.Remove(building);
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

            GroundTile groundTile = GetGroundTile(x, y);

            switch (groundPatternId)
            {
                case "A":
                    {
                        return groundTile == GroundTile.Grass || groundTile == GroundTile.Ground;
                    }
                case "B":
                    {
                        return groundTile == GroundTile.FarmPlot;
                    }
                case "C":
                    {
                        return groundTile == GroundTile.HouseWall;
                    }
                case "D":
                    {
                        return groundTile == GroundTile.HouseFloor;
                    }
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

        protected void CreateTeleport(int x, int y, Teleport teleport)
        {
            if (teleportsMap[x, y] != null)
            {
                throw new Exception("Teleport already exists!");
            }

            teleportsMap[x, y] = teleport;
        }

        protected void RemoveTeleport(Teleport teleport)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    if (teleportsMap[x, y] == teleport)
                    {
                        teleportsMap[x, y] = null;
                        return;
                    }
                }
            }
        }

    }
}
