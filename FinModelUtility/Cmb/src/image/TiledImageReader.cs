using System.IO;
using System.Runtime.CompilerServices;

using fin.image;
using fin.image.io;

using SixLabors.ImageSharp.PixelFormats;

namespace cmb.image {
  public interface IPixelReader<TPixel>
      where TPixel : unmanaged, IPixel<TPixel> {
    IImage<TPixel> CreateImage_(int width, int height);

    unsafe void Decode(IEndianBinaryReader er,
                       TPixel* scan0,
                       int offset);
  }

  public static class TiledImageReader {
    public static TiledImageReader<TPixel> New<TPixel>(
        int width,
        int height,
        IPixelReader<TPixel> reader)
        where TPixel : unmanaged, IPixel<TPixel>
      => new(width, height, reader);
  }

  public class TiledImageReader<TPixel> : IImageReader<IImage<TPixel>>
      where TPixel : unmanaged, IPixel<TPixel> {
    private readonly int width_;
    private readonly int height_;
    private readonly IPixelReader<TPixel> reader_;

    public TiledImageReader(int width,
                            int height,
                            IPixelReader<TPixel> pixelReader) {
      this.width_ = width;
      this.height_ = height;
      this.reader_ = pixelReader;
    }

    public unsafe IImage<TPixel> Read(byte[] srcBytes) {
      using var er =
          new EndianBinaryReader(srcBytes, Endianness.LittleEndian);

      var image = this.reader_.CreateImage_(this.width_, this.height_);
      using var imageLock = image.Lock();
      var scan0 = imageLock.pixelScan0;

      for (var yy = 0; yy < this.height_; yy += 8) {
        for (var xx = 0; xx < this.width_; xx += 8) {
          // Iterate in Morton order inside each tile.
          for (var i = 0; i < 0x40; i++) {
            var x = Morton7_(i);
            var y = Morton7_(i >>> 1);
            var dstOffs = ((yy + y) * this.width_ + xx + x);
            this.reader_.Decode(er, scan0, dstOffs);
          }
        }
      }

      return image;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Morton7_(int n)
        // 0a0b0c => 000abc
      => ((n >>> 2) & 0x04) | ((n >>> 1) & 0x02) | (n & 0x01);
  }
}