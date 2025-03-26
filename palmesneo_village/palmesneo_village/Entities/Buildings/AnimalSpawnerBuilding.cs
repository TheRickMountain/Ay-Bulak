using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class AnimalSpawnerBuilding : Building
    {
        private bool animalSpawned = false;

        public AnimalSpawnerBuilding(GameLocation gameLocation, AnimalSpawnerItem item, Direction direction, Vector2[,] occupiedTiles) 
            : base(gameLocation, item, direction, occupiedTiles)
        {
            
        }

        public override void Update()
        {
            if (animalSpawned == false)
            {
                MTexture texture = ResourcesManager.GetTexture("Sprites", "hen");

                Animal animal = new Animal("chicken", texture, 3.0f);

                GameLocation.AddAnimal(animal);

                animal.SetTilePosition(OccupiedTiles[0, 0]);

                GameLocation.RemoveBuilding(this);

                animalSpawned = true;
            }

            base.Update();
        }

    }
}
