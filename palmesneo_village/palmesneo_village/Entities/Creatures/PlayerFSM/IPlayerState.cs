namespace palmesneo_village
{
    public interface IPlayerState
    {

        void Enter(Player player);
        void Update(Player player, float deltaTime);
        void Exit(Player player);

        bool IsInterruptible();

        void AnimationFrameChanged(Player player, int frameIndex);
    }
}
