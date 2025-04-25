using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public class SmokeSpawnData
    {
        public int X { get; init; }
        public int Y { get; init; }
    
        public Vector2 GetPosition()
        {
            return new Vector2(X, Y);
        }
    }
}
