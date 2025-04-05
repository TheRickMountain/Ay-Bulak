using FontStashSharp;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
namespace palmesneo_village
{
    public class CameraMovement : Entity
    {
        public Entity Target { get; set; }
        public float SmoothFactor { get; set; } = 10f;

        private Rectangle bounds = Rectangle.Empty;
        public Rectangle Bounds 
        {
            get => bounds;
            set
            {
                bounds = value;

                isDirty = true;
            }
        }

        private Rectangle realBounds = Rectangle.Empty;

        private bool isDirty = true;

        public CameraMovement()
        {
            Engine.Camera.Zoom = new Vector2(3, 3);

            // DANGER: При попытке начать новую игру, не выходя из игры, эта привязка останется
            Engine.Instance.ScreenSizeChanged += (x, y) =>
            {
                isDirty = true;
            };
        }

        public override void Update()
        {
            base.Update();

            if(isDirty)
            {
                UpdateBounds();

                isDirty = false;
            }

            Vector2 targetPosition = Target != null ? Target.LocalPosition : Vector2.Zero;

            if (Bounds != Rectangle.Empty)
            {
                targetPosition = Vector2.Clamp(targetPosition,
                    new Vector2(realBounds.X, realBounds.Y),
                    new Vector2(realBounds.Width, realBounds.Height));
            }

            LocalPosition = Vector2.Lerp(LocalPosition, targetPosition, SmoothFactor * Engine.GameDeltaTime);

            Engine.Camera.Position = LocalPosition;
        }

        public Vector2 GetViewportZoomedSize()
        {
            float zoom = Engine.Camera.Zoom.X;

            Vector2 viewportSize = new Vector2(Engine.ViewportWidth, Engine.ViewportHeight);

            return viewportSize / zoom;
        }

        private void UpdateBounds()
        {
            Vector2 viewportZoomedSize = GetViewportZoomedSize();

            int halfVisibleWidth = (int)viewportZoomedSize.X / 2;
            int halfVisibleHeight = (int)viewportZoomedSize.Y / 2;

            bool centerInX = Bounds.Width < (int)viewportZoomedSize.X;
            bool centerInY = Bounds.Height < (int)viewportZoomedSize.Y;

            if(centerInX)
            {
                halfVisibleWidth = Bounds.Width / 2;
            }

            if (centerInY)
            {
                halfVisibleHeight = Bounds.Height / 2;
            }

            realBounds = new Rectangle(
                Bounds.X + halfVisibleWidth,
                Bounds.Y + halfVisibleHeight,
                Bounds.Width - halfVisibleWidth,
                Bounds.Height - halfVisibleHeight
            );
        }
    }
}
