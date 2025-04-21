using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class AnimalFeederBuilding : Building
    {
        public bool IsFull { get; private set; } = false;

        private AnimalFeederItem animalFeederItem;

        public AnimalFeederBuilding(GameLocation gameLocation, AnimalFeederItem item, Direction direction, Vector2[,] occupiedTiles) 
            : base(gameLocation, item, direction, occupiedTiles)
        {
            animalFeederItem = item;
        }

        public override void InteractAlternatively(Item item, PlayerEnergyManager playerEnergyManager)
        {
            if(item.Name == "hay")
            {
                if (IsFull)
                {
                    return;
                }

                IsFull = true;
                Sprite.Texture = animalFeederItem.FullTexture;
            }
        }

        public void Empty()
        {
            IsFull = false;
            Sprite.Texture = animalFeederItem.EmptyTexture;
        }

    }
}
