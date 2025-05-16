using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public enum PanelStyle
    {
        Default,
        Dialog
    }

    public class PanelUI : NineSlicedImageUI
    {
        private TextUI labelText;

        public PanelUI(PanelStyle panelStyle = PanelStyle.Default)
        {
            switch(panelStyle)
            {
                case PanelStyle.Default:
                    Texture = ResourcesManager.GetTexture("Sprites", "UI", "default_panel");
                    break;
                case PanelStyle.Dialog:
                    Texture = ResourcesManager.GetTexture("Sprites", "UI", "dialog_panel");
                    break;
            }
            
         
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
