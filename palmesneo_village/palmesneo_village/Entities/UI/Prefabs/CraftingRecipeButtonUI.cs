using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace palmesneo_village
{
    public class CraftingRecipeButtonUI : ButtonUI
    {
        public CraftingRecipe CraftingRecipe { get; private set; }

        private ImageUI resultItemIcon;
        private TextUI resultItemAmount;

        private const int ICON_SIZE = 16;

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

        public void SetCraftingRecipe(CraftingRecipe craftingRecipe, bool canCraft, Inventory inventory)
        {
            CraftingRecipe = craftingRecipe;

            Item item = craftingRecipe.Result.Item;

            resultItemIcon.IsVisible = true;
            resultItemIcon.Texture = item.Icon;
            resultItemIcon.SelfColor = canCraft ? Color.White : Color.White * 0.4f;

            // Update icom image scale
            if (item.Icon.Width > item.Icon.Height)
            {
                resultItemIcon.Size = new Vector2(ICON_SIZE, ICON_SIZE * (item.Icon.Height / (float)item.Icon.Width));
            }
            else
            {
                resultItemIcon.Size = new Vector2(ICON_SIZE * (item.Icon.Width / (float)item.Icon.Height), ICON_SIZE);
            }

            resultItemAmount.IsVisible = craftingRecipe.Result.Amount > 1;
            resultItemAmount.Text = craftingRecipe.Result.Amount.ToString();
            resultItemAmount.SelfColor = canCraft ? Color.White : Color.White * 0.4f;

            Tooltip = CreateTooltipText(craftingRecipe, inventory);
        }

        private string CreateTooltipText(CraftingRecipe craftingRecipe, Inventory inventory)
        {
            string tooltipText = craftingRecipe.Result.Item.GetTooltipInfo();

            foreach (Ingredient ingredient in craftingRecipe.GetRequiredIngredients())
            {
                int amountOfIngredientsInInventory = inventory.GetItemQuantity(ingredient.Item);
                int requiredAmountOfIngredients = ingredient.Amount;

                if (amountOfIngredientsInInventory < requiredAmountOfIngredients)
                {
                    tooltipText += $"\n{LocalizationManager.GetText(ingredient.Item.Name)}   " +
                        $"/c[{ColorUtils.RED_HEX}]{amountOfIngredientsInInventory}/cd // {requiredAmountOfIngredients}";
                }
                else
                {
                    tooltipText += $"\n{LocalizationManager.GetText(ingredient.Item.Name)}   " +
                        $"{amountOfIngredientsInInventory} // {requiredAmountOfIngredients}";
                }
            }

            return tooltipText;
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
