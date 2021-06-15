using System;
using System.Numerics;

namespace OneWeek2
{
    static class MathHelper
    {
        private static readonly Random RandomInstance = new Random();
        
        public static float Degree2Radian(float degree)
        {
            return (float) (degree * Math.PI / 180);
        }

        /// <summary>
        /// [min, max)の値を返す
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Random(float min=0, float max=1)
        {
            return min + (max - min) * (float) RandomInstance.NextDouble();
        }

        /// <summary>
        /// [min, max]に値をクランプする
        /// </summary>
        /// <param name="x"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Clamp(float x, float min, float max)
        {
            return Math.Clamp(x, min, max);
        }

        private static Vector3 Vector3Random(float min, float max)
        {
            return new(Random(min, max), Random(min, max), Random(min, max));
        }

        public static Vector3 RandomInUnitSphere()
        {
            while (true)
            {
                var p = Vector3Random(-1, 1);
                if (p.LengthSquared()>=1)
                {
                    continue;
                }

                return p;
            }
        }

        public static Vector3 RandomInHemisphere(in Vector3 normal)
        {
            var inUnitSphere = RandomInUnitSphere();
            if (Vector3.Dot(inUnitSphere, normal)>0)
            {
                // inUnitSphereがnormalと同じ方向にあれば採用
                return inUnitSphere; 
            }

            return -inUnitSphere;
        }

        public static Vector3 RandomUnitVector()
        {
            var a = Random(0, 2 * MathF.PI);
            var z = Random(-1, 1);
            var r = MathF.Sqrt(1 - z * z);
            return new Vector3(r * MathF.Cos(a), r * MathF.Sin(a), z);
        }

        public static Vector3 Reflect(in Vector3 v, in Vector3 n)
        {
            return v - 2 * Vector3.Dot(v, n) * n;
        }

        /// <summary>
        /// スネルの法則に基づく屈折の式
        /// </summary>
        /// <param name="uv"></param>
        /// <param name="n"></param>
        /// <param name="etaIOverEatT"></param>
        /// <returns></returns>
        public static Vector3 Refract(in Vector3 uv, in Vector3 n, float etaIOverEatT)
        {
            var cos = MathF.Min(Vector3.Dot(-uv, n), 1);
            var rOutParallel = etaIOverEatT * (uv + cos * n);
            var rOutPrep = -MathF.Sqrt(1 - rOutParallel.LengthSquared()) * n;
            return rOutParallel + rOutPrep;
        }

        public static float Schlick(float cos, float refIdx)
        {
            var r0 = (1 - refIdx) / (1 + refIdx);
            r0 *= r0;
            return r0 + (1 - r0) * MathF.Pow(1 - cos, 5);
        }
    }
}