using Microsoft.Xna.Framework;
using System;

namespace palmesneo_village
{
    public enum Anchor
    {
        TopLeft,
        TopCenter,
        TopRight,
        LeftCenter,
        Center,
        RightCenter,
        BottomLeft,
        BottomCenter,
        BottomRight
    }

    public class EntityUI : Entity
    {
        public string Tooltip { get; set; } = string.Empty;

        public Vector2 Origin { get; set; } = Vector2.Zero;

        private Vector2 size = Vector2.Zero;
        public virtual Vector2 Size
        {
            get { return size; }
            set
            {
                size = Vector2.Clamp(value, MinSize, new Vector2(int.MaxValue, int.MaxValue));
            }
        }

        private Vector2 minSize = Vector2.Zero;
        public Vector2 MinSize
        {
            get { return minSize; }
            set
            {
                minSize = Vector2.Clamp(value, Vector2.Zero, new Vector2(int.MaxValue, int.MaxValue));

                Size = Size;
            }
        }

        public Anchor Anchor { get; set; } = Anchor.TopLeft;

        public override Vector2 GlobalPosition
        {
            get
            {
                if (Parent == null)
                {
                    return LocalPosition;
                }

                Vector2 parentSize = Vector2.Zero;
                Vector2 parentOrigin = Vector2.Zero;

                if (Parent is EntityUI)
                {
                    EntityUI parentUI = (EntityUI)Parent;

                    parentSize = parentUI.Size;
                    parentOrigin = parentUI.Origin;
                }

                float globalX = 0;
                float globalY = 0;

                Vector2 totalOrigin = Origin - parentOrigin;

                switch (Anchor)
                {
                    case Anchor.TopLeft:
                        {
                            globalX = (LocalPosition.X + totalOrigin.X) * Parent.GlobalScale.X + Parent.GlobalPosition.X;
                            globalY = (LocalPosition.Y + totalOrigin.Y) * Parent.GlobalScale.Y + Parent.GlobalPosition.Y;
                        }
                        break;
                    case Anchor.TopCenter:
                        {
                            globalX = (LocalPosition.X + totalOrigin.X + parentSize.X / 2 - Size.X / 2) * Parent.GlobalScale.X + Parent.GlobalPosition.X;
                            globalY = (LocalPosition.Y + totalOrigin.Y) * Parent.GlobalScale.Y + Parent.GlobalPosition.Y;
                        }
                        break;
                    case Anchor.TopRight:
                        {
                            globalX = (LocalPosition.X + totalOrigin.X + parentSize.X - Size.X) * Parent.GlobalScale.X + Parent.GlobalPosition.X;
                            globalY = (LocalPosition.Y + totalOrigin.Y) * Parent.GlobalScale.Y + Parent.GlobalPosition.Y;
                        }
                        break;
                    case Anchor.LeftCenter:
                        {
                            globalX = (LocalPosition.X + totalOrigin.X) * Parent.GlobalScale.X + Parent.GlobalPosition.X;
                            globalY = (LocalPosition.Y + totalOrigin.Y + parentSize.Y / 2 - Size.Y / 2) * Parent.GlobalScale.Y + Parent.GlobalPosition.Y;
                        }
                        break;
                    case Anchor.Center:
                        {
                            globalX = (LocalPosition.X + totalOrigin.X + parentSize.X / 2 - Size.X / 2) * Parent.GlobalScale.X + Parent.GlobalPosition.X;
                            globalY = (LocalPosition.Y + totalOrigin.Y + parentSize.Y / 2 - Size.Y / 2) * Parent.GlobalScale.Y + Parent.GlobalPosition.Y;
                        }
                        break;
                    case Anchor.RightCenter:
                        {
                            globalX = (LocalPosition.X + totalOrigin.X + parentSize.X - Size.X) * Parent.GlobalScale.X + Parent.GlobalPosition.X;
                            globalY = (LocalPosition.Y + totalOrigin.Y + parentSize.Y / 2 - Size.Y / 2) * Parent.GlobalScale.Y + Parent.GlobalPosition.Y;
                        }
                        break;
                    case Anchor.BottomLeft:
                        {
                            globalX = (LocalPosition.X + totalOrigin.X) * Parent.GlobalScale.X + Parent.GlobalPosition.X;
                            globalY = (LocalPosition.Y + totalOrigin.Y + parentSize.Y - Size.Y) * Parent.GlobalScale.Y + Parent.GlobalPosition.Y;
                        }
                        break;
                    case Anchor.BottomCenter:
                        {
                            globalX = (LocalPosition.X + totalOrigin.X + parentSize.X / 2 - Size.X / 2) * Parent.GlobalScale.X + Parent.GlobalPosition.X;
                            globalY = (LocalPosition.Y + totalOrigin.Y + parentSize.Y - Size.Y) * Parent.GlobalScale.Y + Parent.GlobalPosition.Y;
                        }
                        break;
                    case Anchor.BottomRight:
                        {
                            globalX = (LocalPosition.X + totalOrigin.X + parentSize.X - Size.X) * Parent.GlobalScale.X + Parent.GlobalPosition.X;
                            globalY = (LocalPosition.Y + totalOrigin.Y + parentSize.Y - Size.Y) * Parent.GlobalScale.Y + Parent.GlobalPosition.Y;
                        }
                        break;
                }

                return new Vector2(globalX, globalY);
            }
        }

