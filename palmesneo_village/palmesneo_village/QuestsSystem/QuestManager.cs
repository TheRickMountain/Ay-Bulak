using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public class QuestManager
    {

        private List<Quest> quests = new();

        public QuestManager()
        {
            CreateTestQuest();
        }

        public IEnumerable<Quest> GetQuests()
        {
            return quests;
        }

        // Temp
        private void CreateTestQuest()
        {
            Quest quest = new Quest()
            {
                Name = $"Начало деревенской жизни",
                Description = "Вскопай землю лопатой, засади семена\nНе забывай поливать посадки каждый день, чтобы они выросли вовремя",
                Type = QuestType.HARVEST
            };

            quests.Add(quest);
        }
        // Temp

    }
}
