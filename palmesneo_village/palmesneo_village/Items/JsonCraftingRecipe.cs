using System.Collections.Generic;

namespace palmesneo_village
{
    public struct JsonIngredient
    {
        public string Item { get; set; }
        public int Amount { get; set; }
    }

    public class JsonCraftingRecipe
    {
        public JsonIngredient Result { get; init; }

        public List<JsonIngredient> RequiredIngredients { get; init; }

    }
}
