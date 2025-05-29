using Microsoft.Xna.Framework;
using System;

namespace palmesneo_village
{
    public class PlayerDefaultToolState : IPlayerState
    {

        private string animationName;
        private Action onUse;


        public PlayerDefaultToolState(string animationName, Action onUse)
        {
            this.animationName = animationName;
            this.onUse = onUse;
        }

        public void Enter(Player player)
        {
            player.PlayAnimation($"{animationName}_{player.MovementDirection.ToString().ToLower()}");
        }

        public void Update(Player player, float deltaTime)
        {
            if(player.IsCurrentAnimationFinished())
            {
                player.SetState(new PlayerIdleState());
            }
        }

        public void Exit(Player player)
        {
            player.ResetCurrentAnimation();
        }

        public void AnimationFrameChanged(Player player, int frameIndex)
        {
            if(frameIndex == 2)
            {
                onUse?.Invoke();
            }
        }

        public bool IsInterruptible()
        {
            return false;
        }

        
    }
}
