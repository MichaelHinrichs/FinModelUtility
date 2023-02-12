using System.IO;

using SixLabors.ImageSharp.PixelFormats;

namespace fin.image.io {
  public interface ITilePixelIndexer {
    void GetPixelInTile(int index, out int x, out int y);
  }

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
        int tileWidth,
        int tileHeight,
        ITilePixelIndexer tilePixelIndexer,
        IPixelReader<TPixel> pixelReader)
        where TPixel : unmanaged, IPixel<TPixel>
      => new(width,
             height,
             tileWidth,
             tileHeight,
             tilePixelIndexer,
             pixelReader);
  }

  public class TiledImageReader<TPixel> : IImageReader<IImage<TPixel>>
      where TPixel : unmanaged, IPixel<TPixel> {
    private readonly int width_;
    private readonly int height_;
    private readonly int tileWidth_;
    private readonly int tileHeight_;
    private readonly ITilePixelIndexer tilePixelIndexer_;
    private readonly IPixelReader<TPixel> reader_;

    public TiledImageReader(int width,
                            int height,
                            int tileWidth,
                            int tileHeight,
                            ITilePixelIndexer tilePixelIndexer,
                            IPixelReader<TPixel> pixelReader) {
      this.width_ = width;
      this.height_ = height;
      this.tileWidth_ = tileWidth;
      this.tileHeight_ = tileHeight;
      this.tilePixelIndexer_ = tilePixelIndexer;
      this.reader_ = pixelReader;
    }

    public unsafe IImage<TPixel> Read(byte[] srcBytes) {
      using var er =
          new EndianBinaryReader(srcBytes, Endianness.LittleEndian);

      var image = this.reader_.CreateImage_(this.width_, this.height_);
      using var imageLock = image.Lock();
      var scan0 = imageLock.pixelScan0;

      for (var yy = 0; yy < this.height_; yy += this.tileHeight_) {
        for (var xx = 0; xx < this.width_; xx += this.tileWidth_) {
          for (var i = 0; i < this.tileWidth_ * this.tileHeight_; i++) {
            this.tilePixelIndexer_.GetPixelInTile(i, out var x, out var y);
            var dstOffs = ((yy + y) * this.width_ + xx + x);
            this.reader_.Decode(er, scan0, dstOffs);
          }
        }
      }

      return image;
    }
  }
}