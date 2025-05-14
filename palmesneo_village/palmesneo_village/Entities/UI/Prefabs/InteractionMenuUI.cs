using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Tweening;
using System;
using System.Collections.Generic;

namespace palmesneo_village
{
    public class InteractionMenuUI : EntityUI
    {
        public Action<InteractionData> InteractionSelected { get; set; }

        private List<MiddleButtonUI> buttons;

        private Tweener tweener;

        private float startRadius = 30f;
        private float radiusIncreasingStep = 5f;
        private int increaseRadiusAfter = 5;

        public InteractionMenuUI()
        {
            Size = new Vector2(0, 0);

            buttons = new List<MiddleButtonUI>();

            tweener = new Tweener();

            CreateButtons();
        }

        private void CreateButtons()
        {
            for (int i = 0; i < 10; i++)
            {
                MiddleButtonUI button = new MiddleButtonUI();
                button.ActionTriggered += OnButtonPressed;
                button.MouseEntered += OnButtonMouseEntered;
                button.MouseExited += OnButtonMouseExited;

                ImageUI buttonIcon = new ImageUI();
                buttonIcon.Size = new Vector2(16, 16);
                buttonIcon.Anchor = Anchor.Center;
                buttonIcon.Name = "Icon";
                button.AddChild(buttonIcon);

                buttons.Add(button);
            }
        }

        public void Open(List<InteractionData> interactionsList)
        {
            RemoveButtons();

            for (int i = 0; i < interactionsList.Count; i++)
            {
                InteractionData interaction = interactionsList[i];
                MiddleButtonUI button = buttons[i];
                button.Origin = button.Size / 2;
                button.SetMetadata("interaction_data", interaction);
                button.GetChildByName<ImageUI>("Icon").Texture = interaction.Icon;
                button.Tooltip = interaction.Information;

                if (interaction.IsActive)
                {
                    button.IsDisabled = false;
                    button.GetChildByName<ImageUI>("Icon").SelfColor = Color.White;
                }
                else
                {
                    button.IsDisabled = true;
                    button.GetChildByName<ImageUI>("Icon").SelfColor = Color.White * 0.5f;
                }

                AddChild(button);
            }

            RearrangeButtons(interactionsList);
        }

        public override void Update()
        {
            base.Update();

            tweener.Update(Engine.DeltaTime);
        }

        private void RemoveButtons()
        {
            foreach (MiddleButtonUI button in GetChildren())
            {
                RemoveChild(button);
            }
        }

        private void RearrangeButtons(List<InteractionData> interactionsList)
        {
            float radius = CalculateRadius(interactionsList.Count);

            float radiansOfSeparation = MathHelper.TwoPi / interactionsList.Count;

            for (int i = 0; i < interactionsList.Count; i++)
            {
                MiddleButtonUI button = buttons[i];
                button.SetMetadata("is_animating", true);

                button.LocalScale = Vector2.Zero;

                Vector2 buttonPositionOffset = -(button.Size / 2);

                button.LocalPosition = buttonPositionOffset;

                float angle = radiansOfSeparation * i;
                Vector2 targetButtonPosition = (new Vector2(-MathF.Sin(angle), -MathF.Cos(angle)) * radius) + buttonPositionOffset;

                float delayTime = 0.05f * i;

                tweener.TweenTo(
                        target: button,
                        expression: x => button.LocalScale,
                        toValue: new Vector2(1.0f, 1.0f),
                        duration: 0.2f,
                        delay: delayTime)
                        .Easing(EasingFunctions.CubicInOut)
                        .OnEnd(tween => { button.SetMetadata("is_animating", false); });

                tweener.TweenTo(
                        target: button,
                        expression: x => button.LocalPosition,
                        toValue: targetButtonPosition,
                        duration: 0.2f,
                        delay: delayTime)
                        .Easing(EasingFunctions.CubicInOut);
            }
        }

        private float CalculateRadius(int elementsAmount)
        {
            float radius = startRadius;

            if (elementsAmount >= increaseRadiusAfter)
            {
                radius += radiusIncreasingStep * (elementsAmount - increaseRadiusAfter);
            }

            return radius;
        }

        private void OnButtonPressed(ButtonUI button)
        {
            InteractionData interactionData = button.GetMetadata<InteractionData>("interaction_data");

            InteractionSelected?.Invoke(interactionData);
        }

        private void OnButtonMouseEntered(EntityUI button)
        {
            if (button.GetMetadata<bool>("is_animating")) return;

            tweener.TweenTo(
                        target: button,
                        expression: x => button.LocalScale,
                        toValue: new Vector2(1.3f, 1.3f),
                        duration: 0.1f)
                        .Easing(EasingFunctions.CubicInOut);
        }

        private void OnButtonMouseExited(EntityUI button)
        {
            if (button.GetMetadata<bool>("is_animating")) return;

            tweener.TweenTo(
                        target: button,
                        expression: x => button.LocalScale,
                        toValue: new Vector2(1.0f, 1.0f),
                        duration: 0.1f)
                        .Easing(EasingFunctions.CubicInOut);
        }

    }
}
