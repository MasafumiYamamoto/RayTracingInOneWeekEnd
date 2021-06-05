using System;

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
    }
}