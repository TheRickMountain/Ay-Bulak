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
            Complete
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
                case FishingState.Complete:
                    UpdateComplete(player);
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
                    SetState(FishingState.Complete, player);
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
                SetState(FishingState.Complete, player);
            }
        }

        private void UpdateFishPlaying(Player player)
        {
            if (MInput.Mouse.PressedLeftButton)
            {
                fishShadow.ScareAway();

                SetState(FishingState.Complete, player);
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

                SetState(FishingState.Complete, player);
            }
        }

        private void UpdateComplete(Player player)
        {
            if(fishingBobber.CurrentState == FishingBobberEntity.BobberState.Completed)
            {
                player.CurrentLocation.RemoveChild(fishingBobber);

                if (flyingItemImage != null)
                {
                    player.CurrentLocation.AddItem(player.LocalPosition,
                            new ItemContainer()
                            {
                                Item = flyingItemImage.Item,
                                Quantity = 1,
                                ContentAmount = 0
                            });

                    player.CurrentLocation.RemoveChild(flyingItemImage);

                    flyingItemImage = null;
                }

                player.SetState(new PlayerIdleState());
            }
        }

        private void SetState(FishingState newState, Player player)
        {
            FishingState previousState = currentState;
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
                        SetState(FishingState.Complete, player);
                    }
                    break;
                case FishingState.Complete:
                    {
                        player.ResetCurrentAnimation();
                        player.PlayAnimation($"fishing_rod_complete_{player.MovementDirection.ToString().ToLower()}");

                        if (previousState != FishingState.Casting)
                        {
                            ResourcesManager.GetSoundEffect("SoundEffects", "bobber_catch")
                                .Play(1.0f, Calc.Random.Range(0.0f, 0.5f), 0.0f);
                        }

                        Vector2 bobberEndPosition = player.GetTilePosition() * Engine.TILE_SIZE +
                            new Vector2(Engine.TILE_SIZE / 2, -Engine.TILE_SIZE);

                        fishingBobber.Complete(
                            fishingBobber.LocalPosition,
                            bobberEndPosition,
                            Engine.TILE_SIZE * 2);
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
                Engine.ItemsDatabase.GetItemByName("european_perch")
            ];

            return Calc.Random.Choose(loot);
        }

        private void OnFishShadowStateChanged(FishShadow.FishState newState)
        {
            switch (newState)
            {
                case FishShadow.FishState.Playing:
                    {
                        ResourcesManager.GetSoundEffect("SoundEffects", "fish_biting")
                            .Play(1.0f, Calc.Random.Range(0.0f, 0.5f), 0.0f);

                        MInput.GamePads[0].Rumble(6f, 0.2f);

                        fishingBobber.Bite();
                    }
                    break;
                case FishShadow.FishState.Biting:
                    {
                        ResourcesManager.GetSoundEffect("SoundEffects", "bobber_hooked")
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
