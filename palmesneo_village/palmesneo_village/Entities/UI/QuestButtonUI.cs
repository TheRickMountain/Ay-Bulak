using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace palmesneo_village
{
    public class QuestButtonUI : ButtonUI
    {

        public Quest Quest { get; private set; }
        private MTexture texture;
        private TextUI questName;
        public QuestButtonUI(MTexture texture)
        {
            this.texture = texture;
            base.Size = this.texture.Size;

            StatesColors[ButtonUIState.Normal] = Color.White;
            StatesColors[ButtonUIState.Hovered] = Color.DarkGray;
            StatesColors[ButtonUIState.Disabled] = Color.White * 0.5f;
            StatesColors[ButtonUIState.Pressed] = Color.Gray;
            StatesColors[ButtonUIState.Toggled] = new Color((byte)255, (byte)170, (byte)40, (byte)255);

            questName = new TextUI();
            questName.Anchor = Anchor.Center;
            questName.Text = "Test";
            AddChild(questName);
        }
        public override void Render()
        {
            texture?.Draw(GlobalPosition, Origin / (Size / texture.Size), GlobalRotation, GlobalScale * (Size / texture.Size), SelfColor, SpriteEffects.None);

            base.Render();
        }

        public void SetQuest(Quest quest)
        {
            Quest = quest;

            questName.Text = LocalizationManager.GetText(quest.Name);
        }

        public void Clear()
        {
            Quest = null;

            questName.Text = "";
        }

    }
}
