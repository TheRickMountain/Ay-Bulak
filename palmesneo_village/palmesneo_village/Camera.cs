using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public class Camera : IDisposable
    {

        private Matrix matrix = Matrix.Identity;
        private bool changed;

        private Vector2 position = Vector2.Zero;
        private Vector2 zoom = Vector2.One;
        private Vector2 origin = Vector2.Zero;

        public Camera(int viewportWidth, int viewportHeight)
        {
            OnScreenSizeChanged(viewportWidth, viewportHeight);

            Engine.Instance.ScreenSizeChanged += OnScreenSizeChanged;
        }

        private void OnScreenSizeChanged(int width, int height)
        {
            origin = new Vector2(width / 2, height / 2);
            UpdateMatrices();
        }

        private void UpdateMatrices()
        {
            matrix = Matrix.Identity *
                    Matrix.CreateTranslation(new Vector3(-new Vector2((int)Math.Floor(position.X), (int)Math.Floor(position.Y)), 0)) *
                    Matrix.CreateScale(new Vector3(zoom, 1)) *
                    Matrix.CreateTranslation(new Vector3(new Vector2((int)Math.Floor(origin.X), (int)Math.Floor(origin.Y)), 0));

            changed = false;
        }

        public void Dispose()
        {
            Engine.Instance.ScreenSizeChanged -= OnScreenSizeChanged;
        }

        public Matrix Matrix
        {
            get
            {
                if (changed)
                    UpdateMatrices();
                return matrix;
            }
        }

        public Vector2 Position
        {
            get { return position; }
            set
            {
                changed = true;
                position = value;
            }
        }

        public float X
        {
            get { return position.X; }
            set
            {
                changed = true;
                position.X = value;
            }
        }

        public float Y
        {
            get { return position.Y; }
            set
            {
                changed = true;
                position.Y = value;
            }
        }

        public Vector2 Zoom
        {
            get { return zoom; }
            set
            {
                changed = true;
                zoom = value;
            }
        }

    }
}
