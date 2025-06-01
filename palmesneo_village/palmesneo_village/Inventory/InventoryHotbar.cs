using Microsoft.Xna.Framework.Input;
using System;

namespace palmesneo_village
{
    public class InventoryHotbar : Entity
    {
        public int CurrentSlotIndex { get; private set; } = 0;
        public Action<int> CurrentSlotIndexChanged { get; set; }

        public bool IsBlocked { get; set; } = false;

        private int slotsCount;

        public InventoryHotbar(Inventory inventory)
        {
            slotsCount = inventory.Columns;
        }

        public override void Update()
        {
            base.Update();

            if (IsBlocked) return;

            UpdateWheelInput();

            UpdateKeyboardInput();

            UpdateGamepadInput();
        }

        private void UpdateWheelInput()
        {
            int wheelDelta = MInput.Mouse.WheelDelta;

            if (wheelDelta != 0)
            {
                SetCurrentSlot(CurrentSlotIndex - 1 * Math.Sign(wheelDelta));
            }
        }

        private void UpdateKeyboardInput()
        {
            for (int i = 0; i < 10; i++)
            {
                if (MInput.Keyboard.Pressed(Keys.D1 + i))
                {
                    SetCurrentSlot(i);
                }
            }

            if (MInput.Keyboard.Pressed(Keys.D0))
            {
                SetCurrentSlot(9);
            }
        }

        private void UpdateGamepadInput()
        {
            if (InputBindings.Previous.Pressed)
            {
                SetCurrentSlot(CurrentSlotIndex - 1);
            }
            else if (InputBindings.Next.Pressed)
            {
                SetCurrentSlot(CurrentSlotIndex + 1);
            }
        }
        
        public void SetCurrentSlot(int slotIndex)
        {
            if(IsBlocked) return;

            CurrentSlotIndex = Math.Clamp(slotIndex, 0, slotsCount - 1);
            CurrentSlotIndexChanged?.Invoke(CurrentSlotIndex);
        }
    }
}
