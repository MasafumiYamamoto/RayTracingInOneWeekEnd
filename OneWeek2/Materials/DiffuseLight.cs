using System.Numerics;

namespace OneWeek2.Materials
{
    public class DiffuseLight : Material
    {
        public ITexture Emit { get; set; }

        public DiffuseLight(ITexture t)
        {
            Emit = t;
        }

        public override bool Scatter(in Ray ray, in HitRecord hitRecord, out Vector3 attenuation, out Ray scattered, MathHelper mathHelper)
        {
            // TODO:こいつらの設定仮なのでどこかで消したい
            attenuation = Vector3.One;
            scattered = new Ray(ray.Origin, ray.Direction);
            return false;
        }

        public override Vector3 Emitted(float u, float v, in Vector3 p)
        {
            return Emit.Value(u, v, p);
        }
    }
}