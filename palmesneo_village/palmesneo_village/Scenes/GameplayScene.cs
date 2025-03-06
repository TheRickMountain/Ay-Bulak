using Microsoft.Xna.Framework;
using System;

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

            tileSelector = new TileSelector();
            tileSelector.Depth = 100;
            MasterEntity.AddChild(tileSelector);

            PlayerEnergyManager = new PlayerEnergyManager(100, 100);
            PlayerMoneyManager = new PlayerMoneyManager();

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
                var mouseTile = CurrentGameLocation.WorldToMap(MInput.Mouse.GlobalPosition);

                tileSelector.IsVisible = false;
                tileSelector.LocalPosition = CurrentGameLocation.MapToWorld(mouseTile);

                Vector2 playerTile = CurrentGameLocation.WorldToMap(player.LocalPosition);

                Item item = inventoryHotbar.TryGetCurrentSlotItem();

                int tileX = (int)mouseTile.X;
                int tileY = (int)mouseTile.Y;

                if (CanShowTileSelector(playerTile, mouseTile, 4))
                {
                    if (item != null)
                    {
                        if (CurrentGameLocation.CanInteractWithTile(tileX, tileY, item))
                        {
                            tileSelector.IsVisible = true;

                            if (MInput.Mouse.PressedLeftButton)
                            {
                                CurrentGameLocation.InteractWithTile(tileX, tileY, item);

                                PlayerEnergyManager.ConsumeEnergy(1);
                            }
                        }
                    }
                }

                if (MInput.Mouse.PressedRightButton)
                {
                    if (item != null)
                    {
                        if (item is ConsumableItem)
                        {
                            ConsumableItem consumableItem = (ConsumableItem)item;

                            PlayerEnergyManager.AddEnergy(consumableItem.EnergyAmount);

                            Inventory.RemoveItem(consumableItem, 1, inventoryHotbar.CurrentSlotIndex);
                        }
                        else if (item is BuildingItem)
                        {
                            BuildingItem buildingItem = (BuildingItem)item;

                            if (CurrentGameLocation.CanBuildBuilding(tileX, tileY, buildingItem, Direction.Down))
                            {
                                CurrentGameLocation.BuildBuilding(tileX, tileY, buildingItem, Direction.Down);

                                Inventory.RemoveItem(buildingItem, 1, inventoryHotbar.CurrentSlotIndex);
                            }
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

        private bool CanShowTileSelector(Vector2 playerTile, Vector2 mouseTile, int maxDistance)
        {
            if (playerTile == mouseTile)
            {
                return false;
            }

            float distance = Vector2.Distance(playerTile, mouseTile);

            return distance < maxDistance;
        }

    }
}
