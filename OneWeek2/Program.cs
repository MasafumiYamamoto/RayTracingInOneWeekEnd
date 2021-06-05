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
        private static readonly char[] Bars = {'/', '-', '\\', '|'};

        static void WriteColor(StreamWriter streamWriter, Vector3 pixelColor)
        {
            const float whiteValance = 255.999f;
            var r = (int) (pixelColor.X * whiteValance);
            var g = (int) (pixelColor.Y * whiteValance);
            var b = (int) (pixelColor.Z * whiteValance);
            streamWriter.Write($"{r.ToString()} {g.ToString()} {b.ToString()} \n");
        }

        static Vector3 RayColor(in Ray ray, in IHittable world)
        {
            var hitRecord = new HitRecord();
            if (world.Hit(ray, 0, float.MaxValue, ref hitRecord))
            {
                return 0.5f * (hitRecord.Normal + Vector3.One);
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

            var world = new HittableList();
            world.Objects.Add(new Sphere(new Vector3(0, 0, -1), 0.5f));
            world.Objects.Add(new Sphere(new Vector3(0, -100.5f, -1), 100));

            using var streamWriter = new StreamWriter(fileName, false);

            #region ヘッダー書き込み

            streamWriter.Write($"P3\n{ImageWidth.ToString()} {ImageHeight.ToString()}\n255\n");

            #endregion

            for (var j = ImageHeight - 1; j >= 0; j--)
            {
                Console.Write(Bars[j % 4]);
                Console.Write($"{((int) 100.0 * (ImageHeight - j) / ImageHeight).ToString()}%");
                Console.SetCursorPosition(0, Console.CursorTop);

                for (var i = 0; i < ImageWidth; i++)
                {
                    var u = (float) i / (ImageWidth - 1);
                    var v = (float) j / (ImageHeight - 1);

                    var r = new Ray(origin, lowerLeftCorner + u * horizontal + v * vertical - origin);
                    var pixelColor = RayColor(r, world);

                    WriteColor(streamWriter, pixelColor);
                }
            }
        }
    }
}