using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public enum ToolType
    {
        None,
        Axe,
        Pickaxe,
        WateringCan,
        Showel
    }

    public class ToolItem : Item
    {
        public ToolType ToolType { get; init; }
        public int Efficiency { get; init; }
        public int Capacity { get; init; }
    }
}
