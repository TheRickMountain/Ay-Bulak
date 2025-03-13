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
        public bool IsRotatable { get; init; }
        public TeleportData TeleportData { get; init; }

        public Dictionary<Direction, MTexture> DirectionIcon { get; private set; }

        public override void Initialize(MTileset sourceTileset)
        {
            DirectionIcon = new();

            if (IsRotatable)
            {
                DirectionIcon.Add(Direction.Down, ResourcesManager.GetTexture("Items", Name + "_down"));
                DirectionIcon.Add(Direction.Up, ResourcesManager.GetTexture("Items", Name + "_up"));
                DirectionIcon.Add(Direction.Left, ResourcesManager.GetTexture("Items", Name + "_left"));
                DirectionIcon.Add(Direction.Right, ResourcesManager.GetTexture("Items", Name + "_right"));

                Icon = DirectionIcon[Direction.Down];
            }
            else
            {
                DirectionIcon.Add(Direction.Down, ResourcesManager.GetTexture("Items", Name));

                Icon = DirectionIcon[Direction.Down];
            }
        }
    }
}
