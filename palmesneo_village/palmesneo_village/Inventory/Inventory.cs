using System;

namespace palmesneo_village
{
    public class Inventory
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Action<Item, int, int> ItemAdded { get; set; }
        public Action<Item, int, int> ItemRemoved { get; set; }

        private ItemContainer[,] slotsByGrid;
        private ItemContainer[] slotsByIndex;

        public Inventory(int width, int height)
        {
            Width = width;
            Height = height;

            InitializeAndPopulateCollections();
        }

        private void InitializeAndPopulateCollections()
        {
            slotsByGrid = new ItemContainer[Width, Height];
            slotsByIndex = new ItemContainer[Width * Height];

            int slotIndex = 0;

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    ItemContainer itemContainer = new ItemContainer();

                    slotsByGrid[x, y] = itemContainer;

                    slotsByIndex[slotIndex] = itemContainer;

                    slotIndex++;
                }
            }
        }

        public ItemContainer GetSlotItemContainer(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= slotsByIndex.Length)
                return null;

            return slotsByIndex[slotIndex];
        }

        private int GetSlotIndexWithItem(Item item)
        {
            for (int i = 0; i < slotsByIndex.Length; i++)
            {
                if (slotsByIndex[i].Item == item)
                    return i;
            }
            return -1;
        }

        private int GetEmptySlotIndex()
        {
            for (int i = 0; i < slotsByIndex.Length; i++)
            {
                if (slotsByIndex[i].Item == null)
                    return i;
            }
            return -1;
        }

        public bool TryAddItem(ItemContainer itemContainer)
        {
            Item item = itemContainer.Item;
            int quantity = itemContainer.Quantity;

            if (item.IsStackable)
            {
                // Trying to find and add an item to an existing item
                int slotIndexWithItem = GetSlotIndexWithItem(item);

                if (slotIndexWithItem != -1)
                {
                    slotsByIndex[slotIndexWithItem].Quantity += quantity;

                    ItemAdded?.Invoke(item, quantity, slotIndexWithItem);

                    return true;
                }
            }

            // Trying to add an item to an empty slot
            int emptySlotIndex = GetEmptySlotIndex();

            if (emptySlotIndex != -1)
            {
                slotsByIndex[emptySlotIndex] = itemContainer;

                ItemAdded?.Invoke(item, quantity, emptySlotIndex);

                return true;
            }

            return false;
        }

        public void RemoveItem(Item item, int quantity, int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= slotsByIndex.Length)
                throw new IndexOutOfRangeException();

            if (slotsByIndex[slotIndex].Item != item)
                throw new Exception("Item does not match the item in the slot");

            if(slotsByIndex[slotIndex].Quantity < quantity)
                throw new Exception("Quantity is greater than the quantity in the slot");

            slotsByIndex[slotIndex].Quantity -= quantity;

            if (slotsByIndex[slotIndex].Quantity == 0)
            {
                slotsByIndex[slotIndex].Item = null;
            }

            ItemRemoved?.Invoke(item, quantity, slotIndex);
        }
    }
}
