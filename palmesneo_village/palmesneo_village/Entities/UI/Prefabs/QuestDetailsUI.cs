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

        private Quest quest;

        private TextUI descriptionText;

        public QuestDetailsUI()
        {
            descriptionText = new TextUI();
            descriptionText.LocalPosition = new Vector2(5, 5);
            AddChild(descriptionText);
        }

        public void SetQuest(Quest quest)
        {
            this.quest = quest;

            descriptionText.Text = quest.Description;
        }

        public void Clear()
        {
            quest = null;
            descriptionText.Text = string.Empty;
        }

    }
}
