using System.Collections.Generic;

namespace OneWeek2
{
    public class HittableList : IHittable
    {
        public List<IHittable> Objects { get; } = default;

        public HittableList()
        {
            Objects = new List<IHittable>();
        }
        
        public bool Hit(in Ray ray, in float tMin, in float tMax, ref HitRecord rec)
        {
            var tmpRecord = new HitRecord();
            var hitAnything = false;
            var closestSoFar = tMax;
            
            foreach (var hittable in Objects)
            {
                if (hittable.Hit(ray, tMin, closestSoFar, ref tmpRecord))
                {
                    hitAnything = true;
                    closestSoFar = tmpRecord.T;
                    rec = tmpRecord;
                }
            }
            

            return hitAnything;
        }

        public bool BoundingBox(float t0, float t1, ref AABB outputBox)
        {
            if (Objects.Count==0)
            {
                return false;
            }

            AABB tempBox = new AABB();
            var firstBox = true;
            
            foreach (var hittable in Objects)
            {
                if (!hittable.BoundingBox(t0, t1, ref tempBox))
                {
                    return false;
                }

                outputBox = firstBox ? tempBox : AABB.SurroundingBox(tempBox, outputBox);
                firstBox = false;
            }

            return true;
        }
    }
}