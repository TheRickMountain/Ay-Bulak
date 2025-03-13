using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace palmesneo_village
{
    public enum GameState
    {
        Game,
        Inventory,
        Trading,
        SceneTransitionIn,
        DayTransitionIn,
        TransitionOut
    }

    public class GameplayScene : Scene
    {
        public GameLocation CurrentGameLocation 
        {
            get => currentGameLocation;
        }

        public Inventory Inventory { get; private set; }

        public PlayerMoneyManager PlayerMoneyManager { get; private set; }

        public PlayerEnergyManager PlayerEnergyManager { get; private set; }

        private GameState gameState = GameState.Game;

        private InventoryHotbar inventoryHotbar;

        private TimeOfDayManager timeOfDayManager;

        private Player player;

        private TileSelector tileSelector;

        private TextUI timeText;

        private Dictionary<string, GameLocation> gameLocations = new();
        private GameLocation currentGameLocation;
        private GameLocation nextGameLocation;

        private float transitionTimer = 0.0f;
        private ImageUI transitionImage;

        private BuildingSystem buildingSystem;

        private List<EntityUI> gameUIElements = new();

        private InventoryUI inventoryUI;
        private TradingUI tradingUI;

        public override void Begin()
        {
            MasterEntity.IsDepthSortEnabled = true;

            Inventory = new Inventory(10, 4);

            inventoryHotbar = new InventoryHotbar(Inventory);
            MasterEntity.AddChild(inventoryHotbar);

            timeOfDayManager = new TimeOfDayManager();
            MasterUIEntity.AddChild(timeOfDayManager);

            CreatureTemplate creatureTemplate = new CreatureTemplate("Player", ResourcesManager.GetTexture("Sprites", "player"), 100);

            player = new Player(creatureTemplate, Inventory);
            player.LocalPosition = new Vector2(8 * Engine.TILE_SIZE, 6 * Engine.TILE_SIZE);


            RegisterLocation(new FarmLocation("farm"));
            RegisterLocation(new HouseLocation("house"));

            buildingSystem = new BuildingSystem();
            buildingSystem.Depth = 100;
            MasterEntity.AddChild(buildingSystem);

            tileSelector = new TileSelector();
            tileSelector.Depth = 100;
            MasterEntity.AddChild(tileSelector);

            PlayerEnergyManager = new PlayerEnergyManager(100, 100);
            PlayerMoneyManager = new PlayerMoneyManager();

            Inventory.SlotDataChanged += OnInventorySlotDataChanged;
            inventoryHotbar.CurrentSlotIndexChanged += OnInventoryHotbarCurrentSlotIndexChanged;

            #region UI

            InventoryHotbarUI inventoryHotbarUI = new InventoryHotbarUI(Inventory, inventoryHotbar);
            inventoryHotbarUI.Anchor = Anchor.BottomCenter;
            inventoryHotbarUI.LocalPosition = new Vector2(0, -5);
            MasterUIEntity.AddChild(inventoryHotbarUI);

            PlayerEnergyBarUI playerEnergyBarUI = new PlayerEnergyBarUI(PlayerEnergyManager);
            playerEnergyBarUI.Anchor = Anchor.TopLeft;
            playerEnergyBarUI.LocalPosition = new Vector2(5, 5);
            MasterUIEntity.AddChild(playerEnergyBarUI);

            PlayerMoneyUI playerMoneyUI = new PlayerMoneyUI(PlayerMoneyManager);
            playerMoneyUI.Anchor = Anchor.BottomLeft;
            playerMoneyUI.LocalPosition = new Vector2(5, -5);
            MasterUIEntity.AddChild(playerMoneyUI);

            timeText = new TextUI();
            timeText.Anchor = Anchor.TopLeft;
            timeText.LocalPosition = new Vector2(5, 32);
            MasterUIEntity.AddChild(timeText);

            gameUIElements.Add(inventoryHotbarUI);
            gameUIElements.Add(playerEnergyBarUI);
            gameUIElements.Add(playerMoneyUI);
            gameUIElements.Add(timeText);

            inventoryUI = new InventoryUI(Inventory);
            inventoryUI.Anchor = Anchor.Center;

            tradingUI = new TradingUI(Inventory, PlayerMoneyManager);
            tradingUI.Anchor = Anchor.Center;

            transitionImage = new ImageUI();
            transitionImage.Texture = RenderManager.Pixel;
            transitionImage.SelfColor = Color.Black;
            transitionImage.IsVisible = false;
            MasterUIEntity.AddChild(transitionImage);

            #endregion

            GoToLocation("farm");

            PlayerMoneyManager.MoneyAmount = 500;

            Inventory.TryAddItem(Engine.ItemsDatabase.GetItemByName<PickaxeItem>("iron_pickaxe"), 1, 0);
            Inventory.TryAddItem(Engine.ItemsDatabase.GetItemByName<AxeItem>("iron_axe"), 1, 0);
            Inventory.TryAddItem(Engine.ItemsDatabase.GetItemByName<ShowelItem>("iron_showel"), 1, 0);

            WateringCanItem ironWateringCan = Engine.ItemsDatabase.GetItemByName<WateringCanItem>("iron_watering_can");

            Inventory.TryAddItem(ironWateringCan, 1, ironWateringCan.Capacity);

            base.Begin();
        }

        public override void Update()
        {
            transitionImage.Size = MasterUIEntity.Size;

            timeText.Text = timeOfDayManager.GetTimeString();

            switch (gameState)
            {
                case GameState.Trading:
                    {
                        if (InputBindings.Exit.Pressed)
                        {
                            foreach (var gameUIElement in gameUIElements)
                            {
                                MasterUIEntity.AddChild(gameUIElement);
                            }

                            MasterUIEntity.RemoveChild(tradingUI);

                            gameState = GameState.Game;
                        }
                    }
                    break;
                case GameState.Inventory:
                    {
                        if (InputBindings.Exit.Pressed)
                        {
                            foreach (var gameUIElement in gameUIElements)
                            {
                                MasterUIEntity.AddChild(gameUIElement);
                            }

                            MasterUIEntity.RemoveChild(inventoryUI);

                            gameState = GameState.Game;
                        }
                    }
                    break;
                case GameState.Game:
                    {
                        Vector2 mouseTile = CurrentGameLocation.MouseTile;
                        Vector2 playerTile = CurrentGameLocation.WorldToMap(player.LocalPosition);

                        tileSelector.IsVisible = false;

                        int tileX = (int)mouseTile.X;
                        int tileY = (int)mouseTile.Y;

                        int hotbarCurrentSlotIndex = inventoryHotbar.CurrentSlotIndex;

                        Item currentPlayerItem = Inventory.GetSlotItem(hotbarCurrentSlotIndex);

                        // Handle building item selection
                        if (currentPlayerItem is BuildingItem buildingItem)
                        {
                            if (buildingItem.IsRotatable && InputBindings.Rotate.Pressed)
                            {
                                buildingSystem.RotateBuildingPreview();
                            }

                            if (MInput.Mouse.PressedLeftButton)
                            {
                                if (buildingSystem.TryPlaceBuilding(mouseTile))
                                {
                                    Inventory.RemoveItem(buildingItem, 1, inventoryHotbar.CurrentSlotIndex);
                                }
                            }
                        }

                        if (CanShowTileSelector(playerTile, mouseTile, 4))
                        {
                            if (CurrentGameLocation.CanInteractWithTile(tileX, tileY, currentPlayerItem))
                            {
                                tileSelector.IsVisible = true;
                                tileSelector.LocalPosition = CurrentGameLocation.MapToWorld(mouseTile);

                                if (MInput.Mouse.PressedLeftButton)
                                {
                                    CurrentGameLocation.InteractWithTile(tileX, tileY, Inventory, 
                                        inventoryHotbar.CurrentSlotIndex, PlayerEnergyManager);
                                }
                            }
                        }

                        if (MInput.Mouse.PressedRightButton)
                        {
                            if (currentPlayerItem != null)
                            {
                                if (currentPlayerItem is ConsumableItem consumableItem)
                                {
                                    PlayerEnergyManager.AddEnergy(consumableItem.EnergyAmount);

                                    Inventory.RemoveItem(consumableItem, 1, inventoryHotbar.CurrentSlotIndex);
                                }
                            }
                        }

                        if (timeOfDayManager.CurrentHour == 0)
                        {
                            gameState = GameState.DayTransitionIn;
                        }
                    
                        if(InputBindings.Exit.Pressed)
                        {
                            foreach(var gameUIElement in gameUIElements)
                            {
                                MasterUIEntity.RemoveChild(gameUIElement);
                            }

                            MasterUIEntity.AddChild(inventoryUI);
                            inventoryUI.Open();

                            gameState = GameState.Inventory;
                        }

                        // TODO: temp
                        if(MInput.Keyboard.Pressed(Microsoft.Xna.Framework.Input.Keys.T))
                        {
                            foreach (var gameUIElement in gameUIElements)
                            {
                                MasterUIEntity.RemoveChild(gameUIElement);
                            }

                            MasterUIEntity.AddChild(tradingUI);
                            // TODO: Temp
                            tradingUI.Open(new List<Item>() 
                            {
                                Engine.ItemsDatabase.GetItemByName<SeedItem>("tomato_seeds"),
                                Engine.ItemsDatabase.GetItemByName<SeedItem>("garlic_seeds"),
                                Engine.ItemsDatabase.GetItemByName<SeedItem>("eggplant_seeds")
                            });

                            gameState = GameState.Trading;
                        }
                    }
                    break;
                case GameState.SceneTransitionIn:
                    {
                        Engine.TimeRate = 0;

                        transitionTimer += Engine.DeltaTime;

                        transitionImage.SelfColor = Color.Black * transitionTimer;

                        if (transitionTimer >= 1.0f)
                        {
                            transitionTimer = 0.0f;

                            // Location switching
                            if (currentGameLocation != nextGameLocation)
                            {
                                if (currentGameLocation != null)
                                {
                                    currentGameLocation.RemovePlayer(player);

                                    MasterEntity.RemoveChild(currentGameLocation);
                                }

                                currentGameLocation = nextGameLocation;
                                currentGameLocation.Depth = 0;

                                MasterEntity.AddChild(currentGameLocation);

                                currentGameLocation.SetPlayer(player);

                                player.SetGameLocation(currentGameLocation);
                                buildingSystem.SetGameLocation(currentGameLocation);

                                gameState = GameState.TransitionOut;
                            }
                        }
                    }
                    break;
                case GameState.DayTransitionIn:
                    {
                        Engine.TimeRate = 0;

                        transitionTimer += Engine.DeltaTime;

                        transitionImage.SelfColor = Color.Black * transitionTimer;

                        if (transitionTimer >= 1.0f)
                        {
                            transitionTimer = 0.0f;

                            StartNextDay();

                            gameState = GameState.TransitionOut;
                        }
                    }
                    break;
                case GameState.TransitionOut:
                    {
                        transitionTimer += Engine.DeltaTime;

                        transitionImage.SelfColor = Color.Black * (1.0f - transitionTimer);

                        if (transitionTimer >= 1.0f)
                        {
                            transitionTimer = 0.0f;

                            Engine.TimeRate = 1.0f;

                            transitionImage.IsVisible = false;

                            gameState = GameState.Game;
                        }
                    }
                    break;
            }

            base.Update();
        }

        public void StartNextDay()
        {
            timeOfDayManager.StartNextDay();

            foreach (var kvp in gameLocations)
            {
                GameLocation gameLocation = kvp.Value;
                gameLocation.StartNextDay(timeOfDayManager);
            }
        }

        public void GoToLocation(string locationId)
        {
            // Предусматриваем попытку перейти на ту же локацию
            GameLocation newGameLocation = gameLocations[locationId];

            if(currentGameLocation == newGameLocation)
            {
                return;
            }

            nextGameLocation = newGameLocation;

            gameState = GameState.SceneTransitionIn;

            transitionImage.IsVisible = true;
            transitionImage.SelfColor = Color.Black * 0.0f;
        }

        private void RegisterLocation(GameLocation gameLocation)
        {
            gameLocations.Add(gameLocation.Id, gameLocation);
        }

        private bool CanShowTileSelector(Vector2 playerTile, Vector2 mouseTile, int maxDistance)
        {
            if (playerTile == mouseTile)
            {
                return false;
            }

            float distance = Vector2.Distance(playerTile, mouseTile);

            return distance < maxDistance;
        }

        private void OnInventoryHotbarCurrentSlotIndexChanged(int slotIndex)
        {
            SetCurrentPlayerItem(Inventory.GetSlotItem(slotIndex));
        }

        private void OnInventorySlotDataChanged(Inventory inventory, int slotIndex)
        {
            if (slotIndex != inventoryHotbar.CurrentSlotIndex)
                return;

            SetCurrentPlayerItem(inventory.GetSlotItem(slotIndex));
        }

        private void SetCurrentPlayerItem(Item item)
        {
            if (item is BuildingItem buildingItem)
            {
                buildingSystem.SetCurrentBuildingItem(buildingItem);
            }
            else
            {
                buildingSystem.SetCurrentBuildingItem(null);
            }
        }
    }
}
