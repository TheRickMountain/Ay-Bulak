using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public abstract class ScrollBarUI : ImageUI
    {

        private int minValue = 0;

        public int MinValue
        {
            get { return minValue; }
            set 
            { 
                if(minValue != value)
                {
                    minValue = value;

                    if(CurrentValue < minValue)
                    {
                        CurrentValue = minValue;
                    }
                }
            }
        }

        private int maxValue = 100;

        public int MaxValue
        {
            get { return maxValue; }
            set 
            { 
                if(maxValue != value)
                {
                    maxValue = value;
                    
                    if(CurrentValue > maxValue)
                    {
                        CurrentValue = maxValue;
                    }
                }
            }
        }

        public virtual int CurrentValue { get; set; }

        protected ImageUI GrabberUI { get; private set; }

        public ScrollBarUI()
        {
            Texture = RenderManager.Pixel;
            SelfColor = Color.Blue;

            GrabberUI = new ImageUI();
            GrabberUI.Texture = RenderManager.Pixel;
            GrabberUI.SelfColor = Color.Red;
            AddChild(GrabberUI);
        }

        public float GetProgressAsPercent()
        {
            return (float)(CurrentValue - MinValue) / (MaxValue - MinValue);
        }

    }
}
