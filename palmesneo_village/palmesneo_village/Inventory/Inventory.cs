using System;

namespace palmesneo_village
{
    public class Inventory
    {
        public int Columns { get; private set; }
        public int Rows { get; private set; }

        public Action<Inventory, int> SlotDataChanged { get; set; }
        public Action<Inventory> InventoryExpanded { get; set; }

        private int maxRows;

        private ItemContainer[,] slotsByGrid;
        private ItemContainer[] slotsByIndex;

        public Inventory(int columns, int rows, int maxRows)
        {
            Columns = columns;
            Rows = rows;
            this.maxRows = maxRows;

            InitializeAndPopulateCollections();
        }

        private void InitializeAndPopulateCollections()
        {
            slotsByGrid = new ItemContainer[Columns, Rows];
            slotsByIndex = new ItemContainer[Columns * Rows];

            int slotIndex = 0;

            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    ItemContainer itemContainer = new ItemContainer();

                    slotsByGrid[x, y] = itemContainer;

                    slotsByIndex[slotIndex] = itemContainer;

                    slotIndex++;
                }
            }
        }

        public bool IsSlotEmpty(int slotIndex)
        {
            return slotsByIndex[slotIndex].Item == null;
        }

        public Item GetSlotItem(int slotIndex)
        {
            return slotsByIndex[slotIndex].Item;
        }

        public int GetItemQuantity(Item item)
        {
            int quantity = 0;

            for (int i = 0; i < slotsByIndex.Length; i++)
            {
                if (slotsByIndex[i].Item != item) continue;

                quantity += slotsByIndex[i].Quantity;
            }

            return quantity;
        }

        public int GetSlotQuantity(int slotIndex)
        {
            return slotsByIndex[slotIndex].Quantity;
        }

        public int GetSlotContentAmount(int slotIndex)
        {
            return slotsByIndex[slotIndex].ContentAmount;
        }

        public void SubSlotItemContentAmount(int slotIndex, int value)
        {
            ItemContainer itemContainer = slotsByIndex[slotIndex];

            itemContainer.ContentAmount -= value;

            if(itemContainer.ContentAmount < 0)
            {
                throw new InvalidOperationException("Content amount can't be lower than zero!");
            }

            SlotDataChanged?.Invoke(this, slotIndex);
        }

        public void AddSlotItemContentAmount(int slotIndex, int value)
        {
            ItemContainer itemContainer = slotsByIndex[slotIndex];

            ToolItem toolItem = itemContainer.Item as ToolItem;

            itemContainer.ContentAmount = Math.Clamp(itemContainer.ContentAmount + value, 0, toolItem.Capacity);

            SlotDataChanged?.Invoke(this, slotIndex);
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

        public bool CanAddItem(Item item, int quantity)
        {
            if(item.IsStackable)
            {
                if(GetSlotIndexWithItem(item) != -1)
                {
                    return true;
                }
            }
            
            return GetEmptySlotIndex() != -1;
        }

        public void AddItem(Item item, int quantity, int contentAmount, int slotIndex)
        {
            ItemContainer slotItemContainer = slotsByIndex[slotIndex];

            if(slotItemContainer.Item == null)
            {
                slotItemContainer.Item = item;
                slotItemContainer.Quantity = quantity;
                slotItemContainer.ContentAmount = contentAmount;
            }
            else if(slotItemContainer.Item == item)
            {
                slotItemContainer.Quantity += quantity;
            }
            else
            {
                throw new InvalidOperationException($"Slot[{slotIndex}] has another item already");
            }

            SlotDataChanged?.Invoke(this, slotIndex);
        }

        public bool TryAddItem(Item item, int quantity, int contentAmount)
        {
            if (item.IsStackable)
            {
                // Trying to find and add an item to an existing item
                int slotIndexWithItem = GetSlotIndexWithItem(item);

                if (slotIndexWithItem != -1)
                {
                    slotsByIndex[slotIndexWithItem].Quantity += quantity;

                    SlotDataChanged?.Invoke(this, slotIndexWithItem);

                    return true;
                }
            }

            // Trying to add an item to an empty slot
            int emptySlotIndex = GetEmptySlotIndex();

            if (emptySlotIndex != -1)
            {
                slotsByIndex[emptySlotIndex].Item = item;
                slotsByIndex[emptySlotIndex].Quantity = quantity;
                slotsByIndex[emptySlotIndex].ContentAmount = contentAmount;

                SlotDataChanged?.Invoke(this, emptySlotIndex);

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
                slotsByIndex[slotIndex].ContentAmount = 0;
            }

            SlotDataChanged?.Invoke(this, slotIndex);
        }

        public void RemoveItem(Item item, int quantity)
        {
            int leftQuantity = quantity;

            for (int slotIndex = 0; slotIndex < slotsByIndex.Length; slotIndex++)
            {
                if (slotsByIndex[slotIndex].Item != item) continue;

                if (slotsByIndex[slotIndex].Quantity < leftQuantity)
                {
                    leftQuantity -= slotsByIndex[slotIndex].Quantity;

                    RemoveItem(item, slotsByIndex[slotIndex].Quantity, slotIndex);
                }
                else
                {
                    RemoveItem(item, leftQuantity, slotIndex);
                    break;
                }
            }
        }

        public void Expand()
        {
            if(Rows >= maxRows)
            {
                return;
            }

            Rows++;

            ItemContainer[,] newSlotsByGrid = new ItemContainer[Columns, Rows];
            ItemContainer[] newSlotsByIndex = new ItemContainer[Columns * Rows];

            // Копируем старые слоты в новые
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows - 1; y++)
                {
                    newSlotsByGrid[x, y] = slotsByGrid[x, y];
                }
            }

            for (int i = 0; i < slotsByIndex.Length; i++)
            {
                newSlotsByIndex[i] = slotsByIndex[i];
            }

            // Создаем новые слоты
            int slotIndex = Columns * (Rows - 1);
            for (int x = 0; x < Columns; x++)
            {
                ItemContainer itemContainer = new ItemContainer();
                newSlotsByGrid[x, Rows - 1] = itemContainer;
                newSlotsByIndex[slotIndex] = itemContainer;
                slotIndex++;
            }

            slotsByGrid = newSlotsByGrid;
            slotsByIndex = newSlotsByIndex;

            InventoryExpanded?.Invoke(this);
        }
    }
}
