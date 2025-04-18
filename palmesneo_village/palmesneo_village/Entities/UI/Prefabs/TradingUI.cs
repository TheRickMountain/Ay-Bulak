using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using palmesneo_village.Entities;

namespace palmesneo_village
{
    public class TradingUI : EntityUI
    {

        private TraderUI traderUI;
        private TradingInventoryUI playerInventoryUI;
        private TextUI playerMoneyText;

        public TradingUI(Inventory inventory, PlayerMoneyManager playerMoneyManager)
        {
            playerMoneyManager.MoneyAmountChanged += (x) => playerMoneyText.Text = x.ToString();

            traderUI = new TraderUI();
            traderUI.Anchor = Anchor.TopCenter;
            AddChild(traderUI);

            playerInventoryUI = new TradingInventoryUI(inventory, playerMoneyManager);
            playerInventoryUI.Anchor = Anchor.BottomCenter;
            AddChild(playerInventoryUI);

            playerMoneyText = new TextUI();
            playerMoneyText.Anchor = Anchor.BottomLeft;
            playerMoneyText.LocalPosition = new Vector2(0, 16);
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
