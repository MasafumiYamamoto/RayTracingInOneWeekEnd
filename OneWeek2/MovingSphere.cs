using System;
using System.Numerics;

namespace OneWeek2
{
    public class MovingSphere: IHittable
    {
        public Vector3 Center0 { get; }
        public Vector3 Center1 { get; }
        public float Time0 { get; }
        public float Time1 { get; }
        public float Radius { get; }
        
        public Material Material { get; }

        public MovingSphere(Vector3 center0,Vector3 center1,
            float time0, float time1,
            float radius, Material material)
        {
            Center0 = center0;
            Center1 = center1;
            Time0 = time0;
            Time1 = time1;
            Radius = radius;
            Material = material;
        }
        
        public bool Hit(in Ray ray, in float tMin, in float tMax, ref HitRecord rec)
        {
            var oc = ray.Origin - Center(ray.Time);
            var a = ray.Direction.LengthSquared();
            var halfB = Vector3.Dot(oc, ray.Direction);
            var c = oc.LengthSquared() - Radius * Radius;

            var discriminant = halfB * halfB - a * c;
            
            if (discriminant>0)
            {
                var root = MathF.Sqrt(discriminant);

                var temp = (-halfB - root) / a;
                if (temp<tMax && temp>tMin)
                {
                    rec.T = temp;
                    rec.P = ray.At(rec.T);
                    var outwardNormal = (rec.P - Center(ray.Time)) / Radius;
                    rec.SetFaceNormal(ray, outwardNormal);
                    rec.Material = Material;
                    return true;
                }

                temp = (-halfB + root) / a;
                if (temp<tMax&& temp>tMin)
                {
                    rec.T = temp;
                    rec.P = ray.At(rec.T);
                    var outwardNormal = (rec.P - Center(ray.Time)) / Radius;
                    rec.SetFaceNormal(ray, outwardNormal);
                    rec.Material = Material;
                    return true;
                }
            }

            return false;
        }

        private Vector3 Center(float time)
        {
            return Center0 + ((time - Time0) / (Time1 - Time0)) * (Center1 - Center0);
        }
    }
}