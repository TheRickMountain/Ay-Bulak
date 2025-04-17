using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class TextButtonUI : ButtonUI
    {

        private MTexture texture;

        public TextButtonUI()
        {
            texture = ResourcesManager.GetTexture("Sprites", "UI", "text_button");
            Size = texture.Size;
            StatesColors[ButtonUIState.Normal] = Color.White;
            StatesColors[ButtonUIState.Hovered] = Color.DarkGray;
            StatesColors[ButtonUIState.Disabled] = Color.White * 0.5f;
            StatesColors[ButtonUIState.Pressed] = Color.Gray;
            StatesColors[ButtonUIState.Toggled] = new Color((byte)255, (byte)170, (byte)40, (byte)255);
        }

        public override void Render()
        {
            texture?.Draw(GlobalPosition, Origin / (Size / texture.Size), GlobalRotation, GlobalScale * (Size / texture.Size), SelfColor, SpriteEffects.None);

            base.Render();
        }

    }
}
