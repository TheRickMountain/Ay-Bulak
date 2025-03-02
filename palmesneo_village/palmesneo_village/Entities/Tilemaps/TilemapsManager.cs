using System;
using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class TilemapsManager : Entity
    {
        private Tilemap groundTilemap;

        public TilemapsManager(Tilemap groundTilemap)
        {
            this.groundTilemap = groundTilemap;
        }
    }
}
