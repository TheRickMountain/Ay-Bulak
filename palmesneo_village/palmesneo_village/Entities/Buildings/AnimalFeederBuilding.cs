using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class AnimalFeederBuilding : Building
    {
        private AnimalFeederItem animalFeederItem;

        private bool isFull = false;

        public AnimalFeederBuilding(GameLocation gameLocation, AnimalFeederItem item, Direction direction, Vector2[,] occupiedTiles) 
            : base(gameLocation, item, direction, occupiedTiles)
        {
            animalFeederItem = item;
        }

        public override void InteractAlternatively(Item item, PlayerEnergyManager playerEnergyManager)
        {
            if(item.Name == "hay")
            {
                if (isFull)
                {
                    return;
                }

                isFull = true;
                Sprite.Texture = animalFeederItem.FullTexture;
            }
        }

        public void Empty()
        {
            isFull = false;
            Sprite.Texture = animalFeederItem.EmptyTexture;
        }

    }
}
