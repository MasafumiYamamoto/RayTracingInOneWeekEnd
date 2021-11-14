using System.Numerics;

namespace OneWeek2.Materials
{
    public abstract class Material
    {
        public abstract bool Scatter(in Ray ray, in HitRecord hitRecord, out Vector3 attenuation, out Ray scattered, MathHelper mathHelper);

        public virtual Vector3 Emitted(float u, float v, in Vector3 p)
        {
            return Vector3.Zero;
        }
    }
}