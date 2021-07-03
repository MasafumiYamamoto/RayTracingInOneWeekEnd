﻿using System;
using System.IO;
using System.Numerics;
using OneWeek2.Materials;

namespace OneWeek2
{
    class Program
    {
        private const float AspectRatio = 16.0f / 9f;
        private const int ImageWidth = 384;
        private const int ImageHeight = (int) (ImageWidth / AspectRatio);
        private const int SamplesPerPixel = 64;
        private const int MaxDepth = 50;
        private const float Gamma = 2f;
        private static readonly char[] Bars = {'/', '-', '\\', '|'};

        static void WriteColor(StreamWriter streamWriter, Vector3 pixelColor, int samplesPerPixel)
        {
            const float whiteValance = 255.999f;

            // 色の値をサンプル数で除算し、ガンマ補正を行う
            var scale = 1f / samplesPerPixel;
            var r = MathF.Pow(pixelColor.X * scale, 1 / Gamma);
            var g = MathF.Pow(pixelColor.Y * scale, 1 / Gamma);
            var b = MathF.Pow(pixelColor.Z * scale, 1 / Gamma);

            // 各成分を[0,1]にクランプして書き込み
            r = (int) (whiteValance * Math.Clamp(r, 0, 1));
            g = (int) (whiteValance * Math.Clamp(g, 0, 1));
            b = (int) (whiteValance * Math.Clamp(b, 0, 1));

            streamWriter.Write($"{r.ToString()} {g.ToString()} {b.ToString()} \n");
        }

        static Vector3 RayColor(in Ray ray, in IHittable world, int depth)
        {
            var hitRecord = new HitRecord();
            if (depth<=0)
            {
                // 反射回数が一定回数以上になったらその時点で打ち切り
                return Vector3.Zero;
            }
            
            if (world.Hit(ray, 0.001f, float.MaxValue, ref hitRecord))
            {
                if (hitRecord.Material.Scatter(ray, hitRecord, out var attenuation, out var scattered))
                {
                    return attenuation * RayColor(scattered, world, --depth);
                }

                return Vector3.Zero;
            }

            var unitDirection = Vector3.Normalize(ray.Direction);
            var t = 0.5f * (unitDirection.Y + 1);
            return (1 - t) * Vector3.One + t * new Vector3(0.5f, 0.7f, 1f);
        }

        private static HittableList GenerateRandomScene(int worldSize)
        {
            var world = new HittableList();

            var groundMaterial = new Lambertian(new Vector3(0.5f, 0.5f, 0.5f));
            world.Objects.Add(new Sphere(new Vector3(0, -1000, 0), 1000, groundMaterial));
            for (var a = -worldSize; a < worldSize; a++)
            {
                for (var b = -worldSize; b < worldSize; b++)
                {
                    var matSelection = MathHelper.Random();
                    var center = new Vector3(a + 0.9f * MathHelper.Random(), 0.2f, b + 0.9f * MathHelper.Random());

                    if ((center - new Vector3(4, 0.2f, 0)).Length() <= 0.9)
                    {
                        continue;
                    }

                    if (matSelection < 0.8)
                    {
                        // diffuse 
                        var albedo = new Lambertian(MathHelper.RandomColor() * MathHelper.RandomColor());
                        world.Objects.Add(new Sphere(center, 0.2f, albedo));
                    }
                    else if (matSelection < 0.95)
                    {
                        // metal
                        var albedo = MathHelper.RandomColor(0.5f, 1);
                        var fuzz = MathHelper.Random(0, 0.5f);
                        var metal = new Metal(albedo, fuzz);
                        world.Objects.Add(new Sphere(center, 0.2f, metal));
                    }
                    else
                    {
                        // glass
                        var dielectric = new Dielectric(2.5f);
                        world.Objects.Add(new Sphere(center, 0.2f, dielectric));
                    }
                }
            }

            var material1 = new Dielectric(1.5f);
            world.Objects.Add(new Sphere(Vector3.UnitY, 1.0f, material1));
            var material2 = new Lambertian(new Vector3(0.9f, 0.55f, 0f));
            world.Objects.Add(new Sphere(new Vector3(-4f, 1f, 0f), 1.0f, material2));
            var material3 = new Metal(new Vector3(0.7f, 0.6f, 0.5f), 0);
            world.Objects.Add(new Sphere(new Vector3(4, 1, 0), 1, material3));
            return world;
        }

        static void Main(string[] args)
        {
            Console.Write($"Start {DateTime.Now}\n");
            
            var fileName = $"./hoge_{SamplesPerPixel.ToString()}spp.ppm";
            
            var world = GenerateRandomScene(11);
            var lookFrom = new Vector3(13, 2, 3);
            var lookAt = new Vector3(0, 0, 0);
            var viewUp = Vector3.UnitY;
            var distToFocus = 10;
            var aperture = 0.1f;
            var camera = new Camera(lookFrom, lookAt, viewUp, 20, AspectRatio, aperture, distToFocus);
            
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
                        pixelColor += RayColor(r, world, MaxDepth);
                    }
                    
                    WriteColor(streamWriter, pixelColor, SamplesPerPixel);
                }
            }
            Console.Write($"Finish!! {DateTime.Now}\n");
        }
    }
}