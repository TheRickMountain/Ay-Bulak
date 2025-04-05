using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public class AnimalFeederItem : BuildingItem
    {
        public MTexture EmptyTexture { get; private set; }
        public MTexture FullTexture { get; private set; }

        public override void Initialize(MTileset sourceTileset)
        {
            MTexture texture = ResourcesManager.GetTexture("Items", Name);

            EmptyTexture = new MTexture(texture, 0, 0, texture.Width / 2, texture.Height);
            FullTexture = new MTexture(texture, texture.Width / 2, 0, texture.Width / 2, texture.Height);

            DirectionTexture[Direction.Down] = EmptyTexture;

            Icon = EmptyTexture;
        }

    }
}
