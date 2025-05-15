using Microsoft.Win32;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        AnimalHouseWall = 7,
        TentFloor = 8,
        TentWall = 9
    }

    public enum GroundTopTile
    {
        None = -1,
        Moisture = 0
    }

    public enum AirTile
    {
        None = -1,
        Forest = 0
    }

    public class GameLocation : Entity
    {
        public string LocationId { get; private set; }
        public Vector2 MouseTile { get; private set; }

        public int MapWidth { get; private set; }
        public int MapHeight { get; private set; }

        private bool isOutdoor;

        private PathNodeMap pathNodeMap;

        private CameraMovement cameraMovement;

        private Tilemap groundTilemap;
        private Tilemap groundTopTilemap;
        private Tilemap floorPathTilemap;
        private Tilemap airTilemap;
        private Building[,] buildingsMap;
        private FloorPathItem[,] floorPathMap;
        private bool[,] collisionMap;
        private Teleport[,] teleportsMap;

        private Dictionary<Building, List<Teleport>> buildingsTeleports = new();

        private Entity entitiesList;
        private Entity itemsList;
        private RainEffectEntity rainEffect;

        private Player _player;

        public GameLocation(string locationId, int mapWidth, int mapHeight, bool isOutdoor)
        {
            LocationId = locationId;
            MapWidth = mapWidth;
            MapHeight = mapHeight;
            this.isOutdoor = isOutdoor;

            pathNodeMap = new PathNodeMap(mapWidth, mapHeight);

            cameraMovement = new CameraMovement();
            cameraMovement.Bounds = GetBoundaries();
            AddChild(cameraMovement);

            groundTilemap = new Tilemap(TilesetConnection.SidesAndCorners, true, 16, 16, mapWidth, mapHeight);
            groundTopTilemap = new Tilemap(TilesetConnection.SidesAndCorners, false, 16, 16, mapWidth, mapHeight);
            floorPathTilemap = new Tilemap(TilesetConnection.Sides, false, 16, 16, mapWidth, mapHeight);

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

            airTilemap = new Tilemap(TilesetConnection.SidesAndCorners, true, 16, 16, mapWidth, mapHeight);
            airTilemap.Tileset = new MTileset(ResourcesManager.GetTexture("Tilesets", "air_tileset"), 16, 16);
            AddChild(airTilemap);

            if (isOutdoor)
            {
                // TODO: включать/выключать дождь в зависимости от начальной погоды
                rainEffect = new RainEffectEntity();
                rainEffect.Emitting = false;
                AddChild(rainEffect);
            }
        }

        public override void Update()
        {
            MouseTile = Vector2.Clamp(WorldToMap(MInput.Mouse.GlobalPosition), Vector2.Zero, new Vector2(MapWidth - 1, MapHeight - 1));

            if (isOutdoor)
            {
                Vector2 viewportSize = cameraMovement.GetViewportZoomedSize();
                rainEffect.LocalPosition = _player.LocalPosition + new Vector2(0, -(viewportSize.Y / 2));
                rainEffect.EmitterLineLength = (int)viewportSize.X;
            }

            base.Update();
        }

        public void SetGroundTile(int x, int y, GroundTile groundTile)
        {
            groundTilemap.SetCell(x, y, (int)groundTile);

            UpdateTilePassability(x, y);
        }

        public void TrySetGroundTile(int x, int y, GroundTile groundTile)
        {
            if (groundTilemap.TrySetCell(x, y, (int)groundTile))
            {
                UpdateTilePassability(x, y);
            }
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

        public void SetAirTile(int x, int y, AirTile airTile)
        {
            airTilemap.SetCell(x, y, (int)airTile);

            UpdateTilePassability(x, y);
        }

        public void TrySetAirTile(int x, int y, AirTile airTile)
        {
            if(airTilemap.TrySetCell(x, y, (int)airTile))
            {
                UpdateTilePassability(x, y);
            }
        }

        public AirTile GetAirTile(int x, int y)
        {
            return (AirTile)airTilemap.GetCell(x, y);
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

        public void InteractWithTile(int x, int y, Inventory inventory, int activeSlotIndex, PlayerEnergyManager playerEnergyManager)
        {
            Item handItem = inventory.GetSlotItem(activeSlotIndex);

            Building building = buildingsMap[x, y];

            if (building != null)
            {
                building.Interact(inventory, activeSlotIndex, playerEnergyManager);
            }

            if (handItem is ToolItem toolItem)
            {
                switch (toolItem.ToolType)
                {
                    case ToolType.Showel:
                        {
                            if ((GetGroundTile(x, y) == GroundTile.Grass || GetGroundTile(x, y) == GroundTile.Ground)
                                && GetTileFloorPathItem(x, y) == null
                                && building == null)
                            {
                                toolItem.PlaySoundEffect();
                                playerEnergyManager.ConsumeEnergy(1);
                                SetGroundTile(x, y, GroundTile.FarmPlot);
                            }
                        }
                        break;
                    case ToolType.WateringCan:
                        {
                            if (GetGroundTile(x, y) == GroundTile.Water || building is WaterSourceBuilding)
                            {
                                toolItem.PlaySoundEffect();
                                inventory.AddSlotItemContentAmount(activeSlotIndex, toolItem.Capacity);
                            }
                            else if (GetGroundTile(x, y) == GroundTile.FarmPlot)
                            {
                                if (inventory.GetSlotContentAmount(activeSlotIndex) > 0)
                                {
                                    toolItem.PlaySoundEffect();
                                    inventory.SubSlotItemContentAmount(activeSlotIndex, 1);
                                    SetGroundTopTile(x, y, GroundTopTile.Moisture);
                                    playerEnergyManager.ConsumeEnergy(1);
                                }
                            }
                        }
                        break;
                    case ToolType.Pickaxe:
                    case ToolType.Axe:
                        {
                            if(building == null && GetGroundTile(x, y) == GroundTile.FarmPlot)
                            {
                                SetGroundTile(x, y, GroundTile.Ground);

                                SetGroundTopTile(x, y, GroundTopTile.None);

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
                        break;
                }
            }
            else if (handItem is SeedItem seedItem)
            {
                PlantItem plantItem = Engine.ItemsDatabase.GetItemByName<PlantItem>(seedItem.PlantName);

                if (TryBuild(plantItem, x, y, Direction.Down))
                {
                    inventory.RemoveItem(handItem, 1, activeSlotIndex);
                }
            }
            else if (handItem is TreeSeedItem treeSeedItem)
            {
                TreeItem treeItem = Engine.ItemsDatabase.GetItemByName<TreeItem>(treeSeedItem.TreeName);

                if (TryBuild(treeItem, x, y, Direction.Down))
                {
                    inventory.RemoveItem(handItem, 1, activeSlotIndex);
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

        public virtual void StartNextDay(TimeOfDayManager timeOfDayManager)
        {
            foreach (Entity entity in entitiesList.GetChildren())
            {
                if (entity is Building building)
                {
                    building.OnBeforeDayChanged();
                }
            }

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

            if (isOutdoor)
            {
                rainEffect.Emitting = timeOfDayManager.CurrentWeather == Weather.Rainy;
            }

            foreach (Entity entity in entitiesList.GetChildren())
            {
                if (entity is Building building)
                {
                    building.OnAfterDayChanged();
                }
            }
        }

        #region Animals

        public bool TrySpawnAnimal(AnimalItem animalItem, int x, int y)
        {
            if (IsTilePassable(x, y) == false) return false;

            Animal animal;

            if (animalItem is BabyAnimalItem babyAnimalItem)
            {
                animal = new BabyAnimal(babyAnimalItem);
            }
            else if (animalItem is AdultAnimalItem adultAnimalItem)
            {
                animal = new AdultAnimal(adultAnimalItem);
            }
            else
            {
                throw new Exception("Animal item is not valid!");
            }

            animal.SetGameLocation(this);
            animal.SetTilePosition(new Vector2(x, y));
            entitiesList.AddChild(animal);

            return true;
        }

        public void RemoveAnimal(Animal animal)
        {
            entitiesList.RemoveChild(animal);
        }

        public IEnumerable<Animal> GetAnimals()
        {
            foreach (Entity entity in entitiesList.GetChildren())
            {
                if (entity is Animal animal)
                {
                    yield return animal;
                }
            }
        }

        public IEnumerable<T> GetAnimals<T>() where T : Animal
        {
            foreach (T animal in entitiesList.GetChildren<T>())
            {
                yield return animal;
            }
        }

        #endregion

        #region Buildings

        public Building GetBuilding(int x, int y)
        {
            return buildingsMap[x, y];
        }

        public IEnumerable<Building> GetBuildings()
        {
            foreach (Entity entity in entitiesList.GetChildren())
            {
                if (entity is Building building)
                {
                    yield return building;
                }
            }
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
                else if(buildingItem is AutoCrafterItem autoCrafterItem)
                {
                    building = new AutoCrafterBuilding(this, autoCrafterItem, direction, tiles);
                }
                else if (buildingItem is WindowItem windowItem)
                {
                    building = new WindowBuilding(this, windowItem, direction, tiles);
                }
                else if (buildingItem is SprinklerItem sprinklerItem)
                {
                    building = new SprinklerBuilding(this, sprinklerItem, direction, tiles);
                }
                else if (buildingItem is AnimalFeederItem animalFeederItem)
                {
                    building = new AnimalFeederBuilding(this, animalFeederItem, direction, tiles);
                }
                else if (buildingItem is BirdNestItem birdNestItem)
                {
                    building = new BirdNestBuilding(this, birdNestItem, direction, tiles);
                }
                else if (buildingItem is GrassItem grassItem)
                {
                    building = new GrassBuilding(this, grassItem, direction, tiles);
                }
                else if (buildingItem is WaterSourceItem waterSourceItem)
                {
                    building = new WaterSourceBuilding(this, waterSourceItem, direction, tiles);
                }
                else if (buildingItem is StorageItem storageItem)
                {
                    building = new StorageBuilding(this, storageItem, direction, tiles);
                }
                else
                {
                    building = new Building(this, buildingItem, direction, tiles);
                }

                building.LocalPosition = tiles[0, 0] * Engine.TILE_SIZE;

                if (buildingItem.IsFlat)
                {
                    building.Depth = 0;
                }
                else
                {
                    // Добавляем случайное смещение по Y, чтобы спрайты строений не сражались за порядок отрисовки
                    float randomOffset = Calc.Random.Range(0.0f, 0.9f);
                    building.Depth = (tiles[0, tiles.GetLength(1) - 1].Y * Engine.TILE_SIZE) + randomOffset;
                }
                
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
                                else if(teleportData.Location == "tent")
                                {
                                    TentLocation location = new TentLocation(
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
                case "G":
                    {
                        return groundTile == GroundTile.AnimalHouseWall;
                    }
                case "H":
                    {
                        return groundTile == GroundTile.Grass || 
                            groundTile == GroundTile.Ground ||
                            groundTile == GroundTile.HouseFloor ||
                            groundTile == GroundTile.TentFloor;
                    }
                case "I":
                    {
                        return (groundTile == GroundTile.Grass || 
                            groundTile == GroundTile.Ground || 
                            groundTile == GroundTile.CoopHouseFloor) &&
                            floorPathMap[x, y] == null;
                    }
            }

            return false;
        }

        #endregion

        #region Items

        public void AddItem(Vector2 position, ItemContainer itemContainer)
        {
            ItemEntity locationItem = new ItemEntity(itemContainer);
            locationItem.LocalPosition = position;
            itemsList.AddChild(locationItem);
        }

        public void RemoveItemEntity(ItemEntity item)
        {
            itemsList.RemoveChild(item);
        }

        public IEnumerable<ItemEntity> GetItemEntities()
        {
            List<ItemEntity> items = itemsList.GetChildren<ItemEntity>().ToList();

            foreach (ItemEntity item in items)
            {
                yield return item;
            }
        }

        #endregion

        public Teleport TryGetTeleport(int x, int y)
        {
            if (groundTilemap.IsWithinBounds(x, y) == false) return null;

            return teleportsMap[x, y];
        }

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
            if (groundTilemap.IsWithinBounds(x, y) == false) return;

            collisionMap[x, y] = true;

            switch (GetGroundTile(x, y))
            {
                case GroundTile.AnimalHouseWall:
                case GroundTile.HouseWall:
                case GroundTile.TentWall:
                case GroundTile.Water:
                    collisionMap[x, y] = false;
                    break;
            }

            switch(GetAirTile(x, y))
            {
                case AirTile.Forest:
                    collisionMap[x, y] = false;
                    break;
            }

            if (buildingsMap[x, y] != null)
            {
                if (buildingsMap[x, y].IsPassable == false)
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
