using Microsoft.Xna.Framework;
using System;

namespace palmesneo_village
{
    public class FlyingItemImage : ImageEntity
    {
        public Item Item { get; private set; }
        private Vector2 startPosition;
        private Vector2 endPosition;
        private Vector2 middlePosition;
        private float duration;
        private float currentTime;
        private bool isCompleted;

        // Визуальные параметры
        private float rotationSpeed = 8f; // радиан в секунду
        private float bounceAmplitude = 0.2f;

        public bool IsCompleted => isCompleted;

        public FlyingItemImage(Item item, Vector2 from, Vector2 to, float duration = 1.0f)
        {
            Item = item;
            this.startPosition = from;
            this.endPosition = to;
            this.duration = duration;
            this.currentTime = 0f;

            // Создаем дугообразную траекторию
            float heightOffset = Math.Max(32f, Vector2.Distance(from, to) * 0.3f);
            middlePosition = new Vector2(
                (from.X + to.X) / 2,
                Math.Min(from.Y, to.Y) - heightOffset
            );

            LocalPosition = startPosition;

            Texture = item.Icon;
            Centered = true;
        }

        public override void Update()
        {
            base.Update();

            if (isCompleted) return;

            currentTime += Engine.GameDeltaTime;
            float t = Math.Min(currentTime / duration, 1f);

            // Вычисляем позицию по кривой Безье
            LocalPosition = Calc.CalculateBezierPoint(t, startPosition, middlePosition, endPosition);

            // Анимация вращения
            LocalRotation += rotationSpeed * Engine.GameDeltaTime;

            // Анимация масштаба (небольшой bounce эффект)
            float bounceT = MathF.Sin(t * MathF.PI);
            LocalScale = Vector2.One * (1f + bounceAmplitude * bounceT);

            // Fade-in эффект в начале
            if (t < 0.1f)
            {
                float fadeT = t / 0.1f;
                SelfColor = Color.White * fadeT;
            }
            else
            {
                SelfColor = Color.White;
            }

            // Проверяем завершение анимации
            if (t >= 1f)
            {
                isCompleted = true;
            }
        }

    }
}
