using Microsoft.Xna.Framework;
using System;

namespace palmesneo_village
{
    public static class ColorUtils
    {
        public const string YELLOW_HEX = "#F5E61B";

        public static Color HexToColor(string hex)
        {
            return new Color(
                Convert.ToByte(hex.Substring(0, 2), 16),
                Convert.ToByte(hex.Substring(2, 2), 16),
                Convert.ToByte(hex.Substring(4, 2), 16)
            );
        }

    }
}
