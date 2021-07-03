using System.Numerics;

namespace OneWeek2
{
    public abstract class Material
    {
        public abstract bool Scatter(in Ray ray, in HitRecord hitRecord, out Vector3 attenuation, out Ray scattered, MathHelper mathHelper);
    }
}