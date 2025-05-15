using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public enum InteractionType
    {
        Gather,
        Craft,
        Cancel,
        Sleep,
        ManualCraft,
        Open
    }

    public class InteractionData
    {
        public InteractionType InteractionType { get; private set; }
        public MTexture Icon { get; private set; }
        public string Information { get; private set; }
        public bool IsActive { get; private set; }

        public InteractionData(InteractionType interactionType, MTexture icon, string information, bool isActive)
        {
            InteractionType = interactionType;
            Icon = icon;
            Information = information;
            IsActive = isActive;
        }

    }
}
