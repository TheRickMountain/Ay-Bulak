using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class Teleport
    {

        public string Location { get; private set; }
        public Vector2 Tile { get; private set; }

        public Teleport(string location, Vector2 tile)
        {
            Location = location;
            Tile = tile;
        }
    }
}
