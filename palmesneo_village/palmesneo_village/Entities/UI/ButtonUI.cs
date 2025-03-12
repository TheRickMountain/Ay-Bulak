using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace palmesneo_village
{
    public enum ButtonUIState
    {
        Normal,
        Hovered,
        Pressed,
        Disabled,
        Toggled
    }

    public enum ButtonUIActionMode
    {
        ButtonPressed,
        ButtonReleased
    }

    public class ButtonUI : EntityUI
    {
        public Dictionary<ButtonUIState, Color> StatesColors { get; private set; } = new Dictionary<ButtonUIState, Color>
        {
            {ButtonUIState.Normal, Color.White},
            {ButtonUIState.Hovered, Color.White},
            {ButtonUIState.Pressed, Color.White},
            {ButtonUIState.Disabled, Color.White},
            {ButtonUIState.Toggled, Color.White}
        };

        public bool IsDisabled { get; set; } = false;

        public bool IsToggleMode { get; set; } = false;

        public ButtonUIActionMode ActionMode { get; set; } = ButtonUIActionMode.ButtonPressed;

        public Action<ButtonUI> ActionTriggered { get; set; }

        public virtual bool IsToggled { get; set; } = false;

        private bool isHovered = false;
        private bool isPressed = false;

        public override void Update()
        {
            base.Update();

            isHovered = false;

            if (IsDisabled == false && Contains(MInput.Mouse.UIPosition))
            {
                isHovered = true;

                isPressed = false;

                switch (ActionMode)
                {
                    case ButtonUIActionMode.ButtonPressed:
                        {
                            if (MInput.Mouse.PressedLeftButton)
                            {
                                CallAction();
                            }
                        }
                        break;
                    case ButtonUIActionMode.ButtonReleased:
                        {
                            if (MInput.Mouse.ReleasedLeftButton)
                            {
                                CallAction();
                            }
                        }
                        break;
                }

                if (MInput.Mouse.CheckLeftButton)
                {
                    isPressed = true;
                }
            }

            UpdateColor();
        }

        private void UpdateColor()
        {
            if (IsDisabled)
            {
                SelfColor = StatesColors[ButtonUIState.Disabled];
            }
            else
            {
                if (IsToggled)
                {
                    SelfColor = StatesColors[ButtonUIState.Toggled];
                }
                else if(isPressed)
                {
                    SelfColor = StatesColors[ButtonUIState.Pressed];
                }
                else
                {
                    if (isHovered)
                    {
                        SelfColor = StatesColors[ButtonUIState.Hovered];
                    }
                    else
                    {
                        SelfColor = StatesColors[ButtonUIState.Normal];
                    }
                }
            }
        }

        private void CallAction()
        {
            if (IsToggleMode)
            {
                IsToggled = !IsToggled;

                Console.WriteLine($"Toggled: {IsToggled}");
            }

            ActionTriggered?.Invoke(this);
        }

    }
}
