using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class TradingUI : PanelUI
    {

        private Inventory inventory;
        private PlayerMoneyManager playerMoneyManager;

        private GridContainerUI playerInventoryGrid;
        private List<SlotButtonUI> playerInventorySlots;

        private GridContainerUI traderInventoryGrid;
        private List<SlotButtonUI> traderInventorySlots;

        private TextUI playerMoneyText;

        private List<Item> traderItems;

        public TradingUI(Inventory inventory, PlayerMoneyManager playerMoneyManager)
        {
            this.inventory = inventory;
            this.playerMoneyManager = playerMoneyManager;

            CreateTraderInventory();
            CreatePlayerInventory();

            traderInventoryGrid.LocalPosition = new Vector2(0, 8);
            playerInventoryGrid.LocalPosition = new Vector2(0, -8);

            playerMoneyText = new TextUI();
            playerMoneyText.Anchor = Anchor.BottomLeft;
            playerMoneyText.LocalPosition = new Vector2(0, 16);
            AddChild(playerMoneyText);

            Size = new Vector2(
                playerInventoryGrid.Size.X + 16,
                playerInventoryGrid.Size.Y + 16 + 16 + traderInventoryGrid.Size.Y);

            traderItems = new List<Item>();
        }

        private void CreatePlayerInventory()
        {
            playerInventoryGrid = new GridContainerUI();
            playerInventoryGrid.Anchor = Anchor.BottomCenter;
            playerInventoryGrid.Columns = inventory.Width;
            AddChild(playerInventoryGrid);

            playerInventorySlots = new List<SlotButtonUI>(inventory.Width * inventory.Height);

            for (int i = 0; i < inventory.Width * inventory.Height; i++)
            {
                SlotButtonUI inventorySlotUI = new SlotButtonUI();

                int slotIndex = i;

                inventorySlotUI.ActionTriggered += (button) => OnPlayerInventorySlotPressed(button, slotIndex);

                playerInventorySlots.Add(inventorySlotUI);

                playerInventoryGrid.AddChild(inventorySlotUI);
            }
        }

        private void CreateTraderInventory()
        {
            traderInventoryGrid = new GridContainerUI();
            traderInventoryGrid.Anchor = Anchor.TopCenter;
            traderInventoryGrid.Columns = inventory.Width;
            AddChild(traderInventoryGrid);

            traderInventorySlots = new List<SlotButtonUI>(inventory.Width * inventory.Height);

            for (int i = 0; i < inventory.Width * inventory.Height; i++)
            {
                SlotButtonUI inventorySlotUI = new SlotButtonUI();

                int slotIndex = i;

                inventorySlotUI.ActionTriggered += (button) => OnTraderInventorySlotPressed(button, slotIndex);

                traderInventorySlots.Add(inventorySlotUI);

                traderInventoryGrid.AddChild(inventorySlotUI);
            }
        }

        public void Open(List<Item> items)
        {
            traderItems.Clear();
            traderItems.AddRange(items);

            RefreshUI();
        }

        private void RefreshUI()
        {
            UpdatePlayerInventory();
            UpdateTraderInventory();

            playerMoneyText.Text = playerMoneyManager.MoneyAmount.ToString();
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

                    if (item.Price == 0)
                    {
                        playerInventorySlots[slotIndex].IsDisabled = true;
                        playerInventorySlots[slotIndex].GetChildByName<ImageUI>("Icon").SelfColor = Color.White * 0.5f;
                        playerInventorySlots[slotIndex].GetChildByName<TextUI>("Quantity").SelfColor = Color.White * 0.5f;
                    }
                    else
                    {
                        playerInventorySlots[slotIndex].IsDisabled = false;
                        playerInventorySlots[slotIndex].GetChildByName<ImageUI>("Icon").SelfColor = Color.White;
                        playerInventorySlots[slotIndex].GetChildByName<TextUI>("Quantity").SelfColor = Color.White;
                    }
                }
            }
        }

        private void UpdateTraderInventory()
        {
            for (int slotIndex = 0; slotIndex < traderInventorySlots.Count; slotIndex++)
            {
                if (slotIndex < traderItems.Count)
                {
                    Item item = traderItems[slotIndex];

                    traderInventorySlots[slotIndex].SetItem(item, 999, 0);
                }
                else
                {
                    traderInventorySlots[slotIndex].Clear();
                }
            }
        }

        private void OnPlayerInventorySlotPressed(ButtonUI button, int slotIndex)
        {
            Item item = inventory.GetSlotItem(slotIndex);
            int quantity = inventory.GetSlotQuantity(slotIndex);

            playerMoneyManager.MoneyAmount += item.Price * quantity;

            inventory.RemoveItem(item, quantity, slotIndex);

            RefreshUI();
        }

        private void OnTraderInventorySlotPressed(ButtonUI button, int slotIndex)
        {
            if (slotIndex >= traderItems.Count) return;

            Item item = traderItems[slotIndex];

            if (playerMoneyManager.MoneyAmount >= item.Price)
            {
                int quantity = 1;

                inventory.TryAddItem(item, quantity, 0);

                playerMoneyManager.MoneyAmount -= item.Price * quantity;
            }

            RefreshUI();
        }
    }
}
