using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class PlayerIdleState : IPlayerState
    {
        public void Enter(Player player)
        {
            player.PlayAnimation($"idle_{player.MovementDirection.ToString().ToLower()}");
        }

        public void Update(Player player, float deltaTime)
        {
            Vector2 movementVector = new Vector2(InputBindings.MoveHorizontally.Value, InputBindings.MoveVertically.Value);

            if (movementVector != Vector2.Zero)
            {
                player.SetState(new PlayerWalkState());
            }
        }

        public void Exit(Player player)
        {
        }

        public void AnimationFrameChanged(Player player, int frameIndex)
        {
        }

        public bool IsInterruptible()
        {
            return true;
        }
    }
}
