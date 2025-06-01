using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace palmesneo_village
{
    public class Player : Creature
    {
        public Direction MovementDirection { get; private set; } = Direction.Down;

        private Inventory inventory;
        private InventoryHotbar inventoryHotbar;
        private PlayerEnergyManager energyManager;

        private const float COLLISION_CHECK_OFFSET = 4f;

        private const float ITEM_PICKUP_SPEED = 300f;
        private const float ITEM_ATTRACTION_DISTANCE = 48f;
        private const float ITEM_PICKUP_DISTANCE = 5f;

        private SpriteEntity bodySprite;

        private SoundEffectInstance currentSFX;

        private List<SoundEffectInstance> grassShakeSFXs = new();

        private FishingBobberEntity fishingBobber;

        private IPlayerState currentState;

        public Player(string name, MTexture texture, float speed, Inventory inventory, InventoryHotbar inventoryHotbar,
            PlayerEnergyManager energyManager) : 
            base(name, texture, speed)
        {
            this.inventory = inventory;
            this.inventoryHotbar = inventoryHotbar;
            this.energyManager = energyManager;

            IsDepthSortEnabled = true;

            // TODO: перестать использовать BodyImage
            BodyImage.IsVisible = false;

            CreateAndInitializeBodySprite(texture);

            // TODO: создать отдельный класс для воспроизведения звуков
            grassShakeSFXs = new List<SoundEffectInstance>();

            for (int i = 1; i <= 3; i++)
            {
                SoundEffectInstance sfxInstance = ResourcesManager.GetSoundEffect(
                    "SoundEffects", 
                    "Minifantasy_Forgotten_Plains_SFX", 
                    $"01_bush_rustling_{i}").CreateInstance();

                grassShakeSFXs.Add(sfxInstance);
            }

            fishingBobber = new FishingBobberEntity();

            SetState(new PlayerIdleState());
        }

        private void CreateAndInitializeBodySprite(MTexture spritesheet)
        {
            int framesColumns = 10;
            int framesRows = 8;

            int frameWidth = spritesheet.Width / framesColumns;
            int frameHeight = spritesheet.Height / framesRows;

            bodySprite = new SpriteEntity();
            bodySprite.AddAnimation("idle_down", new Animation(spritesheet, 1, 0, frameWidth, frameHeight, 0, 0));
            bodySprite.AddAnimation("idle_left", new Animation(spritesheet, 1, 0, frameWidth, frameHeight, 0, frameHeight));
            bodySprite.AddAnimation("idle_up", new Animation(spritesheet, 1, 0, frameWidth, frameHeight, 0, frameHeight * 2));
            bodySprite.AddAnimation("idle_right", new Animation(spritesheet, 1, 0, frameWidth, frameHeight, 0, frameHeight * 3));

            bodySprite.AddAnimation("walk_down", new Animation(spritesheet, 4, 0, frameWidth, frameHeight, 0, 0));
            bodySprite.AddAnimation("walk_left", new Animation(spritesheet, 4, 0, frameWidth, frameHeight, 0, frameHeight));
            bodySprite.AddAnimation("walk_up", new Animation(spritesheet, 4, 0, frameWidth, frameHeight, 0, frameHeight * 2));
            bodySprite.AddAnimation("walk_right", new Animation(spritesheet, 4, 0, frameWidth, frameHeight, 0, frameHeight * 3));
            AddChild(bodySprite);

            bodySprite.AddAnimation("showel_down", new Animation(spritesheet, 3, 0, frameWidth, frameHeight, frameWidth * 4, 0));
            bodySprite.AddAnimation("showel_left", new Animation(spritesheet, 3, 0, frameWidth, frameHeight, frameWidth * 4, frameHeight));
            bodySprite.AddAnimation("showel_up", new Animation(spritesheet, 3, 0, frameWidth, frameHeight, frameWidth * 4, frameHeight * 2));
            bodySprite.AddAnimation("showel_right", new Animation(spritesheet, 3, 0, frameWidth, frameHeight, frameWidth * 4, frameHeight * 3));

            bodySprite.GetAnimation("showel_down").Loop = false;
            bodySprite.GetAnimation("showel_left").Loop = false;
            bodySprite.GetAnimation("showel_up").Loop = false;
            bodySprite.GetAnimation("showel_right").Loop = false;

            bodySprite.AddAnimation("watering_can_down", new Animation(spritesheet, 3, 0, frameWidth, frameHeight, 0, frameHeight * 4));
            bodySprite.AddAnimation("watering_can_left", new Animation(spritesheet, 3, 0, frameWidth, frameHeight, 0, frameHeight * 5));
            bodySprite.AddAnimation("watering_can_up", new Animation(spritesheet, 3, 0, frameWidth, frameHeight, 0, frameHeight * 6));
            bodySprite.AddAnimation("watering_can_right", new Animation(spritesheet, 3, 0, frameWidth, frameHeight, 0, frameHeight * 7));

            bodySprite.GetAnimation("watering_can_down").Loop = false;
            bodySprite.GetAnimation("watering_can_left").Loop = false;
            bodySprite.GetAnimation("watering_can_up").Loop = false;
            bodySprite.GetAnimation("watering_can_right").Loop = false;

            bodySprite.AddAnimation("axe_down", new Animation(spritesheet, 3, 0, frameWidth, frameHeight, frameWidth * 4, frameHeight * 4));
            bodySprite.AddAnimation("axe_left", new Animation(spritesheet, 3, 0, frameWidth, frameHeight, frameWidth * 4, frameHeight * 5));
            bodySprite.AddAnimation("axe_up", new Animation(spritesheet, 3, 0, frameWidth, frameHeight, frameWidth * 4, frameHeight * 6));
            bodySprite.AddAnimation("axe_right", new Animation(spritesheet, 3, 0, frameWidth, frameHeight, frameWidth * 4, frameHeight * 7));

            bodySprite.GetAnimation("axe_down").Loop = false;
            bodySprite.GetAnimation("axe_left").Loop = false;
            bodySprite.GetAnimation("axe_up").Loop = false;
            bodySprite.GetAnimation("axe_right").Loop = false;

            bodySprite.AddAnimation("scythe_down", new Animation(spritesheet, 3, 0, frameWidth, frameHeight, frameWidth * 7, 0));
            bodySprite.AddAnimation("scythe_left", new Animation(spritesheet, 3, 0, frameWidth, frameHeight, frameWidth * 7, frameHeight));
            bodySprite.AddAnimation("scythe_up", new Animation(spritesheet, 3, 0, frameWidth, frameHeight, frameWidth * 7, frameHeight * 2));
            bodySprite.AddAnimation("scythe_right", new Animation(spritesheet, 3, 0, frameWidth, frameHeight, frameWidth * 7, frameHeight * 3));

            bodySprite.GetAnimation("scythe_down").Loop = false;
            bodySprite.GetAnimation("scythe_left").Loop = false;
            bodySprite.GetAnimation("scythe_up").Loop = false;
            bodySprite.GetAnimation("scythe_right").Loop = false;

            bodySprite.AddAnimation("fishing_rod_down", new Animation(spritesheet, 3, 0, frameWidth, frameHeight, frameWidth * 7, frameHeight * 4));
            bodySprite.AddAnimation("fishing_rod_left", new Animation(spritesheet, 3, 0, frameWidth, frameHeight, frameWidth * 7, frameHeight * 5));
            bodySprite.AddAnimation("fishing_rod_up", new Animation(spritesheet, 3, 0, frameWidth, frameHeight, frameWidth * 7, frameHeight * 6));
            bodySprite.AddAnimation("fishing_rod_right", new Animation(spritesheet, 3, 0, frameWidth, frameHeight, frameWidth * 7, frameHeight * 7));

            bodySprite.GetAnimation("fishing_rod_down").Loop = false;
            bodySprite.GetAnimation("fishing_rod_left").Loop = false;
            bodySprite.GetAnimation("fishing_rod_up").Loop = false;
            bodySprite.GetAnimation("fishing_rod_right").Loop = false;

            bodySprite.AddAnimation("fishing_rod_complete_down", new Animation(spritesheet, 1, 0, frameWidth, frameHeight, frameWidth * 3, frameHeight * 4));
            bodySprite.AddAnimation("fishing_rod_complete_left", new Animation(spritesheet, 1, 0, frameWidth, frameHeight, frameWidth * 3, frameHeight * 5));
            bodySprite.AddAnimation("fishing_rod_complete_up", new Animation(spritesheet, 1, 0, frameWidth, frameHeight, frameWidth * 3, frameHeight * 6));
            bodySprite.AddAnimation("fishing_rod_complete_right", new Animation(spritesheet, 1, 0, frameWidth, frameHeight, frameWidth * 3, frameHeight * 7));

            bodySprite.LocalPosition = new Vector2(-frameWidth / 2, -(frameHeight - (Engine.TILE_SIZE / 2)));

            bodySprite.AnimationFrameChaged += OnBodySpriteAnimationFrameChanged;

            bodySprite.Play("idle_down");
        }

        public override void Update()
        {
            base.Update();

            currentState?.Update(this, Engine.GameDeltaTime);

            UpdateItemsPickup();

            if(currentSFX != null && currentSFX.State == SoundState.Stopped)
            {
                currentSFX = null;
            }
        }

        public void UpdateMovement(Vector2 movementVector)
        {
            if (movementVector != Vector2.Zero)
            {
                movementVector.Normalize();

                MovementDirection = Calc.GetDirection(movementVector);

                float actualSpeed = Speed;

                FloorPathItem floorPathItem = CurrentLocation.GetTileFloorPathItem(GetTilePosition());
                if (floorPathItem != null)
                {
                    actualSpeed = Speed + (Speed * floorPathItem.MovementSpeedBuff);
                }

                Vector2 newPosition = LocalPosition + movementVector * actualSpeed * Engine.GameDeltaTime;

                // Проверяем коллизии перед перемещением
                if (IsValidMovement(newPosition))
                {
                    LocalPosition = newPosition;
                }
                else
                {
                    // Попробуем двигаться только по X или только по Y
                    Vector2 testX = LocalPosition + new Vector2(movementVector.X * actualSpeed * Engine.GameDeltaTime, 0);
                    Vector2 testY = LocalPosition + new Vector2(0, movementVector.Y * actualSpeed * Engine.GameDeltaTime);

                    if (IsValidMovement(testX))
                    {
                        LocalPosition = testX;
                    }
                    else if (IsValidMovement(testY))
                    {
                        LocalPosition = testY;
                    }
                }
            }
        }

        private bool IsValidMovement(Vector2 newPosition)
        {
            // Преобразуем мировые координаты в координаты тайлов
            Vector2 mapPos = CurrentLocation.WorldToMap(newPosition);

            // Проверяем 4 точки вокруг игрока (предполагаем, что размер игрока примерно равен тайлу)
            Vector2[] checkPoints =
            {
                new Vector2(newPosition.X - COLLISION_CHECK_OFFSET, newPosition.Y - COLLISION_CHECK_OFFSET),
                new Vector2(newPosition.X + COLLISION_CHECK_OFFSET, newPosition.Y - COLLISION_CHECK_OFFSET),
                new Vector2(newPosition.X - COLLISION_CHECK_OFFSET, newPosition.Y + COLLISION_CHECK_OFFSET),
                new Vector2(newPosition.X + COLLISION_CHECK_OFFSET, newPosition.Y + COLLISION_CHECK_OFFSET)
            };

            Rectangle boundaries = CurrentLocation.GetBoundaries();

            foreach (Vector2 point in checkPoints)
            {
                // Проверка выхода за границы карты
                if (!boundaries.Contains(point))
                    return false;

                Vector2 tilePos = CurrentLocation.WorldToMap(point);
                int x = (int)tilePos.X;
                int y = (int)tilePos.Y;

                // Проверяем, является ли тайл непроходимым
                if (CurrentLocation.IsTilePassable(x, y) == false)
                    return false;
            }

            return true;
        }

        public void SetState(IPlayerState newState)
        {
            currentState?.Exit(this);
            currentState = newState;
            currentState.Enter(this);
        }

        private void UpdateItemsPickup()
        {
            foreach(ItemEntity itemEntity in CurrentLocation.GetItemEntities())
            {
                ItemContainer itemContainer = itemEntity.ItemContainer;

                if (inventory.CanAddItem(itemContainer.Item, itemContainer.Quantity))
                {
                    UpdateItemEntityAttraction(itemEntity);
                }
            }
        }

        private void UpdateItemEntityAttraction(ItemEntity itemEntity)
        {
            Vector2 direction = LocalPosition - itemEntity.LocalPosition;
            float distance = direction.Length();

            if (distance < ITEM_ATTRACTION_DISTANCE)
            {
                direction.Normalize();
                itemEntity.LocalPosition += direction * ITEM_PICKUP_SPEED * Engine.GameDeltaTime;

                if(distance <= ITEM_PICKUP_DISTANCE)
                {
                    PickupItem(itemEntity);

                    PlayItemPickupSoundEffect();

                    CurrentLocation.RemoveItemEntity(itemEntity);
                }
            }
        }

        private void PickupItem(ItemEntity itemEntity)
        {
            ItemContainer itemContainer = itemEntity.ItemContainer;

            inventory.TryAddItem(itemContainer.Item, itemContainer.Quantity, itemContainer.ContentAmount);
        }

        public void ShakeGrass()
        {
            Vector2 playerTile = GetTilePosition();

            Building building = CurrentLocation.GetBuilding((int)playerTile.X, (int)playerTile.Y);

            if(building != null && building is GrassBuilding grassBuilding)
            {
                grassBuilding.Shake(MovementDirection);

                PlayGrassShakeSoundEffect();
            }
        }

        private void PlayItemPickupSoundEffect()
        {
            Calc.Random.Choose(
                ResourcesManager.GetSoundEffect("SoundEffects", "pop_0"),
                ResourcesManager.GetSoundEffect("SoundEffects", "pop_1"),
                ResourcesManager.GetSoundEffect("SoundEffects", "pop_2"),
                ResourcesManager.GetSoundEffect("SoundEffects", "pop_3"),
                ResourcesManager.GetSoundEffect("SoundEffects", "pop_4"),
                ResourcesManager.GetSoundEffect("SoundEffects", "pop_5")
                ).Play();
        }

        private void PlayGrassShakeSoundEffect()
        {
            if (currentSFX != null) return;

            currentSFX = Calc.Random.Choose(grassShakeSFXs);
            currentSFX.Play();
        }

        public override void DebugRender()
        {
            base.DebugRender();

            Vector2[] checkPoints =
            {
                new Vector2(LocalPosition.X - COLLISION_CHECK_OFFSET, LocalPosition.Y - COLLISION_CHECK_OFFSET),
                new Vector2(LocalPosition.X + COLLISION_CHECK_OFFSET, LocalPosition.Y - COLLISION_CHECK_OFFSET),
                new Vector2(LocalPosition.X - COLLISION_CHECK_OFFSET, LocalPosition.Y + COLLISION_CHECK_OFFSET),
                new Vector2(LocalPosition.X + COLLISION_CHECK_OFFSET, LocalPosition.Y + COLLISION_CHECK_OFFSET)
            };

            RenderManager.Line(checkPoints[0], checkPoints[1], Color.YellowGreen);
            RenderManager.Line(checkPoints[1], checkPoints[3], Color.YellowGreen);
            RenderManager.Line(checkPoints[3], checkPoints[2], Color.YellowGreen);
            RenderManager.Line(checkPoints[2], checkPoints[0], Color.YellowGreen);
        }
    
        public override IEnumerable<InteractionData> GetAvailableInteractions(Inventory inventory)
        {
            yield break;
        }

        public override void Interact(InteractionData interactionData, Inventory inventory)
        {

        }

        public void InteractWithTile(int tileX, int tileY)
        {
            if (currentState?.IsInterruptible() == false) return;

            LookAtTile(new Vector2(tileX, tileY));

            Item item = inventory.GetSlotItem(inventoryHotbar.CurrentSlotIndex);

            if(item is ToolItem toolItem)
            {
                switch(toolItem.ToolType)
                {
                    case ToolType.Showel:
                        {
                            energyManager.ConsumeEnergy(1);
                            SetState(new PlayerDefaultToolState("showel", () =>
                            {
                                CurrentLocation.InteractWithTile(tileX, tileY, inventory, inventoryHotbar.CurrentSlotIndex);
                            }));
                        }
                        break;
                    case ToolType.WateringCan:
                        {
                            energyManager.ConsumeEnergy(1);
                            SetState(new PlayerDefaultToolState("watering_can", () =>
                            {
                                CurrentLocation.InteractWithTile(tileX, tileY, inventory, inventoryHotbar.CurrentSlotIndex);
                            }));
                        }
                        break;
                    case ToolType.Axe:
                        {
                            energyManager.ConsumeEnergy(1);
                            SetState(new PlayerDefaultToolState("axe", () => 
                            {
                                CurrentLocation.InteractWithTile(tileX, tileY, inventory, inventoryHotbar.CurrentSlotIndex);
                            }));
                        }
                        break;
                    case ToolType.Scythe:
                        {
                            energyManager.ConsumeEnergy(1);
                            SetState(new PlayerDefaultToolState("scythe", () =>
                            {
                                ApplyScytheEffect();
                            }));
                        }
                        break;
                    case ToolType.FishingRod:
                        {
                            energyManager.ConsumeEnergy(1);
                            SetState(new PlayerFishingRodState(fishingBobber));
                        }
                        break;
                }
            }
            else
            {
                CurrentLocation.InteractWithTile(tileX, tileY, inventory, inventoryHotbar.CurrentSlotIndex);
            }
        }

        public void PlayAnimation(string animationName)
        {
            bodySprite.Play(animationName);
        }

        public void ResetCurrentAnimation()
        {
            bodySprite.CurrentAnimation?.Reset();
        }

        public bool IsCurrentAnimationFinished()
        {
            if (bodySprite.CurrentAnimation == null) return true;

            return bodySprite.CurrentAnimation.IsFinished;
        }

        public bool CanChangeHotbarSlot()
        {
            if(currentState == null) return false;

            return currentState is PlayerIdleState || currentState is PlayerWalkState;
        }

        private void LookAtTile(Vector2 targetTile)
        {
            Vector2 selfTile = GetTilePosition();
            Vector2 directionToTarget = targetTile - selfTile;

            if (directionToTarget != Vector2.Zero)
            {
                directionToTarget.Normalize();
                MovementDirection = Calc.GetDirection(directionToTarget);
            }
        }

        private void OnBodySpriteAnimationFrameChanged(int frameIndex)
        {
            currentState?.AnimationFrameChanged(this, frameIndex);
        }

        private void ApplyScytheEffect()
        {
            Vector2 baseTile = GetTilePosition();
            List<Point> affectedTiles = new();

            switch (MovementDirection)
            {
                case Direction.Up:
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        affectedTiles.Add(new Point((int)baseTile.X + dx, (int)baseTile.Y - 1));
                        affectedTiles.Add(new Point((int)baseTile.X + dx, (int)baseTile.Y - 2));
                    }
                    break;

                case Direction.Down:
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        affectedTiles.Add(new Point((int)baseTile.X + dx, (int)baseTile.Y + 1));
                        affectedTiles.Add(new Point((int)baseTile.X + dx, (int)baseTile.Y + 2));
                    }
                    break;

                case Direction.Left:
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        affectedTiles.Add(new Point((int)baseTile.X - 1, (int)baseTile.Y + dy));
                        affectedTiles.Add(new Point((int)baseTile.X - 2, (int)baseTile.Y + dy));
                    }
                    break;

                case Direction.Right:
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        affectedTiles.Add(new Point((int)baseTile.X + 1, (int)baseTile.Y + dy));
                        affectedTiles.Add(new Point((int)baseTile.X + 2, (int)baseTile.Y + dy));
                    }
                    break;
            }

            foreach (Point tile in affectedTiles)
            {
                CurrentLocation.InteractWithTile(tile.X, tile.Y, inventory, inventoryHotbar.CurrentSlotIndex);
            }
        }
    }
}