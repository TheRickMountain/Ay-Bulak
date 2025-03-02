using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public class ItemsDatabase
    {
        public Item[] Items { get; init; }

        private Dictionary<string, Item> nameItemPairs;

        public void Initialize(MTileset itemsIcons)
        {
            nameItemPairs = new Dictionary<string, Item>();

            foreach (Item item in Items)
            {
                item.Initialize(itemsIcons);

                nameItemPairs.Add(item.Name, item);
            }
        }

        public Item GetItemByName(string name)
        {
            if (nameItemPairs.ContainsKey(name))
            {
                return nameItemPairs[name];
            }

            return null;
        }
    }
}
