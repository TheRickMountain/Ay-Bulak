using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace palmesneo_village
{
    public class TraderUI : PanelUI
    {
        private VerticalScrollBarUI scrollBarUI;

        private VerticalContainerUI containerUI;

        private const int SLOTS_AMOUNT = 5;

        private List<TraderItemButtonUI> slotsList = new List<TraderItemButtonUI>();

        private Inventory playerInventory;
        private PlayerMoneyManager playerMoneyManager;

        private List<Item> itemsList = new List<Item>();

        public TraderUI()
        {
            NineSlicedImageUI buttonTextureMaker = new NineSlicedImageUI();
            buttonTextureMaker.Texture = ResourcesManager.GetTexture("Sprites", "UI", "button");
            buttonTextureMaker.SliceCenter = new Rectangle(4, 4, 8, 8);
            buttonTextureMaker.Size = new Vector2(256, 24);

            scrollBarUI = new VerticalScrollBarUI();
            scrollBarUI.GrabberPositionChanged += OnScrollBarGrabberPositionChanged;
            scrollBarUI.Anchor = Anchor.RightCenter;
            scrollBarUI.LocalPosition = new Vector2(-8, 0);
            scrollBarUI.MinValue = 0;
            scrollBarUI.MaxValue = SLOTS_AMOUNT;
            AddChild(scrollBarUI);

            containerUI = new VerticalContainerUI();
            containerUI.LocalPosition = new Vector2(8, 8);
            AddChild(containerUI);

            for (int i = 0; i < SLOTS_AMOUNT; i++)
            {
                TraderItemButtonUI traderItemButton = new TraderItemButtonUI(buttonTextureMaker.FinalTexture);
                traderItemButton.ActionTriggered += OnTraderItemButtonPressed;
                containerUI.AddChild(traderItemButton);
                slotsList.Add(traderItemButton);
            }

            scrollBarUI.Size = new Vector2(15, containerUI.Size.Y);

            Size = new Vector2(buttonTextureMaker.Size.X + 5 + scrollBarUI.Size.X + 16, containerUI.Size.Y + 16);
        }

        public void Open(Inventory playerInventory, PlayerMoneyManager playerMoneyManager, List<Item> items)
        {
            this.playerInventory = playerInventory;
            this.playerMoneyManager = playerMoneyManager;

            itemsList.Clear();

            itemsList.AddRange(items);

            scrollBarUI.MinValue = 0;

            if (itemsList.Count < slotsList.Count)
            {
                scrollBarUI.MaxValue = 0;

                for (int i = 0; i < itemsList.Count; i++)
                {
                    slotsList[i].SetItem(itemsList[i]);
                }
            }
            else
            {
                scrollBarUI.MaxValue = itemsList.Count - slotsList.Count;

                for (int i = 0; i < slotsList.Count; i++)
                {
                    slotsList[i].SetItem(itemsList[i]);
                }
            }

            UpdateElements();
        }

        public override void Update()
        {
            if (Contains(MInput.Mouse.UIPosition))
            {
                int wheelDelta = 0;

                if (MInput.Mouse.WheelDelta > 0)
                {
                    wheelDelta = -1;
                }
                else if (MInput.Mouse.WheelDelta < 0)
                {
                    wheelDelta = 1;
                }

                if (wheelDelta != 0)
                {
                    scrollBarUI.CurrentValue += wheelDelta;

                    UpdateElements();
                }
            }

            base.Update();
        }

        private void OnScrollBarGrabberPositionChanged(ScrollBarUI scrollBarUI)
        {
            UpdateElements();
        }

        private void UpdateElements()
        {
            int firstItemIndex = scrollBarUI.CurrentValue;
            int lastItemIndex = scrollBarUI.CurrentValue + slotsList.Count - 1;

            int slotIndex = 0;

            for (int itemIndex = firstItemIndex; itemIndex <= lastItemIndex; itemIndex++)
            {
                slotsList[slotIndex].SetItem(itemsList[itemIndex]);

                slotIndex++;
            }
        }

        private void OnTraderItemButtonPressed(ButtonUI button)
        {
            // Сначала определяем на сколько товаров хватит денег у игрока
            // Потом проверяем на сколько предметов хватит места в инвентаре у игрока
            
            Item item = (button as TraderItemButtonUI).Item;

            int buyAmount = 1;

            // Только стакаемые предметы можно купить в больших количествах
            if(item.IsStackable)
            {
                buyAmount = GetBuyAmount(Keys.LeftShift, Keys.LeftControl);
            }

            int totalBuyPrice = item.Price * buyAmount;

            if(playerMoneyManager.MoneyAmount < totalBuyPrice)
            {
                // Вычисляем, сколько игрок сможет купить данного товара
                buyAmount = playerMoneyManager.MoneyAmount / item.Price;
            }

            if (buyAmount == 0) return;

            // Можем ли мы добавить предмет в инвентарь игрока
            if(playerInventory.CanAddItem(item, buyAmount))
            {
                playerMoneyManager.MoneyAmount -= item.Price * buyAmount;

                playerInventory.TryAddItem(item, buyAmount, 0);
            }
        }

        private int GetBuyAmount(Keys firstHotkey, Keys secondHotkey)
        {
            if (MInput.Keyboard.Check(firstHotkey))
            {
                if (MInput.Keyboard.Check(secondHotkey))
                {
                    return 25;
                }
                else
                {
                    return 5;
                }
            }

            return 1;
        }
    }
}
