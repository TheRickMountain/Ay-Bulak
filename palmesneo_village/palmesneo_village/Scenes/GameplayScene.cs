using Microsoft.Xna.Framework;
using System;

namespace palmesneo_village
{
    public class GameplayScene : Scene
    {
        public GameLocation CurrentGameLocation { get; private set; }

        public Inventory Inventory { get; private set; }

        public PlayerMoneyManager PlayerMoneyManager { get; private set; }

        public PlayerEnergyManager PlayerEnergyManager { get; private set; }

        private InventoryHotbar inventoryHotbar;

        private TimeOfDayManager timeOfDayManager;

        private Player player;

        private TileSelector tileSelector;

        private TextUI timeText;

        public override void Begin()
        {
            Inventory = new Inventory(10, 4);

            inventoryHotbar = new InventoryHotbar(Inventory);
            MasterEntity.AddChild(inventoryHotbar);

            timeOfDayManager = new TimeOfDayManager();
            MasterUIEntity.AddChild(timeOfDayManager);

            FarmLocation farmLocation = new FarmLocation();
            MasterEntity.AddChild(farmLocation);

            CurrentGameLocation = farmLocation;

            CreatureTemplate creatureTemplate = new CreatureTemplate("Player", ResourcesManager.GetTexture("Sprites", "player"), 100);

            player = new Player(creatureTemplate, Inventory);
            player.LocalPosition = new Vector2(20 * Engine.TILE_SIZE, 20 * Engine.TILE_SIZE);
            MasterEntity.AddChild(player);

            player.SetCurrentLocation(farmLocation);

            CameraMovement cameraMovement = new CameraMovement();
            cameraMovement.Target = player;
            cameraMovement.Bounds = farmLocation.GetBoundaries();
            MasterEntity.AddChild(cameraMovement);

            tileSelector = new TileSelector();
            MasterEntity.AddChild(tileSelector);

            InventoryHotbarUI inventoryHotbarUI = new InventoryHotbarUI(Inventory, inventoryHotbar);
            inventoryHotbarUI.Anchor = Anchor.BottomCenter;
            MasterUIEntity.AddChild(inventoryHotbarUI);

            PlayerEnergyManager = new PlayerEnergyManager(100, 100);

            PlayerEnergyBarUI playerEnergyBarUI = new PlayerEnergyBarUI(PlayerEnergyManager);
            playerEnergyBarUI.Anchor = Anchor.TopLeft;
            playerEnergyBarUI.LocalPosition = new Vector2(5, 5);
            MasterUIEntity.AddChild(playerEnergyBarUI);

            PlayerMoneyManager = new PlayerMoneyManager();

            PlayerMoneyUI playerMoneyUI = new PlayerMoneyUI(PlayerMoneyManager);
            playerMoneyUI.Anchor = Anchor.BottomLeft;
            playerMoneyUI.LocalPosition = new Vector2(5, -5);
            MasterUIEntity.AddChild(playerMoneyUI);

            timeText = new TextUI();
            timeText.Anchor = Anchor.TopLeft;
            timeText.LocalPosition = new Vector2(5, 32);
            MasterUIEntity.AddChild(timeText);

            base.Begin();
        }

        public override void Update()
        {
            timeText.Text = timeOfDayManager.GetTimeString();

            var mouseTile = CurrentGameLocation.WorldToMap(MInput.Mouse.GlobalPosition);

            tileSelector.IsVisible = false;
            tileSelector.LocalPosition = CurrentGameLocation.MapToWorld(mouseTile);

            Vector2 playerTile = CurrentGameLocation.WorldToMap(player.LocalPosition);

            Item item = inventoryHotbar.TryGetCurrentSlotItem();

            if (CanShowTileSelector(playerTile, mouseTile, 4))
            {
                if (item != null)
                {
                    int tileX = (int)mouseTile.X;
                    int tileY = (int)mouseTile.Y;

                    if(CurrentGameLocation.CanInteractWithTile(tileX, tileY, item))
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
                if(item != null && item is ConsumableItem)
                {
                    ConsumableItem consumableItem = (ConsumableItem)item;

                    PlayerEnergyManager.AddEnergy(consumableItem.EnergyAmount);

                    Inventory.RemoveItem(consumableItem, 1, inventoryHotbar.CurrentSlotIndex);
                }
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
