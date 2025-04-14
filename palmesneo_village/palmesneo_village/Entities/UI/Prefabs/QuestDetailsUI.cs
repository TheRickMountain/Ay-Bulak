using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class QuestDetailsUI : PanelUI
    {

        public Action<ButtonUI> BackButtonPressed { get; set; }
        public Action<Quest> QuestCompleted { get; set; }

        private PlayerMoneyManager playerMoneyManager;

        private Quest quest;

        private TextUI descriptionText;

        private TextUI rewardTitleText;

        private TextButtonUI backButton;
        private TextButtonUI completeQuestButton;

        public QuestDetailsUI(PlayerMoneyManager playerMoneyManager)
        {
            this.playerMoneyManager = playerMoneyManager;

            descriptionText = new TextUI();
            descriptionText.LocalPosition = new Vector2(5, 5);
            AddChild(descriptionText);

            rewardTitleText = new TextUI();
            rewardTitleText.LocalPosition = new Vector2(0, 20);
            rewardTitleText.Text = "Rewards"; // TODO: localization
            rewardTitleText.SelfColor = Color.Gold;
            rewardTitleText.Anchor = Anchor.TopCenter;
            AddChild(rewardTitleText);

            // [BUTTON] Complete quest
            completeQuestButton = new TextButtonUI();
            completeQuestButton.Anchor = Anchor.BottomRight;
            completeQuestButton.LocalPosition = new Vector2(-5, -5);
            completeQuestButton.ActionTriggered += OnCompleteQuestButtonPressed;
            completeQuestButton.AddChild(new TextUI()
            {
                Text = "Complete Quest", // TODO: localization
                Anchor = Anchor.Center
            });
            AddChild(completeQuestButton);
            completeQuestButton.IsVisible = false;
            completeQuestButton.IsDisabled = true;

            // [BUTTON] back
            backButton = new TextButtonUI();
            backButton.Anchor = Anchor.BottomLeft;
            backButton.LocalPosition = new Vector2(5, -5);
            backButton.ActionTriggered += OnBackButtonPressed;
            backButton.AddChild(new TextUI()
            {
                Text = "Back", // TODO: localization
                Anchor = Anchor.Center
            });
            AddChild(backButton);
        }

        public void SetQuest(Quest quest)
        {
            this.quest = quest;

            descriptionText.Text = quest.Description;

            if(quest.QuestState == QuestState.COMPLETED)
            {
                completeQuestButton.IsVisible = true;
                completeQuestButton.IsDisabled = false;
            }
            else
            {
                completeQuestButton.IsVisible = false;
                completeQuestButton.IsDisabled = true;
            }
        }

        private void OnCompleteQuestButtonPressed(ButtonUI button)
        {
            playerMoneyManager.MoneyAmount += quest.RewardMoney;

            completeQuestButton.IsVisible = false;
            completeQuestButton.IsDisabled = true;

            QuestCompleted?.Invoke(quest);
        }

        private void OnBackButtonPressed(ButtonUI button)
        {
            BackButtonPressed?.Invoke(button);
        }

    }
}
