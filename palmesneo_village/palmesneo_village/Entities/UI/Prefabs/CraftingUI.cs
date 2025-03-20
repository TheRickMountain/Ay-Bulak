using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class CraftingUI : EntityUI
    {

        private Inventory inventory;

        private PanelUI playerInventoryUI;
        private List<SlotButtonUI> playerInventorySlots;

        private CrafterUI crafterUI;

        public CraftingUI(Inventory inventory)
        {
            this.inventory = inventory;

            inventory.SlotDataChanged += (x, y) => UpdatePlayerInventory();

            crafterUI = new CrafterUI();
            crafterUI.Anchor = Anchor.TopCenter;
            AddChild(crafterUI);

            CreatePlayerInventory();

            Size = new Vector2(
                playerInventoryUI.Size.X,
                playerInventoryUI.Size.Y + 16 + crafterUI.Size.Y);
        }

        private void CreatePlayerInventory()
        {
            playerInventoryUI = new PanelUI();
            playerInventoryUI.Anchor = Anchor.BottomCenter;
            AddChild(playerInventoryUI);

            GridContainerUI playerInventoryGrid = new GridContainerUI();
            playerInventoryGrid.Anchor = Anchor.Center;
            playerInventoryGrid.Columns = inventory.Width;
            playerInventoryUI.AddChild(playerInventoryGrid);

            playerInventorySlots = new List<SlotButtonUI>(inventory.Width * inventory.Height);

            for (int i = 0; i < inventory.Width * inventory.Height; i++)
            {
                SlotButtonUI inventorySlotUI = new SlotButtonUI();

                int slotIndex = i;

                playerInventorySlots.Add(inventorySlotUI);

                playerInventoryGrid.AddChild(inventorySlotUI);
            }

            playerInventoryUI.Size = playerInventoryGrid.Size + new Vector2(16, 16);
        }

        public void Open(Inventory playerInventory, List<CraftingRecipe> craftingRecipes)
        {
            crafterUI.Open(playerInventory, craftingRecipes);
        }

        private void UpdatePlayerInventory()
        {
            for (int slotIndex = 0; slotIndex < inventory.Width * inventory.Height; slotIndex++)
            {
                Item item = inventory.GetSlotItem(slotIndex);
                int quantity = inventory.GetSlotQuantity(slotIndex);
                int contentAmount = inventory.GetSlotContentAmount(slotIndex);

                if (item == null)
                {
                    playerInventorySlots[slotIndex].Clear();
                }
                else
                {
                    playerInventorySlots[slotIndex].SetItem(item, quantity, contentAmount);
                }
            }
        }
    }
}
