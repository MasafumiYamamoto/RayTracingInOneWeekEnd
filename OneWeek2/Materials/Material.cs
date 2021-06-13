using System.Numerics;

namespace OneWeek2
{
    public abstract class Material
    {
        public Vector3 Albedo { get; }
        
        public Material(Vector3 albedo)
        {
            Albedo = albedo;
        }

        public abstract bool Scatter(in Ray ray, in HitRecord hitRecord, out Vector3 attenuation, out Ray scattered);
    }
}