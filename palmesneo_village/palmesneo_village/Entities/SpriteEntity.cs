using System.Collections.Generic;

namespace palmesneo_village
{
    public class SpriteEntity : ImageEntity
    {

        private Dictionary<string, Animation> animations;
        private Animation currentAnimation;
        private string currentAnimationKey = "";

        public SpriteEntity()
        {
            animations = new Dictionary<string, Animation>();
        }

        public void AddAnimation(string id, Animation animation)
        {
            animations.Add(id, animation);
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
            currentAnimation = animations[id];
        }

        public override void Update()
        {
            base.Update();

            if (currentAnimation != null)
            {
                currentAnimation.Update(Engine.GameDeltaTime);
                Texture = currentAnimation.GetCurrentFrameTexture();
            }
        }

    }
}
