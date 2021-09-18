using System.Numerics;

namespace OneWeek2.Materials
{
    public class Lambertian : Material
    {
        public Vector3 Albedo { get; }

        public Lambertian(Vector3 albedo)
        {
            Albedo = albedo;
        }
        
        public override bool Scatter(in Ray ray, in HitRecord hitRecord, out Vector3 attenuation, out Ray scattered,
            MathHelper mathHelper)
        {
            var scatterDirection = hitRecord.Normal + mathHelper.RandomUnitVector();
            scattered = new Ray(hitRecord.P, scatterDirection, ray.Time);
            attenuation = Albedo;
            return true;
        }
    }
}