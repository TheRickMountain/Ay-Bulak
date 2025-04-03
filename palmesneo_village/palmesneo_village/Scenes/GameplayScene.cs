using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace palmesneo_village
{
    public enum GameState
    {
        Game,
        PlayerInventory,
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
        private Vector2 nextGameLocationTile;

        private float transitionTimer = 0.0f;
        private ImageUI transitionImage;

        private BuildingSystem buildingSystem;

        private List<EntityUI> gameUIElements = new();

        private PlayerInventoryUI playerInventoryUI;
        private TradingUI tradingUI;

        public override void Begin()
        {
            MasterEntity.IsDepthSortEnabled = true;

            Inventory = new Inventory(10, 4);

            inventoryHotbar = new InventoryHotbar(Inventory);
            MasterEntity.AddChild(inventoryHotbar);

            timeOfDayManager = new TimeOfDayManager();
            timeOfDayManager.Name = "time_of_day_manager";
            MasterEntity.AddChild(timeOfDayManager);

            player = new Player("Player", ResourcesManager.GetTexture("Sprites", "player"), 80, Inventory);

            RegisterLocation(new FarmLocation("farm"));

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

            playerInventoryUI = new PlayerInventoryUI(Inventory);
            playerInventoryUI.Anchor = Anchor.Center;

            tradingUI = new TradingUI(Inventory, PlayerMoneyManager);
            tradingUI.Anchor = Anchor.Center;

            transitionImage = new ImageUI();
            transitionImage.Texture = RenderManager.Pixel;
            transitionImage.SelfColor = Color.Black;
            transitionImage.IsVisible = false;
            MasterUIEntity.AddChild(transitionImage);

            #endregion

            GoToLocation("farm", new Vector2(20, 20));

            PlayerMoneyManager.MoneyAmount = 500;

            Inventory.TryAddItem(Engine.ItemsDatabase.GetItemByName("iron_pickaxe"), 1, 0);
            Inventory.TryAddItem(Engine.ItemsDatabase.GetItemByName("iron_axe"), 1, 0);
            Inventory.TryAddItem(Engine.ItemsDatabase.GetItemByName("iron_showel"), 1, 0);
            Inventory.TryAddItem(Engine.ItemsDatabase.GetItemByName("iron_scythe"), 1, 0);

            ToolItem ironWateringCan = Engine.ItemsDatabase.GetItemByName<ToolItem>("iron_watering_can");

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
                case GameState.PlayerInventory:
                    {
                        if (InputBindings.Exit.Pressed)
                        {
                            foreach (var gameUIElement in gameUIElements)
                            {
                                MasterUIEntity.AddChild(gameUIElement);
                            }

                            MasterUIEntity.RemoveChild(playerInventoryUI);

                            gameState = GameState.Game;
                        }
                    }
                    break;
                case GameState.Game:
                    {
                        Vector2 mouseTile = CurrentGameLocation.MouseTile;
                        Vector2 playerTile = player.GetTilePosition();

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
                        else if (CanShowTileSelector(playerTile, mouseTile, 4))
                        {
                            tileSelector.IsVisible = true;
                            tileSelector.LocalPosition = CurrentGameLocation.MapToWorld(mouseTile);

                            if (MInput.Mouse.PressedLeftButton)
                            {
                                CurrentGameLocation.InteractWithTile(tileX, tileY, Inventory,
                                    inventoryHotbar.CurrentSlotIndex, PlayerEnergyManager, this, false);
                            }
                            else if (MInput.Mouse.PressedRightButton)
                            {
                                CurrentGameLocation.InteractWithTile(tileX, tileY, Inventory,
                                    inventoryHotbar.CurrentSlotIndex, PlayerEnergyManager, this, true);
                            }
                        }

                        if (MInput.Mouse.PressedRightButton)
                        {
                            if (currentPlayerItem is ConsumableItem consumableItem)
                            {
                                PlayerEnergyManager.AddEnergy(consumableItem.EnergyAmount);

                                Inventory.RemoveItem(consumableItem, 1, inventoryHotbar.CurrentSlotIndex);
                            }
                        }

                        if (timeOfDayManager.CurrentHour == 0)
                        {
                            StartNextDay();
                        }
                    
                        if(InputBindings.Exit.Pressed)
                        {
                            OpenPlayerInventoryUI(Engine.CraftingRecipesDatabase.GetCraftingRecipes());
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
                            tradingUI.Open(Inventory, PlayerMoneyManager, new List<Item>() 
                            {
                                Engine.ItemsDatabase.GetItemByName("wheat_flour"),
                                Engine.ItemsDatabase.GetItemByName("tomato_seeds"),
                                Engine.ItemsDatabase.GetItemByName("garlic_seeds"),
                                Engine.ItemsDatabase.GetItemByName("eggplant_seeds"),
                                Engine.ItemsDatabase.GetItemByName("carrot_seeds"),
                                Engine.ItemsDatabase.GetItemByName("cabbage_seeds"),
                                Engine.ItemsDatabase.GetItemByName("beetroot_seeds"),
                                Engine.ItemsDatabase.GetItemByName("strawberry_seeds")
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

                                player.SetTilePosition(nextGameLocationTile);

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

                            timeOfDayManager.StartNextDay();

                            foreach (var kvp in gameLocations)
                            {
                                GameLocation gameLocation = kvp.Value;
                                gameLocation.StartNextDay(timeOfDayManager);
                            }

                            if (timeOfDayManager.CurrentWeather == Weather.Sunny)
                            {
                                ResourcesManager.GetSoundEffect("SoundEffects", "rooster_crow").Play();
                            }

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
            gameState = GameState.DayTransitionIn;

            transitionImage.IsVisible = true;
            transitionImage.SelfColor = Color.Black * 0.0f;
        }

        public void GoToLocation(string locationId, Vector2 tile)
        {
            nextGameLocationTile = tile;

            if(currentGameLocation == gameLocations[locationId])
            {
                return;
            }

            nextGameLocation = gameLocations[locationId];

            gameState = GameState.SceneTransitionIn;

            transitionImage.IsVisible = true;
            transitionImage.SelfColor = Color.Black * 0.0f;
        }

        public void RegisterLocation(GameLocation gameLocation)
        {
            gameLocations.Add(gameLocation.LocationId, gameLocation);
        }

        public void OpenPlayerInventoryUI(IEnumerable<CraftingRecipe> craftingRecipes)
        {
            foreach (var gameUIElement in gameUIElements)
            {
                MasterUIEntity.RemoveChild(gameUIElement);
            }

            MasterUIEntity.AddChild(playerInventoryUI);
            playerInventoryUI.Open(craftingRecipes);

            gameState = GameState.PlayerInventory;
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
