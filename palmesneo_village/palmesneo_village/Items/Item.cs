namespace palmesneo_village
{
    public class Item
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public bool IsStackable { get; init; }
        public bool IsTradable { get; init; }
        public bool IsDroppable { get; init; }
        public int Price { get; init; }

        public MTexture Icon { get; protected set; }

        public virtual void Initialize(MTileset sourceTileset) 
        {
            Icon = sourceTileset[Id];
        }

        public virtual string GetTooltipInfo()
        {
            string tooltip = $"{LocalizationManager.GetText(Name)}";

            if (string.IsNullOrEmpty(Description) == false)
            {
                tooltip += $"\n/c[{ColorUtils.GRAY_HEX}]{LocalizationManager.GetText(Description)}/cd";
            }

            if (Price > 0)
            {
                tooltip += $"\n/c[{ColorUtils.YELLOW_HEX}]{LocalizationManager.GetText("price")}: {Price}/cd";
            }

            return tooltip;
        }

    }
}
