namespace palmesneo_village
{
    public class Item
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public bool IsStackable { get; init; }
        public int Price { get; init; }

        public MTexture Icon { get; protected set; }

        public virtual void Initialize(MTileset sourceTileset) 
        {
            Icon = sourceTileset[Id];
        }

    }
}
