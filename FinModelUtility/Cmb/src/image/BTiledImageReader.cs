using System.IO;
using System.Runtime.CompilerServices;

using fin.image;
using fin.image.io;

using SixLabors.ImageSharp.PixelFormats;

namespace cmb.image {
  public abstract class BTiledImageReader<TPixel> : IImageReader<IImage<TPixel>>
      where TPixel : unmanaged, IPixel<TPixel> {
    private readonly int width_;
    private readonly int height_;

    public BTiledImageReader(int width, int height) {
      this.width_ = width;
      this.height_ = height;
    }

    public unsafe IImage<TPixel> Read(byte[] srcBytes) {
      using var er =
          new EndianBinaryReader(srcBytes, Endianness.LittleEndian);

      var image = this.CreateImage_(this.width_, this.height_);
      using var imageLock = image.Lock();
      var scan0 = imageLock.pixelScan0;

      for (var yy = 0; yy < this.height_; yy += 8) {
        for (var xx = 0; xx < this.width_; xx += 8) {
          // Iterate in Morton order inside each tile.
          for (var i = 0; i < 0x40; i++) {
            var x = Morton7_(i);
            var y = Morton7_(i >>> 1);
            var dstOffs = ((yy + y) * this.width_ + xx + x);
            this.Decode(er, scan0, dstOffs);
          }
        }
      }

      return image;
    }

    protected abstract IImage<TPixel> CreateImage_(int width, int height);

    protected abstract unsafe void Decode(
        IEndianBinaryReader er,
        TPixel* scan0,
        int offset);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Morton7_(int n)
        // 0a0b0c => 000abc
      => ((n >>> 2) & 0x04) | ((n >>> 1) & 0x02) | (n & 0x01);
  }
}