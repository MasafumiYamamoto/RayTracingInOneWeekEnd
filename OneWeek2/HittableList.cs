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
    }
}