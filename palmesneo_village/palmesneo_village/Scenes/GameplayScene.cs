using Microsoft.Xna.Framework;
using System;

namespace palmesneo_village
{
    public class GameplayScene : Scene
    {
        public Inventory Inventory { get; private set; }

        public MoneyAmountManager MoneyAmountManager { get; private set; }

        private InventoryHotbar inventoryHotbar;

        private TimeOfDayManager timeOfDayManager;

        private Player player;

        private GameLocation currentGameLocation;

        private TileSelector tileSelector;

        private Vector2 mouseTile;

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

            currentGameLocation = farmLocation;

            CreatureTemplate creatureTemplate = new CreatureTemplate("Player", ResourcesManager.GetTexture("Sprites", "player"), 100);

            player = new Player(creatureTemplate);
            MasterEntity.AddChild(player);

            CameraMovement cameraMovement = new CameraMovement();
            cameraMovement.Target = player;
            cameraMovement.Bounds = farmLocation.GetBoundaries();
            MasterEntity.AddChild(cameraMovement);

            tileSelector = new TileSelector();
            MasterEntity.AddChild(tileSelector);

            InventoryHotbarUI inventoryHotbarUI = new InventoryHotbarUI(Inventory, inventoryHotbar);
            inventoryHotbarUI.Anchor = Anchor.BottomCenter;
            MasterUIEntity.AddChild(inventoryHotbarUI);

            MoneyAmountManager = new MoneyAmountManager();

            MoneyAmountUI moneyAmountUI = new MoneyAmountUI(MoneyAmountManager);
            moneyAmountUI.Anchor = Anchor.BottomLeft;
            moneyAmountUI.LocalPosition = new Vector2(5, -5);
            MasterUIEntity.AddChild(moneyAmountUI);

            timeText = new TextUI();
            timeText.Anchor = Anchor.TopLeft;
            timeText.LocalPosition = new Vector2(5, 5);
            MasterUIEntity.AddChild(timeText);

            base.Begin();
        }

        public override void Update()
        {
            tileSelector.IsVisible = false;

            timeText.Text = timeOfDayManager.GetTimeString();

            mouseTile = currentGameLocation.WorldToMap(MInput.Mouse.GlobalPosition);

            tileSelector.LocalPosition = currentGameLocation.MapToWorld(mouseTile);

            Vector2 playerTile = currentGameLocation.WorldToMap(player.LocalPosition);

            float distanceBetweenPlayerAndMouseTile = Vector2.Distance(playerTile, mouseTile);

            if(distanceBetweenPlayerAndMouseTile < 4)
            {
                Item item = inventoryHotbar.TryGetCurrentSlotItem();

                if (item != null)
                {
                    int tileX = (int)mouseTile.X;
                    int tileY = (int)mouseTile.Y;

                    if(currentGameLocation.CanInteractWithTile(tileX, tileY, item))
                    {
                        tileSelector.IsVisible = true;

                        if (MInput.Mouse.PressedLeftButton)
                        {
                            currentGameLocation.InteractWithTile(tileX, tileY, item);
                        }
                    }
                }
            }

            base.Update();
        }

    }
}