        public Action<EntityUI> MouseEntered { get; set; }
        public Action<EntityUI> MouseExited { get; set; }

        private Matrix transformMatrix = Matrix.Identity;
        private Matrix invertedTransformMatrix = Matrix.Identity;

        private bool isHovered = false;

        public EntityUI()
        {
            Size = new Vector2(20, 20);
        }

        public override void Update()
        {
            UpdateTransformMatrix();

            base.Update();

            UpdateMouseHover();

            if (string.IsNullOrEmpty(Tooltip) == false)
            {
                if (Contains(MInput.Mouse.UIPosition))
                {
                    Engine.CurrentScene.ShowTooltip(Tooltip);
                }
            }
        }

        private void UpdateMouseHover()
        {
            if (MouseEntered != null || MouseExited != null)
            {
                if (isHovered == false && Contains(MInput.Mouse.UIPosition))
                {
                    isHovered = true;
                    MouseEntered?.Invoke(this);
                }
                else if (isHovered == true && Contains(MInput.Mouse.UIPosition) == false)
                {
                    isHovered = false;
                    MouseExited?.Invoke(this);
                }
            }
        }

        public override void DebugRender()
        {
            Vector2 topLeft = TransformVector(new Vector2(-0.5f, -0.5f), false);
            Vector2 topRight = TransformVector(new Vector2(Size.X + 0.5f, -0.5f), false);
            Vector2 bottomLeft = TransformVector(new Vector2(-0.5f, Size.Y + 0.5f), false);
            Vector2 bottomRight = TransformVector(new Vector2(Size.X + 0.5f, Size.Y + 0.5f), false);

            // Bounding rect
            RenderManager.Line(topLeft, topRight, Color.Orange, 1f);
            RenderManager.Line(topRight, bottomRight, Color.Orange, 1f);
            RenderManager.Line(bottomRight, bottomLeft, Color.Orange, 1f);
            RenderManager.Line(bottomLeft, topLeft, Color.Orange, 1f);

            // Pivot point
            RenderManager.Rect(GlobalPosition.X, GlobalPosition.Y, 1, 1, Color.OrangeRed);

            base.DebugRender();
        }

        public bool Contains(Vector2 value)
        {
            Vector2 processedValue = TransformVector(value, true);

            bool xCheck = processedValue.X >= 0 && processedValue.X <= Size.X;
            bool yCheck = processedValue.Y >= 0 && processedValue.Y <= Size.Y;

            return xCheck && yCheck;
        }

        public bool ContainsY(Vector2 value)
        {
            Vector2 processedValue = TransformVector(value, true);

            return processedValue.Y >= 0 && processedValue.Y <= Size.Y;
        }

        private void UpdateTransformMatrix()
        {
            float cos = MathF.Cos(GlobalRotation);
            float sin = MathF.Sin(GlobalRotation);
            transformMatrix.M11 = GlobalScale.X * cos;
            transformMatrix.M12 = GlobalScale.X * sin;
            transformMatrix.M21 = GlobalScale.Y * (-sin);
            transformMatrix.M22 = GlobalScale.Y * cos;
            transformMatrix.M41 = ((-Origin.X * transformMatrix.M11) + (-Origin.Y) * transformMatrix.M21) + GlobalPosition.X;
            transformMatrix.M42 = ((-Origin.X * transformMatrix.M12) + (-Origin.Y) * transformMatrix.M22) + GlobalPosition.Y;

            invertedTransformMatrix = Matrix.Invert(transformMatrix);
        }

        private Vector2 TransformVector(Vector2 value, bool invert)
        {
            if (invert)
            {
                return Vector2.Transform(value, invertedTransformMatrix);
            }
            else
            {
                return Vector2.Transform(value, transformMatrix);
            }
        }

    }
}
