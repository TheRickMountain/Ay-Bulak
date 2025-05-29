using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace palmesneo_village
{
    public class PlayerWalkState : IPlayerState
    {
        public void Enter(Player player)
        {
        }

        public void Update(Player player, float deltaTime)
        {
            player.PlayAnimation($"walk_{player.MovementDirection.ToString().ToLower()}");

            Vector2 movementVector = new Vector2(InputBindings.MoveHorizontally.Value, InputBindings.MoveVertically.Value);

            player.UpdateMovement(movementVector);

            player.ShakeGrass();

            if (movementVector == Vector2.Zero)
            {
                player.SetState(new PlayerIdleState());
            }
        }

        public void Exit(Player player)
        {
        }

        public void AnimationFrameChanged(Player player, int frameIndex)
        {
            if (frameIndex == 1 || frameIndex == 3)
            {
                Vector2 tilePosition = player.GetTilePosition();

                FloorPathItem floorPathItem = player.CurrentLocation.GetTileFloorPathItem(tilePosition);
                if (floorPathItem != null)
                {
                    var footstepSFX = ResourcesManager.GetSoundEffect(floorPathItem.FootstepSoundEffect);

                    if (footstepSFX == null)
                    {
                        Debug.WriteLine($"Sfx '{floorPathItem.FootstepSoundEffect}' not found!");
                    }
                    else
                    {
                        footstepSFX.Play(0.5f, Calc.Random.Range(0.0f, 0.5f), 0.0f);
                    }
                }
                else
                {
                    GroundTile groundTile = player.CurrentLocation.GetGroundTile((int)tilePosition.X, (int)tilePosition.Y);

                    switch (groundTile)
                    {
                        case GroundTile.FarmPlot:
                        case GroundTile.Ground:
                        case GroundTile.CoopHouseFloor:
                            {
                                ResourcesManager.GetSoundEffect(
                                    "SoundEffects",
                                    "RPG_Essentials_Free",
                                    "12_Player_Movement_SFX",
                                    "45_Landing_01").Play(0.5f, Calc.Random.Range(0.0f, 0.5f), 0.0f);
                            }
                            break;
                        case GroundTile.Grass:
                            {
                                ResourcesManager.GetSoundEffect(
                                    "SoundEffects",
                                    "RPG_Essentials_Free",
                                    "12_Player_Movement_SFX",
                                    "03_Step_grass_03").Play(0.5f, Calc.Random.Range(0.0f, 0.5f), 0.0f);
                            }
                            break;
                        case GroundTile.HouseFloor:
                            {
                                ResourcesManager.GetSoundEffect(
                                    "SoundEffects",
                                    "RPG_Essentials_Free",
                                    "12_Player_Movement_SFX",
                                    "12_Step_wood_03").Play(0.5f, Calc.Random.Range(0.0f, 0.5f), 0.0f);
                            }
                            break;
                    }
                }
            }
        }

        public bool IsInterruptible()
        {
            return true;
        }
    }
}
