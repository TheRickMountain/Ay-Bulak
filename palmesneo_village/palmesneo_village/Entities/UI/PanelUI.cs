using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class PanelUI : NineSlicedImageUI
    {
        private TextUI labelText;

        public PanelUI()
        {
            Texture = ResourcesManager.GetTexture("Sprites", "UI", "panel");
            SliceCenter = new Rectangle(4, 4, 4, 4);

            labelText = new TextUI();
            labelText.LocalPosition = new Vector2(8, 1);
            AddChild(labelText);
        }

        public void SetLabel(string label)
        {
            labelText.Text = label;
        }

    }
}
