using System;
using System.Numerics;

namespace OneWeek2
{
    public class Camera
    {
        public Vector3 Origin { get; }
        public Vector3 LowerLeftCorner { get; }
        public Vector3 Horizontal { get; }
        public Vector3 Vertical { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lookFrom">カメラ位置</param>
        /// <param name="lookAt">カメラ注視点</param>
        /// <param name="viewUp">カメラ上ベクトル</param>
        /// <param name="vFov">垂直視野(角度)</param>
        /// <param name="aspectRatio">アスペクト比率</param>
        public Camera(Vector3 lookFrom, Vector3 lookAt, Vector3 viewUp, float vFov, float aspectRatio)
        {
            var theta = MathHelper.Degree2Radian(vFov);
            var h = MathF.Tan(theta / 2);
            var viewportHeight = 2 * h;
            var viewportWidth = aspectRatio * viewportHeight;

            var w = Vector3.Normalize(lookFrom - lookAt);
            var u = Vector3.Normalize(Vector3.Cross(viewUp, w));
            var v = Vector3.Cross(w, u);

            Origin = lookFrom;
            Horizontal = viewportWidth * u;
            Vertical = viewportHeight * v;
            LowerLeftCorner = Origin - Horizontal / 2 - Vertical / 2 - w;
        }

        public Ray GetRay(float u, float v)
        {
            return new Ray(Origin, LowerLeftCorner + u * Horizontal + v * Vertical - Origin);
        }
    }
}