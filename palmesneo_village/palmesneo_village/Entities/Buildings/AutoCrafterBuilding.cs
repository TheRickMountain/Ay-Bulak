using Microsoft.Xna.Framework;
using MonoGame.Extended.Tweening;
using System;
using System.Collections.Generic;

namespace palmesneo_village
{
    public enum AutoCrafterState
    {
        Idle,
        Crafting,
        Finished
    }

    public class AutoCrafterBuilding : CrafterBuilding
    {
        private AutoCrafterState crafterState = AutoCrafterState.Idle;

        private CraftingRecipe currentCraftingRecipe;

        private float craftingTimer = 0.0f;

        private Tweener tweener;

        private ImageEntity messageBubble;
        private ImageEntity messageBubbleIcon;

        public AutoCrafterBuilding(GameLocation gameLocation, AutoCrafterItem item, Direction direction, Vector2[,] occupiedTiles)
            : base(gameLocation, item, direction, occupiedTiles)
        {
            tweener = new Tweener();

            Sprite.Centered = true;
            Sprite.LocalPosition = new Vector2(Engine.TILE_SIZE / 2, Engine.TILE_SIZE / 2);

            messageBubble = new ImageEntity();
            messageBubble.Texture = ResourcesManager.GetTexture("Sprites", "UI", "message_bubble");
            messageBubble.IsVisible = false;
            messageBubble.SelfColor = Color.White * 0.75f;
            messageBubble.Centered = true;
            messageBubble.LocalPosition = new Vector2(0, -Sprite.Texture.Size.Y);
            Sprite.AddChild(messageBubble);

            messageBubbleIcon = new ImageEntity();
            messageBubbleIcon.Texture = RenderManager.Pixel;
            messageBubbleIcon.SelfColor = Color.White * 0.75f;
            messageBubbleIcon.Centered = true;
            messageBubbleIcon.LocalPosition = new Vector2(0, -2);
            messageBubble.AddChild(messageBubbleIcon);            
        }

        public override void Update()
        {
            base.Update();

            switch(crafterState)
            {
                case AutoCrafterState.Crafting:
                    {
                        tweener.Update(Engine.GameDeltaTime);

                        craftingTimer -= Engine.GameDeltaTime;

                        if (craftingTimer <= 0)
                        {
                            craftingTimer = 0;

                            StopAnimation();

                            messageBubble.IsVisible = true;
                            messageBubbleIcon.Texture = currentCraftingRecipe.Result.Item.Icon;

                            crafterState = AutoCrafterState.Finished;
                        }
                    }
                    break;
            }
        }

        public override void InteractAlternatively(Item item, PlayerEnergyManager playerEnergyManager)
        {
        }

        public void Interact(InteractionData interactionData, Inventory inventory)
        {
            switch(interactionData.InteractionType)
            {
                case InteractionType.Craft:
                    {
                        currentCraftingRecipe = ((CrafterInteractionData)interactionData).CraftingRecipe;

                        craftingTimer = currentCraftingRecipe.CraftingTimeInHours * TimeOfDayManager.MINUTES_PER_HOUR;

                        RemoveIngredientsFromInventory(currentCraftingRecipe.RequiredIngredients, inventory);

                        StartAnimation();

                        crafterState = AutoCrafterState.Crafting;
                    }
                    break;
                case InteractionType.Gather:
                    {
                        messageBubble.IsVisible = false;

                        SpawnIngredient(currentCraftingRecipe.Result);

                        crafterState = AutoCrafterState.Idle;
                    }
                    break;
                case InteractionType.Cancel:
                    {
                        StopAnimation();

                        SpawnIngredients(currentCraftingRecipe.RequiredIngredients);

                        crafterState = AutoCrafterState.Idle;
                    }
                    break;
            }
        }

        public IEnumerable<InteractionData> GetAvailableInteractions(Inventory inventory)
        {
            switch(crafterState)
            {
                case AutoCrafterState.Idle:
                    {
                        foreach(var craftingRecipe in CraftingRecipes)
                        {
                            string interactionDataInformation = TooltipBuilder.CreateTooltipText(craftingRecipe, inventory);
                            bool canCraft = CanCraft(craftingRecipe, inventory);
                            yield return new CrafterInteractionData(craftingRecipe, interactionDataInformation, canCraft);
                        }
                    }
                    break;
                case AutoCrafterState.Finished:
                    {
                        yield return new InteractionData(
                            InteractionType.Gather, 
                            ResourcesManager.GetTexture("Sprites", "gather_icon"), 
                            LocalizationManager.GetText("gather"),
                            true);
                    }
                    break;
                case AutoCrafterState.Crafting:
                    {
                        yield return new InteractionData(
                            InteractionType.Cancel,
                            ResourcesManager.GetTexture("Sprites", "cancel_icon"), 
                            LocalizationManager.GetText("cancel"),
                            true);
                    }
                    break;
            }
        }

        private void SpawnIngredients(IReadOnlyList<Ingredient> ingredients)
        {
            foreach(Ingredient ingredient in ingredients)
            {
                SpawnIngredient(ingredient);
            }
        }
    
        private void SpawnIngredient(Ingredient ingredient)
        {
            ItemContainer itemContainer = new ItemContainer();
            itemContainer.Item = Engine.ItemsDatabase.GetItemByName(ingredient.Item.Name);
            itemContainer.Quantity = ingredient.Amount;
            GameLocation.AddItem(OccupiedTiles[0, 0] * Engine.TILE_SIZE, itemContainer);
        }

        private void RemoveIngredientsFromInventory(IReadOnlyList<Ingredient> ingredients, Inventory inventory)
        {
            foreach (Ingredient ingredient in ingredients)
            {
                inventory.RemoveItem(ingredient.Item, ingredient.Amount);
            }
        }

        private void StartAnimation()
        {
            Sprite.LocalScale = Vector2.One;

            tweener.TweenTo(
                    target: Sprite,
                    expression: trunk => Sprite.LocalScale,
                    toValue: new Vector2(1.2f, 0.8f),
                    duration: 0.75f)
                .AutoReverse()
                .RepeatForever()
                .Easing(EasingFunctions.Linear);
        }

        private void StopAnimation()
        {
            tweener.CancelAndCompleteAll();
            Sprite.LocalScale = Vector2.One;
        }
    
        private bool CanCraft(CraftingRecipe craftingRecipe, Inventory inventory)
        {
            foreach (Ingredient ingredient in craftingRecipe.RequiredIngredients)
            {
                int amountOfIngredientsInInventory = inventory.GetTotalItemQuantity(ingredient.Item);
                int requiredAmountOfIngredients = ingredient.Amount;
                if (amountOfIngredientsInInventory < requiredAmountOfIngredients)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
