using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace palmesneo_village
{
    public struct ColorPoint
    {
        public float Position;
        public Color Color;

        public ColorPoint(float position, Color color)
        {
            Position = position;
            Color = color;
        }
    }

    public enum InterpolationType
    {
        Linear,
        Cubic
    }

    public class GradientTexture1D : MTexture
    {
        private readonly int _width;
        
        private Color[] _colors;

        private readonly List<ColorPoint> _colorPoints;

        private InterpolationType _interpolationType;

        public GradientTexture1D(int width, List<ColorPoint> colorPoints, InterpolationType interpolationType)
            : base(new Texture2D(Engine.Instance.GraphicsDevice, width, 1))
        {
            _width = width;
            _colorPoints = colorPoints;
            _interpolationType = interpolationType;

            _colorPoints.Sort((a, b) => a.Position.CompareTo(b.Position));

            _colors = new Color[_width];

            GenerateColorsData();

            Texture.SetData(GetColors());
        }

        private void GenerateColorsData()
        {
            for (int x = 0; x < _width; x++)
            {
                // Определяем положение на градиенте (0.0f - 1.0f)
                float t = (float)x / (_width - 1);

                // Находим индексы точек, между которыми находится текущая позиция
                int index1 = 0;
                while (index1 < _colorPoints.Count - 1 && _colorPoints[index1 + 1].Position <= t)
                {
                    index1++;
                }

                // index2 будет равен index1, если t совпадает с последней точкой
                int index2 = Math.Min(index1 + 1, _colorPoints.Count - 1);

                // Интерполируем цвет
                float blend = 0.0f;
                if (index1 != index2)
                {
                    blend = (t - _colorPoints[index1].Position) / (_colorPoints[index2].Position - _colorPoints[index1].Position);
                }

                switch(_interpolationType)
                {
                    case InterpolationType.Linear:
                        _colors[x] = Color.Lerp(_colorPoints[index1].Color, _colorPoints[index2].Color, blend);
                        break;
                    case InterpolationType.Cubic:
                        _colors[x] = Calc.CubicInterpolate(_colorPoints[index1].Color, _colorPoints[index2].Color, blend);
                        break;
                }
                
            }
        }

        public Color GetColor(float time)
        {
            // Проверяем, что время находится в диапазоне [0.0f, 1.0f]
            time = MathHelper.Clamp(time, 0.0f, 1.0f);

            // Вычисляем координату X на текстуре
            int x = (int)(time * (_width - 1));

            return _colors[x];
        }

        public Color[] GetColors()
        {
            return _colors;
        }
    }
}
