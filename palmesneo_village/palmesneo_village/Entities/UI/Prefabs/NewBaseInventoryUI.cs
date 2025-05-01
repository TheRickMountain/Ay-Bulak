using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public abstract class NewBaseInventoryUI : PanelUI
    {
        protected Inventory CurrentInventory { get; private set; }

        private List<SlotButtonUI> slotPool = new();

        private List<SlotButtonUI> activeSlots = new();

        private GridContainerUI grid;
        private const int MAX_SLOTS_TO_POOL = 50;

        protected NewBaseInventoryUI()
        {
            grid = new GridContainerUI();
            grid.Anchor = Anchor.Center;
            AddChild(grid);

            InitializeSlotPool(MAX_SLOTS_TO_POOL);

            activeSlots = new List<SlotButtonUI>();
        }

        private void InitializeSlotPool(int count)
        {
            for (int i = 0; i < count; i++)
            {
                int slotIndex = i;
                SlotButtonUI slot = new SlotButtonUI();
                slot.ActionTriggered += (button) => OnInventorySlotPressed(button, slotIndex);
                slotPool.Add(slot);
            }
        }

        public void Open(Inventory inventory)
        {
            CurrentInventory = inventory;
            CurrentInventory.SlotDataChanged += OnInventorySlotDataChanged;

            grid.Columns = CurrentInventory.Columns;

            int requiredSlotsAmount = CurrentInventory.Columns * CurrentInventory.Rows;

            ClearActiveSlots();

            SetupActiveSlots(requiredSlotsAmount);

            UpdateActiveSlots();
        }

        public void Close()
        {
            CurrentInventory.SlotDataChanged -= OnInventorySlotDataChanged;
            CurrentInventory = null;
        }

        protected abstract void OnInventorySlotPressed(ButtonUI button, int slotIndex);

        private void ClearActiveSlots()
        {
            foreach (var slot in activeSlots)
            {
                slot.Clear();
                grid.RemoveChild(slot);
            }

            activeSlots.Clear();
        }

        private void SetupActiveSlots(int requiredCount)
        {
            if (requiredCount > slotPool.Count)
            {
                Console.WriteLine($"Warning: Required slots ({requiredCount}) exceed pool size ({slotPool.Count}). " +
                    $"Consider increasing MAX_SLOTS_TO_PREPARE.");
                requiredCount = slotPool.Count; // Ограничиваемся размером пула
            }

            for (int i = 0; i < requiredCount; i++)
            {
                SlotButtonUI slot = slotPool[i];
                grid.AddChild(slot);
                activeSlots.Add(slot);
            }

            Size = grid.Size + new Vector2(16, 16);
        }

        private void OnInventorySlotDataChanged(Inventory inventory, int slotIndex)
        {
            UpdateActiveSlots();
        }

        private void UpdateActiveSlots()
        {
            for (int slotIndex = 0; slotIndex < activeSlots.Count; slotIndex++)
            {
                Item item = CurrentInventory.GetSlotItem(slotIndex);
                int quantity = CurrentInventory.GetSlotQuantity(slotIndex);
                int contentAmount = CurrentInventory.GetSlotContentAmount(slotIndex);

                if (item == null)
                {
                    activeSlots[slotIndex].Clear();
                }
                else
                {
                    activeSlots[slotIndex].SetItem(item, quantity, contentAmount);
                    ModifySlotAppearance(slotIndex, item);
                }
            }
        }

        protected virtual void ModifySlotAppearance(int slotIndex, Item item)
        {
        }
    }
}
