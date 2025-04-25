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
        public TreeItem[] TreeItems { get; init; }
        public SeedItem[] SeedItems { get; init; }
        public TreeSeedItem[] TreeSeedItems { get; init; }
        public BedItem[] BedItems { get; init; }
        public ResourceItem[] ResourceItems { get; init; }
        public FloorPathItem[] FloorPathItems { get; init; }
        public ManualCrafterItem[] ManualCrafterItems { get; init; }
        public WindowItem[] WindowItems { get; init; }
        public SprinklerItem[] SprinklerItems { get; init; }
        public GateItem[] GateItems { get; init; }
        public AnimalSpawnerItem[] AnimalSpawnerItems { get; init; }
        public AnimalFeederItem[] AnimalFeederItems { get; init; }
        public BirdNestItem[] BirdNestItems { get; init; }
        public BackpackItem[] BackpackItems { get; init; }
        public GrassItem[] GrassItems { get; init; }
        public WaterSourceItem[] WaterSourceItems { get; init; }

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
            ReadAndInitializeCollection(WindowItems, itemsIcons);
            ReadAndInitializeCollection(SprinklerItems, itemsIcons);
            ReadAndInitializeCollection(GateItems, itemsIcons);
            ReadAndInitializeCollection(AnimalSpawnerItems, itemsIcons);
            ReadAndInitializeCollection(AnimalFeederItems, itemsIcons);
            ReadAndInitializeCollection(BirdNestItems, itemsIcons);
            ReadAndInitializeCollection(BackpackItems, itemsIcons);
            ReadAndInitializeCollection(GrassItems, itemsIcons);
            ReadAndInitializeCollection(WaterSourceItems, itemsIcons);

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
