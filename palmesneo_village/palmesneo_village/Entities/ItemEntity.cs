using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tweening;
using System.Diagnostics;

namespace palmesneo_village
{
    public class ItemEntity : ImageEntity
    {
        private const float MAX_LANDING_DISTANCE = 32f;

        public ItemContainer ItemContainer { get; }

        private Tweener tweener;

        private bool isLandingAnimationStarted = false;

        public ItemEntity(ItemContainer itemContainer)
        {
            ItemContainer = itemContainer;

            Texture = ItemContainer.Item.Icon;
            Centered = true;

            LocalScale = new Vector2(0.8f, 0.8f);

            Depth = 1000000;

            tweener = new Tweener();
        }

        public override void Update()
        {
            tweener.Update(Engine.GameDeltaTime);

            if (isLandingAnimationStarted == false)
            {
                StartLandingAnimation();

                isLandingAnimationStarted = true;
            }

            base.Update();
        }

        private void StartLandingAnimation()
        {
            Vector2 landingDirection = Calc.Random.Range(
                            new Vector2(-1),
                            new Vector2(1));

            Vector2 landingPosition = LocalPosition + (landingDirection * MAX_LANDING_DISTANCE);

            tweener.TweenTo(
                target: this,
                expression: item => LocalPosition,
                toValue: landingPosition,
                duration: 0.3f)
                .Easing(EasingFunctions.CubicOut);
        }

    }
}
