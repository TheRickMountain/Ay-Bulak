using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public class FarmLocation : GameLocation
    {

        public FarmLocation(string id, TimeOfDayManager timeOfDayManager) : base(id, 64, 64, timeOfDayManager)
        {

        }

    }
}
