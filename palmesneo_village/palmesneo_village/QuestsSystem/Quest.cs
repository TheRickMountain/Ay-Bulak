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

    public abstract class Quest
    {

        public string Name { get; init; }
        public string Description { get; init; }
        public QuestType Type { get; init; }
    }
}
