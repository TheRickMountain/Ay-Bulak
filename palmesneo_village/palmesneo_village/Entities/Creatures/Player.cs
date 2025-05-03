using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace palmesneo_village
{
    public class Player : Creature
    {
        private Inventory inventory;

        private const float COLLISION_CHECK_OFFSET = 4f;

        private const float ITEM_PICKUP_SPEED = 300f;
        private const float ITEM_ATTRACTION_DISTANCE = 48f;
        private const float ITEM_PICKUP_DISTANCE = 5f;

        private SpriteEntity bodySprite;

        private Direction movementDirection = Direction.Down;

        private bool isMoving = false;

        private SoundEffectInstance currentSFX;

        private List<SoundEffectInstance> grassShakeSFXs = new();

        public Player(string name, MTexture texture, float speed, Inventory inventory) : 
            base(name, texture, speed)
        {
            this.inventory = inventory;

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
        }

        private void CreateAndInitializeBodySprite(MTexture spritesheet)
        {
            int framesColumns = 4;
            int framesRows = 4;

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

            bodySprite.LocalPosition = new Vector2(-frameWidth / 2, -(frameHeight - (Engine.TILE_SIZE / 2)));

            bodySprite.Play("idle_down");
        }

        public override void Update()
        {
            base.Update();

            UpdateMovement();

            UpdateItemsPickup();

            ShakeGrass();

            if(currentSFX != null && currentSFX.State == SoundState.Stopped)
            {
                currentSFX = null;
            }
        }

        protected void UpdateMovement()
        {
            Vector2 movement = new Vector2(InputBindings.MoveHorizontally.Value, InputBindings.MoveVertically.Value);
            
            isMoving = movement != Vector2.Zero;

            if (movement != Vector2.Zero)
            {
                movement.Normalize();

                movementDirection = Calc.GetDirection(movement);

                float actualSpeed = Speed;

                FloorPathItem floorPathItem = CurrentLocation.GetTileFloorPathItem(CurrentLocation.WorldToMap(LocalPosition));
                if (floorPathItem != null)
                {
                    actualSpeed = Speed + (Speed * floorPathItem.MovementSpeedBuff);
                }

                Vector2 newPosition = LocalPosition + movement * actualSpeed * Engine.GameDeltaTime;

                // Проверяем коллизии перед перемещением
                if (IsValidMovement(newPosition))
                {
                    LocalPosition = newPosition;
                }
                else
                {
                    // Попробуем двигаться только по X или только по Y
                    Vector2 testX = LocalPosition + new Vector2(movement.X * actualSpeed * Engine.GameDeltaTime, 0);
                    Vector2 testY = LocalPosition + new Vector2(0, movement.Y * actualSpeed * Engine.GameDeltaTime);

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

            if(movement != Vector2.Zero)
            {
                bodySprite.Play($"walk_{movementDirection.ToString().ToLower()}");
            }
            else
            {
                bodySprite.Play($"idle_{movementDirection.ToString().ToLower()}");
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

        private void ShakeGrass()
        {
            if (isMoving == false) return;
        
            Vector2 playerTile = CurrentLocation.WorldToMap(LocalPosition);

            Building building = CurrentLocation.GetBuilding((int)playerTile.X, (int)playerTile.Y);

            if(building != null && building is GrassBuilding grassBuilding)
            {
                grassBuilding.Shake(movementDirection);

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
    }
}