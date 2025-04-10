using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class QuestsUI : HorizontalContainerUI
    {

        private ActiveQuestsUI activeQuestsUI;
        private QuestDetailsUI questDetailsUI;

        public QuestsUI(QuestManager questManager)
        {
            activeQuestsUI = new ActiveQuestsUI(questManager);
            activeQuestsUI.QuestSelected += OnActiveQuestSelected;
            AddChild(activeQuestsUI);

            questDetailsUI = new QuestDetailsUI();
            questDetailsUI.Size = activeQuestsUI.Size;
            AddChild(questDetailsUI);

            Size = new Vector2(Size.X, activeQuestsUI.Size.Y);
        }

        public void Open()
        {
            activeQuestsUI.Open();
            questDetailsUI.Clear();
        }

        private void OnActiveQuestSelected(Quest quest)
        {
            questDetailsUI.SetQuest(quest);
        }

    }
}
