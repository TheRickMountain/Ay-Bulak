using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public class QuestManager
    {

        private List<Quest> activeQuests = new();

        public QuestManager()
        {
            CreateTestQuests();
        }

        private void CreateTestQuests()
        {
            Quest quest = new Quest();
            quest.QuestType = QuestType.HARVEST;
            quest.Name = "First steps";
            quest.Description = "Plant and harvest your first plant";
            quest.HarvestPlantName = "tomato_plant";
            quest.QuestState = QuestState.NEW;
            activeQuests.Add(quest);

        }

        public IEnumerable<Quest> GetQuests()
        {
            foreach (var quest in activeQuests)
            {
                yield return quest;
            }
        }

        public void OnHarvestPlant(string plantName)
        {
            foreach(var quest in activeQuests)
            {
                if(quest.QuestState == QuestState.COMPLETED)
                {
                    continue;
                }

                switch (quest.QuestType)
                {
                    case QuestType.HARVEST:
                        {
                            if (quest.HarvestPlantName == plantName)
                            {
                                quest.QuestState = QuestState.COMPLETED;

                                ResourcesManager.GetSoundEffect("SoundEffects", "quest_completed").Play();
                            }
                        }
                        break;
                }
            }
        }

        public bool HasNewOrCompletedQuests()
        {
            foreach (var quest in activeQuests)
            {
                if (quest.QuestState == QuestState.NEW || quest.QuestState == QuestState.COMPLETED)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
