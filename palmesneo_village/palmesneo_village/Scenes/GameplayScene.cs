using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace palmesneo_village
{
    public class GameplayScene : Scene
    {
        public GameLocation CurrentGameLocation 
        {
            get => currentGameLocation;
            private set 
            {
                // Попытка переключиться на ту же локацию
                if(currentGameLocation == value)
                {
                    return;
                }

                nextGameLocation = value; 
            }
        }

        public Inventory Inventory { get; private set; }

        public PlayerMoneyManager PlayerMoneyManager { get; private set; }

        public PlayerEnergyManager PlayerEnergyManager { get; private set; }

        private InventoryHotbar inventoryHotbar;

        private TimeOfDayManager timeOfDayManager;

        private Player player;

        private TileSelector tileSelector;

        private TextUI timeText;

        private GameLocation currentGameLocation;
        private GameLocation nextGameLocation;

        private FarmLocation farmLocation;
        private HouseLocation houseLocation;

        private BuildingSystem buildingSystem;

        private Item currentPlayerItem;

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
            player.LocalPosition = new Vector2(5 * Engine.TILE_SIZE, 5 * Engine.TILE_SIZE);

            farmLocation = new FarmLocation();
            houseLocation = new HouseLocation();

            CurrentGameLocation = farmLocation;

            buildingSystem = new BuildingSystem(farmLocation);
            MasterEntity.AddChild(buildingSystem.Preview);

            tileSelector = new TileSelector();
            tileSelector.Depth = 100;
            MasterEntity.AddChild(tileSelector);

            PlayerEnergyManager = new PlayerEnergyManager(100, 100);
            PlayerMoneyManager = new PlayerMoneyManager();

            Inventory.ItemAdded += OnInventoryItemChanged;
            Inventory.ItemRemoved += OnInventoryItemChanged;
            inventoryHotbar.CurrentSlotIndexChanged += OnInventoryHotbarCurrentSlotIndexChanged;

            #region UI

            InventoryHotbarUI inventoryHotbarUI = new InventoryHotbarUI(Inventory, inventoryHotbar);
            inventoryHotbarUI.Anchor = Anchor.BottomCenter;
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

            #endregion

            base.Begin();
        }

        public override void Update()
        {
            // Temp
            if(MInput.Keyboard.Pressed(Microsoft.Xna.Framework.Input.Keys.Q))
            {
                CurrentGameLocation = farmLocation;
            }
            else if (MInput.Keyboard.Pressed(Microsoft.Xna.Framework.Input.Keys.E))
            {
                CurrentGameLocation = houseLocation;
            }
            // Temp

            timeText.Text = timeOfDayManager.GetTimeString();

            if (CurrentGameLocation != null)
            {
                Vector2 mouseTile = CurrentGameLocation.WorldToMap(MInput.Mouse.GlobalPosition);
                Vector2 playerTile = CurrentGameLocation.WorldToMap(player.LocalPosition);

                tileSelector.IsVisible = false;

                int tileX = (int)mouseTile.X;
                int tileY = (int)mouseTile.Y;

                // Handle building item selection
                if (currentPlayerItem is BuildingItem buildingItem)
                {
                    buildingSystem.UpdatePreviewPosition(mouseTile);

                    // Handle rotation
                    if (buildingItem.IsRotatable && InputBindings.Rotate.Pressed)
                    {
                        buildingSystem.RotateBuildingPreview();
                    }

                    if(MInput.Mouse.PressedLeftButton)
                    {
                        if (buildingSystem.TryPlaceBuilding(mouseTile))
                        {
                            Inventory.RemoveItem(buildingItem, 1, inventoryHotbar.CurrentSlotIndex);
                        }
                    }
                }

                if (CanShowTileSelector(playerTile, mouseTile, 4))
                {
                    if (currentPlayerItem != null)
                    {
                        if (CurrentGameLocation.CanInteractWithTile(tileX, tileY, currentPlayerItem))
                        {
                            tileSelector.IsVisible = true;
                            tileSelector.LocalPosition = CurrentGameLocation.MapToWorld(mouseTile);

                            if (MInput.Mouse.PressedLeftButton)
                            {
                                CurrentGameLocation.InteractWithTile(tileX, tileY, currentPlayerItem);

                                if(currentPlayerItem is SeedItem)
                                {
                                    Inventory.RemoveItem(currentPlayerItem, 1, inventoryHotbar.CurrentSlotIndex);
                                }
                                else
                                {
                                    PlayerEnergyManager.ConsumeEnergy(1);
                                }
                            }
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
            }

            // Location switching
            if(currentGameLocation != nextGameLocation)
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

                player.SetCurrentLocation(currentGameLocation);
            }

            base.Update();
        }

        public override void Render()
        {
            base.Render();

            if (CurrentGameLocation != null && currentPlayerItem is BuildingItem)
            {
                Vector2 mouseTile = CurrentGameLocation.WorldToMap(MInput.Mouse.GlobalPosition);
                buildingSystem.RenderPreview(mouseTile);
            }
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

        private void OnInventoryItemChanged(Item item, int quantity, int slotIndex)
        {
            if (slotIndex != inventoryHotbar.CurrentSlotIndex)
                return;

            SetCurrentPlayerItem(Inventory.GetSlotItem(slotIndex));
        }

        private void SetCurrentPlayerItem(Item item)
        {
            currentPlayerItem = item;

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
