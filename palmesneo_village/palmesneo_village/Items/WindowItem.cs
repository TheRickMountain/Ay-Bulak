using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public class WindowItem : BuildingItem
    {
        public MTexture DayTexture { get; private set; }
        public MTexture NightTexture { get; private set; }

        public WindowItem()
        {
            
        }

        public override void Initialize(MTileset sourceTileset)
        {
            MTexture texture = ResourcesManager.GetTexture("Items", Name);

            DayTexture = new MTexture(texture, 0, 0, texture.Width / 2, texture.Height);
            NightTexture = new MTexture(texture, texture.Width / 2, 0, texture.Width / 2, texture.Height);

            DirectionTexture[Direction.Down] = DayTexture;

            Icon = DayTexture;
        }

    }
}
