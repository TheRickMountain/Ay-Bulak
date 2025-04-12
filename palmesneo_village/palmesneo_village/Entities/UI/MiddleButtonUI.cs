using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class MiddleButtonUI : ButtonUI
    {

        public MTexture Texture { get; set; }

        public MiddleButtonUI()
        {
            Texture = ResourcesManager.GetTexture("Sprites", "UI", "middle_button");
            Size = new Vector2(24, 24);
            StatesColors[ButtonUIState.Normal] = Color.White;
            StatesColors[ButtonUIState.Hovered] = Color.DarkGray;
            StatesColors[ButtonUIState.Disabled] = Color.White * 0.5f;
            StatesColors[ButtonUIState.Pressed] = Color.Gray;
            StatesColors[ButtonUIState.Toggled] = new Color((byte)255, (byte)170, (byte)40, (byte)255);
        }

        public override void Render()
        {
            Texture?.Draw(GlobalPosition, Origin / (Size / Texture.Size), GlobalRotation, GlobalScale * (Size / Texture.Size), SelfColor, SpriteEffects.None);

            base.Render();
        }

    }
}
