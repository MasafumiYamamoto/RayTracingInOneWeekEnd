using System;
using System.Numerics;

namespace OneWeek2.Materials
{
    /// <summary>
    /// 誘電体マテリアル
    /// </summary>
    public class Dielectric : Material
    {
        public float RefIdx { get; }
        
        public Dielectric(float refIdx)
        {
            RefIdx = refIdx;
        }

        public override bool Scatter(in Ray ray, in HitRecord hitRecord, out Vector3 attenuation, out Ray scattered)
        {
            attenuation = Vector3.One;
            float etaIOverEtaT = 0;
            if (hitRecord.FrontFace)
            {
                etaIOverEtaT = 1 / RefIdx;
            }
            else
            {
                etaIOverEtaT = RefIdx;
            }
            
            var unitDirection = Vector3.Normalize(ray.Direction);

            var cos = MathF.Min(Vector3.Dot(-unitDirection, hitRecord.Normal), 1);
            var sin = MathF.Sqrt(1 - cos * cos);
            
            // 全反射
            if (etaIOverEtaT * sin > 1)
            {
                var reflected = MathHelper.Reflect(unitDirection, hitRecord.Normal);
                scattered = new Ray(hitRecord.P, reflected);
                return true;
            }

            var reflectProb = MathHelper.Schlick(cos, etaIOverEtaT);
            if (MathHelper.Random() < reflectProb)
            {
                var reflected = MathHelper.Reflect(unitDirection, hitRecord.Normal);
                scattered = new Ray(hitRecord.P, reflected);
                return true;
            }
            var refracted = MathHelper.Refract(unitDirection, hitRecord.Normal, etaIOverEtaT);
            scattered = new Ray(hitRecord.P, refracted);
            return true;
        }
    }
}