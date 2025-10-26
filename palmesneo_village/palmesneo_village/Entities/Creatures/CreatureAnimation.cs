using Microsoft.Xna.Framework;
using MonoGame.Extended.Tweening;

namespace palmesneo_village
{
    public class CreatureAnimation : Entity
    {
        private CreatureMovement _movement;
        private CreatureBodyVisual _bodyVisual;

        private Tweener _idleTweener;

        private FloatWrapper _idleTweenerXFloat;
        private FloatWrapper _idleTweenerYFloat;

        private Vector2 _bodyVisualOriginalLocalPosition;

        public CreatureAnimation(CreatureMovement movement, CreatureBodyVisual bodyVisual)
        {
            _movement = movement;
            _bodyVisual = bodyVisual;

            _idleTweener = new Tweener();

            _idleTweenerXFloat = new FloatWrapper(-1.0f);
            _idleTweenerYFloat = new FloatWrapper(0.0f);

            _bodyVisualOriginalLocalPosition = bodyVisual.LocalPosition;

            InitializeIdleAnimation();
        }

        public override void Update()
        {
            _idleTweener.Update(Engine.GameDeltaTime);

            if (_movement.Direction == Direction.Left)
            {
                _bodyVisual.Flip();
            }
            else
            {
                _bodyVisual.Unflip();
            }

            switch(_movement.State)
            {
                case MovementState.Success:
                    {
                        _bodyVisual.LocalPosition = _bodyVisualOriginalLocalPosition + new Vector2(_idleTweenerXFloat.Value, _idleTweenerYFloat.Value);
                    }
                    break;
                case MovementState.Running:
                case MovementState.Completion:
                    {
                        _bodyVisual.LocalPosition = _bodyVisualOriginalLocalPosition;
                        _bodyVisual.SetBodyRotation(_movement.GetRotation());
                    }
                    break;
            }

            base.Update();
        }

        private void InitializeIdleAnimation()
        {
            _idleTweener.TweenTo(
                    target: _idleTweenerXFloat,
                    expression: x => _idleTweenerXFloat.Value,
                    toValue: 1f,
                    duration: 0.5f)
                .Easing(EasingFunctions.SineOut)
                .AutoReverse()
                .RepeatForever();

            _idleTweener.TweenTo(
                    target: _idleTweenerYFloat,
                    expression: y => _idleTweenerYFloat.Value,
                    toValue: -2,
                    duration: 0.25f)
                .Easing(EasingFunctions.SineOut)
                .AutoReverse()
                .RepeatForever();
        }

    }
}
