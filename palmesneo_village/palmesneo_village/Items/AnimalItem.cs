using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;

namespace palmesneo_village
{
    public class AnimalItem : Item
    {
        public float MovementSpeed { get; init; }

        public Dictionary<Direction, MTexture> DirectionTexture { get; private set; } = new();

        public override void Initialize(MTileset sourceTileset)
        {
            MTexture texture = ResourcesManager.GetTexture("Items", Name);

            int directionTextureWidth = texture.Width / 4;
            int directionTextureHeight = texture.Height;

            foreach (Direction direction in Enum.GetValues<Direction>())
            {
                MTexture directionTexture = new MTexture(texture, new Rectangle(
                    directionTextureWidth * (int)direction,
                    0,
                    directionTextureWidth,
                    directionTextureHeight));

                DirectionTexture.Add(direction, directionTexture);
            }

            Icon = DirectionTexture[Direction.Left];
        }
    }
}
