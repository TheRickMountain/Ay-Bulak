﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public class ItemsDatabase
    {
        public Item[] Items { get; init; }
        public ToolItem[] ToolItems { get; init; }
        public ConsumableItem[] ConsumableItems { get; init; }
        public BuildingItem[] BuildingItems { get; init; }
        public PlantItem[] PlantItems { get; init; }
        public TreeItem[] TreeItems { get; init; }
        public SeedItem[] SeedItems { get; init; }
        public TreeSeedItem[] TreeSeedItems { get; init; }
        public BedItem[] BedItems { get; init; }
        public ResourceItem[] ResourceItems { get; init; }
        public FloorPathItem[] FloorPathItems { get; init; }
        public ManualCrafterItem[] ManualCrafterItems { get; init; }
        public AutoCrafterItem[] AutoCrafterItems { get; init; }
        public WindowItem[] WindowItems { get; init; }
        public SprinklerItem[] SprinklerItems { get; init; }
        public AnimalFeederItem[] AnimalFeederItems { get; init; }
        public BirdNestItem[] BirdNestItems { get; init; }
        public BackpackItem[] BackpackItems { get; init; }
        public GrassItem[] GrassItems { get; init; }
        public WaterSourceItem[] WaterSourceItems { get; init; }
        public StorageItem[] StorageItems { get; init; }
        public AnimalItem[] AnimalItems { get; init; }
        public AdultAnimalItem[] AdultAnimalItems { get; init; }
        public BabyAnimalItem[] BabyAnimalItems { get; init; }

        private Dictionary<string, Item> nameItemPairs;

        private Dictionary<int, FloorPathItem> floorPathItemsByTilesetIndex;

        public void Initialize(MTileset itemsIcons)
        {
            nameItemPairs = new Dictionary<string, Item>();

            ReadAndInitializeCollection(Items, itemsIcons);
            ReadAndInitializeCollection(ToolItems, itemsIcons);
            ReadAndInitializeCollection(ConsumableItems, itemsIcons);
            ReadAndInitializeCollection(BuildingItems, itemsIcons);
            ReadAndInitializeCollection(PlantItems, itemsIcons);
            ReadAndInitializeCollection(TreeItems, itemsIcons);
            ReadAndInitializeCollection(SeedItems, itemsIcons);
            ReadAndInitializeCollection(TreeSeedItems, itemsIcons);
            ReadAndInitializeCollection(BedItems, itemsIcons);
            ReadAndInitializeCollection(ResourceItems, itemsIcons);
            ReadAndInitializeCollection(FloorPathItems, itemsIcons);
            ReadAndInitializeCollection(ManualCrafterItems, itemsIcons);
            ReadAndInitializeCollection(AutoCrafterItems, itemsIcons);
            ReadAndInitializeCollection(WindowItems, itemsIcons);
            ReadAndInitializeCollection(SprinklerItems, itemsIcons);
            ReadAndInitializeCollection(AnimalFeederItems, itemsIcons);
            ReadAndInitializeCollection(BirdNestItems, itemsIcons);
            ReadAndInitializeCollection(BackpackItems, itemsIcons);
            ReadAndInitializeCollection(GrassItems, itemsIcons);
            ReadAndInitializeCollection(WaterSourceItems, itemsIcons);
            ReadAndInitializeCollection(StorageItems, itemsIcons);
            ReadAndInitializeCollection(AnimalItems, itemsIcons);
            ReadAndInitializeCollection(AdultAnimalItems, itemsIcons);
            ReadAndInitializeCollection(BabyAnimalItems, itemsIcons);

            floorPathItemsByTilesetIndex = new Dictionary<int, FloorPathItem>();

            foreach(var floorPathItem in FloorPathItems)
            {
                floorPathItemsByTilesetIndex.Add(floorPathItem.TilesetIndex, floorPathItem);
            }
        }

        public Item GetItemByName(string name)
        {
            if (nameItemPairs.ContainsKey(name))
            {
                return nameItemPairs[name];
            }

            return null;
        }

        public T GetItemByName<T>(string name) where T : Item
        {
            if (nameItemPairs.ContainsKey(name))
            {
                return (T)nameItemPairs[name];
            }

            return null;
        }
    
        public FloorPathItem GetFloorPathItemByTilesetIndex(int tilesetIndex)
        {
            return floorPathItemsByTilesetIndex[tilesetIndex];
        }

        public Ingredient ConvertJsonIngredient(JsonIngredient jsonIngredient)
        {
            Item item = GetItemByName(jsonIngredient.Item);

            if (item == null)
            {
                throw new Exception($"Item with name {jsonIngredient.Item} not found in the database.");
            }

            return new Ingredient(item, jsonIngredient.Amount);
        }

        public CraftingRecipe ConvertJsonCraftingRecipe(JsonCraftingRecipe jsonCraftingRecipe)
        {
            Ingredient resultIngredient = ConvertJsonIngredient(jsonCraftingRecipe.Result);

            List<Ingredient> requiredIngredients = new();

            foreach (JsonIngredient jsonIngredient in jsonCraftingRecipe.RequiredIngredients)
            {
                requiredIngredients.Add(ConvertJsonIngredient(jsonIngredient));
            }

            float craftingTimeInHours = jsonCraftingRecipe.CraftingTimeInHours;

            return new CraftingRecipe(resultIngredient, requiredIngredients, craftingTimeInHours);
        }

        private void ReadAndInitializeCollection(Item[] itemsArray, MTileset itemsIcons)
        {
            foreach(Item item in itemsArray)
            {
                item.Initialize(itemsIcons);

                nameItemPairs.Add(item.Name, item);
            }
        }

        
    }
}
