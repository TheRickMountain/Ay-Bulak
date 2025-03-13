using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public static class GameCommands
    {
#if !CONSOLE

        [Command("set_time_speed", "Set time speed")]
        private static void SetTimeSpeed(int value)
        {
            Engine.TimeRate = value;
        }

        [Command("start_next_day", "Start next day")]
        private static void StartNextDay()
        {
            if (Engine.CurrentScene is GameplayScene)
            {
                ((GameplayScene)Engine.CurrentScene).StartNextDay();
            }
        }

        [Command("go_to_location", "Transition to location")]
        private static void GoToLocation(string locationId, int x, int y)
        {
            if (Engine.CurrentScene is GameplayScene)
            {
                ((GameplayScene)Engine.CurrentScene).GoToLocation(locationId, new Vector2(x, y) * Engine.TILE_SIZE);
            }
        }

        [Command("add_item", "Adds an item to the players inventory")]
        private static void AddItem(string itemName, int itemAmount)
        {
            Item item = Engine.ItemsDatabase.GetItemByName<Item>(itemName);

            if (item == null) 
            {
                Engine.Commands.Log($"Item '{itemName}' not found", Color.Red);

                return;
            }

            if (Engine.CurrentScene is GameplayScene)
            {
                ((GameplayScene)Engine.CurrentScene).Inventory.TryAddItem(item, itemAmount, 0);
            }
        }

        [Command("spawn_item", "Spawns an item in the current game location")]
        private static void SpawnItem(string itemName, int itemAmount)
        {
            Item item = Engine.ItemsDatabase.GetItemByName<Item>(itemName);

            if (Engine.CurrentScene is GameplayScene)
            {
                ItemContainer itemContainer = new ItemContainer();
                itemContainer.Item = item;
                itemContainer.Quantity = itemAmount;
                ((GameplayScene)Engine.CurrentScene).CurrentGameLocation
                    .AddItem(MInput.Mouse.GlobalPosition, itemContainer);
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
