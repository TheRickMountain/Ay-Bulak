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
        Crafting,
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

        private InventoryUI inventoryUI;
        private TradingUI tradingUI;
        private CraftingUI craftingUI;

        public override void Begin()
        {
            MasterEntity.IsDepthSortEnabled = true;

            Inventory = new Inventory(10, 4);

            inventoryHotbar = new InventoryHotbar(Inventory);
            MasterEntity.AddChild(inventoryHotbar);

            timeOfDayManager = new TimeOfDayManager();
            MasterUIEntity.AddChild(timeOfDayManager);

            CreatureTemplate creatureTemplate = new CreatureTemplate("Player", ResourcesManager.GetTexture("Sprites", "player"), 80);

            player = new Player(creatureTemplate, Inventory);
            player.LocalPosition = new Vector2(8 * Engine.TILE_SIZE, 6 * Engine.TILE_SIZE);

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

            inventoryUI = new InventoryUI(Inventory);
            inventoryUI.Anchor = Anchor.Center;

            tradingUI = new TradingUI(Inventory, PlayerMoneyManager);
            tradingUI.Anchor = Anchor.Center;

            craftingUI = new CraftingUI(Inventory);
            craftingUI.Anchor = Anchor.Center;

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
                case GameState.Crafting:
                    {
                        if (InputBindings.Exit.Pressed)
                        {
                            foreach (var gameUIElement in gameUIElements)
                            {
                                MasterUIEntity.AddChild(gameUIElement);
                            }

                            MasterUIEntity.RemoveChild(craftingUI);

                            gameState = GameState.Game;
                        }
                    }
                    break;
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
                        else if (CanShowTileSelector(playerTile, mouseTile, 4))
                        {
                            if (CurrentGameLocation.CanInteractWithTile(tileX, tileY, currentPlayerItem))
                            {
                                tileSelector.IsVisible = true;
                                tileSelector.LocalPosition = CurrentGameLocation.MapToWorld(mouseTile);

                                if (MInput.Mouse.PressedLeftButton)
                                {
                                    CurrentGameLocation.InteractWithTile(tileX, tileY, Inventory, 
                                        inventoryHotbar.CurrentSlotIndex, PlayerEnergyManager, this);
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
                            StartNextDay();
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
                            tradingUI.Open(Inventory, PlayerMoneyManager, new List<Item>() 
                            {
                                Engine.ItemsDatabase.GetItemByName<SeedItem>("tomato_seeds"),
                                Engine.ItemsDatabase.GetItemByName<SeedItem>("garlic_seeds"),
                                Engine.ItemsDatabase.GetItemByName<SeedItem>("eggplant_seeds"),
                                Engine.ItemsDatabase.GetItemByName<SeedItem>("carrot_seeds"),
                                Engine.ItemsDatabase.GetItemByName<SeedItem>("cabbage_seeds"),
                                Engine.ItemsDatabase.GetItemByName<SeedItem>("beetroot_seeds")
                            });

                            gameState = GameState.Trading;
                        }

                        // TODO: Temp
                        if (MInput.Keyboard.Pressed(Microsoft.Xna.Framework.Input.Keys.C))
                        {
                            foreach (var gameUIElement in gameUIElements)
                            {
                                MasterUIEntity.RemoveChild(gameUIElement);
                            }

                            MasterUIEntity.AddChild(craftingUI);

                            // TODO: Temp
                            craftingUI.Open(Inventory, new List<CraftingRecipe>()
                            {
                                new CraftingRecipe(new Ingredient(Engine.ItemsDatabase.GetItemByName("wooden_path"), 1),
                                new List<Ingredient>() { new Ingredient(Engine.ItemsDatabase.GetItemByName("wood"), 1)}),

                                new CraftingRecipe(new Ingredient(Engine.ItemsDatabase.GetItemByName("stone_path"), 1),
                                new List<Ingredient>() { new Ingredient(Engine.ItemsDatabase.GetItemByName("stone"), 1)}),

                                 new CraftingRecipe(new Ingredient(Engine.ItemsDatabase.GetItemByName("garlic_seeds"), 1),
                                new List<Ingredient>() { new Ingredient(Engine.ItemsDatabase.GetItemByName("wood"), 5)}),

                                new CraftingRecipe(new Ingredient(Engine.ItemsDatabase.GetItemByName("cabbage_seeds"), 1),
                                new List<Ingredient>() { new Ingredient(Engine.ItemsDatabase.GetItemByName("stone"), 1)}),

                                 new CraftingRecipe(new Ingredient(Engine.ItemsDatabase.GetItemByName("tomato_seeds"), 1),
                                new List<Ingredient>() { new Ingredient(Engine.ItemsDatabase.GetItemByName("wood"), 1)}),

                                new CraftingRecipe(new Ingredient(Engine.ItemsDatabase.GetItemByName("beetroot_seeds"), 1),
                                new List<Ingredient>() { new Ingredient(Engine.ItemsDatabase.GetItemByName("stone"), 1)}),

                                 new CraftingRecipe(new Ingredient(Engine.ItemsDatabase.GetItemByName("tomato"), 1),
                                new List<Ingredient>() { new Ingredient(Engine.ItemsDatabase.GetItemByName("wood"), 1)}),

                                new CraftingRecipe(new Ingredient(Engine.ItemsDatabase.GetItemByName("garlic"), 1),
                                new List<Ingredient>() { new Ingredient(Engine.ItemsDatabase.GetItemByName("stone"), 1)}),

                                 new CraftingRecipe(new Ingredient(Engine.ItemsDatabase.GetItemByName("beetroot"), 1),
                                new List<Ingredient>() { new Ingredient(Engine.ItemsDatabase.GetItemByName("wood"), 1)}),

                                new CraftingRecipe(new Ingredient(Engine.ItemsDatabase.GetItemByName("carrot"), 1),
                                new List<Ingredient>() { new Ingredient(Engine.ItemsDatabase.GetItemByName("stone"), 1)}),

                                 new CraftingRecipe(new Ingredient(Engine.ItemsDatabase.GetItemByName("cabbage"), 1),
                                new List<Ingredient>() { new Ingredient(Engine.ItemsDatabase.GetItemByName("wood"), 1)})
                            });

                            gameState = GameState.Crafting;
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

                                player.LocalPosition = nextGameLocationTile * Engine.TILE_SIZE;

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
