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

        [Command("spawn_item", "Spawns an item in the current game location")]
        private static void SpawnItem(string itemName, int itemAmount)
        {
            Item item = Engine.ItemsDatabase.GetItemByName(itemName);

            if (Engine.CurrentScene is GameplayScene)
            {
                ((GameplayScene)Engine.CurrentScene).CurrentGameLocation
                    .AddItem(MInput.Mouse.GlobalPosition, item, itemAmount);
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
                ((GameplayScene)Engine.CurrentScene).PlayerMoneyManager.MoneyAmount += amount;
            }
        }

        [Command("add_energy", "Adds energy to the player")]
        private static void AddEnergy(int amount)
        {
            if (Engine.CurrentScene is GameplayScene)
            {
                ((GameplayScene)Engine.CurrentScene).PlayerEnergyManager.AddEnergy(amount);
            }
        }

        [Command("consumer_energy", "Consumes energy from the player")]
        private static void ConsumeEnergy(int amount)
        {
            if (Engine.CurrentScene is GameplayScene)
            {
                ((GameplayScene)Engine.CurrentScene).PlayerEnergyManager.ConsumeEnergy(amount);
            }
        }

        [Command("set_max_energy", "Sets the max energy of the player")]
        private static void SetMaxEnergy(int value)
        {
            if (Engine.CurrentScene is GameplayScene)
            {
                ((GameplayScene)Engine.CurrentScene).PlayerEnergyManager.SetMaxEnergy(value);
            }
        }
#endif
    }
}
