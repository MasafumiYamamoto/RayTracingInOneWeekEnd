using System.Numerics;
using OneWeek2.Materials;

namespace OneWeek2
{
    /// <summary>
    /// 辺が軸に平行な直方体
    /// </summary>
    public class Box : IHittable
    {
        private readonly Vector3 _boxMin;
        private readonly Vector3 _boxMax;
        private readonly HittableList _sides;
        
        public Box(in Vector3 p0, in Vector3 p1, Material material)
        {
            _boxMin = p0;
            _boxMax = p1;
            _sides = new HittableList();

            _sides.Objects.Add(new XYRect(p0.X, p1.X, p0.Y, p1.Y, p1.Z, material));
            _sides.Objects.Add(new XYRect(p0.X, p1.X, p0.Y, p1.Y, p0.Z, material));

            _sides.Objects.Add(new XZRect(p0.X, p1.X, p0.Z, p1.Z, p1.Y, material));
            _sides.Objects.Add(new XZRect(p0.X, p1.X, p0.Z, p1.Z, p0.Y, material));
            
            _sides.Objects.Add(new YZRect(p0.Y, p1.Y, p0.Z, p1.Z, p1.X, material));
            _sides.Objects.Add(new YZRect(p0.Y, p1.Y, p0.Z, p1.Z, p0.X, material));
        }
        
        
        public bool Hit(in Ray ray, in float tMin, in float tMax, ref HitRecord rec)
        {
            return _sides.Hit(ray, tMin, tMax, ref rec);
        }

        public bool BoundingBox(float t0, float t1, ref AABB outputBox)
        {
            outputBox = new AABB(_boxMin, _boxMax);
            return true;
        }
    }
}