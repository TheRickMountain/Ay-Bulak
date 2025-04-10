using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace palmesneo_village
{
    public class ActiveQuestsUI : PanelUI
    {
        private QuestManager questManager;

        private VerticalScrollBarUI scrollBarUI;

        private VerticalContainerUI containerUI;

        private const int SLOTS_AMOUNT = 5;

        private List<QuestButtonUI> slotsList = new List<QuestButtonUI>();

        private List<Quest> questsList = new List<Quest>();

        public Action<Quest> QuestSelected { get; set; }

        public ActiveQuestsUI(QuestManager questManager)
        {
            this.questManager = questManager;

            NineSlicedImageUI buttonTextureMaker = new NineSlicedImageUI();
            buttonTextureMaker.Texture = ResourcesManager.GetTexture("Sprites", "UI", "button");
            buttonTextureMaker.SliceCenter = new Rectangle(4, 4, 8, 8);
            buttonTextureMaker.Size = new Vector2(256, 24);

            scrollBarUI = new VerticalScrollBarUI();
            scrollBarUI.GrabberPositionChanged += OnScrollBarGrabberPositionChanged;
            scrollBarUI.Anchor = Anchor.RightCenter;
            scrollBarUI.MinValue = 0;
            scrollBarUI.LocalPosition = new Vector2(-8, 0);
            AddChild(scrollBarUI);

            containerUI = new VerticalContainerUI();
            containerUI.LocalPosition = new Vector2(8, 8);
            AddChild(containerUI);

            for (int i = 0; i < SLOTS_AMOUNT; i++)
            {
                QuestButtonUI questButton = new QuestButtonUI(buttonTextureMaker.FinalTexture);
                questButton.ActionTriggered += OnQuestButtonPressed;
                containerUI.AddChild(questButton);
                slotsList.Add(questButton);
            }

            scrollBarUI.Size = new Vector2(15, containerUI.Size.Y);

            Size = new Vector2(buttonTextureMaker.Size.X + 5 + scrollBarUI.Size.X + 16, containerUI.Size.Y + 16);
        }

        public void Open()
        {
            questsList.Clear();

            questsList.AddRange(questManager.GetQuests());

            if (questsList.Count < slotsList.Count)
            {
                scrollBarUI.MaxValue = 0;

                for (int i = 0; i < questsList.Count; i++)
                {
                    slotsList[i].SetQuest(questsList[i]);
                }
            }
            else
            {
                scrollBarUI.MaxValue = questsList.Count - slotsList.Count;

                for (int i = 0; i < slotsList.Count; i++)
                {
                    slotsList[i].SetQuest(questsList[i]);
                }
            }

            int scrollBarMaxValue = questsList.Count - SLOTS_AMOUNT;

            if (scrollBarMaxValue < 0)
            {
                scrollBarUI.MaxValue = 0;
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
            foreach(var slot in slotsList)
            {
                slot.Clear();
                slot.IsVisible = false;
                slot.IsDisabled = true;
            }

            int firstItemIndex = scrollBarUI.CurrentValue;
            int lastItemIndex = scrollBarUI.CurrentValue + slotsList.Count - 1;

            if (questsList.Count < slotsList.Count)
            {
                for (int i = 0; i < questsList.Count; i++)
                {
                    slotsList[i].IsVisible = true;
                    slotsList[i].IsDisabled = false;
                    slotsList[i].SetQuest(questsList[i]);
                }
            }
            else
            {
                int slotIndex = 0;

                for (int itemIndex = firstItemIndex; itemIndex <= lastItemIndex; itemIndex++)
                {
                    slotsList[slotIndex].IsVisible = true;
                    slotsList[slotIndex].IsDisabled = false;
                    slotsList[slotIndex].SetQuest(questsList[itemIndex]);

                    slotIndex++;
                }
            }
        }

        private void OnQuestButtonPressed(ButtonUI buttonUI)
        {
            QuestSelected?.Invoke((buttonUI as QuestButtonUI).Quest);
        }

    }
}
