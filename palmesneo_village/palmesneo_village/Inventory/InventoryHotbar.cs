using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace palmesneo_village
{
    public class InventoryHotbar : Entity
    {
        public int CurrentSlotIndex { get; private set; } = 0;
        public Action<int> CurrentSlotIndexChanged { get; set; }

        private Inventory inventory;
        private int slotsCount;

        public InventoryHotbar(Inventory inventory)
        {
            this.inventory = inventory;
            slotsCount = inventory.Width;
        }

        public override void Update()
        {
            base.Update();

            UpdateWheelInput();

            UpdateKeyboardInput();
        }

        public Item TryGetCurrentSlotItem()
        {
            return inventory.GetSlot(CurrentSlotIndex).Item;
        }

        private void UpdateWheelInput()
        {
            int wheelDelta = MInput.Mouse.WheelDelta;

            if (wheelDelta != 0)
            {
                CurrentSlotIndex = Math.Clamp(CurrentSlotIndex - 1 * Math.Sign(wheelDelta), 0, slotsCount - 1);

                CurrentSlotIndexChanged?.Invoke(CurrentSlotIndex);
            }
        }

        private void UpdateKeyboardInput()
        {
            for (int i = 0; i < 10; i++)
            {
                if (MInput.Keyboard.Pressed(Keys.D1 + i))
                {
                    CurrentSlotIndex = i;
                    CurrentSlotIndexChanged?.Invoke(CurrentSlotIndex);
                }
            }

            if (MInput.Keyboard.Pressed(Keys.D0))
            {
                CurrentSlotIndex = 9;
                CurrentSlotIndexChanged?.Invoke(CurrentSlotIndex);
            }
        }
    }
}
