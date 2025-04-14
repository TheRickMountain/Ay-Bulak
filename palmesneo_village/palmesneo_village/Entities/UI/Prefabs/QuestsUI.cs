using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class QuestsUI : EntityUI
    {
        private QuestManager questManager;

        private ActiveQuestsUI activeQuestsUI;
        private QuestDetailsUI questDetailsUI;

        public QuestsUI(QuestManager questManager, PlayerMoneyManager playerMoneyManager)
        {
            this.questManager = questManager;

            activeQuestsUI = new ActiveQuestsUI(questManager);
            activeQuestsUI.QuestSelected += OnActiveQuestsUIQuestSelected;
            activeQuestsUI.Anchor = Anchor.Center;
            
            questDetailsUI = new QuestDetailsUI(playerMoneyManager);
            questDetailsUI.Size = activeQuestsUI.Size;
            questDetailsUI.Anchor = Anchor.Center;
            questDetailsUI.BackButtonPressed += OnQuestDetailsUIBackButtonPressed;
            questDetailsUI.QuestCompleted += OnQuestDetailsUIQuestCompleted;

            Size = new Vector2(Size.X, activeQuestsUI.Size.Y);
        }

        public void Open()
        {
            if(activeQuestsUI.Parent != null)
            {
                RemoveChild(activeQuestsUI);
            }

            if(questDetailsUI.Parent != null)
            {
                RemoveChild(questDetailsUI);
            }

            AddChild(activeQuestsUI);
            
            activeQuestsUI.Open();
        }

        private void OnActiveQuestsUIQuestSelected(Quest quest)
        {
            RemoveChild(activeQuestsUI);
            
            AddChild(questDetailsUI);

            questDetailsUI.SetQuest(quest);
        }

        private void OnQuestDetailsUIBackButtonPressed(ButtonUI button)
        {
            RemoveChild(questDetailsUI);

            AddChild(activeQuestsUI);

            activeQuestsUI.Open();
        }

        private void OnQuestDetailsUIQuestCompleted(Quest quest)
        {
            questManager.RemoveQuest(quest);
        }

    }
}
