using Microsoft.Xna.Framework;
namespace palmesneo_village
{
    public class CameraMovement : Entity
    {
        public Entity Target { get; set; }

        public CameraMovement()
        {
            Engine.Camera.Zoom = new Vector2(2, 2);
        }

        public override void Update()
        {
            base.Update();

            Vector2 targetPosition = Target != null ? Target.LocalPosition : Vector2.Zero;

            LocalPosition = Vector2.Lerp(LocalPosition, targetPosition, 10 * Engine.GameDeltaTime);

            Engine.Camera.Position = LocalPosition;
        }
    }
}
