using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Screens.Transitions;
using System;
using System.Collections.Generic;

namespace palmesneo_village
{
    public class PlayerFishingRodState : IPlayerState
    {
        private enum FishingState
        {
            None,
            Casting,
            WaitingForFishSpawning,
            FishSpawning,
            FishPlaying,
            FishCatchingWindow,
            FishAte,
            FlyingItem
        }

        private FishingState currentState = FishingState.None;

        private FishingBobberEntity fishingBobber;
        private FlyingItemImage flyingItemImage;
        private FishShadow fishShadow;

        private Range<float> waitingTimeRange = new Range<float>(2.0f, 10.0f);
        private float castingDistance = 64f;
        private float timer = 0.0f;

        private Player player;

        public PlayerFishingRodState(FishingBobberEntity fishingBobber)
        {
            this.fishingBobber = fishingBobber;

            fishShadow = new FishShadow();
            fishShadow.StateChanged += OnFishShadowStateChanged;
        }

        public void Enter(Player player)
        {
            this.player = player;

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
                case FishingState.WaitingForFishSpawning:
                    UpdateWaitingForFishSpawning(player);
                    break;
                case FishingState.FishPlaying:
                    UpdateFishPlaying(player);
                    break;
                case FishingState.FishCatchingWindow:
                    UpdateFishCatchingWindow(player);
                    break;
                case FishingState.FlyingItem:
                    UpdateFlyingItem(player);
                    break;
            }
        }

        private void UpdateCasting(Player player)
        {
            if (fishingBobber.CurrentState == FishingBobberEntity.BobberState.WaterEntering)
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

                    SetState(FishingState.WaitingForFishSpawning, player);
                }
            }
        }

        private void UpdateWaitingForFishSpawning(Player player)
        {
            timer -= Engine.GameDeltaTime;

            if(timer < 0)
            {
                SetState(FishingState.FishSpawning, player);
            }

            if (MInput.Mouse.PressedLeftButton)
            {
                StopFishing(player);
            }
        }

        private void UpdateFishPlaying(Player player)
        {
            if (MInput.Mouse.PressedLeftButton)
            {
                fishShadow.ScareAway();

                StopFishing(player);
            }
        }

        private void UpdateFishCatchingWindow(Player player)
        {
            if (MInput.Mouse.PressedLeftButton)
            {
                player.CurrentLocation.RemoveChild(fishShadow);

                Item lootItem = GetLoot();

                flyingItemImage = new FlyingItemImage(lootItem, fishingBobber.LocalPosition, player.LocalPosition);

                player.CurrentLocation.AddChild(flyingItemImage);

                ResourcesManager.GetSoundEffect("SoundEffects", "bobber_catch")
                    .Play(1.0f, Calc.Random.Range(0.0f, 0.5f), 0.0f);

                player.CurrentLocation.RemoveChild(fishingBobber);

                player.ResetCurrentAnimation();
                player.PlayAnimation($"idle_{player.MovementDirection.ToString().ToLower()}");

                SetState(FishingState.FlyingItem, player);
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

        private void SetState(FishingState newState, Player player)
        {
            currentState = newState;

            switch (currentState)
            {
                case FishingState.WaitingForFishSpawning:
                    {
                        timer = Calc.Random.Range(waitingTimeRange);
                    }
                    break;
                case FishingState.FishSpawning:
                    {
                        player.CurrentLocation.AddChild(fishShadow);
                        fishShadow.SetBobberPosition(fishingBobber.LocalPosition);

                        SetState(FishingState.FishPlaying, player);
                    }
                    break;
                case FishingState.FishAte:
                    {
                        StopFishing(player);
                    }
                    break;
            }
        }

        private void StopFishing(Player player)
        {
            player.CurrentLocation.RemoveChild(fishingBobber);

            player.SetState(new PlayerIdleState());

            SetState(FishingState.None, player);
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

                SetState(FishingState.Casting, player);
            }
        }

        public bool IsInterruptible()
        {
            return false;
        }

        private Item GetLoot()
        {
            List<Item> loot =
            [
                Engine.ItemsDatabase.GetItemByName("prussian_carp"),
                Engine.ItemsDatabase.GetItemByName("common_bream"),
            ];

            return Calc.Random.Choose(loot);
        }

        private void OnFishShadowStateChanged(FishShadow.FishState newState)
        {
            switch (newState)
            {
                case FishShadow.FishState.Playing:
                    {
                        ResourcesManager.GetSoundEffect("SoundEffects", "bobber_splash")
                            .Play(1.0f, Calc.Random.Range(0.0f, 0.5f), 0.0f);

                        MInput.GamePads[0].Rumble(6f, 0.2f);

                        fishingBobber.Bite();
                    }
                    break;
                case FishShadow.FishState.Biting:
                    {
                        ResourcesManager.GetSoundEffect("SoundEffects", "bobber_catch")
                            .Play(1.0f, Calc.Random.Range(0.0f, 0.5f), 0.0f);

                        MInput.GamePads[0].Rumble(12f, 0.4f);

                        fishingBobber.Bite();

                        SetState(FishingState.FishCatchingWindow, player);
                    }
                    break;
                case FishShadow.FishState.Ate:
                    {
                        SetState(FishingState.FishAte, player);
                    }
                    break;
            }
        }
    }
}
