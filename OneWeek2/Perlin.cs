using System;
using System.Collections.Generic;
using System.Numerics;

namespace OneWeek2
{
    /// <summary>
    /// PerlinNoise
    /// </summary>
    public class Perlin : IDisposable
    {
        public Perlin()
        {
            var mathHelper = new MathHelper();
            _ranVec = new List<Vector3>(PointCount);
            _ranFloat = new List<float>(PointCount);
            for (var i = 0; i < PointCount; i++)
            {
                _ranVec.Add(mathHelper.RandomInUnitSphere());
                _ranFloat.Add(mathHelper.Random());
            }

            _permX = PerlinGeneratePerm(mathHelper);
            _permY = PerlinGeneratePerm(mathHelper);
            _permZ = PerlinGeneratePerm(mathHelper);
        }

        public float Noise(in Vector3 p)
        {
            var u = p.X - MathF.Floor(p.X);
            var v = p.Y - MathF.Floor(p.Y);
            var w = p.Z - MathF.Floor(p.Z);
            u = Interp(u);
            v = Interp(v);
            w = Interp(w);
            var i = (int)MathF.Floor(p.X);
            var j = (int)MathF.Floor(p.Y);
            var k = (int)MathF.Floor(p.Z);
            var c = new Vector3[2, 2, 2];

            for (var di = 0; di < 2; di++)
            {
                for (var dj = 0; dj < 2; dj++)
                {
                    for (var dk = 0; dk < 2; dk++)
                    {
                        c[di, dj, dk] =
                            _ranVec[_permX[(i + di) & 255] ^ _permY[(j + dj) & 255] ^ _permZ[(k + dk) & 255]];
                    }
                }
            }

            return PerlinInterp(c, u, v, w);
        }

        private const int PointCount = 256;
        private readonly List<float> _ranFloat;
        private readonly List<int> _permX;
        private readonly List<int> _permY;
        private readonly List<int> _permZ;
        private readonly List<Vector3> _ranVec;

        public void Dispose()
        {
            _ranFloat.Clear();
            _permX.Clear();
            _permY.Clear();
            _permZ.Clear();
            _ranVec.Clear();
        }

        public float Turb(in Vector3 p, int depth = 7)
        {
            var accum = 0f;
            var tempP = p;
            var weight = 1f;

            for (var i = 0; i < depth; i++)
            {
                accum += weight * Noise(tempP);
                weight *= 0.5f;
                tempP *= 2;
            }

            return MathF.Abs(accum);
        }

        private List<int> PerlinGeneratePerm(MathHelper mathHelper)
        {
            var p = new List<int>(PointCount);
            for (var i = 0; i < PointCount; i++)
            {
                p.Add(i);
            }

            Permute(p, PointCount, mathHelper);
            return p;
        }

        private void Permute(List<int> p, int n, MathHelper mathHelper)
        {
            for (var i = n - 1; i > 0; i--)
            {
                var target = mathHelper.RandomInt(0, i);
                (p[i], p[target]) = (p[target], p[i]);
            }
        }

        private float TrilinearInterp(float[,,] c, float u, float v, float w)
        {
            var accum = 0f;
            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < 2; j++)
                {
                    for (var k = 0; k < 2; k++)
                    {
                        accum += (i * u + (1 - i) * (1 - u)) *
                                 (j * v + (1 - j) * (1 - v)) *
                                 (k * w + (1 - k) * (1 - w)) * c[i, j, k];
                    }
                }
            }

            return accum;
        }

        private float PerlinInterp(Vector3[,,] c, float u, float v, float w)
        {
            var uu = Interp(u);
            var vv = Interp(v);
            var ww = Interp(w);
            var accum = 0f;

            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < 2; j++)
                {
                    for (var k = 0; k < 2; k++)
                    {
                        var weight = new Vector3(u - i, v - j, w - k);
                        accum += (i * uu + (1 - i) * (1 - uu)) *
                                 (j * vv + (1 - j) * (1 - vv)) *
                                 (k * ww + (1 - k) * (1 - ww)) * Vector3.Dot(c[i, j, k], weight);
                    }
                }
            }

            return accum;
        }

        private static float Interp(float x)
        {
            return x * x * (3 - 2 * x);
        }
    }
}