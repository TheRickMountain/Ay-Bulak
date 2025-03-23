using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class TradingUI : EntityUI
    {

        private Inventory inventory;
        private PlayerMoneyManager playerMoneyManager;

        private PanelUI playerInventoryUI;
        private List<SlotButtonUI> playerInventorySlots;

        private TraderUI traderUI;

        private TextUI playerMoneyText;

        public TradingUI(Inventory inventory, PlayerMoneyManager playerMoneyManager)
        {
            this.inventory = inventory;
            this.playerMoneyManager = playerMoneyManager;

            inventory.SlotDataChanged += (x, y) => UpdatePlayerInventory();
            playerMoneyManager.MoneyAmountChanged += (x) => playerMoneyText.Text = x.ToString();

            traderUI = new TraderUI();
            traderUI.Anchor = Anchor.TopCenter;
            AddChild(traderUI);

            CreatePlayerInventory();

            playerMoneyText = new TextUI();
            playerMoneyText.Anchor = Anchor.BottomLeft;
            playerMoneyText.LocalPosition = new Vector2(0, 16);
            AddChild(playerMoneyText);

            Size = new Vector2(
                playerInventoryUI.Size.X,
                playerInventoryUI.Size.Y + 16 + traderUI.Size.Y);
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

                inventorySlotUI.ActionTriggered += (button) => OnPlayerInventorySlotPressed(button, slotIndex);

                playerInventorySlots.Add(inventorySlotUI);

                playerInventoryGrid.AddChild(inventorySlotUI);
            }

            playerInventoryUI.Size = playerInventoryGrid.Size + new Vector2(16, 16);
        }

        public void Open(Inventory playerInventory, PlayerMoneyManager playerMoneyManager, List<Item> items)
        {
            traderUI.Open(playerInventory, playerMoneyManager, items);
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

        private void OnPlayerInventorySlotPressed(ButtonUI button, int slotIndex)
        {
            Item item = inventory.GetSlotItem(slotIndex);
            int quantity = inventory.GetSlotQuantity(slotIndex);

            playerMoneyManager.MoneyAmount += item.Price * quantity;

            inventory.RemoveItem(item, quantity, slotIndex);
        }
    }
}
