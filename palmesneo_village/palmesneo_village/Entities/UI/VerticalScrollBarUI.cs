using Microsoft.Xna.Framework;
using System;

namespace palmesneo_village
{
    public class VerticalScrollBarUI : ScrollBarUI
    {

        public override Vector2 Size 
        { 
            get => base.Size;
            set 
            { 
                base.Size = value;

                if (GrabberUI != null)
                {
                    GrabberUI.Size = new Vector2(base.Size.X, GrabberUI.Size.Y);
                }
            }
        }

        public override int CurrentValue 
        {
            get 
            {
                float scrollPath = Size.Y - GrabberUI.Size.Y;

                float progress = GrabberUI.LocalPosition.Y / scrollPath;

                return (int)Math.Round((MaxValue - MinValue) * progress + MinValue); 
            }
            set 
            {
                int clampedValue = Math.Clamp(value, MinValue, MaxValue);

                float progress = (float)(clampedValue - MinValue) / (MaxValue - MinValue);

                float scrollPath = Size.Y - GrabberUI.Size.Y;

                GrabberUI.LocalPosition = new Vector2(GrabberUI.LocalPosition.X, scrollPath * progress);

                GrabberPositionChanged?.Invoke(this);
            }
        }

        public Action<ScrollBarUI> GrabberPositionChanged { get; set; }

        private bool isGrabbed = false;

        private Vector2 lastMousePosition = Vector2.Zero;

        public VerticalScrollBarUI()
        {

        }

        public override void Update()
        {
            SelfColor = Color.Black;
            GrabberUI.SelfColor = Color.White;

            if (Contains(MInput.Mouse.UIPosition))
            {
                Engine.IsMouseOnUI = true;

                if (GrabberUI.Contains(MInput.Mouse.UIPosition))
                {
                    GrabberUI.SelfColor = Color.DarkGray;

                    if (MInput.Mouse.PressedLeftButton)
                    {
                        isGrabbed = true;

                        lastMousePosition = MInput.Mouse.UIScaledPosition;
                    }
                }
            }

            if(isGrabbed)
            {
                Engine.IsMouseOnUI = true;

                GrabberUI.SelfColor = Color.Gray;

                Vector2 mouseDelta = Vector2.Zero;

                if (ContainsY(MInput.Mouse.UIPosition))
                {
                    mouseDelta = MInput.Mouse.UIScaledPosition - lastMousePosition;
                }

                lastMousePosition = MInput.Mouse.UIScaledPosition;

                GrabberUI.LocalPosition = new Vector2(GrabberUI.LocalPosition.X, GrabberUI.LocalPosition.Y + mouseDelta.Y);

                GrabberUI.LocalPosition = Vector2.Clamp(GrabberUI.LocalPosition, Vector2.Zero, Size - GrabberUI.Size);

                GrabberPositionChanged?.Invoke(this);
            }

            if(MInput.Mouse.ReleasedLeftButton)
            {
                isGrabbed = false;
            }

            base.Update();
        }

    }
}
