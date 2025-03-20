using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace palmesneo_village
{
    public class CraftingRecipeButtonUI : ButtonUI
    {
        private ImageUI resultItemIcon;
        private TextUI resultItemAmount;

        public CraftingRecipe CraftingRecipe { get; private set; }

        public CraftingRecipeButtonUI()
        {
            Size = new Vector2(24, 24);
            StatesColors[ButtonUIState.Normal] = Color.White;
            StatesColors[ButtonUIState.Hovered] = Color.DarkGray;
            StatesColors[ButtonUIState.Disabled] = Color.White * 0.5f;
            StatesColors[ButtonUIState.Pressed] = Color.Gray;
            StatesColors[ButtonUIState.Toggled] = new Color((byte)255, (byte)170, (byte)40, (byte)255);

            resultItemIcon = new ImageUI();
            resultItemIcon.Size = new Vector2(16, 16);
            resultItemIcon.Anchor = Anchor.Center;
            resultItemIcon.Texture = RenderManager.Pixel;
            AddChild(resultItemIcon);

            resultItemAmount = new TextUI();
            resultItemAmount.Anchor = Anchor.BottomRight;
            resultItemAmount.Text = "0";
            AddChild(resultItemAmount);
        }

        public void SetCraftingRecipe(CraftingRecipe craftingRecipe, bool canCraft)
        {
            CraftingRecipe = craftingRecipe;

            resultItemIcon.IsVisible = true;
            resultItemIcon.Texture = craftingRecipe.Result.Item.Icon;
            resultItemIcon.SelfColor = canCraft ? Color.White : Color.White * 0.4f;

            resultItemAmount.IsVisible = craftingRecipe.Result.Amount > 1;
            resultItemAmount.Text = craftingRecipe.Result.Amount.ToString();
            resultItemAmount.SelfColor = canCraft ? Color.White : Color.White * 0.4f;

            Tooltip = craftingRecipe.ToString();
        }

        public void Clear()
        {
            CraftingRecipe = null;

            resultItemIcon.IsVisible = false;
            resultItemAmount.IsVisible = false;

            Tooltip = string.Empty;
        }

    }
}
