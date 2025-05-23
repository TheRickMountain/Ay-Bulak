using Microsoft.Xna.Framework;
using System;

namespace palmesneo_village
{
    public class Animation
    {
        public int FramesPerSecond
        {
            get => framesPerSecond;
            set => framesPerSecond = MathHelper.Clamp(value, 0, 60);
        }

        public MTexture[] Frames { get; private set; }

        public bool Loop { get; set; } = true;
        public bool IsFinished { get; private set; }

        public int CurrentFrame { get; private set; }

        private int framesPerSecond;
        private float timer = 0;

        private int defaultFrame;
        private int lastFrame = -1;

        public Action<int> FrameChanged { get; set; }

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
            if(Frames.Length == 1 && FramesPerSecond == 0)
            {
                CurrentFrame = defaultFrame;
                return;
            }

            if (IsFinished) return;

            timer += dt;
            if (timer >= (1.0f / FramesPerSecond))
            {
                timer -= (1.0f / FramesPerSecond);

                CurrentFrame++;

                if(CurrentFrame >= Frames.Length)
                {
                    if(Loop)
                    {
                        CurrentFrame = 0;
                    }
                    else
                    {
                        CurrentFrame = Frames.Length - 1;
                        IsFinished = true;
                    }
                }

                if (CurrentFrame != lastFrame)
                {
                    lastFrame = CurrentFrame;
                    FrameChanged?.Invoke(CurrentFrame);
                }
            }
        }

        public MTexture GetCurrentFrameTexture()
        {
            return Frames[CurrentFrame];
        }

        public void Reset()
        {
            CurrentFrame = defaultFrame;
            timer = 0.0f;
            lastFrame = -1;
            IsFinished = false;
        }
    }
}
