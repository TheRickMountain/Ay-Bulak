using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace palmesneo_village
{
    public class InventoryUI : BaseInventoryUI
    {
        private Player player;

        private ItemContainer grabbedItemContainer;

        private EntityUI grabbedItemContainerVisualiser;

        public InventoryUI(Inventory inventory, Player player) : base(inventory)
        {
            this.player = player;

            grabbedItemContainer = new ItemContainer();

            grabbedItemContainerVisualiser = new EntityUI();
            grabbedItemContainerVisualiser.Size = new Vector2(16, 16);
            grabbedItemContainerVisualiser.AddChild(new ImageUI() 
            {
                Name = "Icon",
                Size = new Vector2(16, 16)
            });
            grabbedItemContainerVisualiser.AddChild(new TextUI()
            {
                Name = "Quantity",
                Anchor = Anchor.BottomRight,
                LocalPosition = new Vector2(4, 4)
            });
            grabbedItemContainerVisualiser.LocalPosition = new Vector2(10, 10);
        }

        public override void Update()
        {
            base.Update();

            if (Contains(MInput.Mouse.UIPosition))
            {
                Engine.IsMouseOnUI = true;
            }
            else
            {
                if(grabbedItemContainer.Item != null && MInput.Mouse.PressedLeftButton)
                {
                    ItemContainer itemContainer = new ItemContainer();
                    itemContainer.Item = grabbedItemContainer.Item;
                    itemContainer.Quantity = grabbedItemContainer.Quantity;
                    itemContainer.ContentAmount = grabbedItemContainer.ContentAmount;

                    player.CurrentLocation.AddItem(player.LocalPosition, itemContainer);

                    ClearGrabbedItem();
                }
            }
        }

        public bool IsItemGrabbed() => grabbedItemContainer.Item != null;

        protected override void OnInventorySlotPressed(ButtonUI button, int slotIndex)
        {
            // Кладем предмет в пустой слот
            if(Inventory.IsSlotEmpty(slotIndex))
            {
                if (grabbedItemContainer.Item != null)
                {
                    Inventory.AddItem(
                        grabbedItemContainer.Item,
                        grabbedItemContainer.Quantity,
                        grabbedItemContainer.ContentAmount,
                        slotIndex);

                    ClearGrabbedItem();
                }
            }
            else
            {
                if (grabbedItemContainer.Item != null) // Свапаем предметы
                {
                    Item tempItem = Inventory.GetSlotItem(slotIndex);
                    int tempQuantity = Inventory.GetSlotQuantity(slotIndex);
                    int tempContentAmount = Inventory.GetSlotContentAmount(slotIndex);

                    // Объединяем стакаемые предметы
                    if (tempItem == grabbedItemContainer.Item && tempItem.IsStackable)
                    {
                        Inventory.AddItem(
                            grabbedItemContainer.Item, 
                            grabbedItemContainer.Quantity,
                            grabbedItemContainer.ContentAmount, 
                            slotIndex);

                        ClearGrabbedItem();
                    }
                    else
                    {
                        Inventory.RemoveItem(tempItem, tempQuantity, slotIndex);

                        Inventory.AddItem(
                            grabbedItemContainer.Item,
                            grabbedItemContainer.Quantity,
                            grabbedItemContainer.ContentAmount,
                            slotIndex);

                        GrabItem(tempItem, tempQuantity, tempContentAmount);
                    }
                }
                else // Берем предмет из слота
                {
                    GrabItem(
                        Inventory.GetSlotItem(slotIndex),
                        Inventory.GetSlotQuantity(slotIndex),
                        Inventory.GetSlotContentAmount(slotIndex));

                    Inventory.RemoveItem(grabbedItemContainer.Item, grabbedItemContainer.Quantity, slotIndex);
                }
            }
        }

        private void ClearGrabbedItem()
        {
            grabbedItemContainer.Clear();
            Parent.Parent.GetChildByName<ImageUI>("Cursor").RemoveChild(grabbedItemContainerVisualiser);
        }

        private void GrabItem(Item item, int quantity, int contentAmount)
        {
            grabbedItemContainer.Item = item;
            grabbedItemContainer.Quantity = quantity;
            grabbedItemContainer.ContentAmount = contentAmount;

            grabbedItemContainerVisualiser.GetChildByName<ImageUI>("Icon").Texture = item.Icon;
            grabbedItemContainerVisualiser.GetChildByName<TextUI>("Quantity").Text = quantity.ToString();

            if (Parent.Parent.GetChildByName<ImageUI>("Cursor").ContainsChild(grabbedItemContainerVisualiser) == false)
            {
                Parent.Parent.GetChildByName<ImageUI>("Cursor").AddChild(grabbedItemContainerVisualiser);
            }
        }
    }
}
