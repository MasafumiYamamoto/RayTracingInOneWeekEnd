using System;
using System.IO;
using System.Numerics;

namespace OneWeek2
{
    class Program
    {
        private const float AspectRatio = 16.0f / 9f;
        private const int ImageWidth = 384;
        private const int ImageHeight = (int) (ImageWidth / AspectRatio);
        private static readonly char[] _bars = {'/', '-', '\\', '|'};


        static void WriteColor(ref string outStr, Vector3 pixelColor)
        {
            outStr += $"{pixelColor.X.ToString()} {pixelColor.Y.ToString()} {pixelColor.Z.ToString()} \n";
        }

        static void WriteColor(StreamWriter streamWriter, Vector3 pixelColor)
        {
            const float whiteValance = 255.999f;
            var r = (int) (pixelColor.X * whiteValance);
            var g = (int) (pixelColor.Y * whiteValance);
            var b = (int) (pixelColor.Z * whiteValance);
            streamWriter.Write($"{r.ToString()} {g.ToString()} {b.ToString()} \n");
        }

        /// <summary>
        /// rayと球の交差判定
        /// </summary>
        /// <param name="center">球の中心</param>
        /// <param name="radius">球の半径</param>
        /// <param name="ray">飛ばしているレイ</param>
        /// <returns></returns>
        private static bool HitSphere(Vector3 center, float radius, Ray ray)
        {
            var oc = ray.Origin - center;
            var a = Vector3.Dot(ray.Direction, ray.Direction);
            var b = 2 * Vector3.Dot(oc, ray.Direction);
            var c = Vector3.Dot(oc, oc) - radius * radius;
            var discriminant = b * b - 4 * a * c;
            return discriminant > 0;
        }

        static Vector3 RayColor(in Ray ray)
        {
            if (HitSphere(new Vector3(0, 0, -1), 0.5f, ray))
            {
                return Vector3.UnitX;
            }

            var unitDirection = Vector3.Normalize(ray.Direction);
            var t = 0.5f * (unitDirection.Y + 1);
            return (1 - t) * Vector3.One + t * new Vector3(0.5f, 0.7f, 1f);
        }

        static void Main(string[] args)
        {
            const string fileName = "./hoge.ppm";

            const float viewportHeight = 2;
            const float viewportWidth = AspectRatio * viewportHeight;
            const float focalLength = 1;

            var origin = Vector3.Zero;
            var horizontal = Vector3.UnitX * viewportWidth;
            var vertical = Vector3.UnitY * viewportHeight;
            var lowerLeftCorner = origin - horizontal / 2 - vertical / 2 - Vector3.UnitZ * focalLength;

            using var streamWriter = new StreamWriter(fileName, false);

            #region ヘッダー書き込み

            streamWriter.Write($"P3\n{ImageWidth.ToString()} {ImageHeight.ToString()}\n255\n");

            #endregion

            for (var j = ImageHeight - 1; j >= 0; j--)
            {
                Console.Write(_bars[j % 4]);
                Console.Write($"{((int) 100.0 * (ImageHeight - j) / ImageHeight).ToString()}%");
                Console.SetCursorPosition(0, Console.CursorTop);

                for (var i = 0; i < ImageWidth; i++)
                {
                    var u = (float) i / (ImageWidth - 1);
                    var v = (float) j / (ImageHeight - 1);

                    var r = new Ray(origin, lowerLeftCorner + u * horizontal + v * vertical - origin);
                    var pixelColor = RayColor(r);

                    // WriteColor(ref content, pixelColor);
                    WriteColor(streamWriter, pixelColor);
                }
            }
        }
    }
}