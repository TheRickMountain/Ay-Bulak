using Microsoft.Xna.Framework;
using MonoGame.Extended.Tweening;

namespace palmesneo_village
{
    public class OpenQuestsButtonUI : MiddleButtonUI
    {
        private ImageUI icon;

        private Tweener tweener;

        private float animationRepeatDelay = 0.5f;

        private QuestManager questManager;

        public OpenQuestsButtonUI(QuestManager questManager)
        {
            this.questManager = questManager;

            Tooltip = LocalizationManager.GetText("quests");

            icon = new ImageUI();
            icon.Anchor = Anchor.Center;
            icon.Texture = ResourcesManager.GetTexture("Sprites", "UI", "quest_icon");
            icon.Size = new Vector2(16, 16);
            icon.Origin = icon.Texture.Size / 2;
            AddChild(icon);

            tweener = new Tweener();

            tweener.TweenTo(
                 target: icon,
                 expression: item => icon.LocalRotation,
                 toValue: 0.5f,
                 duration: 0.5f)
                .AutoReverse()
                .RepeatForever(animationRepeatDelay)
                .Easing(EasingFunctions.ElasticInOut);

            tweener.TweenTo(
                 target: icon,
                 expression: item => icon.LocalScale,
                 toValue: new Vector2(1.5f, 1.5f),
                 duration: 0.5f)
                .AutoReverse()
                .RepeatForever(animationRepeatDelay)
                .Easing(EasingFunctions.Linear);
        }

        public override void Update()
        {
            base.Update();

            if (questManager.HasNewOrCompletedQuests())
            {
                tweener.Update(Engine.DeltaTime);
            }
            else
            {
                ResetIconData();
            } 
        }

        private void ResetIconData()
        {
            icon.LocalRotation = 0;
            icon.LocalScale = new Vector2(1, 1);
        }

    }
}
