using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class WorldInteractionManager : Entity
    {
        private Tilemap tilemap;
        private InventoryHotbar inventoryHotbar;

        private TileSelector tileSelector;

        private Vector2 mouseTile;

        public WorldInteractionManager(Tilemap tilemap, InventoryHotbar inventoryHotbar)
        {
            this.tilemap = tilemap;
            this.inventoryHotbar = inventoryHotbar;

            tileSelector = new TileSelector();
            AddChild(tileSelector);
        }

        public override void Update()
        {
            base.Update();

            mouseTile = tilemap.WorldToMap(MInput.Mouse.GlobalPosition);

            tileSelector.LocalPosition = tilemap.MapToWorld(mouseTile);

            // TEMP: test
            if(MInput.Mouse.PressedLeftButton)
            {
                Item item = inventoryHotbar.TryGetCurrentSlotItem();

                if(item != null)
                {
                    if(item is ToolItem toolItem)
                    {
                        if (toolItem.ToolType == ToolType.Hoe)
                        {
                            tilemap.SetCell((int)mouseTile.X, (int)mouseTile.Y, 3);
                        }
                    }
                }
            }
            // TEMP: test
        }

    }
}
