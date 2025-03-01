using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class DevScene : Scene
    {

        private Player player;

        public DevScene()
        {
            
        }

        public override void Begin()
        {
            MTileset tileset = new MTileset(ResourcesManager.GetTexture("Tilesets", "ground_summer_tileset"), 16, 16);

            Tilemap tilemap = new Tilemap(TilesetConnection.SidesAndCorners, 16, 16, 64, 64);
            tilemap.Tileset = tileset;

            tilemap.SetCell(1, 1, 0);

            MasterEntity.AddChild(tilemap);

            player = new Player(new CreatureTemplate("Player", RenderManager.Pixel, 100f));
            MasterEntity.AddChild(player);

            CameraMovement cameraMovement = new CameraMovement();
            cameraMovement.Target = player;
            MasterEntity.AddChild(cameraMovement);

            base.Begin();
        }

        public override void Update()
        {
            base.Update();
        }

    }
}
