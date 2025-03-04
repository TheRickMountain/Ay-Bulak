using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public class BuildingItem : Item
    {

        public int Width { get; init; }
        public int Height { get; init; }
        public string[,] GroundPattern { get; init; }

        public override void Initialize(MTileset sourceTileset)
        {
            Icon = ResourcesManager.GetTexture("Items", Name);
        }
    }
}
