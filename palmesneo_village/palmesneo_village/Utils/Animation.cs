using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class Animation
    {
        public int FramesPerSecond
        {
            get { return framesPerSecond; }
            set
            {
                framesPerSecond = MathHelper.Clamp(value, 0, 60);
            }
        }

        public MTexture[] Frames { get; private set; }

        private int framesPerSecond;
        private float timer = 0;

        private int currentFrame;
        private int defaultFrame;

        public Animation(MTexture[] frames, int defaultFrame, int speed)
        {
            Frames = frames;
            this.defaultFrame = defaultFrame;

            FramesPerSecond = speed;
            Reset();
        }

        public Animation(MTexture texture, int frameCount, int defaultFrame, int frameWidth, int frameHeight, int xOffset, int yOffset, int speed = 5, bool readVertical = false)
        {
            Frames = new MTexture[frameCount];
            this.defaultFrame = defaultFrame;

            for (int i = 0; i < frameCount; i++)
            {
                if (readVertical)
                {
                    Frames[i] = new MTexture(texture, xOffset, yOffset + (frameHeight * i), frameWidth, frameHeight);
                }
                else
                {
                    Frames[i] = new MTexture(texture, xOffset + (frameWidth * i), yOffset, frameWidth, frameHeight);
                }
            }

            FramesPerSecond = speed;
            Reset();
        }

        public void Update(float dt)
        {
            if (Frames.Length == 1)
            {
                currentFrame = defaultFrame;
            }
            else
            {
                if (FramesPerSecond == 0)
                {
                    currentFrame = defaultFrame;
                }
                else
                {
                    timer += dt;
                    if (timer >= (1.0f / FramesPerSecond))
                    {
                        currentFrame = (currentFrame + 1) % Frames.Length;
                        timer = 0.0f;
                    }
                }
            }
        }

        public MTexture GetCurrentFrameTexture()
        {
            return Frames[currentFrame];
        }

        public void Reset()
        {
            currentFrame = defaultFrame;
            timer = 0.0f;
        }
    }
}
