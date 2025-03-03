using Microsoft.Xna.Framework;
using palmesneo_village;

namespace palmesneo_village
{
    public class PlayerMoneyUI : HorizontalContainerUI
    {

        private ImageUI coinIcon;

        private TextUI moneyText;

        public PlayerMoneyUI(PlayerMoneyManager moneyAmountManager)
        {
            coinIcon = new ImageUI();
            coinIcon.Texture = ResourcesManager.GetTexture("Sprites","coin_icon");
            coinIcon.Size = new Vector2(24, 24);
            AddChild(coinIcon);

            moneyText = new TextUI();
            moneyText.Text = $"{moneyAmountManager.MoneyAmount}";
            AddChild(moneyText);

            moneyAmountManager.MoneyAmountChanged += (mouneyAmount) =>
            {
                moneyText.Text = $"{mouneyAmount}";
            };
        }

    }
}
