using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class PlayerFishingRodState : IPlayerState
    {
        private enum FishingState
        {
            None,
            Casting,
            Waiting
        }

        private FishingState fishingState = FishingState.None;

        private FishingBobberEntity fishingBobber;

        private float castingDistance = 64f;

        public PlayerFishingRodState(FishingBobberEntity fishingBobber)
        {
            this.fishingBobber = fishingBobber;
        }

        public void Enter(Player player)
        {
            player.PlayAnimation($"fishing_rod_{player.MovementDirection.ToString().ToLower()}");

            ResourcesManager.GetSoundEffect("SoundEffects", "fishing_rod_swoosh")
                .Play(1.0f, Calc.Random.Range(0.0f, 0.5f), 0.0f);
        }

        public void Update(Player player, float deltaTime)
        {
            switch (fishingState)
            {
                case FishingState.Casting:
                    {
                        if(fishingBobber.State == FishingBobberEntity.BobberState.Floating)
                        {
                            ResourcesManager.GetSoundEffect("SoundEffects", "bobber_splash")
                                .Play(1.0f, Calc.Random.Range(0.0f, 0.5f), 0.0f);

                            fishingState = FishingState.Waiting;
                        }
                    }
                    break;
                case FishingState.Waiting:
                    {
                        if(MInput.Mouse.PressedLeftButton)
                        {
                            player.CurrentLocation.RemoveChild(fishingBobber);

                            player.SetState(new PlayerIdleState());

                            fishingState = FishingState.None;
                        }
                    }
                    break;
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
                // TODO: более корректно вычислить конечную позицию поплавка

                Vector2 bobberStartPosition = player.GetTilePosition() * Engine.TILE_SIZE + 
                    new Vector2(Engine.TILE_SIZE / 2, -Engine.TILE_SIZE);

                Vector2 bobberCastingDirection = Calc.GetVector2(player.MovementDirection);

                Vector2 bobberEndPosition = 
                    (player.GetTilePosition() * Engine.TILE_SIZE + new Vector2(Engine.TILE_SIZE) / 2)
                    + bobberCastingDirection * castingDistance;

                player.CurrentLocation.AddChild(fishingBobber);
                fishingBobber.BeginCast(
                    bobberStartPosition,
                    bobberEndPosition, 
                    Engine.TILE_SIZE * 2);

                fishingState = FishingState.Casting;
            }
        }

        public bool IsInterruptible()
        {
            return false;
        }
    }
}
