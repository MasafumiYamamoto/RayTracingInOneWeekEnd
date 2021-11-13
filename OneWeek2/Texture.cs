using System;
using System.Numerics;

namespace OneWeek2
{
    /// <summary>
    /// テクスチャのインターフェース
    /// </summary>
    public interface ITexture
    {
        public Vector3 Value(float u, float v, in Vector3 p);
    }

    /// <summary>
    /// 定数テクスチャ
    /// </summary>
    public class SolidColor : ITexture
    {
        public Vector3 Value(float u, float v, in Vector3 p)
        {
            return _colorValue;
        }

        private readonly Vector3 _colorValue;

        public SolidColor()
        {
        }

        public SolidColor(Vector3 colorValue)
        {
            _colorValue = colorValue;
        }
    }

    /// <summary>
    /// 縞模様テクスチャ
    /// </summary>
    public class CheckerTexture : ITexture
    {
        public CheckerTexture()
        {
        }

        public CheckerTexture(ITexture t0, ITexture t1)
        {
            _even = t0;
            _odd = t1;
        }

        public Vector3 Value(float u, float v, in Vector3 p)
        {
            var sines = MathF.Sin(10 * p.X) * MathF.Sin(10 * p.Y) * MathF.Sin(10 * p.Z);
            return sines < 0 ? _odd.Value(u, v, p) : _even.Value(u, v, p);
        }

        private readonly ITexture _even;
        private readonly ITexture _odd;
    }

    public class NoiseTexture : ITexture
    {
        public Vector3 Value(float u, float v, in Vector3 p)
        {
            return Vector3.One * 0.5f * (1 + MathF.Sin(Scale * p.Z + 10 * _noise.Turb(p))); // 大理石
            // return Vector3.One * _noise.Turb(p * Scale);   // 乱流そのまま
            // return Vector3.One * 0.5f * (1 + _noise.Noise(p * Scale)); // Noiseは負の値をとりうるために[0,1]に変換する
        }

        private readonly Perlin _noise = new();
        private const float Scale = 5;
    }
}