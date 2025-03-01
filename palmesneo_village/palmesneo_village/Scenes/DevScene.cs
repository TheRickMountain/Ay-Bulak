using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class DevScene : Scene
    {

        public DevScene()
        {
            
        }

        public override void Begin()
        {
            base.Begin();

            MTileset tileset = new MTileset(ResourcesManager.GetTexture("Tilesets", "ground_summer_tileset"), 16, 16);

            Tilemap tilemap = new Tilemap(TilesetConnection.SidesAndCorners, 16, 16, 64, 64);
            tilemap.Tileset = tileset;

            tilemap.SetCell(1, 1, 0);

            MasterEntity.AddChild(tilemap);
        }

        public override void Update()
        {
            base.Update();
        }

    }
}
