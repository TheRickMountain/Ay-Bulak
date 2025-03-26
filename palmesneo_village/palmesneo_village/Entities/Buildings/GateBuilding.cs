using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MonoGame.Extended.Graphics;

namespace palmesneo_village
{

    public class GateBuilding : Building
    {
        public bool IsOpen { get; private set; } = false;

        private GateItem gateItem;

        public GateBuilding(GameLocation gameLocation, GateItem item, Direction direction, Vector2[,] occupiedTiles)
            : base(gameLocation, item, direction, occupiedTiles)
        {
            gateItem = item;
        }

        public override void InteractAlternatively(Item item, PlayerEnergyManager playerEnergyManager)
        {
            IsOpen = !IsOpen;

            if (IsOpen)
            {
                Sprite.Texture = gateItem.OpenTexture;
                ResourcesManager.GetSoundEffect(gateItem.OpenSoundEffect).Play();
            }
            else
            {
                Sprite.Texture = gateItem.CloseTexture;
                ResourcesManager.GetSoundEffect(gateItem.CloseSoundEffect).Play();
            }

            foreach (Vector2 tile in OccupiedTiles)
            {
                GameLocation.UpdateTilePassability((int)tile.X, (int)tile.Y);
            }

        }

    }
}
