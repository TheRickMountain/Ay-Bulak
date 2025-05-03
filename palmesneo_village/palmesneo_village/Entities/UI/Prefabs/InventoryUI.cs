using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public class InventoryUI : NewBaseInventoryUI
    {
        public Action<int> SlotPressed { get; set; }

        protected override void OnInventorySlotPressed(ButtonUI button, int slotIndex)
        {
            SlotPressed?.Invoke(slotIndex);
        }
    }
}
