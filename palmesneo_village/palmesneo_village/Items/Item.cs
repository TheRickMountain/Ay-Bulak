namespace palmesneo_village
{
    public class Item
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public ItemCategory Category { get; init; }
        public bool IsStackable { get; init; }

        public MTexture Icon { get; private set; }

        public void Initialize(MTileset sourceTileset) 
        {
            Icon = sourceTileset[Id];
        }

    }
}
