using System.IO;

using SixLabors.ImageSharp.PixelFormats;

namespace fin.image.io {
  public static class PixelImageReader {
    public static PixelImageReader<TPixel> New<TPixel>(
        int width,
        int height,
        IPixelReader<TPixel> pixelReader,
        Endianness endianness = Endianness.LittleEndian)
        where TPixel : unmanaged, IPixel<TPixel>
      => New(width,
             height,
             new BasicPixelIndexer(width),
             pixelReader,
             endianness);

    public static PixelImageReader<TPixel> New<TPixel>(
        int width,
        int height,
        IPixelIndexer pixelIndexer,
        IPixelReader<TPixel> pixelReader,
        Endianness endianness = Endianness.LittleEndian)
        where TPixel : unmanaged, IPixel<TPixel>
      => new(width,
             height,
             pixelIndexer,
             pixelReader,
             endianness);
  }

  public class PixelImageReader<TPixel> : IImageReader<IImage<TPixel>>
      where TPixel : unmanaged, IPixel<TPixel> {
    private readonly int width_;
    private readonly int height_;
    private readonly IPixelIndexer pixelIndexer_;
    private readonly IPixelReader<TPixel> pixelReader_;
    private readonly Endianness endianness_;

    public PixelImageReader(int width,
                            int height,
                            IPixelIndexer pixelIndexer,
                            IPixelReader<TPixel> pixelReader,
                            Endianness endianness = Endianness.LittleEndian) {
      this.width_ = width;
      this.height_ = height;
      this.pixelIndexer_ = pixelIndexer;
      this.pixelReader_ = pixelReader;
      this.endianness_ = endianness;
    }

    public unsafe IImage<TPixel> Read(byte[] srcBytes) {
      using var er = new EndianBinaryReader(srcBytes, this.endianness_);

      var image = this.pixelReader_.CreateImage(this.width_, this.height_);
      using var imageLock = image.Lock();
      var scan0 = imageLock.pixelScan0;

      for (var i = 0;
           i < this.width_ * this.height_;
           i += this.pixelReader_.PixelsPerRead) {
        this.pixelIndexer_.GetPixelCoordinates(i, out var x, out var y);
        var dstOffs = y * this.width_ + x;
        this.pixelReader_.Decode(er, scan0, dstOffs);
      }

      return image;
    }
  }
}