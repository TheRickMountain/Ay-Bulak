using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class DevScene : Scene
    {

        private ImageUI testImage;

        public DevScene()
        {
            
        }

        public override void Begin()
        {
            base.Begin();

            testImage = new ImageUI();
            testImage.Texture = ResourcesManager.GetTexture("tileset");
            testImage.LocalScale = new Vector2(2, 4);
            MasterUIEntity.AddChild(testImage);
        }

        public override void Update()
        {
            testImage.LocalRotation += 1.0f * Engine.GameDeltaTime;

            base.Update();
        }

    }
}
