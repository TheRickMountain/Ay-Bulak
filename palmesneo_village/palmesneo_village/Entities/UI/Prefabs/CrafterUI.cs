using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Tweening;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace palmesneo_village
{
    public class CrafterUI : PanelUI
    {
        private VerticalScrollBarUI scrollBarUI;

        private Inventory inventory;
        private List<CraftingRecipe> craftingRecipesList = new();

        private CraftingRecipeButtonUI[,] craftingRecipeButtonsArray;
        private GridContainerUI gridContainer;

        private const int COLUMNS = 10;
        private const int ROWS = 5;

        private Tweener tweener = new Tweener();

        public CrafterUI(Inventory inventory)
        {
            this.inventory = inventory;
            inventory.SlotDataChanged += (inventory, slotIndex) => UpdateCraftingRecipes();

            scrollBarUI = new VerticalScrollBarUI();
            scrollBarUI.GrabberPositionChanged += OnScrollBarGrabberPositionChanged;
            scrollBarUI.Anchor = Anchor.RightCenter;
            scrollBarUI.LocalPosition = new Vector2(-8, 0);
            AddChild(scrollBarUI);

            gridContainer = new GridContainerUI();
            gridContainer.Anchor = Anchor.LeftCenter;
            gridContainer.LocalPosition = new Vector2(8, 0);
            gridContainer.Columns = COLUMNS;
            AddChild(gridContainer);

            craftingRecipeButtonsArray = new CraftingRecipeButtonUI[COLUMNS, ROWS];

            for (int y = 0; y < ROWS; y++)
            {
                for (int x = 0; x < COLUMNS; x++)
                {
                    CraftingRecipeButtonUI craftingRecipeButton = new CraftingRecipeButtonUI();
                    craftingRecipeButton.Origin = craftingRecipeButton.Size / 2;
                    craftingRecipeButton.ActionTriggered += OnCraftingRecipeButtonPressed;
                    craftingRecipeButton.MouseEntered += OnCraftingRecipeButtonMouseEntered;
                    craftingRecipeButton.MouseExited += OnCraftingRecipeButtonMouseExited;
                    gridContainer.AddChild(craftingRecipeButton);

                    craftingRecipeButtonsArray[x, y] = craftingRecipeButton;
                }
            }

            scrollBarUI.Size = new Vector2(15, gridContainer.Size.Y);

            Size = gridContainer.Size + new Vector2(5 + 16 + scrollBarUI.Size.X, 16);
        }

        public void Open(IEnumerable<CraftingRecipe> craftingRecipes)
        {
            craftingRecipesList.Clear();

            craftingRecipesList.AddRange(craftingRecipes);

            int scrollBarMaxValue = (int)Math.Ceiling((float)craftingRecipesList.Count / (float)COLUMNS) - COLUMNS;

            if(scrollBarMaxValue < 0)
            {
                scrollBarUI.MaxValue = 0;
            }

            UpdateCraftingRecipes();
        }

        public override void Update()
        {
            tweener.Update(Engine.DeltaTime);

            if (Contains(MInput.Mouse.UIPosition))
            {
                int wheelDelta = 0;

                if (MInput.Mouse.WheelDelta > 0)
                {
                    wheelDelta = -1;
                }
                else if (MInput.Mouse.WheelDelta < 0)
                {
                    wheelDelta = 1;
                }

                if (wheelDelta != 0)
                {
                    scrollBarUI.CurrentValue += wheelDelta;

                    UpdateCraftingRecipes();
                }
            }

            base.Update();
        }

        private void UpdateCraftingRecipes()
        {
            int firstRow = scrollBarUI.CurrentValue;
            int lastRow = scrollBarUI.CurrentValue + ROWS - 1;

            int i = 0;

            for (int currentRow = firstRow; currentRow <= lastRow; currentRow++)
            {
                for (int currentColumn = 0; currentColumn < COLUMNS; currentColumn++)
                {
                    int craftingRecipeIndex = currentRow * COLUMNS + currentColumn;

                    if (craftingRecipesList.Count <= craftingRecipeIndex)
                    {
                        craftingRecipeButtonsArray[currentColumn, i].Clear();
                    }
                    else
                    {
                        CraftingRecipe craftingRecipe = craftingRecipesList[craftingRecipeIndex];

                        string cantCraftReason;

                        bool canCraft = CanCraft(craftingRecipe, out cantCraftReason);

                        craftingRecipeButtonsArray[currentColumn, i].SetCraftingRecipe(craftingRecipe, canCraft, inventory);
                        craftingRecipeButtonsArray[currentColumn, i].Tooltip += $"\n/c[{ColorUtils.RED_HEX}]{cantCraftReason}/cd";
                    }
                }

                i++;
            }
        }

        private void OnCraftingRecipeButtonMouseEntered(EntityUI buttonUI)
        {
            CraftingRecipeButtonUI craftingRecipeButtonUI = (CraftingRecipeButtonUI)buttonUI;

            CraftingRecipe craftingRecipe = craftingRecipeButtonUI.CraftingRecipe;

            if (craftingRecipe == null) return;

            ResourcesManager.GetSoundEffect("SoundEffects", "UI Soundpack", "Abstract2").Play();

            tweener.TweenTo(
                        target: craftingRecipeButtonUI,
                        expression: x => craftingRecipeButtonUI.LocalScale,
                        toValue: new Vector2(1.3f, 1.3f),
                        duration: 0.2f)
                        .Easing(EasingFunctions.CubicInOut);
        }

        private void OnCraftingRecipeButtonMouseExited(EntityUI buttonUI)
        {
            CraftingRecipeButtonUI craftingRecipeButtonUI = (CraftingRecipeButtonUI)buttonUI;

            CraftingRecipe craftingRecipe = craftingRecipeButtonUI.CraftingRecipe;

            if (craftingRecipe == null) return;

            tweener.TweenTo(
                        target: craftingRecipeButtonUI,
                        expression: x => craftingRecipeButtonUI.LocalScale,
                        toValue: new Vector2(1.0f, 1.0f),
                        duration: 0.2f)
                        .Easing(EasingFunctions.CubicInOut);
        }

        private void OnCraftingRecipeButtonPressed(ButtonUI buttonUI)
        {
            CraftingRecipeButtonUI craftingRecipeButtonUI = (CraftingRecipeButtonUI)buttonUI;

            CraftingRecipe craftingRecipe = craftingRecipeButtonUI.CraftingRecipe;

            if (craftingRecipe == null) return;

            Item resultItem = craftingRecipe.Result.Item;
            int resultAmount = craftingRecipe.Result.Amount;

            if (CanCraft(craftingRecipe, out _) == false)
            {
                return;
            }

            if(inventory.CanAddItem(resultItem, resultAmount) == false)
            {
                return;
            }

            inventory.TryAddItem(resultItem, resultAmount, 0);

            foreach (Ingredient ingredient in craftingRecipe.RequiredIngredients)
            {
                inventory.RemoveItem(ingredient.Item, ingredient.Amount);
            }

            UpdateCraftingRecipes();

            tweener.TweenTo(
                        target: craftingRecipeButtonUI,
                        expression: x => craftingRecipeButtonUI.LocalScale,
                        toValue: new Vector2(0.7f, 0.7f),
                        duration: 0.1f)
                        .Easing(EasingFunctions.CubicInOut)
                        .OnEnd(tween =>
                        {
                            tweener.TweenTo(
                                target: craftingRecipeButtonUI,
                                expression: x => craftingRecipeButtonUI.LocalScale,
                                toValue: new Vector2(1.3f, 1.3f),
                                duration: 0.1f)
                                .Easing(EasingFunctions.BounceOut);
                        });

            PlayItemPickupSoundEffect();
        }

        private void PlayItemPickupSoundEffect()
        {
            Calc.Random.Choose(
                ResourcesManager.GetSoundEffect("SoundEffects", "pop_0"),
                ResourcesManager.GetSoundEffect("SoundEffects", "pop_1"),
                ResourcesManager.GetSoundEffect("SoundEffects", "pop_2"),
                ResourcesManager.GetSoundEffect("SoundEffects", "pop_3"),
                ResourcesManager.GetSoundEffect("SoundEffects", "pop_4"),
                ResourcesManager.GetSoundEffect("SoundEffects", "pop_5")
                ).Play();
        }

        private void OnScrollBarGrabberPositionChanged(ScrollBarUI scrollBarUI)
        {
            UpdateCraftingRecipes();
        }

        private bool CanCraft(CraftingRecipe craftingRecipe, out string reasonMessage)
        {
            if(inventory.CanAddItem(craftingRecipe.Result.Item, craftingRecipe.Result.Amount) == false)
            {
                reasonMessage = LocalizationManager.GetText("not_enough_space_in_inventory");
                return false;
            }

            foreach (Ingredient ingredient in craftingRecipe.RequiredIngredients)
            {
                if (inventory.GetTotalItemQuantity(ingredient.Item) < ingredient.Amount)
                {
                    reasonMessage = string.Empty;
                    return false;
                }
            }

            reasonMessage = string.Empty;

            return true;
        }
    }
}
