using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public class GateItem : BuildingItem
    {

        public MTexture OpenTexture { get; private set; }
        public MTexture CloseTexture { get; private set; }
        public string OpenSoundEffect { get; init; }
        public string CloseSoundEffect { get; init; }

        public override void Initialize(MTileset sourceTileset)
        {
            MTexture texture = ResourcesManager.GetTexture("Items", Name);

            CloseTexture = new MTexture(texture, 0, 0, texture.Width / 2, texture.Height);
            OpenTexture = new MTexture(texture, texture.Width / 2, 0, texture.Width / 2, texture.Height);

            DirectionIcon[Direction.Down] = CloseTexture;

            Icon = CloseTexture;
        }

    }
}
