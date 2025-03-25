using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace palmesneo_village
{
    public class Player : Creature
    {
        private Inventory inventory;

        private const float COLLISION_CHECK_OFFSET = 4f;

        public Player(string name, MTexture texture, float speed, Inventory inventory) : 
            base(name, texture, speed)
        {
            this.inventory = inventory;
        }

        public override void Update()
        {
            base.Update();

            UpdateMovement();

            CheckForItemPickup();

            Depth = (int)LocalPosition.Y;
        }

        protected override Direction UpdateMovement()
        {
            Vector2 movement = new Vector2(InputBindings.MoveHorizontally.Value, InputBindings.MoveVertically.Value);

            if (movement != Vector2.Zero)
            {
                movement.Normalize();

                float actualSpeed = Speed;

                FloorPathItem floorPathItem = CurrentLocation.GetTileFloorPathItem(CurrentLocation.WorldToMap(LocalPosition));
                if (floorPathItem != null)
                {
                    actualSpeed = Speed + (Speed * floorPathItem.MovementSpeefBuff);
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

            return Direction.Down;
        }

        private bool IsValidMovement(Vector2 newPosition)
        {
            // Преобразуем мировые координаты в координаты тайлов
            Vector2 mapPos = CurrentLocation.WorldToMap(newPosition);

            // Проверяем 4 точки вокруг игрока (предполагаем, что размер игрока примерно равен тайлу)
            Vector2[] checkPoints = new Vector2[]
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

        private void CheckForItemPickup()
        {
            // TODO: check if inventory has enough space for pickup

            foreach (LocationItem locationItem in CurrentLocation.GetLocationItems(GlobalPosition))
            {
                if (locationItem.IsActive && locationItem.CanBePickedUp(GlobalPosition))
                {
                    locationItem.StartPickup(GlobalPosition);
                }
                else if (!locationItem.IsActive)
                {
                    PickupItem(locationItem);

                    CurrentLocation.RemoveItem(locationItem);
                }
            }
        }

        private void PickupItem(LocationItem locationItem)
        {
            // TODO: play pickup sound
            ItemContainer itemContainer = locationItem.ItemContainer;

            inventory.TryAddItem(itemContainer.Item, itemContainer.Quantity, itemContainer.ContentAmount);
        }
    }
}