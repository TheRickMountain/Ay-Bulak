using System;
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
        public SeedItem[] SeedItems { get; init; }
        public TreeSeedItem[] TreeSeedItems { get; init; }
        public WateringCanItem[] WateringCanItems { get; init; }
        public ShowelItem[] ShowelItems { get; init; }
        public PickaxeItem[] PickaxeItems { get; init; }
        public AxeItem[] AxeItems { get; init; }
        public WaterSourceItem[] WaterSourceItems { get; init; }
        public BedItem[] BedItems { get; init; }

        private Dictionary<string, Item> nameItemPairs;

        public void Initialize(MTileset itemsIcons)
        {
            nameItemPairs = new Dictionary<string, Item>();

            ReadAndInitializeCollection(Items, itemsIcons);
            ReadAndInitializeCollection(ToolItems, itemsIcons);
            ReadAndInitializeCollection(ConsumableItems, itemsIcons);
            ReadAndInitializeCollection(BuildingItems, itemsIcons);
            ReadAndInitializeCollection(PlantItems, itemsIcons);
            ReadAndInitializeCollection(SeedItems, itemsIcons);
            ReadAndInitializeCollection(TreeSeedItems, itemsIcons);
            ReadAndInitializeCollection(WateringCanItems, itemsIcons);
            ReadAndInitializeCollection(ShowelItems, itemsIcons);
            ReadAndInitializeCollection(PickaxeItems, itemsIcons);
            ReadAndInitializeCollection(AxeItems, itemsIcons);
            ReadAndInitializeCollection(WaterSourceItems, itemsIcons);
            ReadAndInitializeCollection(BedItems, itemsIcons);
        }

        public T GetItemByName<T>(string name) where T : Item
        {
            if (nameItemPairs.ContainsKey(name))
            {
                return (T)nameItemPairs[name];
            }

            return null;
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
