using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class SlotButtonUI : ButtonUI
    {
        private ImageUI iconImage;
        private TextUI quantityText;
        private ProgressBarUI contentProgress;

        private MTexture texture;

        private const int ICON_SIZE = 16;

        public SlotButtonUI()
        {
            texture = ResourcesManager.GetTexture("Sprites", "UI", "slot");
            Size = new Vector2(24, 24);
            StatesColors[ButtonUIState.Normal] = Color.White;
            StatesColors[ButtonUIState.Hovered] = Color.DarkGray;
            StatesColors[ButtonUIState.Disabled] = Color.White * 0.5f;
            StatesColors[ButtonUIState.Pressed] = Color.Gray;
            StatesColors[ButtonUIState.Toggled] = new Color((byte)255, (byte)170, (byte)40, (byte)255);

            iconImage = new ImageUI();
            iconImage.Name = "Icon";
            iconImage.Anchor = Anchor.Center;
            iconImage.IsVisible = false;
            AddChild(iconImage);

            quantityText = new TextUI();
            quantityText.Name = "Quantity";
            quantityText.Anchor = Anchor.TopLeft;
            quantityText.IsVisible = false;
            quantityText.SelfColor = Color.White;
            AddChild(quantityText);

            contentProgress = new ProgressBarUI();
            contentProgress.Anchor = Anchor.BottomCenter;
            contentProgress.IsVisible = false;
            contentProgress.FrontColor = Color.LightBlue;
            contentProgress.BackColor = Color.Black;
            contentProgress.Size = new Vector2(24, 3);
            AddChild(contentProgress);
        }

        public override void Render()
        {
            texture?.Draw(GlobalPosition, Origin / (Size / texture.Size), GlobalRotation, GlobalScale * (Size / texture.Size), SelfColor, SpriteEffects.None);

            base.Render();
        }

        public void SetItem(Item item, int quantity, int contentAmount)
        {
            iconImage.IsVisible = true;
            quantityText.IsVisible = true;

            iconImage.Texture = item.Icon;
            quantityText.Text = $"{quantity}";

            // Update icom image scale
            if (item.Icon.Width > item.Icon.Height)
            {
                iconImage.Size = new Vector2(ICON_SIZE, ICON_SIZE * (item.Icon.Height / (float)item.Icon.Width));
            }
            else
            {
                iconImage.Size = new Vector2(ICON_SIZE * (item.Icon.Width / (float)item.Icon.Height), ICON_SIZE);
            }

            if (item is ToolItem toolItem && toolItem.Capacity > 0)
            {
                contentProgress.IsVisible = true;

                contentProgress.MinValue = 0;
                contentProgress.MaxValue = toolItem.Capacity;
                contentProgress.CurrentValue = contentAmount;
            }
            else
            {
                contentProgress.IsVisible = false;
            }

            Tooltip = item.GetTooltipInfo();
        }

        public void Clear()
        {
            iconImage.IsVisible = false;
            quantityText.IsVisible = false;
            contentProgress.IsVisible = false;

            Tooltip = string.Empty;
        }
    }
}
