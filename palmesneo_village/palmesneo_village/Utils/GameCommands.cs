﻿using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public static class GameCommands
    {
#if !CONSOLE

        private static bool AssertItem(string itemName, int itemAmount)
        {
            Item item = Engine.ItemsDatabase.GetItemByName<Item>(itemName);
            
            if (item == null)
            {
                Engine.Commands.Log($"Item '{itemName}' not found", Color.Red);
                return false;
            }

            if (itemAmount <= 0)
            {
                Engine.Commands.Log("Item amount can't be lower than 1", Color.Red);
                return false;
            }

            return true;
        }

        [Command("spawn_animal", "Spawns an animal in the current game location")]
        private static void SpawnAnimal(string animaItemName)
        {
            AnimalItem animalItem = Engine.ItemsDatabase.GetItemByName<AnimalItem>(animaItemName);

            if(animalItem == null)
            {
                Engine.Commands.Log($"Animal '{animaItemName}' not found", Color.Red);
                return;
            }

            if (Engine.CurrentScene is GameplayScene gameplayScene)
            {
                GameLocation currentGameLocation = gameplayScene.CurrentGameLocation;
                
                Vector2 spawnTile = currentGameLocation.WorldToMap(MInput.Mouse.GlobalPosition);

                gameplayScene.CurrentGameLocation.TrySpawnAnimal(animalItem, (int)spawnTile.X, (int)spawnTile.Y);
            }
        }

        [Command("spawn_npc", "Spawns an NPC in the current game location")]
        private static void SpawnNPC()
        {
            if(Engine.CurrentScene is GameplayScene gameplayScene)
            {
                GameLocation currentGameLocation = gameplayScene.CurrentGameLocation;

                Vector2 spawnTile = currentGameLocation.WorldToMap(MInput.Mouse.GlobalPosition);

                ((GameplayScene)Engine.CurrentScene).CurrentGameLocation.TrySpawnNPC((int)spawnTile.X, (int)spawnTile.Y);
            }
        }

        [Command("set_time_speed", "Set time speed")]
        private static void SetTimeSpeed(int value)
        {
            Engine.DefaultTimeRate = value;
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

        [Command("expand_inventory", "Expand inventory to next expansion level")]
        private static void ExpandInventory()
        {
            if (Engine.CurrentScene is GameplayScene)
            {
                ((GameplayScene)Engine.CurrentScene).PlayerInventory.Expand();
            }
        }

        [Command("add_item", "Adds an item to the players inventory")]
        private static void AddItem(string itemName, int itemAmount)
        {
            if (AssertItem(itemName, itemAmount) == false) return;

            if (Engine.CurrentScene is GameplayScene)
            {
                Item item = Engine.ItemsDatabase.GetItemByName<Item>(itemName);

                ((GameplayScene)Engine.CurrentScene).PlayerInventory.TryAddItem(item, itemAmount, 0);
            }
        }

        [Command("add_item_to_slot", "Adds an item to the players inventory")]
        private static void AddItemToSlot(string itemName, int itemAmount, int slotIndex)
        {
            if (AssertItem(itemName, itemAmount) == false) return;

            if (Engine.CurrentScene is GameplayScene)
            {
                Item item = Engine.ItemsDatabase.GetItemByName<Item>(itemName);

                ((GameplayScene)Engine.CurrentScene).PlayerInventory.AddItem(item, itemAmount, 0, slotIndex);
            }
        }

        [Command("spawn_item", "Spawns an item in the current game location")]
        private static void SpawnItem(string itemName, int itemAmount)
        {
            if (AssertItem(itemName, itemAmount) == false) return;

            if (Engine.CurrentScene is GameplayScene)
            {
                Item item = Engine.ItemsDatabase.GetItemByName<Item>(itemName);

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
