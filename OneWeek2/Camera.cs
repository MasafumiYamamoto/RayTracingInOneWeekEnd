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
        
        public float LensRadius { get; }
        
        /// <summary>
        /// シャッターの開閉開始時間
        /// </summary>
        public float Time0 { get; }
        /// <summary>
        /// シャッターの開閉終了時間
        /// </summary>
        public float Time1 { get; }
        public Vector3 U { get; }
        
        public Vector3 V { get; }
        
        public Vector3 W { get; }

        private readonly MathHelper _mathHelper = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lookFrom">カメラ位置</param>
        /// <param name="lookAt">カメラ注視点</param>
        /// <param name="viewUp">カメラ上ベクトル</param>
        /// <param name="vFov">垂直視野(角度)</param>
        /// <param name="aspectRatio">アスペクト比率</param>
        /// <param name="aperture"></param>
        /// <param name="focusDistance"></param>
        /// <param name="time0"></param>
        /// <param name="time1"></param>
        public Camera(Vector3 lookFrom, Vector3 lookAt, Vector3 viewUp, float vFov, float aspectRatio, float aperture, float focusDistance, float time0, float time1)
        {
            var theta = _mathHelper.Degree2Radian(vFov);
            var h = MathF.Tan(theta / 2);
            var viewportHeight = 2 * h;
            var viewportWidth = aspectRatio * viewportHeight;

            W = Vector3.Normalize(lookFrom - lookAt);
            U = Vector3.Normalize(Vector3.Cross(viewUp, W));
            V = Vector3.Cross(W, U);

            Origin = lookFrom;
            Horizontal = focusDistance * viewportWidth * U;
            Vertical = focusDistance * viewportHeight * V;
            LowerLeftCorner = Origin - Horizontal / 2 - Vertical / 2 - focusDistance * W;

            LensRadius = aperture / 2;
            Time0 = time0;
            Time1 = time1;
        }

        public Ray GetRay(float s, float t)
        {
            var rd = LensRadius * _mathHelper.RandomInUnitDisc();
            var offset = U * rd.X + V * rd.Y;

            return new Ray(Origin + offset,
                LowerLeftCorner + s * Horizontal + t * Vertical - Origin - offset,
                _mathHelper.Random(Time0,Time1));
        }
    }
}