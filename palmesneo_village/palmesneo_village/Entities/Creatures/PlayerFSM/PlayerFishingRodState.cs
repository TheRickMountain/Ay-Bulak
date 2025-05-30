using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens.Transitions;
using System;

namespace palmesneo_village
{
    public class PlayerFishingRodState : IPlayerState
    {
        private enum FishingState
        {
            None,
            Casting,
            Waiting,
            FishBiting,
            Catch,
            FlyingItem
        }

        private FishingState currentState = FishingState.None;

        private FishingBobberEntity fishingBobber;
        private ImageEntity attentionImage;
        private FlyingItemImage flyingItemImage;

        private float castingDistance = 64f;
        private float charmingDistance = 40f;
        private float catchTimeWindow = 2.0f;
        private float timer = 0.0f;

        private FishShadow currentFish;

        public PlayerFishingRodState(FishingBobberEntity fishingBobber)
        {
            this.fishingBobber = fishingBobber;

            attentionImage = new ImageEntity();
            attentionImage.Texture = ResourcesManager.GetTexture("Sprites", "attention_icon");
        }

        public void Enter(Player player)
        {
            player.PlayAnimation($"fishing_rod_{player.MovementDirection.ToString().ToLower()}");

            ResourcesManager.GetSoundEffect("SoundEffects", "fishing_rod_swoosh")
                .Play(1.0f, Calc.Random.Range(0.0f, 0.5f), 0.0f);
        }

        public void Update(Player player, float deltaTime)
        {
            switch (currentState)
            {
                case FishingState.Casting:
                    UpdateCasting(player);
                    break;
                case FishingState.Waiting:
                    UpdateWaiting(player);
                    break;
                case FishingState.FishBiting:
                    UpdateFishBiting(player);
                    break;
                case FishingState.Catch:
                    UpdateCatch(player);
                    break;
                case FishingState.FlyingItem:
                    UpdateFlyingItem(player);
                    break;
            }
        }

        private void UpdateCasting(Player player)
        {
            if (fishingBobber.State == FishingBobberEntity.BobberState.Floating)
            {
                Vector2 bobberTilePosition = player.CurrentLocation.WorldToMap(fishingBobber.LocalPosition);

                GroundTile groundTile = player.CurrentLocation.GetGroundTile(
                    (int)bobberTilePosition.X,
                    (int)bobberTilePosition.Y);

                if (groundTile != GroundTile.Water)
                {
                    StopFishing(player);
                }
                else
                {
                    ResourcesManager.GetSoundEffect("SoundEffects", "bobber_splash")
                        .Play(1.0f, Calc.Random.Range(0.0f, 0.5f), 0.0f);

                    currentState = FishingState.Waiting;
                }
            }
        }

        private void UpdateWaiting(Player player)
        {
            foreach (var fish in player.CurrentLocation.GetFish())
            {
                float distance = Vector2.Distance(fishingBobber.LocalPosition, fish.LocalPosition);

                float angleBetweenFishAndBobber = Calc.AngleDegrees(fishingBobber.LocalPosition - fish.LocalPosition);

                float fishAngleDegrees = MathHelper.ToDegrees(fish.LocalRotation) % 359;

                if (fishAngleDegrees < 0)
                {
                    fishAngleDegrees += 359;
                }

                float diff = fishAngleDegrees - angleBetweenFishAndBobber;

                if (distance < charmingDistance)
                {
                    if ((diff >= 0 && diff <= 30) || (diff >= -30 && diff < 0))
                    {
                        currentFish = fish;

                        currentFish.SetTarget(fishingBobber.LocalPosition);

                        currentState = FishingState.FishBiting;

                        break;
                    }
                }
            }

            if (MInput.Mouse.PressedLeftButton)
            {
                StopFishing(player);
            }
        }

        private void UpdateFishBiting(Player player)
        {
            float distance = Vector2.Distance(fishingBobber.LocalPosition, currentFish.LocalPosition);

            if (distance < 4)
            {
                ResourcesManager.GetSoundEffect("SoundEffects", "bobber_splash")
                        .Play(1.0f, Calc.Random.Range(0.0f, 0.5f), 0.0f);

                player.CurrentLocation.AddChild(attentionImage);
                attentionImage.LocalPosition = fishingBobber.LocalPosition + new Vector2(0, -Engine.TILE_SIZE);

                currentState = FishingState.Catch;
            }

            if (MInput.Mouse.PressedLeftButton)
            {
                currentFish?.ScareAway();

                StopFishing(player);
            }
        }

        private void UpdateCatch(Player player)
        {
            timer += Engine.GameDeltaTime;

            if (timer >= catchTimeWindow)
            {
                player.CurrentLocation.RemoveChild(attentionImage);
                player.CurrentLocation.RemoveChild(fishingBobber);

                currentFish?.ScareAway();

                player.SetState(new PlayerIdleState());

                currentState = FishingState.None;
            }
            else
            {
                if (MInput.Mouse.PressedLeftButton)
                {
                    player.CurrentLocation.RemoveFish(currentFish);

                    player.CurrentLocation.RemoveChild(attentionImage);

                    Item lootItem = currentFish.GetLoot();

                    flyingItemImage = new FlyingItemImage(lootItem, fishingBobber.LocalPosition, player.LocalPosition);

                    player.CurrentLocation.AddChild(flyingItemImage);

                    ResourcesManager.GetSoundEffect("SoundEffects", "bobber_catch")
                        .Play(1.0f, Calc.Random.Range(0.0f, 0.5f), 0.0f);

                    player.CurrentLocation.RemoveChild(fishingBobber);

                    player.ResetCurrentAnimation();
                    player.PlayAnimation($"idle_{player.MovementDirection.ToString().ToLower()}");

                    currentState = FishingState.FlyingItem;
                }
            }
        }

        private void UpdateFlyingItem(Player player)
        {
            if(flyingItemImage.IsCompleted)
            {
                player.CurrentLocation.AddItem(player.LocalPosition,
                        new ItemContainer()
                        {
                            Item = flyingItemImage.Item,
                            Quantity = 1,
                            ContentAmount = 0
                        });

                player.CurrentLocation.RemoveChild(flyingItemImage);

                player.SetState(new PlayerIdleState());

                currentState = FishingState.None;
            }
        }

        private void StopFishing(Player player)
        {
            player.CurrentLocation.RemoveChild(fishingBobber);

            player.SetState(new PlayerIdleState());

            currentState = FishingState.None;
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

                currentState = FishingState.Casting;
            }
        }

        public bool IsInterruptible()
        {
            return false;
        }
    }
}
