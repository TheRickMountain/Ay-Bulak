using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;

namespace palmesneo_village
{
    public static class Calc
    {
        public static Direction GetDirection(Vector2 vector)
        {
            if (Math.Abs(vector.X) > Math.Abs(vector.Y))
            {
                return vector.X > 0 ? Direction.Right : Direction.Left;
            }
            else
            {
                return vector.Y > 0 ? Direction.Down : Direction.Up;
            }
        }

        public static Vector2[,] GetVector2DArray(Vector2 position, int width, int height)
        {
            int tileX = (int)position.X;
            int tileY = (int)position.Y;

            Vector2[,] tiles = new Vector2[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    tiles[i, j] = new Vector2(tileX + i, tileY + j);
                }
            }

            return tiles;
        }

        public static T[,] RotateMatrix<T>(T[,] oldMatrix, Direction direction)
        {
            int times = 0;

            switch (direction)
            {
                case Direction.Down:
                    times = 0;
                    break;
                case Direction.Left:
                    times = 1;
                    break;
                case Direction.Up:
                    times = 2;
                    break;
                case Direction.Right:
                    times = 3;
                    break;
            }

            if (times == 0)
                return oldMatrix;

            T[,] newMatrix = oldMatrix;

            for (int t = 0; t < times; t++)
            {
                newMatrix = RotateMatrixCCW(newMatrix);
            }

            return newMatrix;
        }

        private static T[,] RotateMatrixCCW<T>(T[,] oldMatrix)
        {
            T[,] newMatrix = new T[oldMatrix.GetLength(1), oldMatrix.GetLength(0)];

            int newColumn, newRow = 0;

            for (int oldColumn = oldMatrix.GetLength(1) - 1; oldColumn >= 0; oldColumn--)
            {
                newColumn = 0;

                for (int oldRow = 0; oldRow < oldMatrix.GetLength(0); oldRow++)
                {
                    newMatrix[newRow, newColumn] = oldMatrix[oldRow, oldColumn];
                    newColumn++;
                }
                newRow++;
            }

            return newMatrix;
        }

        public static float Angle(Vector2 from, Vector2 to)
        {
            return (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
        }

        public static Vector2 Perpendicular(this Vector2 vector)
        {
            return new Vector2(-vector.Y, vector.X);
        }

        public static Color CubicInterpolate(Color color1, Color color2, float amount)
        {
            // Интерполяция каждого цветового канала (R, G, B)
            byte r = (byte)CubicInterpolate(color1.R, color2.R, amount);
            byte g = (byte)CubicInterpolate(color1.G, color2.G, amount);
            byte b = (byte)CubicInterpolate(color1.B, color2.B, amount);

            // Создайте новый цвет с интерполированными значениями
            return new Color(r, g, b);
        }

        public static float CubicInterpolate(float v0, float v1, float t)
        {
            // Вычисление коэффициентов сплайна
            float a = 2 * v0 - 2 * v1 + 1;
            float b = -3 * v0 + 3 * v1 - 2;
            float c = 1;
            float d = v0;

            // Применение формулы сплайна
            return a * t * t * t + b * t * t + c * t + d;
        }

        public static MTexture ToTexture(this FastNoiseLite noise, int width, int height)
        {
            float[,] noiseData = new float[width, height];

            for (int x = 0; x < noiseData.GetLength(0); x++)
            {
                for (int y = 0; y < noiseData.GetLength(1); y++)
                {
                    noiseData[x, y] = noise.GetNoise(x, y);
                }
            }

            Color[] colorArray = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float normalizedValue = (noiseData[y, x] + 1.0f) / 2.0f;
                    colorArray[y * width + x] = new Color(normalizedValue, normalizedValue, normalizedValue, 1.0f);
                }
            }

            Texture2D texture = new Texture2D(Engine.Instance.GraphicsDevice, width, height);
            texture.SetData(colorArray);

