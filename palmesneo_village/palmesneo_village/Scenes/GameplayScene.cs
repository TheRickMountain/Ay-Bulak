using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace palmesneo_village
{
    public enum GameState
    {
        Game,
        Crafter,
        Inventory,
        StorageInventory,
        Trading,
        Quests,
        InteractionMenu,
        SceneTransitionIn,
        DayTransitionIn,
        TransitionOut
    }

    public class GameplayScene : Scene
    {
        public const int PLAYER_INTERACTION_DISTANCE = 3;

        public GameLocation CurrentGameLocation 
        {
            get => currentGameLocation;
        }

        public Inventory PlayerInventory { get; private set; }

        public PlayerMoneyManager PlayerMoneyManager { get; private set; }

        public PlayerEnergyManager PlayerEnergyManager { get; private set; }

        public static QuestManager QuestManager { get; private set; }

        private GameState gameState = GameState.Game;

        private InventoryHotbar inventoryHotbar;

        private TimeOfDayManager timeOfDayManager;
        
        private Player player;

        private TileSelector tileSelector;

        private TextUI timeText;

        private CalendarUI calendarUI;

        private Dictionary<string, GameLocation> gameLocations = new();
        private GameLocation currentGameLocation;
        private GameLocation nextGameLocation;
        private Vector2 nextGameLocationTile;

        private float transitionTimer = 0.0f;
        private ImageUI transitionImage;

        private BuildingSystem buildingSystem;

        private List<EntityUI> gameUIElements = new();

        private CrafterUI crafterUI;
        private DraggableInventoryUI inventoryUI;
        private StorageInventoryUI storageInventoryUI;
        private TradingUI tradingUI;
        private QuestsUI questsUI;
        private InteractionMenuUI interactionMenuUI;

        private MiddleButtonUI openQuestsButtonUI;

        private InteractableEntity selectedInteractableEntity;

        public override void Begin()
        {
            MasterEntity.IsDepthSortEnabled = true;

            PlayerInventory = new Inventory(10, 2, 4);

            inventoryHotbar = new InventoryHotbar(PlayerInventory);
            MasterEntity.AddChild(inventoryHotbar);

            timeOfDayManager = new TimeOfDayManager();
            timeOfDayManager.Name = "time_of_day_manager";
            MasterEntity.AddChild(timeOfDayManager);

            QuestManager = new QuestManager();

            PlayerEnergyManager = new PlayerEnergyManager(100, 100);

            player = new Player("Player", ResourcesManager.GetTexture("Sprites", "player"), 80, PlayerInventory, 
                inventoryHotbar, PlayerEnergyManager);

            RegisterLocation(new FarmLocation("farm", timeOfDayManager));

            buildingSystem = new BuildingSystem();
            buildingSystem.Depth = 100;
            MasterEntity.AddChild(buildingSystem);

            tileSelector = new TileSelector();
            tileSelector.Depth = 100;
            MasterEntity.AddChild(tileSelector);

            
            PlayerMoneyManager = new PlayerMoneyManager();

            PlayerInventory.SlotDataChanged += OnInventorySlotDataChanged;
            inventoryHotbar.CurrentSlotIndexChanged += OnInventoryHotbarCurrentSlotIndexChanged;

            #region UI

            InventoryHotbarUI inventoryHotbarUI = new InventoryHotbarUI(PlayerInventory, inventoryHotbar);
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

            calendarUI = new CalendarUI(timeOfDayManager);
            calendarUI.Anchor = Anchor.TopRight;
            calendarUI.LocalPosition = new Vector2(-5, 5);
            MasterUIEntity.AddChild(calendarUI);

            timeText = new TextUI();
            timeText.Anchor = Anchor.TopRight;
            timeText.LocalPosition = new Vector2(-5, 55);
            timeText.Text = "Test";
            MasterUIEntity.AddChild(timeText);

            openQuestsButtonUI = new OpenQuestsButtonUI(QuestManager);
            openQuestsButtonUI.Anchor = Anchor.TopRight;
            openQuestsButtonUI.LocalPosition = new Vector2(-5, timeText.LocalPosition.Y + timeText.Size.Y + 5);
            openQuestsButtonUI.ActionTriggered += (x) => { OpenQuestsUI(); };
            MasterUIEntity.AddChild(openQuestsButtonUI);

            gameUIElements.Add(inventoryHotbarUI);
            gameUIElements.Add(playerEnergyBarUI);
            gameUIElements.Add(playerMoneyUI);
            gameUIElements.Add(calendarUI);
            gameUIElements.Add(timeText);
            gameUIElements.Add(openQuestsButtonUI);

            crafterUI = new CrafterUI(PlayerInventory);
            crafterUI.Anchor = Anchor.Center;

            inventoryUI = new DraggableInventoryUI(player);
            inventoryUI.Anchor = Anchor.Center;

            storageInventoryUI = new StorageInventoryUI();
            storageInventoryUI.Anchor = Anchor.Center;

            tradingUI = new TradingUI(PlayerInventory, PlayerMoneyManager);
            tradingUI.Anchor = Anchor.Center;

            questsUI = new QuestsUI(QuestManager, PlayerMoneyManager);
            questsUI.Anchor = Anchor.Center;

            interactionMenuUI = new InteractionMenuUI();
            interactionMenuUI.InteractionSelected += OnInteractionMenuInteractionSelected;

            transitionImage = new ImageUI();
            transitionImage.Texture = RenderManager.Pixel;
            transitionImage.SelfColor = Color.Black;
            transitionImage.IsVisible = false;
            MasterUIEntity.AddChild(transitionImage);

            #endregion

            GoToLocation("farm", new Vector2(20, 20));

            PlayerMoneyManager.MoneyAmount = 500;

            PlayerInventory.TryAddItem(Engine.ItemsDatabase.GetItemByName("iron_axe"), 1, 0);
            PlayerInventory.TryAddItem(Engine.ItemsDatabase.GetItemByName("iron_showel"), 1, 0);
            PlayerInventory.TryAddItem(Engine.ItemsDatabase.GetItemByName("iron_scythe"), 1, 0);
            PlayerInventory.TryAddItem(Engine.ItemsDatabase.GetItemByName("fishing_rod"), 1, 0);

            ToolItem ironWateringCan = Engine.ItemsDatabase.GetItemByName<ToolItem>("iron_watering_can");

            PlayerInventory.TryAddItem(ironWateringCan, 1, ironWateringCan.Capacity);

            base.Begin();
        }

        public override void Update()
        {
            ChageCursorTextureToDefault();

            transitionImage.Size = MasterUIEntity.Size;

            timeText.Text = timeOfDayManager.GetTimeString();

            switch(gameState)
            {
                case GameState.Game:
                    Engine.CurrentTimeRate = Engine.DefaultTimeRate;
                    break;
                default:
                    Engine.CurrentTimeRate = 0.0f;
                    break;
            }

            switch (gameState)
            {
                case GameState.InteractionMenu:
                    {
                        if (InputBindings.Exit.Pressed)
                        {
                            foreach (var gameUIElement in gameUIElements)
                            {
                                MasterUIEntity.AddChild(gameUIElement);
                            }

                            MasterUIEntity.RemoveChild(interactionMenuUI);

                            gameState = GameState.Game;
                        }
                    }
                    break;
                case GameState.Quests:
                    {
                        if (InputBindings.Exit.Pressed)
                        {
                            foreach (var gameUIElement in gameUIElements)
                            {
                                MasterUIEntity.AddChild(gameUIElement);
                            }

                            MasterUIEntity.RemoveChild(questsUI);

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
                        if (InputBindings.Exit.Pressed && inventoryUI.IsItemGrabbed() == false)
                        {
                            foreach (var gameUIElement in gameUIElements)
                            {
                                MasterUIEntity.AddChild(gameUIElement);
                            }

                            inventoryUI.Close();
                            MasterUIEntity.RemoveChild(inventoryUI);

                            gameState = GameState.Game;
                        }
                    }
                    break;
                case GameState.Crafter:
                    {
                        if (InputBindings.Exit.Pressed)
                        {
                            foreach (var gameUIElement in gameUIElements)
                            {
                                MasterUIEntity.AddChild(gameUIElement);
                            }

                            MasterUIEntity.RemoveChild(crafterUI);

                            gameState = GameState.Game;
                        }
                    }
                    break;
                case GameState.StorageInventory:
                    {
                        if (InputBindings.Exit.Pressed)
                        {
                            foreach (var gameUIElement in gameUIElements)
                            {
                                MasterUIEntity.AddChild(gameUIElement);
                            }

                            storageInventoryUI.Close();
                            MasterUIEntity.RemoveChild(storageInventoryUI);

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

                        Item currentPlayerItem = PlayerInventory.GetSlotItem(hotbarCurrentSlotIndex);

                        InteractableEntity interactableEntity = TryGetInteractableEntityOnTile(mouseTile);

                        if (interactableEntity != null)
                        {
                            Color cursorColor = Color.White;

                            if (Vector2.Distance(playerTile, mouseTile) > PLAYER_INTERACTION_DISTANCE)
                            {
                                cursorColor = Color.White * 0.5f;
                            }

                            ChangeCursor(ResourcesManager.GetTexture("Sprites", "interaction_cursor"), cursorColor);
                        }

                        bool canUseInventoryItem = true;

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
                                    PlayerInventory.RemoveItem(buildingItem, 1, inventoryHotbar.CurrentSlotIndex);
                                }
                            }
                        }
                        else if (Vector2.Distance(playerTile, mouseTile) <= PLAYER_INTERACTION_DISTANCE)
                        {
                            tileSelector.IsVisible = true;
                            tileSelector.LocalPosition = CurrentGameLocation.MapToWorld(mouseTile);

                            if (MInput.Mouse.PressedLeftButton)
                            {
                                player.InteractWithTile(tileX, tileY);
                            }
                            else if (MInput.Mouse.PressedRightButton)
                            {
                                if (TryToTeleport(mouseTile))
                                {
                                    canUseInventoryItem = false;
                                }
                                else if (interactableEntity != null)
                                {
                                    selectedInteractableEntity = interactableEntity;

                                    OpenInteractionMenuUI(selectedInteractableEntity.
                                        GetAvailableInteractions(PlayerInventory)
                                        .ToList());

                                    canUseInventoryItem = false;
                                }
                            }
                        }

                        if (MInput.Mouse.PressedRightButton && canUseInventoryItem)
                        {
                            if (currentPlayerItem is ConsumableItem consumableItem)
                            {
                                PlayerEnergyManager.AddEnergy(consumableItem.EnergyAmount);

                                PlayerInventory.RemoveItem(currentPlayerItem, 1, inventoryHotbar.CurrentSlotIndex);
                            }
                            else if(currentPlayerItem is BackpackItem)
                            {
                                PlayerInventory.Expand();

                                PlayerInventory.RemoveItem(currentPlayerItem, 1, inventoryHotbar.CurrentSlotIndex);

                                ResourcesManager.GetSoundEffect("SoundEffects", "Minifantasy_Dungeon_SFX", "04_sack_open_3").Play();
                            }
                        }

                        if (timeOfDayManager.CurrentHour == 0)
                        {
                            StartNextDay();
                        }

                        if (InputBindings.Exit.Pressed)
                        {
                            OpenInventoryUI();
                        }

                        // TODO: temp
                        if (MInput.Keyboard.Pressed(Microsoft.Xna.Framework.Input.Keys.T))
                        {
                            foreach (var gameUIElement in gameUIElements)
                            {
                                MasterUIEntity.RemoveChild(gameUIElement);
                            }

                            MasterUIEntity.AddChild(tradingUI);
                            // TODO: Temp
                            tradingUI.Open(PlayerInventory, PlayerMoneyManager, new List<Item>() 
                            {
                                Engine.ItemsDatabase.GetItemByName("small_backpack"),
                                Engine.ItemsDatabase.GetItemByName("sprinkler"),
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
                        transitionTimer += Engine.DeltaTime;

                        transitionImage.SelfColor = Color.Black * transitionTimer;

                        if (transitionTimer >= 1.0f)
                        {
                            transitionTimer = 0.0f;

                            timeOfDayManager.StartNextDay();

                            foreach (var kvp in gameLocations)
                            {
                                GameLocation gameLocation = kvp.Value;
                                gameLocation.StartNextDay();
                            }

                            if (timeOfDayManager.CurrentWeather == Weather.Sun && timeOfDayManager.CurrentSeason != Season.Winter)
                            {
                                ResourcesManager.GetSoundEffect("SoundEffects", "rooster_crow").Play();
                            }

                            PlayerEnergyManager.RefillEnergy();

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

                            transitionImage.IsVisible = false;

                            gameState = GameState.Game;
                        }
                    }
                    break;
            }

            base.Update();
        }

        private bool TryToTeleport(Vector2 tile)
        {
            if (CurrentGameLocation.TryGetTeleport((int)tile.X, (int)tile.Y) is Teleport teleport)
            {
                GoToLocation(teleport.Location, teleport.Tile);

                return true;
            }

            return false;
        }

        private InteractableEntity TryGetInteractableEntityOnTile(Vector2 tile)
        {
            // Check all creatures to interact with 
            foreach (Creature creature in CurrentGameLocation.GetCreatures())
            {
                if (creature.GetTilePosition() == tile)
                {
                    List<InteractionData> interactionList = creature.GetAvailableInteractions(PlayerInventory).ToList();

                    if (interactionList.Count > 0) return creature;
                }
            }

            if (CurrentGameLocation.GetBuilding((int)tile.X, (int)tile.Y) is InteractableEntity interactableEntity)
            {
                List<InteractionData> interactionList = interactableEntity.GetAvailableInteractions(PlayerInventory).ToList();

                if (interactionList.Count > 0) return interactableEntity;
            }

            return null;
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

        public void OpenInventoryUI()
        {
            foreach (var gameUIElement in gameUIElements)
            {
                MasterUIEntity.RemoveChild(gameUIElement);
            }

            MasterUIEntity.AddChild(inventoryUI);
            inventoryUI.Open(PlayerInventory);

            gameState = GameState.Inventory;
        }

        public void OpenCrafterUI(IEnumerable<CraftingRecipe> craftingRecipes)
        {
            foreach (var gameUIElement in gameUIElements)
            {
                MasterUIEntity.RemoveChild(gameUIElement);
            }
            
            MasterUIEntity.AddChild(crafterUI);
            crafterUI.Open(craftingRecipes);

            gameState = GameState.Crafter;
        }

        public void OpenStorageInventoryUI(Inventory storageInventory)
        {
            foreach (var gameUIElement in gameUIElements)
            {
                MasterUIEntity.RemoveChild(gameUIElement);
            }

            MasterUIEntity.AddChild(storageInventoryUI);
            storageInventoryUI.Open(storageInventory, PlayerInventory);
            
            gameState = GameState.StorageInventory;
        }

        public void OpenQuestsUI()
        {
            foreach (var gameUIElement in gameUIElements)
            {
                MasterUIEntity.RemoveChild(gameUIElement);
            }

            MasterUIEntity.AddChild(questsUI);
            questsUI.Open();

            gameState = GameState.Quests;
        }

        public void OpenInteractionMenuUI(List<InteractionData> interactionList)
        {
            foreach (var gameUIElement in gameUIElements)
            {
                MasterUIEntity.RemoveChild(gameUIElement);
            }

            MasterUIEntity.AddChild(interactionMenuUI);
            interactionMenuUI.Open(interactionList);

            interactionMenuUI.LocalPosition = MInput.Mouse.UIScaledPosition;

            gameState = GameState.InteractionMenu;
        }

        private void OnInventoryHotbarCurrentSlotIndexChanged(int slotIndex)
        {
            SetCurrentPlayerItem(PlayerInventory.GetSlotItem(slotIndex));
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

        private void OnInteractionMenuInteractionSelected(InteractionData interactionData)
        {
            foreach (var gameUIElement in gameUIElements)
            {
                MasterUIEntity.AddChild(gameUIElement);
            }

            MasterUIEntity.RemoveChild(interactionMenuUI);

            gameState = GameState.Game;

            selectedInteractableEntity.Interact(interactionData, PlayerInventory);
        }
    }
}
