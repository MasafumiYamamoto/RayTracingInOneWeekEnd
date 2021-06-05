﻿using System;
using System.IO;
using System.Numerics;

namespace OneWeek2
{
    class Program
    {
        private const float AspectRatio = 16.0f / 9f;
        private const int ImageWidth = 384;
        private const int ImageHeight = (int) (ImageWidth / AspectRatio);
        private const int SamplesPerPixel = 64;
        private static readonly char[] Bars = {'/', '-', '\\', '|'};

        static void WriteColor(StreamWriter streamWriter, Vector3 pixelColor, int samplesPerPixel)
        {
            const float whiteValance = 255.999f;

            var r = (pixelColor.X * whiteValance);
            var g =  (pixelColor.Y * whiteValance);
            var b =  (pixelColor.Z * whiteValance);

            var scale = 1f / samplesPerPixel;
            r *= scale;
            g *= scale;
            b *= scale;
            
            r = (int) r;
            g = (int) g;
            b = (int) b;

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
            const float viewportHeight = 2;
            const float viewportWidth = AspectRatio * viewportHeight;
            const float focalLength = 1;

            var fileName = $"./hoge_{SamplesPerPixel.ToString()}spp.ppm";
            var origin = Vector3.Zero;
            var horizontal = Vector3.UnitX * viewportWidth;
            var vertical = Vector3.UnitY * viewportHeight;
            var lowerLeftCorner = origin - horizontal / 2 - vertical / 2 - Vector3.UnitZ * focalLength;

            var world = new HittableList();
            world.Objects.Add(new Sphere(new Vector3(0, 0, -1), 0.5f));
            world.Objects.Add(new Sphere(new Vector3(0, -100.5f, -1), 100));

            var camera = new Camera();
            
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
                    var pixelColor = Vector3.Zero;
                    for (var s = 0; s < SamplesPerPixel; s++)
                    {
                        var u = (float) (i + MathHelper.Random()) / (ImageWidth - 1);
                        var v = (float) (j + MathHelper.Random()) / (ImageHeight - 1);

                        var r = camera.GetRay(u, v);
                        pixelColor += RayColor(r, world);
                    }

                    WriteColor(streamWriter, pixelColor, SamplesPerPixel);
                }
            }
        }
    }
}