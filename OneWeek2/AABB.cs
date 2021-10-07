using System;
using System.Numerics;

namespace OneWeek2
{
    public class AABB
    {
        public Vector3 Min { get; }

        public Vector3 Max { get; }

        public AABB()
        {
        }
        
        public AABB(in Vector3 min, in Vector3 max)
        {
            Min = min;
            Max = max;
        }

        /// <summary>
        /// AABBとレイの交差判定
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="tMin">AABBのMin境界との現在の交差点</param>
        /// <param name="tMax">AABBのMax境界との現在の交差点</param>
        /// <returns></returns>
        public bool Hit(in Ray ray, float tMin, float tMax)
        {
            tMin = MathF.Min(CalcHitPoint(Min.X, Max.X, ray.Origin.X, ray.Direction.X, true), tMin);
            tMax = MathF.Max(CalcHitPoint(Min.X, Max.X, ray.Origin.X, ray.Direction.X, false), tMax);
            if (tMax <= tMin)
            {
                return false;
            }
            
            tMin = MathF.Min(CalcHitPoint(Min.Y, Max.Y, ray.Origin.Y, ray.Direction.Y, true), tMin);
            tMax = MathF.Max(CalcHitPoint(Min.Y, Max.Y, ray.Origin.Y, ray.Direction.Y, false), tMax);
            if (tMax <= tMin)
            {
                return false;
            }
            
            tMin = MathF.Min(CalcHitPoint(Min.Z, Max.Z, ray.Origin.Z, ray.Direction.Z, true), tMin);
            tMax = MathF.Max(CalcHitPoint(Min.Z, Max.Z, ray.Origin.Z, ray.Direction.Z, false), tMax);
            if (tMax <= tMin)
            {
                return false;
            }
            return true;
        }
        
        public static AABB SurroundingBox(AABB box0, AABB box1)
        {
            var small = new Vector3(MathF.Min(box0.Min.X, box1.Min.X),
                MathF.Min(box0.Min.Y, box1.Max.Y),
                MathF.Min(box0.Min.Z, box1.Max.Z));
            var big = new Vector3(MathF.Max(box0.Min.X, box1.Min.X),
                MathF.Max(box0.Min.Y, box1.Max.Y),
                MathF.Max(box0.Min.Z, box1.Max.Z));
            return new AABB(small, big);
        }

        private float CalcHitPoint(float minValue, float maxValue, float rayOrigin, float rayDirection, bool isMin)
        {
            if (isMin)
            {
                return MathF.Min((minValue - rayOrigin) / rayDirection, (maxValue - rayOrigin) / rayDirection);
            }
            
            return MathF.Max((minValue - rayOrigin) / rayDirection, (maxValue - rayOrigin) / rayDirection);
        }
    }
}