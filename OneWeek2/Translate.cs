using System.Numerics;

namespace OneWeek2
{
    /// <summary>
    /// インスタンスの移動を制御するクラス
    /// </summary>
    public class Translate : IHittable
    {
        private readonly Vector3 _offset;
        private readonly IHittable _hittable;

        public Translate(IHittable hittable, in Vector3 displacement)
        {
            _hittable = hittable;
            _offset = displacement;
        }

        public bool Hit(in Ray ray, in float tMin, in float tMax, ref HitRecord rec)
        {
            var movedRay = new Ray(ray.Origin - _offset, ray.Direction, ray.Time);
            if (!_hittable.Hit(movedRay, tMin, tMax, ref rec))
            {
                return false;
            }

            rec.P += _offset;
            rec.SetFaceNormal(movedRay, rec.Normal);
            return true;
        }

        public bool BoundingBox(float t0, float t1, ref AABB outputBox)
        {
            if (!_hittable.BoundingBox(t0, t1, ref outputBox))
            {
                return false;
            }

            outputBox = new AABB(outputBox.Min + _offset, outputBox.Max + _offset);
            return true;
        }
    }
}