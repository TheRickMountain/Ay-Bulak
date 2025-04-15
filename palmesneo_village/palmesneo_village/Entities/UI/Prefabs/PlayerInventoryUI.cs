using Microsoft.Xna.Framework;
using System.Collections;
using System.Collections.Generic;

namespace palmesneo_village
{
    public class PlayerInventoryUI : EntityUI
    {

        private InventoryUI inventoryUI;
        private CrafterUI crafterUI;

        public PlayerInventoryUI(Inventory inventory, Player player)
        {
            crafterUI = new CrafterUI(inventory);
            crafterUI.Anchor = Anchor.LeftCenter;
            AddChild(crafterUI);

            inventoryUI = new InventoryUI(inventory, player);
            inventoryUI.Anchor = Anchor.RightCenter;
            AddChild(inventoryUI);

            Size = crafterUI.Size + inventoryUI.Size + new Vector2(5);
        }

        public void Open(IEnumerable<CraftingRecipe> craftingRecipes)
        {
            crafterUI.Open(craftingRecipes);
        }

        public bool IsItemGrabbed() => inventoryUI.IsItemGrabbed();

    }
}
