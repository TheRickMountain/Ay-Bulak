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
        public bool IsPassable { get; init; }
        public TeleportData TeleportData { get; init; }

        public Dictionary<Direction, MTexture> DirectionIcon { get; private set; } = new()
        {
            {Direction.Down, RenderManager.Pixel},
            {Direction.Up, RenderManager.Pixel},
            {Direction.Left, RenderManager.Pixel},
            {Direction.Right, RenderManager.Pixel}
        };

        public override void Initialize(MTileset sourceTileset)
        {
            if (IsRotatable)
            {
                DirectionIcon[Direction.Down] = ResourcesManager.GetTexture("Items", Name + "_down");
                DirectionIcon[Direction.Up] = ResourcesManager.GetTexture("Items", Name + "_up");
                DirectionIcon[Direction.Left] = ResourcesManager.GetTexture("Items", Name + "_left");
                DirectionIcon[Direction.Right] = ResourcesManager.GetTexture("Items", Name + "_right");

                Icon = DirectionIcon[Direction.Down];
            }
            else
            {
                DirectionIcon[Direction.Down] = ResourcesManager.GetTexture("Items", Name);

                Icon = DirectionIcon[Direction.Down];
            }
        }
    }
}
