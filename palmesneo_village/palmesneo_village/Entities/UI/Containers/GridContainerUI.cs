using Microsoft.Xna.Framework;
using System;

namespace palmesneo_village
{
    public class GridContainerUI : EntityUI
    {
        private int columns = 3;
        public int Columns
        {
            get => columns;
            set
            {
                columns = Math.Clamp(value, 1, int.MaxValue);

                isDirty = true;
            }
        }

        private int hSeparation = 4;
        public int HSeparation
        {
            get => hSeparation;
            set
            {
                hSeparation = Math.Clamp(value, 0, int.MaxValue);

                isDirty = true;
            }
        }

        private int vSeparation = 4;
        public int VSeparation
        {
            get => vSeparation;
            set
            {
                vSeparation = Math.Clamp(value, 0, int.MaxValue);

                isDirty = true;
            }
        }

        private bool isDirty = false;

        public override void Update()
        {
            base.Update();

            if (isDirty)
            {
                UpdateChildrenTransformation();

                isDirty = false;
            }
        }

        public override void AddChild(Entity entity)
        {
            base.AddChild(entity);

            isDirty = true;
        }

        public override void RemoveChild(Entity entity)
        {
            base.RemoveChild(entity);

            isDirty = true;
        }

        private void UpdateChildrenTransformation()
        {
            int i = 0;

            float maxWidth = 0;
            float maxHeight = 0;

            foreach (var child in GetChildren<EntityUI>())
            {
                int row = i / Columns;
                int column = i % Columns;

                var position = new Vector2(
                    column * (child.Size.X + hSeparation),
                    row * (child.Size.Y + vSeparation)
                );

                child.LocalPosition = position;

                float childRightPositionX = child.LocalPosition.X + child.Size.X;
                float childBottomPositionY = child.LocalPosition.Y + child.Size.Y;

                if (maxWidth < childRightPositionX)
                {
                    maxWidth = childRightPositionX;
                }

                if (maxHeight < childBottomPositionY)
                {
                    maxHeight = childBottomPositionY;
                }

                i++;
            }

            Size = new Vector2(maxWidth, maxHeight);
        }
    }
}
