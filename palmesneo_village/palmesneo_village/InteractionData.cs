using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public class InteractionData
    {
        public MTexture Icon { get; private set; }
        public string Information { get; private set; }

        public InteractionData(MTexture icon, string information)
        {
            Icon = icon;
            Information = information;
        }

    }
}
