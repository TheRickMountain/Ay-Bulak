using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public class Inventory
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        private ItemContainer[,] slots;

        public Inventory(int width, int height)
        {
            Width = width;
            Height = height;

            slots = new ItemContainer[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    slots[x, y] = new ItemContainer();
                }
            }
        }

        public ItemContainer GetSlot(int gridX, int gridY)
        {
            if (gridX < 0 || gridX >= Width || gridY < 0 || gridY >= Height)
                return null;

            return slots[gridX, gridY];
        }

    }
}
