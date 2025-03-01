using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;

namespace palmesneo_village
{
    public static class BitmaskGenerator
    {
        private static Dictionary<int, int> bitmask8BitTileIdPairs = new Dictionary<int, int>();

        static BitmaskGenerator()
        {
            JObject jsonData = JObject.Parse(File.ReadAllText(Path.Combine(Engine.ContentDirectory, "Tilesets//blob_sets.json")));

            foreach (var member in jsonData["members"])
            {
                int id = member.Value<int>("id");
                int bitmask = member.Value<int>("role");

                bitmask8BitTileIdPairs.Add(bitmask, id);
            }
        }

        public static int Get4BitBitmask(bool top, bool left, bool right, bool bottom)
        {
            int bitmask = 0;

            bitmask = top ? bitmask + 1 : bitmask;
            bitmask = left ? bitmask + 2 : bitmask;
            bitmask = right ? bitmask + 4 : bitmask;
            bitmask = bottom ? bitmask + 8 : bitmask;

            return bitmask;
        }

        public static int Get8BitBitmask(bool topLeft, bool top, bool topRight, bool left,
            bool right, bool bottomLeft, bool bottom, bool bottomRight)
        {
            int bitmask = 0;

            bitmask = top ? bitmask + 2 : bitmask;
            bitmask = left ? bitmask + 128 : bitmask;
            bitmask = right ? bitmask + 8 : bitmask;
            bitmask = bottom ? bitmask + 32 : bitmask;

            bitmask = (top && left) && topLeft ? bitmask + 1 : bitmask;
            bitmask = (top && right) && topRight ? bitmask + 4 : bitmask;
            bitmask = (bottom && left) && bottomLeft ? bitmask + 64 : bitmask;
            bitmask = (bottom && right) && bottomRight ? bitmask + 16 : bitmask;

            return bitmask;
        }

        public static int GetBitmask8BitTileId(int bitmask)
        {
            return bitmask8BitTileIdPairs[bitmask];
        }

    }
}
