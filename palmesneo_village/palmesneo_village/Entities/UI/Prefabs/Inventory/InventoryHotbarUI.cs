using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace palmesneo_village
{
    public class InventoryHotbarUI : HorizontalContainerUI
    {
        private Inventory inventory;
        private InventoryHotbar inventoryHotbar;

        private List<InventorySlotUI> slots;

        private ImageUI slotSelector;

        public InventoryHotbarUI(Inventory inventory, InventoryHotbar inventoryHotbar)
        {
            this.inventory = inventory;
            this.inventoryHotbar = inventoryHotbar;

            inventory.ItemAdded += OnInventoryItemChanged;
            inventory.ItemRemoved += OnInventoryItemChanged;
            inventoryHotbar.CurrentSlotIndexChanged += OnInventoryHotbarCurrentSlotIndexChanged;

            slots = new List<InventorySlotUI>();

            for (int i = 0; i < inventory.Width; i++)
            {
                InventorySlotUI slot = new InventorySlotUI();

                slots.Add(slot);

                AddChild(slot);
            }

            slotSelector = new ImageUI();
            slotSelector.Texture = RenderManager.Pixel;
            slotSelector.SelfColor = Color.Red;
            slotSelector.Size = new Vector2(16, 16);
            AddChild(slotSelector);

            Size = new Vector2(Size.X, slots[0].Size.Y);

            UpdateSlotSelectorPosition(inventoryHotbar.CurrentSlotIndex);
        }

        private void UpdateSlotSelectorPosition(int slotIndex)
        {
            InventorySlotUI slot = slots[slotIndex];
            slotSelector.LocalPosition = slot.LocalPosition;
        }

        private void OnInventoryItemChanged(Item item, int quantity, int slotIndex)
        {
            if ((slots.Count - 1) < slotIndex)
                return;

            ItemContainer itemContainer = inventory.GetSlotItemContainer(slotIndex);

            if (itemContainer == null)
            {
                slots[slotIndex].Clear();
            }
            else
            {
                slots[slotIndex].SetItem(itemContainer);
            }
        }
        
        private void OnInventoryHotbarCurrentSlotIndexChanged(int slotIndex)
        {
            UpdateSlotSelectorPosition(slotIndex);
        }
    }
}
