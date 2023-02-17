using System.IO;

using fin.image.io.tile;

using SixLabors.ImageSharp.PixelFormats;

namespace fin.image.io {
  public static class TiledImageReader {
    public static TiledImageReader<TPixel> New<TPixel>(
        int width,
        int height,
        int tileWidth,
        int tileHeight,
        IPixelReader<TPixel> pixelReader)
        where TPixel : unmanaged, IPixel<TPixel>
      => New(width,
             height,
             tileWidth,
             tileHeight,
             new BasicTilePixelIndexer(tileWidth),
             pixelReader);

    public static TiledImageReader<TPixel> New<TPixel>(
        int width,
        int height,
        int tileWidth,
        int tileHeight,
        ITilePixelIndexer tilePixelIndexer,
        IPixelReader<TPixel> pixelReader)
        where TPixel : unmanaged, IPixel<TPixel>
      => New(width,
             height,
             new BasicTileReader<TPixel>(
                 tileWidth,
                 tileHeight,
                 tilePixelIndexer,
                 pixelReader));

    public static TiledImageReader<TPixel> New<TPixel>(
        int width,
        int height,
        ITileReader<TPixel> tileReader)
        where TPixel : unmanaged, IPixel<TPixel>
      => new(width,
             height,
             tileReader);
  }

  public class TiledImageReader<TPixel> : IImageReader<IImage<TPixel>>
      where TPixel : unmanaged, IPixel<TPixel> {
    private readonly int width_;
    private readonly int height_;
    private readonly ITileReader<TPixel> tileReader_;

    public TiledImageReader(int width,
                            int height,
                            ITileReader<TPixel> tileReader) {
      this.width_ = width;
      this.height_ = height;
      this.tileReader_ = tileReader;
    }

    public unsafe IImage<TPixel> Read(byte[] srcBytes) {
      using var er =
          new EndianBinaryReader(srcBytes, Endianness.LittleEndian);

      var image = this.tileReader_.CreateImage(this.width_, this.height_);
      using var imageLock = image.Lock();
      var scan0 = imageLock.pixelScan0;

      var tileXCount = this.width_ / this.tileReader_.TileWidth;
      var tileYCount = this.height_ / this.tileReader_.TileHeight;

      for (var tileY = 0; tileY < tileYCount; ++tileY) {
        for (var tileX = 0; tileX < tileXCount; ++tileX) {
          this.tileReader_.Decode(er,
                                  scan0,
                                  tileX,
                                  tileY,
                                  this.width_,
                                  this.height_);
        }
      }

      return image;
    }
  }
}