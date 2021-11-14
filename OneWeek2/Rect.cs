using System.Numerics;
using OneWeek2.Materials;

namespace OneWeek2
{
    public class XYRect : IHittable
    {
        private readonly float _x0;
        private readonly float _x1;
        private readonly float _y0;
        private readonly float _y1;
        private readonly float _k;
        private readonly Material _material;

        public XYRect(float x0, float x1, float y0, float y1, float k, Material material)
        {
            _x0 = x0;
            _x1 = x1;
            _y0 = y0;
            _y1 = y1;
            _k = k;
            _material = material;
        }

        public bool Hit(in Ray ray, in float tMin, in float tMax, ref HitRecord rec)
        {
            var t = (_k - ray.Origin.Z) / ray.Direction.Z;
            if (t < tMin || t > tMax)
            {
                return false;
            }

            var x = ray.Origin.X + t * ray.Direction.X;
            var y = ray.Origin.Y + t * ray.Direction.Y;
            if (x < _x0 || x > _x1 || y < _y0 || y > _y1)
            {
                return false;
            }

            rec.U = (x - _x0) / (_x1 - _x0);
            rec.V = (y - _y0) / (_y1 - _y0);
            rec.T = t;

            var outwardNormal = Vector3.UnitZ;
            rec.SetFaceNormal(ray, outwardNormal);
            rec.Material = _material;
            rec.P = ray.At(t);
            return true;
        }

        public bool BoundingBox(float t0, float t1, ref AABB outputBox)
        {
            // AABBの辺の長さを0にすると都合が悪いのでZ方向に少しだけ厚みを持たせる
            outputBox = new AABB(new Vector3(_x0, _y0, _k - 0.001f), new Vector3(_x1, _y1, _k + 0.001f));
            return true;
        }
    }

    public class XZRect : IHittable
    {
        private readonly float _x0;
        private readonly float _x1;
        private readonly float _z0;
        private readonly float _z1;
        private readonly float _k;
        private readonly Material _material;

        public XZRect(float x0, float x1, float y0, float y1, float k, Material material)
        {
            _x0 = x0;
            _x1 = x1;
            _z0 = y0;
            _z1 = y1;
            _k = k;
            _material = material;
        }

        public bool Hit(in Ray ray, in float tMin, in float tMax, ref HitRecord rec)
        {
            var t = (_k - ray.Origin.Y) / ray.Direction.Y;
            if (t < tMin || t > tMax)
            {
                return false;
            }

            var x = ray.Origin.X + t * ray.Direction.X;
            var z = ray.Origin.Z + t * ray.Direction.Z;
            if (x < _x0 || x > _x1 || z < _z0 || z > _z1)
            {
                return false;
            }

            rec.U = (x - _x0) / (_x1 - _x0);
            rec.V = (z - _z0) / (_z1 - _z0);
            rec.T = t;

            var outwardNormal = Vector3.UnitY;
            rec.SetFaceNormal(ray, outwardNormal);
            rec.Material = _material;
            rec.P = ray.At(t);
            return true;
        }

        public bool BoundingBox(float t0, float t1, ref AABB outputBox)
        {
            // AABBの辺の長さを0にすると都合が悪いのでy方向に少しだけ厚みを持たせる
            outputBox = new AABB(new Vector3(_x0, _k - 0.001f, _z0), new Vector3(_x1, _k + 0.001f, _z1));
            return true;
        }
    }

    public class YZRect : IHittable
    {
        private readonly float _y0;
        private readonly float _y1;
        private readonly float _z0;
        private readonly float _z1;
        private readonly float _k;
        private readonly Material _material;

        public YZRect(float x0, float x1, float y0, float y1, float k, Material material)
        {
            _z0 = x0;
            _z1 = x1;
            _y0 = y0;
            _y1 = y1;
            _k = k;
            _material = material;
        }

        public bool Hit(in Ray ray, in float tMin, in float tMax, ref HitRecord rec)
        {
            var t = (_k - ray.Origin.X) / ray.Direction.X;
            if (t < tMin || t > tMax)
            {
                return false;
            }

            var z = ray.Origin.Z + t * ray.Direction.Z;
            var y = ray.Origin.Y + t * ray.Direction.Y;
            if (z < _z0 || z > _z1 || y < _y0 || y > _y1)
            {
                return false;
            }

            rec.U = (z - _z0) / (_z1 - _z0);
            rec.V = (y - _y0) / (_y1 - _y0);
            rec.T = t;

            var outwardNormal = Vector3.UnitX;
            rec.SetFaceNormal(ray, outwardNormal);
            rec.Material = _material;
            rec.P = ray.At(t);
            return true;
        }

        public bool BoundingBox(float t0, float t1, ref AABB outputBox)
        {
            // AABBの辺の長さを0にすると都合が悪いのでX方向に少しだけ厚みを持たせる
            outputBox = new AABB(new Vector3(_k - 0.001f, _y0, _z0), new Vector3(_k + 0.001f, _y1, _z1));
            return true;
        }
    }
}