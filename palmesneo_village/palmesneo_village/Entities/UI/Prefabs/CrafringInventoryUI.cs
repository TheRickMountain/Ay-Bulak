using Microsoft.Xna.Framework;
using System.Collections;
using System.Collections.Generic;

namespace palmesneo_village
{
    public class CrafringInventoryUI : EntityUI
    {
        private Inventory inventory;

        private DraggableInventoryUI inventoryUI;
        private CrafterUI crafterUI;

        public CrafringInventoryUI(Inventory inventory, Player player)
        {
            this.inventory = inventory;

            crafterUI = new CrafterUI(inventory);
            crafterUI.Anchor = Anchor.LeftCenter;
            AddChild(crafterUI);

            inventoryUI = new DraggableInventoryUI(player);
            inventoryUI.Anchor = Anchor.RightCenter;
            AddChild(inventoryUI);
        }

        public void Open(IEnumerable<CraftingRecipe> craftingRecipes)
        {
            inventoryUI.Open(inventory);
            crafterUI.Open(craftingRecipes);

            Size = crafterUI.Size + inventoryUI.Size + new Vector2(5);
        }

        public void Close()
        {
            inventoryUI.Close();
        }

        public bool IsItemGrabbed() => inventoryUI.IsItemGrabbed();

    }
}
