using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public enum QuestType
    {
        HARVEST
    }

    public enum QuestState
    {
        NONE,
        NEW,
        COMPLETED
    }

    public class Quest
    {

        public string Name { get; set; }
        public string Description { get; set; }
        public QuestType QuestType { get; set; }
        public QuestState QuestState { get; set; } = QuestState.NONE;

        public string HarvestPlantName { get; set; }

        public Quest()
        {
        }
    }
}
