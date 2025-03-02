using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public enum ToolType
    {
        Hoe,
        WateringCan
    }

    public class ToolItemTypesData
    {
        public static string GetName(ToolType toolType)
        {
            switch (toolType)
            {
                case ToolType.Hoe: return LocalizationManager.GetText("hoe");
                case ToolType.WateringCan: return LocalizationManager.GetText("watering_can");
                default: return $"enum.{toolType}";
            }
        }
    }
}
