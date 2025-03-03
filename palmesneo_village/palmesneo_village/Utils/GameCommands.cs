using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public static class GameCommands
    {
#if !CONSOLE

        [Command("add_item", "Adds an item to the players inventory")]
        private static void AddItem(string itemName, int itemAmount)
        {
            Item item = Engine.ItemsDatabase.GetItemByName(itemName);

            if(Engine.CurrentScene is GameplayScene)
            {
                ((GameplayScene)Engine.CurrentScene).Inventory.TryAddItem(item, itemAmount);
            }
        }

        [Command("ui_scale", "Sets UI scale")]
        private static void SetUIScale(float value)
        {
            Engine.GlobalUIScale = value;
        }

        [Command("debug_render", "Activate/Deactivate debug render")]
        private static void SetDebugRender(bool value)
        {
            Engine.DebugRender = value;
        }

        [Command("add_money", "Adds money to the player")]
        private static void AddMoney(int amount)
        {
            if (Engine.CurrentScene is GameplayScene)
            {
                ((GameplayScene)Engine.CurrentScene).MoneyAmountManager.MoneyAmount += amount;
            }
        }
#endif
    }
}
