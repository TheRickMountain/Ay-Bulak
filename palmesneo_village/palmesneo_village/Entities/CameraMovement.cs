using FontStashSharp;
using Microsoft.Xna.Framework;
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
            Engine.Camera.Zoom = new Vector2(2, 2);

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
                UpdateBounds(Engine.ViewportWidth, Engine.ViewportHeight);

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

        private void UpdateBounds(int screenWidth, int screenHeight)
        {
            float zoom = Engine.Camera.Zoom.X;

            int visibleWidthInWorld = (int)(screenWidth / zoom);
            int visibleHeightInWorld = (int)(screenHeight / zoom);

            int halfVisibleWidth = visibleWidthInWorld / 2;
            int halfVisibleHeight = visibleHeightInWorld / 2;

            realBounds = new Rectangle(
                Bounds.X + halfVisibleWidth,
                Bounds.Y + halfVisibleHeight,
                Bounds.Width - halfVisibleWidth,
                Bounds.Height - halfVisibleHeight
            );
        }
    }
}
