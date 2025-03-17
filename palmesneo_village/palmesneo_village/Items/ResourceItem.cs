namespace palmesneo_village
{
    public class ResourceItem : BuildingItem
    {

        public int Strength { get; init; }
        public string ItemName { get; init; }
        public int ItemAmount { get; init; }
        public ToolType RequiredToolType { get; init; }

    }
}
