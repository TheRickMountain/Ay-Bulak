namespace palmesneo_village
{
    public enum ItemCategory
    {
        Resources,
        Food,
        Seeds,
        Tools,
        Outfits,
        Medicines
    }

    public class ItemCategoriesData
    {

        public static string GetName(ItemCategory itemCategory)
        {
            switch (itemCategory)
            {
                case ItemCategory.Resources: return LocalizationManager.GetText("resources");
                case ItemCategory.Food: return LocalizationManager.GetText("food");
                case ItemCategory.Seeds: return LocalizationManager.GetText("seeds");
                case ItemCategory.Tools: return LocalizationManager.GetText("tools");
                case ItemCategory.Outfits: return LocalizationManager.GetText("outfits");
                case ItemCategory.Medicines: return LocalizationManager.GetText("medicines");
                default: return $"enum.{itemCategory}";
            }
        }

    }
}
