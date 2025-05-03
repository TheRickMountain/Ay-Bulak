using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace palmesneo_village
{
    public class StorageInventoryUI : EntityUI
    {
        private Inventory storageInventory;
        private Inventory playerInventory;

        private InventoryUI storageInventoryUI;
        private InventoryUI playerInventoryUI;

        public StorageInventoryUI()
        {
            storageInventoryUI = new InventoryUI();
            storageInventoryUI.Anchor = Anchor.TopCenter;
            AddChild(storageInventoryUI);

            playerInventoryUI = new InventoryUI();
            playerInventoryUI.Anchor = Anchor.BottomCenter;
            AddChild(playerInventoryUI);

            storageInventoryUI.SlotPressed += (slotIndex) => OnInventorySlotPressed(storageInventory, slotIndex);
            playerInventoryUI.SlotPressed += (slotIndex) => OnInventorySlotPressed(playerInventory, slotIndex);
        }

        public void Open(Inventory storageInventory, Inventory playerInventory)
        {
            this.storageInventory = storageInventory;
            this.playerInventory = playerInventory;
            
            storageInventoryUI.Open(storageInventory);
            playerInventoryUI.Open(playerInventory);

            Size = storageInventoryUI.Size + playerInventoryUI.Size + new Vector2(5);
        }

        public void Close()
        {
            storageInventoryUI.Close();
            playerInventoryUI.Close();

            storageInventoryUI.SlotPressed -= (slotIndex) => OnInventorySlotPressed(storageInventory, slotIndex);
            playerInventoryUI.SlotPressed -= (slotIndex) => OnInventorySlotPressed(playerInventory, slotIndex);
        }

        private void OnInventorySlotPressed(Inventory inventory, int slotIndex)
        {
            Inventory toInventory = inventory == playerInventory ? storageInventory : playerInventory;

            if (inventory.IsSlotEmpty(slotIndex)) return;

            Item item = inventory.GetSlotItem(slotIndex);
            int quantity = inventory.GetItemQuantity(item);
            int contentAmount = inventory.GetSlotContentAmount(slotIndex);

            if (toInventory.CanAddItem(item, quantity) == false) return;

            inventory.RemoveItem(item, quantity, slotIndex);

            toInventory.TryAddItem(item, quantity, contentAmount);
        }
    }
}
