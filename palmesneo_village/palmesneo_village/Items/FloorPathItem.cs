using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public class FloorPathItem : BuildingItem
    {
        public int TilesetIndex { get; init; }
        public float MovementSpeefBuff { get; init; }

        public override void Initialize(MTileset sourceTileset)
        {
            DirectionIcon[Direction.Down] = Engine.FloorPathTileset[TilesetIndex * 16];

            Icon = DirectionIcon[Direction.Down];
        }
    }
}
