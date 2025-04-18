using Microsoft.Xna.Framework;

namespace palmesneo_village.Entities
{
    public class TradingInventoryUI : BaseInventoryUI
    {
        private PlayerMoneyManager playerMoneyManager;

        public TradingInventoryUI(Inventory inventory, PlayerMoneyManager playerMoneyManager) : base(inventory)
        {
            this.playerMoneyManager = playerMoneyManager;
        }

        protected override void OnInventorySlotPressed(ButtonUI button, int slotIndex)
        {
            Item item = Inventory.GetSlotItem(slotIndex);

            if (item == null || item.Price == 0)
                return;

            int quantity = Inventory.GetSlotQuantity(slotIndex);

            playerMoneyManager.MoneyAmount += item.Price * quantity;

            Inventory.RemoveItem(item, quantity, slotIndex);
        }

        protected override void ModifySlotAppearance(int slotIndex, Item item)
        {
            if (item.Price == 0)
            {
                InventorySlots[slotIndex].IsDisabled = true;
                InventorySlots[slotIndex].GetChildByName<ImageUI>("Icon").SelfColor = Color.White * 0.5f;
                InventorySlots[slotIndex].GetChildByName<TextUI>("Quantity").SelfColor = Color.White * 0.5f;
            }
            else
            {
                InventorySlots[slotIndex].IsDisabled = false;
                InventorySlots[slotIndex].GetChildByName<ImageUI>("Icon").SelfColor = Color.White;
                InventorySlots[slotIndex].GetChildByName<TextUI>("Quantity").SelfColor = Color.White;
            }
        }
    }
}
