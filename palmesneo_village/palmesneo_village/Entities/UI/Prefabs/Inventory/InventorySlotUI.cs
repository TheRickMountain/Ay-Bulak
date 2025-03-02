using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class InventorySlotUI : ImageUI
    {

        private ImageUI iconImage;
        private TextUI quantityText;

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
        }

        public void SetItem(Item item, int quantity)
        {
            iconImage.IsVisible = true;
            quantityText.IsVisible = true;

            iconImage.Texture = item.Icon;
            quantityText.Text = $"{quantity}";
        }

        public void Clear()
        {
            iconImage.IsVisible = false;
            quantityText.IsVisible = false;
        }

    }
}