            return new MTexture(texture);
        }

        public static float SignThreshold(float value, float threshold)
        {
            if (Math.Abs(value) >= threshold)
                return Math.Sign(value);
            else
                return 0;
        }

        public static Vector2 SnappedNormal(this Vector2 vec, float slices)
        {
            float divider = MathHelper.TwoPi / slices;

            float angle = vec.Angle();
            angle = (float)Math.Floor((angle + divider / 2f) / divider) * divider;
            return AngleToVector(angle, 1f);
        }

        public static Vector2 Snapped(this Vector2 vec, float slices)
        {
            float divider = MathHelper.TwoPi / slices;

            float angle = vec.Angle();
            angle = (float)Math.Floor((angle + divider / 2f) / divider) * divider;
            return AngleToVector(angle, vec.Length());
        }

        public static float Angle(this Vector2 vector)
        {
            return (float)Math.Atan2(vector.Y, vector.X);
        }

        public static Vector2 AngleToVector(float angleRadians, float length)
        {
            return new Vector2((float)Math.Cos(angleRadians) * length, (float)Math.Sin(angleRadians) * length);
        }

        public static T[,] RotateArrayClockwise<T>(T[,] src)
        {
            int width = src.GetUpperBound(0) + 1;
            int height = src.GetUpperBound(1) + 1;
            T[,] dst = new T[height, width];

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    int newRow = col;
                    int newCol = height - (row + 1);

                    dst[newCol, newRow] = src[col, row];
                }
            }

            return dst;
        }

        public static T GetMiddleElement<T>(T[,] array)
        {
            int rows = array.GetLength(0);
            int cols = array.GetLength(1);

            int midRow = rows / 2;
            int midCol = cols / 2;

            return array[midRow, midCol];
        }

        public static Vector2[,] SelectTiles(Vector2 startTile, Vector2 endTile)
        {
            // Убеждаемся, что startTile находится в левом верхнем углу, а endTile - в правом нижнем.
            int startX = Math.Min((int)startTile.X, (int)endTile.X);
            int startY = Math.Min((int)startTile.Y, (int)endTile.Y);
            int endX = Math.Max((int)startTile.X, (int)endTile.X);
            int endY = Math.Max((int)startTile.Y, (int)endTile.Y);

            // Вычисляем ширину и высоту области выделения.
            int width = endX - startX + 1;
            int height = endY - startY + 1;

            // Создаем массив для хранения координат выбранных плиток.
            Vector2[,] tiles = new Vector2[width, height];

            // Заполняем массив координатами плиток.
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tiles[x, y] = new Vector2(startX + x, startY + y);
                }
            }

            return tiles;
        }

        public static Rectangle CreateRectangle(Vector2 startTile, Vector2 endTile, int scale)
        {
            // Убеждаемся, что startTile находится в левом верхнем углу, а endTile - в правом нижнем.
            int startX = Math.Min((int)startTile.X, (int)endTile.X);
            int startY = Math.Min((int)startTile.Y, (int)endTile.Y);
            int endX = Math.Max((int)startTile.X, (int)endTile.X);
            int endY = Math.Max((int)startTile.Y, (int)endTile.Y);

            // Вычисляем ширину и высоту области выделения.
            int width = endX - startX + 1;
            int height = endY - startY + 1;

            return new Rectangle(startX * scale, startY * scale, width * scale, height * scale);
        }

        #region Give Me

        public static T GiveMe<T>(int index, T a, T b)
        {
            switch (index)
            {
                default:
                    throw new Exception("Index was out of range!");

                case 0:
                    return a;
                case 1:
                    return b;
            }
        }

        public static T GiveMe<T>(int index, T a, T b, T c)
        {
            switch (index)
            {
                default:
                    throw new Exception("Index was out of range!");

                case 0:
                    return a;
                case 1:
                    return b;
                case 2:
                    return c;
            }
        }

        public static T GiveMe<T>(int index, T a, T b, T c, T d)
        {
            switch (index)
            {
                default:
                    throw new Exception("Index was out of range!");

                case 0:
                    return a;
                case 1:
                    return b;
                case 2:
                    return c;
                case 3:
                    return d;
            }
        }

        public static T GiveMe<T>(int index, T a, T b, T c, T d, T e)
        {
            switch (index)
            {
                default:
                    throw new Exception("Index was out of range!");

                case 0:
                    return a;
                case 1:
                    return b;
                case 2:
                    return c;
                case 3:
                    return d;
                case 4:
                    return e;
            }
        }

        public static T GiveMe<T>(int index, T a, T b, T c, T d, T e, T f)
        {
            switch (index)
            {
                default:
                    throw new Exception("Index was out of range!");

                case 0:
                    return a;
                case 1:
                    return b;
                case 2:
                    return c;
                case 3:
                    return d;
                case 4:
                    return e;
                case 5:
                    return f;
            }
        }

        #endregion

        #region Random

        public static Random Random = new Random();
        private static Stack<Random> randomStack = new Stack<Random>();

        public static void PushRandom(int newSeed)
        {
            randomStack.Push(Calc.Random);
            Calc.Random = new Random(newSeed);
        }

        public static void PushRandom(Random random)
        {
            randomStack.Push(Calc.Random);
            Calc.Random = random;
        }

        public static void PushRandom()
        {
            randomStack.Push(Calc.Random);
            Calc.Random = new Random();
        }

        public static void PopRandom()
        {
            Calc.Random = randomStack.Pop();
        }

        #region Choose

        public static T Choose<T>(this Random random, T a, T b)
        {
            return GiveMe<T>(random.Next(2), a, b);
        }

        public static T Choose<T>(this Random random, T a, T b, T c)
        {
            return GiveMe<T>(random.Next(3), a, b, c);
        }

        public static T Choose<T>(this Random random, T a, T b, T c, T d)
        {
            return GiveMe<T>(random.Next(4), a, b, c, d);
        }

        public static T Choose<T>(this Random random, T a, T b, T c, T d, T e)
        {
            return GiveMe<T>(random.Next(5), a, b, c, d, e);
        }

        public static T Choose<T>(this Random random, T a, T b, T c, T d, T e, T f)
        {
            return GiveMe<T>(random.Next(6), a, b, c, d, e, f);
        }

        public static T Choose<T>(this Random random, params T[] choices)
        {
            return choices[random.Next(choices.Length)];
        }

        public static T Choose<T>(this Random random, List<T> choices)
        {
            return choices[random.Next(choices.Count)];
        }

        #endregion

        #region Range

        /// <summary>
        /// Returns a random integer between min (inclusive) and max (inclusive)
        /// </summary>
        /// <param name="random"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Range(this Random random, Range<int> range)
        {
            return range.Min + random.Next((range.Max + 1) - range.Min);
        }

        /// <summary>
        /// Returns a random float between min (inclusive) and max (inclusive)
        /// </summary>
        /// <param name="random"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Range(this Random random, Range<float> range)
        {
            return range.Min + random.NextFloat((range.Max + 1) - range.Min);
        }

        /// <summary>
        /// Returns a random integer between min (inclusive) and max (exclusive)
        /// </summary>
        /// <param name="random"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Range(this Random random, int min, int max)
        {
            return min + random.Next(max - min);
        }

        /// <summary>
        /// Returns a random float between min (inclusive) and max (exclusive)
        /// </summary>
        /// <param name="random"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Range(this Random random, float min, float max)
        {
            return min + random.NextFloat(max - min);
        }

        /// <summary>
        /// Returns a random Vector2, and x- and y-values of which are between min (inclusive) and max (exclusive)
        /// </summary>
        /// <param name="random"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static Vector2 Range(this Random random, Vector2 min, Vector2 max)
        {
            return min + new Vector2(random.NextFloat(max.X - min.X), random.NextFloat(max.Y - min.Y));
        }

        #endregion

        public static int Facing(this Random random)
        {
            return (random.NextFloat() < 0.5f ? -1 : 1);
        }

        public static bool Chance(this Random random, float chance)
        {
            return random.NextFloat() < chance;
        }

        public static float NextFloat(this Random random)
        {
            return (float)random.NextDouble();
        }

        public static float NextFloat(this Random random, float max)
        {
            return random.NextFloat() * max;
        }

        public static float NextAngle(this Random random)
        {
            return random.NextFloat() * MathHelper.TwoPi;
        }

        private static int[] shakeVectorOffsets = new int[] { -1, -1, 0, 1, 1 };

        public static Vector2 ShakeVector(this Random random)
        {
            return new Vector2(random.Choose(shakeVectorOffsets), random.Choose(shakeVectorOffsets));
        }

        #endregion

    }
}
