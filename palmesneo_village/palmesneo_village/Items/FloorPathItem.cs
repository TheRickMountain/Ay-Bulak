﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public class FloorPathItem : BuildingItem
    {
        public int TilesetIndex { get; init; }
        public float MovementSpeedBuff { get; init; }
        public string FootstepSoundEffect { get; init; }

        public override void Initialize(MTileset sourceTileset)
        {
            DirectionTexture[Direction.Down] = Engine.FloorPathTileset[TilesetIndex * 16];

            Icon = DirectionTexture[Direction.Down];
        }
    }
}
