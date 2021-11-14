using System;
using System.IO;
using System.Numerics;
using System.Drawing;
using System.Drawing.Imaging;

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

    public class ImageTexture : ITexture, IDisposable
    {
        private const int BytesPerPixel = 3;
        private int _width;
        private int _height;

        private byte[] _data;

        // 画像1行に含まれるバイト数
        private int _bytesPerScanline;

        public ImageTexture(string filePath)
        {
            // ファイルを開く
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            if (!fileStream.CanRead)
            {
                Console.Error.WriteLine($"{filePath} cannot read");
                _data = null;
                _width = 0;
                _height = 0;
            }

#pragma warning disable CA1416
            var image = new Bitmap(filePath);
            _width = image.Width;
            _height = image.Height;
            var rect = new Rectangle(0, 0, image.Width, image.Height);
            var bmpData = image.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            _data = new byte[bmpData.Stride * image.Height];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, _data, 0, _data.Length);
            image.UnlockBits(bmpData);
#pragma warning restore CA1416

            _bytesPerScanline = BytesPerPixel * _width;
            // 閉じる
            fileStream.Close();
        }

        public Vector3 Value(float u, float v, in Vector3 p)
        {
            // テクスチャのデータがない場合はそのことがわかるように特定色を出す
            if (_data == null)
            {
                return new Vector3(1, 1, 1);
            }

            // 入力されたテクスチャ座標を[0,1]に落とす
            u = Math.Clamp(u, 0f, 1f);
            v = 1f - Math.Clamp(v, 0, 1f);

            var i = (int)(u * _width);
            var j = (int)(v * _height);

            // 座標系を更に切り捨てる
            if (i >= _width)
            {
                i = _width - 1;
            }

            if (j >= _height)
            {
                j = _height - 1;
            }

            const float colorScale = 1 / 255f;
            var pos = j * _bytesPerScanline + i * BytesPerPixel;
            var b = _data[pos];
            var g = _data[pos + 1];
            var r = _data[pos + 2];
            return new Vector3(r,g,b) * colorScale;
        }

        public void Dispose()
        {
            _data = null;
        }
    }
}