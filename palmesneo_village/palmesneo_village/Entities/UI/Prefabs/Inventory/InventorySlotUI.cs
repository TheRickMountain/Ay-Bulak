using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class InventorySlotUI : ImageUI
    {

        public InventorySlotUI()
        {
            Texture = RenderManager.Pixel;
            Size = new Vector2(32, 32);
        }

    }
}
