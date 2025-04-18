using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using palmesneo_village.Entities;

namespace palmesneo_village
{
    public class TradingUI : VerticalContainerUI
    {

        private TraderUI traderUI;
        private TradingInventoryUI playerInventoryUI;
        private TextUI playerMoneyText;

        public TradingUI(Inventory inventory, PlayerMoneyManager playerMoneyManager)
        {
            playerMoneyManager.MoneyAmountChanged += (x) => playerMoneyText.Text = x.ToString();

            traderUI = new TraderUI();
            AddChild(traderUI);

            playerInventoryUI = new TradingInventoryUI(inventory, playerMoneyManager);
            AddChild(playerInventoryUI);

            playerMoneyText = new TextUI();
            playerMoneyText.Text = "Test";
            AddChild(playerMoneyText);

            Size = new Vector2(
                playerInventoryUI.Size.X,
                playerInventoryUI.Size.Y + 16 + traderUI.Size.Y);
        }

        public void Open(Inventory playerInventory, PlayerMoneyManager playerMoneyManager, List<Item> items)
        {
            traderUI.Open(playerInventory, playerMoneyManager, items);
        }
    }
}
