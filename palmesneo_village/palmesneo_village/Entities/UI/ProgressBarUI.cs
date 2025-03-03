using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace palmesneo_village
{
    public class ProgressBarUI : EntityUI
    {
        private float minValue = 0.0f;
        private float maxValue = 0.0f;
        private float currentValue = 0.0f;

        public float MinValue 
        {
            get => minValue;
            set
            {
                minValue = value;

                currentValue = Math.Clamp(currentValue, MinValue, MaxValue);
            }
        }
        
        public float MaxValue
        {
            get => maxValue;
            set
            {
                maxValue = value;

                currentValue = Math.Clamp(currentValue, MinValue, MaxValue);
            }
        }
        
        public float CurrentValue 
        {
            get => currentValue; 
            set
            {
                currentValue = Math.Clamp(value, MinValue, MaxValue);
            }
        }

        public float Progress
        {
            get => (CurrentValue - MinValue) / (MaxValue - MinValue);
        }

        public MTexture BackTexture { get; set; }
        public MTexture FrontTexture { get; set; }

        public Color BackColor { get; set; } = Color.Red;
        public Color FrontColor { get; set; } = Color.Blue;

        public ProgressBarUI()
        {
            BackTexture = RenderManager.Pixel;
            FrontTexture = RenderManager.Pixel;
        }

        public override void Render()
        {
            BackTexture?.Draw(GlobalPosition, Origin / (Size / BackTexture.Size), GlobalRotation, GlobalScale * (Size / BackTexture.Size), BackColor, SpriteEffects.None);

            Vector2 temp = new Vector2(Size.X * Progress, Size.Y) / FrontTexture.Size;

            FrontTexture?.Draw(GlobalPosition, Origin / temp, GlobalRotation, GlobalScale * temp, FrontColor, SpriteEffects.None);

            base.Render();
        }

    }
}
