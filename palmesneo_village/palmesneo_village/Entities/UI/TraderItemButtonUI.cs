using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace palmesneo_village
{
    public class TraderItemButtonUI : ButtonUI
    {

        private MTexture texture;

        private ImageUI itemIcon;

        private TextUI itemName;

        private ImageUI coinIcon;

        private TextUI itemPrice;

        public TraderItemButtonUI(MTexture texture)
        {
            this.texture = texture;
            base.Size = this.texture.Size;
            StatesColors[ButtonUIState.Normal] = Color.White;
            StatesColors[ButtonUIState.Hovered] = Color.DarkGray;
            StatesColors[ButtonUIState.Disabled] = Color.White * 0.5f;
            StatesColors[ButtonUIState.Pressed] = Color.Gray;
            StatesColors[ButtonUIState.Toggled] = new Color((byte)255, (byte)170, (byte)40, (byte)255);

            itemIcon = new ImageUI();
            itemIcon.Size = new Vector2(16, 16);
            itemIcon.Anchor = Anchor.LeftCenter;
            itemIcon.LocalPosition = new Vector2(8, 0);
            itemIcon.Texture = RenderManager.Pixel;
            AddChild(itemIcon);

            itemName = new TextUI();
            itemName.Anchor = Anchor.Center;
            itemName.Text = "Item Name";
            AddChild(itemName);

            coinIcon = new ImageUI();
            coinIcon.Anchor = Anchor.RightCenter;
            coinIcon.Texture = ResourcesManager.GetTexture("Sprites", "UI", "small_coin_icon");
            coinIcon.Size = coinIcon.Texture.Size;
            coinIcon.LocalPosition = new Vector2(-8, 0);
            AddChild(coinIcon);

            itemPrice = new TextUI();
            itemPrice.Anchor = Anchor.RightCenter;
            itemPrice.Text = "10000";
            itemPrice.LocalPosition = new Vector2(-(8 + coinIcon.Size.X + 5), 0);
            AddChild(itemPrice);
        }

        public override void Render()
        {
            texture?.Draw(GlobalPosition, Origin / (Size / texture.Size), GlobalRotation, GlobalScale * (Size / texture.Size), SelfColor, SpriteEffects.None);

            base.Render();
        }

        public void SetItem(Item item)
        {
            itemIcon.Texture = item.Icon;
            itemName.Text = LocalizationManager.GetText(item.Name);
            itemPrice.Text = item.Price.ToString();
        }
    }
}
