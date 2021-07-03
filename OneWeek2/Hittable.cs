using System.Numerics;

namespace OneWeek2
{
    public class HitRecord
    {
        public float T;
        public Vector3 P;
        public Vector3 Normal;
        public bool FrontFace;
        public Material Material;

        public void SetFaceNormal(Ray ray, Vector3 outwardNormal)
        {
            FrontFace = Vector3.Dot(ray.Direction, outwardNormal) < 0;
            Normal = FrontFace ? outwardNormal : -outwardNormal;
        }

        public void Clear()
        {
            T = 0;
            P = Vector3.Zero;
            Normal = Vector3.UnitY;
            FrontFace = true;
            Material = default;
        }
    }
    
    public interface IHittable
    {
        bool Hit(in Ray ray, in float tMin, in float tMax, ref HitRecord rec);
    }
}