using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace palmesneo_village
{
    public class InventoryUI : PanelUI
    {
        private Inventory inventory;
        private Player player;

        private GridContainerUI grid;

        private List<SlotButtonUI> inventorySlots;

        private ItemContainer grabbedItemContainer;

        private EntityUI grabbedItemContainerVisualiser;

        public InventoryUI(Inventory inventory, Player player)
        {
            this.inventory = inventory;
            this.player = player;

            inventory.SlotDataChanged += (inventory, slotIndex) => RefreshInventory();

            grabbedItemContainer = new ItemContainer();

            grid = new GridContainerUI();
            grid.Anchor = Anchor.Center;
            grid.Columns = inventory.Width;
            AddChild(grid);

            inventorySlots = new List<SlotButtonUI>(inventory.Width * inventory.Height);

            for (int i = 0; i < inventory.Width * inventory.Height; i++)
            {
                SlotButtonUI inventorySlotUI = new SlotButtonUI();

                int slotIndex = i;

                inventorySlotUI.ActionTriggered += (button) => OnInventorySlotPressed(button, slotIndex);

                inventorySlots.Add(inventorySlotUI);

                grid.AddChild(inventorySlotUI);

            }

            Size = grid.Size + new Vector2(16, 16);

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

        private void OnInventorySlotPressed(ButtonUI button, int slotIndex)
        {
            // Кладем предмет в пустой слот
            if(inventory.IsSlotEmpty(slotIndex))
            {
                if (grabbedItemContainer.Item != null)
                {
                    inventory.AddItem(
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
                    Item tempItem = inventory.GetSlotItem(slotIndex);
                    int tempQuantity = inventory.GetSlotQuantity(slotIndex);
                    int tempContentAmount = inventory.GetSlotContentAmount(slotIndex);

                    // Объединяем стакаемые предметы
                    if (tempItem == grabbedItemContainer.Item && tempItem.IsStackable)
                    {
                        inventory.AddItem(
                            grabbedItemContainer.Item, 
                            grabbedItemContainer.Quantity,
                            grabbedItemContainer.ContentAmount, 
                            slotIndex);

                        ClearGrabbedItem();
                    }
                    else
                    {
                        inventory.RemoveItem(tempItem, tempQuantity, slotIndex);

                        inventory.AddItem(
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
                        inventory.GetSlotItem(slotIndex),
                        inventory.GetSlotQuantity(slotIndex),
                        inventory.GetSlotContentAmount(slotIndex));

                    inventory.RemoveItem(grabbedItemContainer.Item, grabbedItemContainer.Quantity, slotIndex);
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
    
        private void RefreshInventory()
        {
            for (int slotIndex = 0; slotIndex < inventory.Width * inventory.Height; slotIndex++)
            {
                Item item = inventory.GetSlotItem(slotIndex);
                int quantity = inventory.GetSlotQuantity(slotIndex);
                int contentAmount = inventory.GetSlotContentAmount(slotIndex);

                if (item == null)
                {
                    inventorySlots[slotIndex].Clear();
                }
                else
                {
                    inventorySlots[slotIndex].SetItem(item, quantity, contentAmount);
                }
            }
        }
    }
}
