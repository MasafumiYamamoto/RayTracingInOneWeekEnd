using System;
using System.Numerics;

namespace OneWeek2
{
    public class Sphere : IHittable
    {
        private readonly Vector3 _center;
        private readonly float _radius;
        private readonly Material _material;

        public Vector3 Center => _center;

        public float Radius => _radius;

        public Material Material => _material;
        
        public Sphere(Vector3 cen, float r, Material material)
        {
            _center = cen;
            _radius = r;
            _material = material;
        }
        
        public bool Hit(in Ray ray, in float tMin, in float tMax, ref HitRecord rec)
        {
            var oc = ray.Origin - _center;
            var a = ray.Direction.LengthSquared();
            var halfB = Vector3.Dot(oc, ray.Direction);
            var c = oc.LengthSquared() - _radius * _radius;
            var discriminant = halfB * halfB - a * c;
            
            if (discriminant > 0)
            {
                var root = MathF.Sqrt(discriminant);
                var tmp = (-halfB - root) / a;
                if (tmp < tMax && tmp > tMin)
                {
                    rec.T = tmp;
                    rec.P = ray.At(rec.T);
                    var outwardNormal = (rec.P - _center) / _radius;
                    rec.SetFaceNormal(ray, outwardNormal);
                    rec.Material = _material;
                    return true;
                }

                tmp = (-halfB + root) / a;
                if (tmp < tMax && tmp > tMin)
                {
                    rec.T = tmp;
                    rec.P = ray.At(rec.T);
                    var outwardNormal = (rec.P - _center) / _radius;
                    rec.SetFaceNormal(ray, outwardNormal);
                    rec.Material = _material;
                    return true;
                }
            }

            return false;
        }

        public bool BoundingBox(float t0, float t1, ref AABB outputBox)
        {
            outputBox = new AABB(_center - new Vector3(_radius, _radius, _radius),
                _center + new Vector3(_radius, _radius, _radius));
            return true;
        }
    }
}