using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public enum VExpansionMode
    {
        UpToDown,
        DownToUp
    }

    public class VerticalContainerUI : EntityUI
    {
        // TODO: возможность родителя блокировать изменение Transform для ребенка
        // TODO: ширина HorizontalContainerUI не может быть меньше суммы ширин всех детей, но может быть больше
        // TODO: высотка HorizontalContainerUI не может быть меньше высоты самого высокого ребенка, но может быть больше

        private VExpansionMode expansionMode = VExpansionMode.UpToDown;
        public VExpansionMode ExpansionMode
        {
            get => expansionMode;
            set
            {
                expansionMode = value;

                UpdateChildrenTransformation();
            }
        }

        private int separation = 4;
        public int Separation
        {
            get => separation;
            set
            {
                separation = value;

                UpdateChildrenTransformation();
            }
        }

        public override void AddChild(Entity entity)
        {
            base.AddChild(entity);

            UpdateChildrenTransformation();
        }

        public override void RemoveChild(Entity entity)
        {
            base.RemoveChild(entity);

            UpdateChildrenTransformation();
        }

        private void UpdateChildrenTransformation()
        {
            switch (ExpansionMode)
            {
                case VExpansionMode.UpToDown:
                    {
                        int iteration = 0;

                        int totalChildrenHeight = 0;

                        foreach (var children in GetChildren<EntityUI>())
                        {
                            children.LocalPosition = new Vector2(0, totalChildrenHeight + (iteration * Separation));

                            totalChildrenHeight += (int)children.Size.Y;

                            iteration++;
                        }

                        Vector2 newSize = new Vector2(Size.X, totalChildrenHeight + ((iteration - 1) * Separation));

                        Size = Vector2.Clamp(newSize, Vector2.Zero, new Vector2(int.MaxValue, int.MaxValue));
                    }
                    break;
                case VExpansionMode.DownToUp:
                    {
                        // TODO: Come up with solution
                    }
                    break;
            }
        }

    }
}
