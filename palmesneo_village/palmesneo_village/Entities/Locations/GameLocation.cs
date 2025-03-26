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
        CoopHouseFloor = 6,
        AnimalHouseWall = 7
    }

    public enum GroundTopTile
    {
        None = -1,
        Moisture = 0
    }

    public class GameLocation : Entity
    {
        public string LocationId { get; private set; }
        public Vector2 MouseTile { get; private set; }

        public int MapWidth { get; private set; }
        public int MapHeight { get; private set; }

        private PathNodeMap pathNodeMap;

        private CameraMovement cameraMovement;

        private Tilemap groundTilemap;
        private Tilemap groundTopTilemap;
        private Tilemap floorPathTilemap;
        private Tilemap airTopTilemap;
        private Building[,] buildingsMap;
        private FloorPathItem[,] floorPathMap;
        private bool[,] collisionMap;
        private Teleport[,] teleportsMap;

        private Dictionary<Building, List<Teleport>> buildingsTeleports = new();

        private Entity entitiesList;
        private Entity itemsList;

        private Player _player;

        public GameLocation(string locationId, int mapWidth, int mapHeight)
        {
            LocationId = locationId;
            MapWidth = mapWidth;
            MapHeight = mapHeight;

            pathNodeMap = new PathNodeMap(mapWidth, mapHeight);

            cameraMovement = new CameraMovement();
            cameraMovement.Bounds = GetBoundaries();
            AddChild(cameraMovement);

            groundTilemap = new Tilemap(TilesetConnection.SidesAndCorners, 16, 16, mapWidth, mapHeight);
            groundTopTilemap = new Tilemap(TilesetConnection.SidesAndCorners, 16, 16, mapWidth, mapHeight);
            floorPathTilemap = new Tilemap(TilesetConnection.Sides, 16, 16, mapWidth, mapHeight);

            groundTilemap.Tileset = new MTileset(ResourcesManager.GetTexture("Tilesets", "ground_summer_tileset"), 16, 16);
            groundTopTilemap.Tileset = new MTileset(ResourcesManager.GetTexture("Tilesets", "ground_top_tileset"), 16, 16);
            floorPathTilemap.Tileset = new MTileset(ResourcesManager.GetTexture("Tilesets", "floor_path_tileset"), 16, 16);

            AddChild(groundTilemap);
            AddChild(groundTopTilemap);
            AddChild(floorPathTilemap);

            buildingsMap = new Building[mapWidth, mapHeight];

            floorPathMap = new FloorPathItem[mapWidth, mapHeight];

            collisionMap = new bool[mapWidth, mapHeight];

            teleportsMap = new Teleport[mapWidth, mapHeight];

            entitiesList = new Entity();
            entitiesList.IsDepthSortEnabled = true;
            AddChild(entitiesList);

            itemsList = new Entity();
            itemsList.IsDepthSortEnabled = true;
            AddChild(itemsList);

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

            UpdateTilePassability(x, y);
        }

        public GroundTile GetGroundTile(int x, int y)
        {
            return (GroundTile)groundTilemap.GetCell(x, y);
        }

        public void SetGroundTopTile(int x, int y, GroundTopTile groundTopTile)
        {
            groundTopTilemap.SetCell(x, y, (int)groundTopTile);

            UpdateTilePassability(x, y);
        }

        public GroundTopTile GetGroundTopTile(int x, int y)
        {
            return (GroundTopTile)groundTopTilemap.GetCell(x, y);
        }

        public void SetTileFloorPathItem(int x, int y, FloorPathItem floorPathItem)
        {
            if (floorPathItem == null)
            {
                floorPathMap[x, y] = null;
                floorPathTilemap.SetCell(x, y, -1);
            }
            else
            {
                floorPathMap[x, y] = floorPathItem;
                floorPathTilemap.SetCell(x, y, floorPathItem.TilesetIndex);
            }
        }

        public FloorPathItem GetTileFloorPathItem(int x, int y)
        {
            return floorPathMap[x, y];
        }

        public FloorPathItem GetTileFloorPathItem(Vector2 tile)
        {
            return floorPathMap[(int)tile.X, (int)tile.Y];
        }

        public void SetAirTile(int x, int y, int terrainId)
        {
            airTopTilemap.SetCell(x, y, terrainId);
        }

        public void AddAnimal(Animal animal)
        {
            entitiesList.AddChild(animal);
            animal.SetGameLocation(this);
        }

        public void RemoveAnimal(Animal animal)
        {
            entitiesList.RemoveChild(animal);
            animal.SetGameLocation(null);
        }

        public void SetPlayer(Player player)
        {
            if (_player != null)
            {
                throw new Exception("Player is already set!");
            }

            _player = player;

            entitiesList.AddChild(player);
            cameraMovement.Target = player;
        }

        public void RemovePlayer(Player player)
        {
            if (_player != player)
            {
                throw new Exception("Incrorrect player!");
            }

            entitiesList.RemoveChild(_player);
            cameraMovement.Target = null;

            _player = null;
        }

        public Rectangle GetBoundaries()
        {
            return new Rectangle(0, 0, MapWidth * Engine.TILE_SIZE, MapHeight * Engine.TILE_SIZE);
        }

        public IEnumerable<Vector2> GetNeighbourTiles(Vector2 tile, bool includeDiagonal)
        {
            int tileX = (int)tile.X;
            int tileY = (int)tile.Y;

            // Определяем смещения для соседних тайлов
            var directions = new List<(int dx, int dy)>
            {
                (-1, 0),  // Левый
                (1, 0),   // Правый
                (0, -1),  // Верхний
                (0, 1)    // Нижний
            };

            // Добавляем диагональные направления, если требуется
            if (includeDiagonal)
            {
                directions.AddRange(new[]
                {
                    (-1, -1), // Левый верхний
                    (1, -1),  // Правый верхний
                    (-1, 1),  // Левый нижний
                    (1, 1)    // Правый нижний
                });
            }

            // Возвращаем все действительные соседние тайлы
            foreach (var (dx, dy) in directions)
            {
                int newX = tileX + dx;
                int newY = tileY + dy;

                if (newX >= 0 && newX < MapWidth && newY >= 0 && newY < MapHeight)
                {
                    yield return new Vector2(newX, newY);
                }
            }
        }

        public bool CanInteractWithTile(int x, int y, Item handItem)
        {
            GroundTile groundTile = GetGroundTile(x, y);
            GroundTopTile groundTopTile = GetGroundTopTile(x, y);

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
            else if (building is ManualCrafterBuilding)
            {
                return true;
            }
            else if (building is GateBuilding)
            {
                return true;
            }

            if (handItem is ToolItem toolItem)
            {
                if (toolItem.ToolType == ToolType.Showel)
                {
                    if (building != null) return false;

                    if (groundTile == GroundTile.Grass || groundTile == GroundTile.Ground) return true;
                }
                else if (toolItem.ToolType == ToolType.WateringCan)
                {
                    if (building is WaterSourceBuilding) return true;

                    if (groundTile == GroundTile.Water) return true;

                    if (groundTile == GroundTile.FarmPlot && groundTopTile != GroundTopTile.Moisture) return true;
                }
                else if (toolItem.ToolType == ToolType.Pickaxe ||
                    toolItem.ToolType == ToolType.Axe ||
                    toolItem.ToolType == ToolType.Scythe)
                {
                    if (building != null && building.CanInteract(toolItem)) return true;

                    if (floorPathTilemap.GetCell(x, y) >= 0) return true;
                }
            }
            else if (handItem is SeedItem seedItem)
            {
                if (groundTile == GroundTile.FarmPlot && building == null) return true;
            }
            else if (handItem is TreeSeedItem treeSeedItem)
            {
                if (building != null) return false;

                if (groundTile == GroundTile.Ground || groundTile == GroundTile.Grass) return true;
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
                        plantBuilding.Interact(null);
                        return;
                    }
                }
                else if (building is BedBuilding)
                {
                    gameplayScene.StartNextDay();
                    return;
                }
                else if (building is ManualCrafterBuilding manualCrafterBuilding)
                {
                    gameplayScene.OpenPlayerInventoryUI(manualCrafterBuilding.CraftingRecipes);
                    return;
                }
                else if (building is GateBuilding gateBuilding)
                {
                    gateBuilding.Interact(null);
                    return;
                }
            }

            Item handItem = inventory.GetSlotItem(slotIndex);

            if (handItem is ToolItem toolItem)
            {
                toolItem.PlaySoundEffect();

                if (toolItem.ToolType == ToolType.Showel)
                {
                    SetGroundTile(x, y, GroundTile.FarmPlot);

                    playerEnergyManager.ConsumeEnergy(1);
                }
                else if (toolItem.ToolType == ToolType.WateringCan)
                {
                    if (GetGroundTile(x, y) == GroundTile.Water || building is WaterSourceBuilding)
                    {
                        inventory.AddSlotItemContentAmount(slotIndex, toolItem.Capacity);
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
                else if (toolItem.ToolType == ToolType.Pickaxe ||
                    toolItem.ToolType == ToolType.Axe ||
                    toolItem.ToolType == ToolType.Scythe)
                {
                    if (building != null)
                    {
                        building.Interact(toolItem);

                        playerEnergyManager.ConsumeEnergy(1);
                    }
                    else if (floorPathTilemap.GetCell(x, y) >= 0)
                    {
                        AddItem(new Vector2(x, y) * Engine.TILE_SIZE, new ItemContainer()
                        {
                            Item = Engine.ItemsDatabase.GetFloorPathItemByTilesetIndex(floorPathTilemap.GetCell(x, y)),
                            Quantity = 1
                        });

                        SetTileFloorPathItem(x, y, null);

                        playerEnergyManager.ConsumeEnergy(1);
                    }
                }
            }
            else if (handItem is SeedItem seedItem)
            {
                PlantItem plantItem = Engine.ItemsDatabase.GetItemByName<PlantItem>(seedItem.PlantName);

                Vector2[,] tiles = new Vector2[1, 1];
                tiles[0, 0] = new Vector2(x, y);

                Build(plantItem, tiles, Direction.Down);

                inventory.RemoveItem(handItem, 1, slotIndex);
            }
            else if (handItem is TreeSeedItem treeSeedItem)
            {
                TreeItem treeItem = Engine.ItemsDatabase.GetItemByName<TreeItem>(treeSeedItem.TreeName);

                Vector2[,] tiles = new Vector2[1, 1];
                tiles[0, 0] = new Vector2(x, y);

                Build(treeItem, tiles, Direction.Down);

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

                        if (groundTile == GroundTile.FarmPlot)
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

            foreach (Entity entity in entitiesList.GetChildren())
            {
                if (entity is Building building)
                {
                    building.OnDayChanged();
                }
            }
        }

        #region Buildings

        public Building GetBuilding(int x, int y)
        {
            return buildingsMap[x, y];
        }

        public bool TryBuild(BuildingItem buildingItem, int x, int y, Direction direction)
        {
            string[,] groundPattern = Calc.RotateMatrix(buildingItem.GroundPattern, direction);

            Vector2[,] tiles = Calc.GetVector2DArray(new Vector2(x, y), groundPattern.GetLength(0), groundPattern.GetLength(1));

            if (CanBuildHere(tiles, groundPattern))
            {
                Build(buildingItem, tiles, direction);

                return true;
            }

            return false;
        }

        public void RemoveBuilding(Building building)
        {
            entitiesList.RemoveChild(building);

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
            if (buildingItem is FloorPathItem floorPathItem)
            {
                SetTileFloorPathItem((int)tiles[0, 0].X, (int)tiles[0, 0].Y, floorPathItem);
            }
            else
            {
                Building building;

                if (buildingItem is PlantItem plantItem)
                {
                    building = new PlantBuilding(this, plantItem, direction, tiles);
                }
                else if (buildingItem is TreeItem treeItem)
                {
                    building = new TreeBuilding(this, treeItem, direction, tiles);
                }
                else if (buildingItem is WaterSourceItem waterSourceItem)
                {
                    building = new WaterSourceBuilding(this, waterSourceItem, direction, tiles);
                }
                else if (buildingItem is BedItem bedItem)
                {
                    building = new BedBuilding(this, bedItem, direction, tiles);
                }
                else if (buildingItem is ResourceItem resourceItem)
                {
                    building = new ResourceBuilding(this, resourceItem, direction, tiles);
                }
                else if (buildingItem is ManualCrafterItem manualCrafterItem)
                {
                    building = new ManualCrafterBuilding(this, manualCrafterItem, direction, tiles);
                }
                else if (buildingItem is WindowItem windowItem)
                {
                    building = new WindowBuilding(this, windowItem, direction, tiles);
                }
                else if (buildingItem is SprinklerItem sprinklerItem)
                {
                    building = new SprinklerBuilding(this, sprinklerItem, direction, tiles);
                }
                else if (buildingItem is GateItem gateItem)
                {
                    building = new GateBuilding(this, gateItem, direction, tiles);
                }
                else if(buildingItem is AnimalSpawnerItem animalSpawnerItem)
                {
                    building = new AnimalSpawnerBuilding(this, animalSpawnerItem, direction, tiles);
                }
                else
                {
                    building = new Building(this, buildingItem, direction, tiles);
                }

                building.LocalPosition = tiles[0, 0] * Engine.TILE_SIZE;
                building.Depth = (int)tiles[0, tiles.GetLength(1) - 1].Y * Engine.TILE_SIZE;
                entitiesList.AddChild(building);

                RegisterBuildingTiles(building, tiles);

                if (buildingItem.TeleportData != null)
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

                                if (buildingsTeleports.ContainsKey(building) == false)
                                {
                                    buildingsTeleports.Add(building, new List<Teleport>());
                                }

                                buildingsTeleports[building].Add(teleport);

                                CreateTeleport((int)teleportEnterTile.X, (int)teleportEnterTile.Y, teleport);

                                // Создаем локацию для строения
                                if (teleportData.Location == "house")
                                {
                                    HouseLocation location = new HouseLocation(
                                        enterLocationId, new Teleport(LocationId, teleportEnterTile + new Vector2(0, 1)));

                                    ((GameplayScene)Engine.CurrentScene).RegisterLocation(location);
                                }
                                else if (teleportData.Location == "coop")
                                {
                                    CoopLocation location = new CoopLocation(
                                        enterLocationId, new Teleport(LocationId, teleportEnterTile + new Vector2(0, 1)));

                                    ((GameplayScene)Engine.CurrentScene).RegisterLocation(location);
                                }
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

                    UpdateTilePassability(tileX, tileY);
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

                    UpdateTilePassability(tileX, tileY);
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
                case "E":
                    {
                        return (groundTile == GroundTile.Grass || groundTile == GroundTile.Ground) &&
                            floorPathMap[x, y] == null;
                    }
                case "F":
                    {
                        return groundTile == GroundTile.CoopHouseFloor;
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

        public void UpdateTilePassability(int x, int y)
        {
            collisionMap[x, y] = true;

            switch (GetGroundTile(x, y))
            {
                case GroundTile.AnimalHouseWall:
                case GroundTile.HouseWall:
                case GroundTile.Water:
                    collisionMap[x, y] = false;
                    break;
            }

            if (buildingsMap[x, y] != null)
            {
                if (buildingsMap[x, y] is GateBuilding gateBuilding)
                {
                    collisionMap[x, y] = gateBuilding.IsOpen;
                }
                else if (buildingsMap[x, y].IsPassable == false)
                {
                    collisionMap[x, y] = false;
                }
            }

            pathNodeMap.TryGetNode(x, y).IsWalkable = collisionMap[x, y];
        }

        public List<PathNode> FindPath(Vector2 startTile, Vector2 targetTile, bool adjacent)
        {
            PathNode startNode = pathNodeMap.TryGetNode((int)startTile.X, (int)startTile.Y);
            PathNode targetNode = pathNodeMap.TryGetNode((int)targetTile.X, (int)targetTile.Y);
            return pathNodeMap.FindPath(startNode, targetNode, adjacent);
        }

        public PathNode GetPathNode(int x, int y)
        {
            return pathNodeMap.TryGetNode(x, y);
        }
    }
}
