using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class WorldInteractionManager : Entity
    {
        private Tilemap tilemap;

        private TileSelector tileSelector;

        private Vector2 mouseTile;

        public WorldInteractionManager(Tilemap tilemap)
        {
            this.tilemap = tilemap;

            tileSelector = new TileSelector();
            AddChild(tileSelector);
        }

        public override void Update()
        {
            base.Update();

            mouseTile = tilemap.WorldToMap(MInput.Mouse.GlobalPosition);

            tileSelector.LocalPosition = tilemap.MapToWorld(mouseTile);

            // TEMP: test
            if(MInput.Mouse.PressedLeftButton)
            {
                tilemap.SetCell((int)mouseTile.X, (int)mouseTile.Y, 1);
            }
            // TEMP: test
        }

    }
}
