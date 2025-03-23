using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace palmesneo_village
{
    public class InventoryHotbarUI : HorizontalContainerUI
    {
        private List<SlotButtonUI> slots;

        private ImageUI slotSelector;

        public InventoryHotbarUI(Inventory inventory, InventoryHotbar inventoryHotbar)
        {
            inventory.SlotDataChanged += OnInventorySlotDataChanged;
            inventoryHotbar.CurrentSlotIndexChanged += OnInventoryHotbarCurrentSlotIndexChanged;

            slots = new List<SlotButtonUI>();

            for (int i = 0; i < inventory.Width; i++)
            {
                SlotButtonUI slot = new SlotButtonUI();

                int slotIndex = i;

                slot.ActionTriggered += (x) => 
                {
                    inventoryHotbar.SetCurrentSlot(slotIndex);
                };

                slots.Add(slot);

                AddChild(slot);
            }

            slotSelector = new ImageUI();
            slotSelector.Texture = ResourcesManager.GetTexture("Sprites", "UI", "slot_selector");
            slotSelector.Size = new Vector2(28, 28);
            AddChild(slotSelector);

            Size = new Vector2(Size.X, slots[0].Size.Y);

            UpdateSlotSelectorPosition(inventoryHotbar.CurrentSlotIndex);
        }

        private void UpdateSlotSelectorPosition(int slotIndex)
        {
            SlotButtonUI slot = slots[slotIndex];
            slotSelector.LocalPosition = slot.LocalPosition - new Vector2(2, 2);
        } 

        private void OnInventorySlotDataChanged(Inventory inventory, int slotIndex)
        {
            if ((slots.Count - 1) < slotIndex)
                return;

            Item item = inventory.GetSlotItem(slotIndex);
            int quantity = inventory.GetSlotQuantity(slotIndex);
            int contentAmount = inventory.GetSlotContentAmount(slotIndex);

            if (item == null)
            {
                slots[slotIndex].Clear();
            }
            else
            {
                slots[slotIndex].SetItem(item, quantity, contentAmount);
            }
        }
        
        private void OnInventoryHotbarCurrentSlotIndexChanged(int slotIndex)
        {
            UpdateSlotSelectorPosition(slotIndex);
        }
    }
}
