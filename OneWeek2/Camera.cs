using System.Numerics;

namespace OneWeek2
{
    public class Camera
    {
        private const float AspectRatio = 16f / 9f;
        private const float ViewportHeight = 2f;
        private const float ViewportWidth = AspectRatio * ViewportHeight;
        private const float FocalLength = 1f;

        public Vector3 Origin { get; }
        public Vector3 LowerLeftCorner { get; }
        public Vector3 Horizontal { get; }
        public Vector3 Vertical { get; }
        
        public Camera()
        {
            Origin = Vector3.Zero;
            Horizontal = new Vector3(ViewportWidth, 0, 0);
            Vertical = new Vector3(0, ViewportHeight, 0);
            LowerLeftCorner = Origin - Horizontal / 2 - Vertical / 2 - new Vector3(0, 0, FocalLength);
        }

        public Ray GetRay(float u, float v)
        {
            return new Ray(Origin, LowerLeftCorner + u * Horizontal + v * Vertical - Origin);
        }
    }
}