using System;
using System.Collections.Generic;

namespace OneWeek2
{
    public class BVHNode : IHittable
    {
        public IHittable Left { get; }
        public IHittable Right { get; }
        public AABB Box { get; }
        
        private readonly MathHelper _mathHelper = new();
        private int axis;

        public BVHNode(ref List<IHittable> objects, float time0, float time1)
        {
            axis = _mathHelper.RandomInt(0, 2);
            
            if (objects.Count == 1)
            {
                Left = objects[0];
                Right = objects[0];
            }else if (objects.Count == 2)
            {
                if (BoxCompare(objects[0], objects[1]) > 0)
                {
                    Left = objects[0];
                    Right = objects[1];
                }
                else
                {
                    Left = objects[1];
                    Right = objects[0];
                }
            }
            else
            {
                // 要素が3つ以上ならオブジェクトリストをソートして分割する
                objects.Sort(BoxCompare);
                var leftList = objects.GetRange(0, objects.Count / 2);
                var rightList = objects.GetRange(objects.Count / 2, objects.Count - objects.Count / 2);
                Left = new BVHNode(ref leftList, time0, time1);
                Right = new BVHNode(ref rightList, time0, time1);
            }

            var boxLeft = new AABB();
            var boxRight = new AABB();
            if (!Left.BoundingBox(time0, time1, ref boxLeft) || !Right.BoundingBox(time0, time1, ref boxRight))
            {
                // Console.WriteLine("No bounding box in BVH constructor");
            }

            Box = AABB.SurroundingBox(boxLeft, boxRight);
        }
        
        public bool Hit(in Ray ray, in float tMin, in float tMax, ref HitRecord rec)
        {
            // AABBと衝突していなければ非採用確定
            if (!Box.Hit(ray, tMin, tMax))
            {
                return false;
            }

            // AABBと衝突していれば子を更に見ていく
            var hitLeft = Left.Hit(ray, tMin, tMax, ref rec);
            var hitRight = Right.Hit(ray, tMin, tMax, ref rec);

            return hitLeft || hitRight;
        }

        public bool BoundingBox(float t0, float t1, ref AABB outputBox)
        {
            outputBox = Box;
            return true;
        }

        private int BoxCompare(IHittable a, IHittable b)
        {
            if (BoxCompare(a, b, axis))
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        private bool BoxCompare(IHittable a, IHittable b, int compareAxis)
        {
            var boxA = new AABB();
            var boxB = new AABB();
            if (!a.BoundingBox(0, 0, ref boxA) || !b.BoundingBox(0, 0, ref boxB))
            {
                // Console.WriteLine("No Bounding Box in BVH node constructor");
            }
            
            switch (compareAxis)
            {
                case 0:
                    return boxA.Min.X < boxB.Min.X;
                case 1:
                    return boxA.Min.Y < boxB.Max.Y;
                case 2:
                    return boxA.Min.Z < boxB.Max.Z;
                default:
                    throw new InvalidOperationException($"Invalid axis {compareAxis}");
            }
        }

        private int BoxXCompare(IHittable a, IHittable b)
        {
            return BoxCompare(a, b, 0) ? 1 : 0;
        }

        private int BoxYCompare(IHittable a, IHittable b)
        {
            return BoxCompare(a, b, 1) ? 1 : 0;
        }

        private int BoxZCompare(IHittable a, IHittable b)
        {
            return BoxCompare(a, b, 2) ? 1 : 0;
        }
    }
}