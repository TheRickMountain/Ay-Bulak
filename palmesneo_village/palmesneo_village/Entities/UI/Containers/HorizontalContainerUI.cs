using Microsoft.Xna.Framework;
using System.Linq;

namespace palmesneo_village
{
    public enum HExpansionMode
    {
        LeftToRight,
        RightToLeft,
        Center
    }

    public class HorizontalContainerUI : EntityUI
    {
        // TODO: возможность родителя блокировать изменение Transform для ребенка
        // TODO: ширина HorizontalContainerUI не может быть меньше суммы ширин всех детей, но может быть больше
        // TODO: высотка HorizontalContainerUI не может быть меньше высоты самого высокого ребенка, но может быть больше

        private HExpansionMode expansionMode = HExpansionMode.LeftToRight;
        public HExpansionMode ExpansionMode 
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
            switch(ExpansionMode)
            {
                case HExpansionMode.LeftToRight:
                    {
                        int iteration = 0;

                        int totalChildrenWidth = 0;

                        foreach(var child in GetChildren<EntityUI>())
                        {
                            child.LocalPosition = new Vector2(totalChildrenWidth + (iteration * Separation), 0);

                            totalChildrenWidth += (int)child.Size.X;

                            iteration++;
                        }

                        Size = new Vector2(totalChildrenWidth + ((iteration - 1) * Separation), Size.Y);
                    }
                    break;
                case HExpansionMode.RightToLeft:
                    {
                        // TODO: Come up with solution
                    }
                    break;
                case HExpansionMode.Center:
                    {
                        float totalChildrenWidthWithSeparation = GetTotalChildrenWidthWithSeparation();

                        float childrenStartX = (Size.X / 2) - (totalChildrenWidthWithSeparation / 2);

                        int iteration = 0;

                        int totalChildrenWidth = 0;

                        foreach (var child in GetChildren<EntityUI>())
                        {
                            child.LocalPosition = new Vector2(childrenStartX + totalChildrenWidth + (iteration * Separation), 0);

                            totalChildrenWidth += (int)child.Size.X;

                            iteration++;
                        }

                        Size = new Vector2(totalChildrenWidth + ((iteration - 1) * Separation), Size.Y);
                    }
                    break;
            }
        }

        private float GetTotalChildrenWidthWithSeparation()
        {
            float totalChildrenWidth = 0;

            foreach (var children in GetChildren<EntityUI>())
            {
                totalChildrenWidth += children.Size.X;
            }

            int childrenAmount = GetChildren<EntityUI>().Count();

            if (childrenAmount > 1)
            {
                totalChildrenWidth += (childrenAmount - 1) * separation;
            }

            return totalChildrenWidth;
        }

    }
}
