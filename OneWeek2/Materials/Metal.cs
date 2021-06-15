using System.Numerics;

namespace OneWeek2.Materials
{
    public class Metal : Material
    {
        public Vector3 Albedo { get; }
        public float Fuzz { get; }
        
        public Metal(Vector3 albedo, float fuzz)
        {
            Albedo = albedo;
            Fuzz = fuzz < 1 ? fuzz : 1;
        }

        public override bool Scatter(in Ray ray, in HitRecord hitRecord, out Vector3 attenuation, out Ray scattered)
        {
            var reflected = MathHelper.Reflect(Vector3.Normalize(ray.Direction), hitRecord.Normal);
            scattered = new Ray(hitRecord.P, reflected + Fuzz * MathHelper.RandomInUnitSphere());
            attenuation = Albedo;
            return Vector3.Dot(scattered.Direction, hitRecord.Normal) > 0;
        }
    }
}