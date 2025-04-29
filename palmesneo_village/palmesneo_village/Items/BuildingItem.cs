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

        /// <summary>
        /// This building's sprite has no height and should not participate in DepthSorting
        /// </summary>
        public bool IsFlat { get; init; }
        public TeleportData TeleportData { get; init; }
        public SmokeSpawnData SmokeSpawnData { get; init; }

        public Dictionary<Direction, MTexture> DirectionTexture { get; private set; } = new()
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
                DirectionTexture[Direction.Down] = ResourcesManager.GetTexture("Items", Name + "_down");
                DirectionTexture[Direction.Up] = ResourcesManager.GetTexture("Items", Name + "_up");
                DirectionTexture[Direction.Left] = ResourcesManager.GetTexture("Items", Name + "_left");
                DirectionTexture[Direction.Right] = ResourcesManager.GetTexture("Items", Name + "_right");
            }
            else
            {
                DirectionTexture[Direction.Down] = ResourcesManager.GetTexture("Items", Name);
            }

            Icon = DirectionTexture[Direction.Down];
        }
    }
}
