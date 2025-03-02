using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace palmesneo_village
{
    public class InventoryHotbarUI : HorizontalContainerUI
    {
        private Inventory inventory;

        private List<InventorySlotUI> slots;

        private ImageUI slotSelector;

        private int currentSlotIndex = 0;

        public InventoryHotbarUI(Inventory inventory)
        {
            this.inventory = inventory;

            slots = new List<InventorySlotUI>();

            for (int i = 0; i < inventory.Width; i++)
            {
                InventorySlotUI slot = new InventorySlotUI();

                slots.Add(slot);

                AddChild(slot);
            }

            slotSelector = new ImageUI();
            slotSelector.Texture = RenderManager.Pixel;
            slotSelector.SelfColor = Color.Red;
            slotSelector.Size = new Vector2(16, 16);
            AddChild(slotSelector);

            Size = new Vector2(Size.X, slots[0].Size.Y);

            UpdateSlotSelectorPosition(currentSlotIndex);
        }

        public override void Update()
        {
            base.Update();

            UpdateWheelInput();

            UpdateKeyboardInput();
        }

        private void UpdateWheelInput()
        {
            int wheelDelta = MInput.Mouse.WheelDelta;

            if (wheelDelta != 0)
            {
                currentSlotIndex = Math.Clamp(currentSlotIndex - 1 * Math.Sign(wheelDelta), 0, slots.Count - 1);

                UpdateSlotSelectorPosition(currentSlotIndex);
            }
        }

        private void UpdateKeyboardInput()
        {
            for (int i = 0; i < 10; i++)
            {
                if (MInput.Keyboard.Pressed(Keys.D1 + i))
                {
                    currentSlotIndex = i;
                    UpdateSlotSelectorPosition(currentSlotIndex);
                }
            }

            if (MInput.Keyboard.Pressed(Keys.D0))
            {
                currentSlotIndex = 9;
                UpdateSlotSelectorPosition(currentSlotIndex);
            }
        }

        private void UpdateSlotSelectorPosition(int slotIndex)
        {
            InventorySlotUI slot = slots[slotIndex];
            slotSelector.LocalPosition = slot.LocalPosition;
        }

    }
}
