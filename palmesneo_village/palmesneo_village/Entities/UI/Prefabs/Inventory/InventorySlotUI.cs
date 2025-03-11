using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class InventorySlotUI : ImageUI
    {

        private ImageUI iconImage;
        private TextUI quantityText;
        private ProgressBarUI contentProgress;

        public InventorySlotUI()
        {
            Texture = RenderManager.Pixel;
            Size = new Vector2(32, 32);

            iconImage = new ImageUI();
            iconImage.Size = new Vector2(24, 24);
            iconImage.Anchor = Anchor.Center;
            iconImage.IsVisible = false;
            AddChild(iconImage);

            quantityText = new TextUI();
            quantityText.Anchor = Anchor.TopLeft;
            quantityText.IsVisible = false;
            quantityText.SelfColor = Color.Black;
            AddChild(quantityText);

            contentProgress = new ProgressBarUI();
            contentProgress.Anchor = Anchor.BottomCenter;
            contentProgress.IsVisible = false;
            contentProgress.FrontColor = Color.LightBlue;
            contentProgress.BackColor = Color.Black;
            contentProgress.Size = new Vector2(32, 4);
            AddChild(contentProgress);
        }

        public void SetItem(Item item, int quantity, int contentAmount)
        {
            iconImage.IsVisible = true;
            quantityText.IsVisible = true;

            if (item is WateringCanItem wateringCanItem)
            {
                contentProgress.IsVisible = true;

                contentProgress.MinValue = 0;
                contentProgress.MaxValue = wateringCanItem.Capacity;
                contentProgress.CurrentValue = contentAmount;
            }
            else
            {
                contentProgress.IsVisible = false;
            }

            iconImage.Texture = item.Icon;
            quantityText.Text = $"{quantity}";
        }

        public void Clear()
        {
            iconImage.IsVisible = false;
            quantityText.IsVisible = false;
            contentProgress.IsVisible = false;
        }

    }
}
