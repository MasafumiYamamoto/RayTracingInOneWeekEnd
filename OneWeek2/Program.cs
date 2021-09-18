using System;
using System.IO;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
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

        static Vector3 RayColor(in Ray ray, in IHittable world, int depth, MathHelper mathHelper)
        {
            var hitRecord = new HitRecord();
            hitRecord.Clear();
            if (depth<=0)
            {
                // 反射回数が一定回数以上になったらその時点で打ち切り
                return Vector3.Zero;
            }
            
            if (world.Hit(ray, 0.001f, float.MaxValue, ref hitRecord))
            {
                if (hitRecord.Material.Scatter(ray, hitRecord, out var attenuation, out var scattered, mathHelper))
                {
                    return attenuation * RayColor(scattered, world, --depth, mathHelper);
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
            var mathHelper = new MathHelper();

            var groundMaterial = new Lambertian(new Vector3(0.5f, 0.5f, 0.5f));
            world.Objects.Add(new Sphere(new Vector3(0, -1000, 0), 1000, groundMaterial));
            for (var a = -worldSize; a < worldSize; a++)
            {
                for (var b = -worldSize; b < worldSize; b++)
                {
                    var matSelection = mathHelper.Random();
                    var center = new Vector3(a + 0.9f * mathHelper.Random(), 0.2f, b + 0.9f * mathHelper.Random());

                    if ((center - new Vector3(4, 0.2f, 0)).Length() <= 0.9)
                    {
                        continue;
                    }

                    if (matSelection < 0.8)
                    {
                        // diffuse 
                        var albedo = new Lambertian(mathHelper.RandomColor() * mathHelper.RandomColor());
                        var center2 = center + new Vector3(0, mathHelper.Random(0, 0.5f), 0);
                        world.Objects.Add(new MovingSphere(center, center2, 0, 1, 0.2f, albedo));
                    }
                    else if (matSelection < 0.95)
                    {
                        // metal
                        var albedo = mathHelper.RandomColor(0.5f, 1);
                        var fuzz = mathHelper.Random(0, 0.5f);
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

        private static float finLines = 0;

        static void DebugProgress()
        {
            var j = 0;
            while (finLines < ImageHeight)
            {
                Thread.Sleep(33);
                Console.Write(Bars[j++ % 4]);
                Console.Write($"{((int) (10000.0 * finLines / ImageHeight) / 100.0).ToString()}%");
                Console.SetCursorPosition(0, Console.CursorTop);
            }
        }

        static void Main(string[] args)
        {
            Console.Write($"Start {DateTime.Now}\n");
            
            var fileName = $"./hoge_{SamplesPerPixel.ToString()}spp.ppm";
            
            var world = GenerateRandomScene(3);
            var lookFrom = new Vector3(13, 2, 3);
            var lookAt = new Vector3(0, 0, 0);
            var viewUp = Vector3.UnitY;
            var distToFocus = 10;
            var aperture = 0.1f;
            var camera = new Camera(lookFrom, lookAt, viewUp, 20, AspectRatio, aperture, distToFocus, 0, 1);
            
            using var streamWriter = new StreamWriter(fileName, false);
            var pixelColors = new Vector3[ImageWidth * ImageHeight];

            #region ヘッダー書き込み

            streamWriter.Write($"P3\n{ImageWidth.ToString()} {ImageHeight.ToString()}\n255\n");

            #endregion

            // 可視化用Task開始
            Task.Run(DebugProgress);
            
            var options = new ParallelOptions {MaxDegreeOfParallelism = 4};
            Parallel.For(0, ImageHeight - 1, options, j =>
            {
                // make mathHelper instance 
                var mathHelper = new MathHelper();

                for (var i = 0; i < ImageWidth; i++)
                {
                    // var hitRecord = new HitRecord();
                    pixelColors[i] = Vector3.Zero;
                    var pixelColor = Vector3.Zero;
                    for (var s = 0; s < SamplesPerPixel; s++)
                    {
                        var u = (float) (i + mathHelper.Random()) / (ImageWidth - 1);
                        var v = (float) (j + mathHelper.Random()) / (ImageHeight - 1);

                        var r = camera.GetRay(u, v);
                        pixelColor += RayColor(r, world, MaxDepth, mathHelper);
                    }

                    pixelColors[i + j * ImageWidth] = pixelColor;
                }

                finLines++;
            });
            for (var j = ImageHeight-1; j >=0; j--)
            {
                for (var i = 0; i < ImageWidth; i++)
                {
                    var pixelColor = pixelColors[i + j * ImageWidth];
                    WriteColor(streamWriter, pixelColor, SamplesPerPixel);
                }
            }

            Console.Write($"Finish!! {DateTime.Now}\n");
        }
    }
}