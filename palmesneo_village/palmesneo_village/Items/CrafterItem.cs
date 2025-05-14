namespace palmesneo_village
{
    public class CrafterItem : BuildingItem
    {
        public JsonCraftingRecipe[] CraftingRecipes { get; init; }

        public override void Initialize(MTileset sourceTileset)
        {
            base.Initialize(sourceTileset);
        }
    }
}
