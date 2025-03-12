using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace palmesneo_village
{
    public class InventoryUI : PanelUI
    {
        private Inventory inventory;

        private GridContainerUI grid;

        private List<SlotButtonUI> inventorySlots;

        private ItemContainer itemContainer;

        public InventoryUI(Inventory inventory)
        {
            this.inventory = inventory;

            grid = new GridContainerUI();
            grid.Anchor = Anchor.Center;
            grid.Columns = inventory.Width;
            AddChild(grid);

            inventorySlots = new List<SlotButtonUI>(inventory.Width * inventory.Height);

            for (int x = 0; x < inventory.Width; x++)
            {
                for (int y = 0; y < inventory.Height; y++)
                {
                    SlotButtonUI inventorySlotUI = new SlotButtonUI();

                    inventorySlots.Add(inventorySlotUI);

                    grid.AddChild(inventorySlotUI);
                }
            }

            Size = grid.Size + new Vector2(16, 16);
        }

        public void Open()
        {
            for (int i = 0; i < inventory.Width * inventory.Height; i++)
            {
                Item item = inventory.GetSlotItem(i);
                if (item == null) continue;

                int quantity = inventory.GetSlotQuantity(i);
                int contentAmount = inventory.GetSlotContentAmount(i);

                inventorySlots[i].SetItem(item, quantity, contentAmount);
            }
        }

    }
}
