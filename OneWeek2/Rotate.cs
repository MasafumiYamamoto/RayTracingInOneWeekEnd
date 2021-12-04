using System;
using System.Numerics;

namespace OneWeek2
{
    /// <summary>
    /// Y軸回りに回転させるクラス
    /// </summary>
    public class RotateY : IHittable
    {
        private readonly IHittable _hittable;
        private readonly float _sinTheta;
        private readonly float _cosTheta;
        private readonly bool _hasBox;
        private readonly AABB _bBox;

        public RotateY(IHittable p, float angle, MathHelper mathHelper)
        {
            _hittable = p;
            var radians = mathHelper.Degree2Radian(angle);
            _sinTheta = MathF.Sin(radians);
            _cosTheta = MathF.Cos(radians);
            _hasBox = _hittable.BoundingBox(0, 1, ref _bBox);
            var minVec = float.MaxValue * Vector3.One;
            var maxVec = float.MinValue * Vector3.One;

            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < 2; j++)
                {
                    for (var k = 0; k < 2; k++)
                    {
                        var x = i * _bBox.Max.X + (1 - i) * _bBox.Min.X;
                        var y = j * _bBox.Max.Y + (1 - j) * _bBox.Min.Y;
                        var z = k * _bBox.Max.Z + (1 - k) * _bBox.Min.Z;
                        
                        // ワールド空間に移す
                        var worldX = _cosTheta * x + _sinTheta * z;
                        var worldZ = -_sinTheta * x + _cosTheta * z;

                        var tester = new Vector3(worldX, y, worldZ);

                        minVec.X = MathF.Min(minVec.X, tester.X);
                        minVec.Y = MathF.Min(minVec.Y, tester.Y);
                        minVec.Z = MathF.Min(minVec.Z, tester.Z);

                        maxVec.X = MathF.Max(maxVec.X, tester.X);
                        maxVec.Y = MathF.Max(maxVec.Y, tester.Y);
                        maxVec.Z = MathF.Max(maxVec.Z, tester.Z);
                    }
                }
            }

            _bBox = new AABB(minVec, maxVec);
        }

        public bool Hit(in Ray ray, in float tMin, in float tMax, ref HitRecord rec)
        {
            var origin = ray.Origin;
            var direction = ray.Direction;
            // 一旦逆変換かけてローカル座標に移す
            origin.X = _cosTheta * ray.Origin.X - _sinTheta * ray.Origin.Z;
            origin.Z = _sinTheta * ray.Origin.X + _cosTheta * ray.Origin.Z;
            direction.X = _cosTheta * ray.Direction.X - _sinTheta * ray.Direction.Z;
            direction.Z = _sinTheta * ray.Direction.X + _cosTheta * ray.Direction.Z;

            var rotatedRay = new Ray(origin, direction, ray.Time);

            if (!_hittable.Hit(rotatedRay, tMin, tMax, ref rec))
            {
                return false;
            }

            var p = rec.P;
            var normal = rec.Normal;

            // 元のワールド空間に戻す
            p.X = _cosTheta * rec.P.X + _sinTheta * rec.P.Z;
            p.Z = -_sinTheta * rec.P.X + _cosTheta * rec.P.Z;

            normal.X = _cosTheta * rec.Normal.X + _sinTheta * rec.Normal.Z;
            normal.Z = -_sinTheta * rec.Normal.X + _cosTheta * rec.Normal.Z;

            rec.P = p;
            rec.SetFaceNormal(rotatedRay, normal);

            return true;
        }

        public bool BoundingBox(float t0, float t1, ref AABB outputBox)
        {
            outputBox = _bBox;
            return _hasBox;
        }
    }
}