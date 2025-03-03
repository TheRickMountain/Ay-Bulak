using Microsoft.Xna.Framework;
using System;

namespace palmesneo_village
{
    public class GameplayScene : Scene
    {
        public Inventory Inventory { get; private set; }

        private InventoryHotbar inventoryHotbar;

        private Player player;

        private GameLocation currentGameLocation;

        private TileSelector tileSelector;

        private Vector2 mouseTile;

        public override void Begin()
        {
            Inventory = new Inventory(10, 4);

            inventoryHotbar = new InventoryHotbar(Inventory);
            MasterEntity.AddChild(inventoryHotbar);

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

            InventoryHotbarUI inventoryHotbarUI = new InventoryHotbarUI(Inventory, inventoryHotbar);
            inventoryHotbarUI.Anchor = Anchor.BottomCenter;
            MasterUIEntity.AddChild(inventoryHotbarUI);

            tileSelector = new TileSelector();
            MasterEntity.AddChild(tileSelector);

            base.Begin();
        }

        public override void Update()
        {
            mouseTile = currentGameLocation.WorldToMap(MInput.Mouse.GlobalPosition);

            tileSelector.LocalPosition = currentGameLocation.MapToWorld(mouseTile);

            if (MInput.Mouse.PressedLeftButton)
            {
                Item item = inventoryHotbar.TryGetCurrentSlotItem();

                if (currentGameLocation.CanInteractWithTile((int)mouseTile.X, (int)mouseTile.Y, item))
                {
                    currentGameLocation.InteractWithTile((int)mouseTile.X, (int)mouseTile.Y, item);
                }
            }

            base.Update();
        }

    }
}
