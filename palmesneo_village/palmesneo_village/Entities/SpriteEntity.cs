using System;
using System.Collections.Generic;

namespace palmesneo_village
{
    public class SpriteEntity : ImageEntity
    {
        public Animation CurrentAnimation { get; private set; }

        public Action<int> AnimationFrameChaged { get; set; }

        private Dictionary<string, Animation> animations;
        private string currentAnimationKey = "";

        public SpriteEntity()
        {
            animations = new Dictionary<string, Animation>();
        }

        public void AddAnimation(string id, Animation animation)
        {
            animations.Add(id, animation);
            animation.FrameChanged += OnAnimationFrameChanged;
        }

        public Animation GetAnimation(string id)
        {
            return animations[id];
        }

        public void Play(string id)
        {
            if (currentAnimationKey == id)
                return;

            currentAnimationKey = id;
            CurrentAnimation = animations[id];
        }

        public override void Update()
        {
            base.Update();

            if (CurrentAnimation != null)
            {
                CurrentAnimation.Update(Engine.GameDeltaTime);
                Texture = CurrentAnimation.GetCurrentFrameTexture();
            }
        }

        private void OnAnimationFrameChanged(int frameIndex)
        {
            AnimationFrameChaged?.Invoke(frameIndex);
        }

    }
}
