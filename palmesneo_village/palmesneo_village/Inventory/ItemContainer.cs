using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public class ItemContainer
    {

        public Item Item { get; set; }
        public int Quantity { get; set; }
        public int ContentAmount { get; set; }

        public void Clear()
        {
            Item = null;
            Quantity = 0;
            ContentAmount = 0;
        }

    }
}
