using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace palmesneo_village
{
    public abstract class BaseInventoryUI : PanelUI
    {
        protected Inventory Inventory { get; private set; }
        protected List<SlotButtonUI> InventorySlots { get; private set; }

        private GridContainerUI grid;

        public BaseInventoryUI(Inventory inventory)
        {
            this.Inventory = inventory;

            inventory.SlotDataChanged += (inventory, slotIndex) => RefreshInventory();

            grid = new GridContainerUI();
            grid.Anchor = Anchor.Center;
            grid.Columns = inventory.Width;
            AddChild(grid);

            InventorySlots = new List<SlotButtonUI>(inventory.Width * inventory.Height);

            for (int i = 0; i < inventory.Width * inventory.Height; i++)
            {
                SlotButtonUI inventorySlotUI = new SlotButtonUI();

                int slotIndex = i;

                inventorySlotUI.ActionTriggered += (button) => OnInventorySlotPressed(button, slotIndex);

                InventorySlots.Add(inventorySlotUI);

                grid.AddChild(inventorySlotUI);
            }

            Size = grid.Size + new Vector2(16, 16);
        }

        protected abstract void OnInventorySlotPressed(ButtonUI button, int slotIndex);

        private void RefreshInventory()
        {
            for (int slotIndex = 0; slotIndex < Inventory.Width * Inventory.Height; slotIndex++)
            {
                Item item = Inventory.GetSlotItem(slotIndex);
                int quantity = Inventory.GetSlotQuantity(slotIndex);
                int contentAmount = Inventory.GetSlotContentAmount(slotIndex);

                if (item == null)
                {
                    InventorySlots[slotIndex].Clear();
                }
                else
                {
                    InventorySlots[slotIndex].SetItem(item, quantity, contentAmount);
                    ModifySlotAppearance(slotIndex, item);
                }
            }
        }

        protected virtual void ModifySlotAppearance(int slotIndex, Item item)
        {
        }
    }
}
