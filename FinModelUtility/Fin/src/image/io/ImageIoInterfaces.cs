using SixLabors.ImageSharp.PixelFormats;
using System.IO;

namespace fin.image.io {
  public interface IPixelReader<TPixel>
      where TPixel : unmanaged, IPixel<TPixel> {
    IImage<TPixel> CreateImage(int width, int height);

    unsafe void Decode(IEndianBinaryReader er,
                       TPixel* scan0,
                       int offset);

    int PixelsPerRead => 1;
  }

  public interface IPixelIndexer {
    void GetPixelCoordinates(int index, out int x, out int y);
  }

  public interface ITileReader<TPixel>
      where TPixel : unmanaged, IPixel<TPixel> {
    IImage<TPixel> CreateImage(int width, int height);

    int TileWidth { get; }
    int TileHeight { get; }

    unsafe void Decode(IEndianBinaryReader er,
                       TPixel* scan0,
                       int tileX,
                       int tileY,
                       int imageWidth,
                       int imageHeight);
  }

  public interface IImageReader {
    IImage Read(byte[] srcBytes);
  }

  public interface IImageReader<out TImage> : IImageReader
      where TImage : IImage {
    new TImage Read(byte[] srcBytes);
    IImage IImageReader.Read(byte[] srcBytes) => Read(srcBytes);
  }
}