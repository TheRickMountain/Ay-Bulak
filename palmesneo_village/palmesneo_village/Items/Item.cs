namespace palmesneo_village
{
    public class ItemReader
    {
    }

    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ItemCategory Category { get; set; }
        public bool IsStackable { get; set; }

        public MTexture Icon { get; private set; }

        public void Initialize(MTileset sourceTileset) 
        {
            Icon = sourceTileset[Id];
        }

    }
}
