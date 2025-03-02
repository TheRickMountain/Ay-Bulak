using Microsoft.Xna.Framework;
using System;

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

            for (int x = 0; x < 64; x++)
            {
                for (int y = 0; y < 64; y++)
                {
                    tilemap.SetCell(x, y, 0);
                }
            }

            MasterEntity.AddChild(tilemap);

            TilemapsManager tilemapsManager = new TilemapsManager(tilemap);
            MasterEntity.AddChild(tilemapsManager);

            WorldInteractionManager worldInteractionManager = new WorldInteractionManager(tilemap);
            MasterEntity.AddChild(worldInteractionManager);

            player = new Player(new CreatureTemplate("Player", ResourcesManager.GetTexture("Sprites", "player"), 100f));
            MasterEntity.AddChild(player);

            int tilemapWidth = tilemap.TileColumns * Engine.TILE_SIZE;
            int tilemapHeight = tilemap.TileRows * Engine.TILE_SIZE;

            CameraMovement cameraMovement = new CameraMovement();
            cameraMovement.Target = player;
            cameraMovement.Bounds = new Rectangle(0, 0, tilemapWidth, tilemapHeight);
            MasterEntity.AddChild(cameraMovement);

            base.Begin();
        }

        public override void Update()
        {
            base.Update();
        }

    }
}
