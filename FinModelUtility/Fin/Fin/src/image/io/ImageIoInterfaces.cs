using schema.binary;

using SixLabors.ImageSharp.PixelFormats;

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
    IImage ReadImage(byte[] srcBytes,
                Endianness endianness = Endianness.LittleEndian) {
      using var er = new EndianBinaryReader(srcBytes, endianness);
      return this.ReadImage(er);
    }

    IImage ReadImage(IEndianBinaryReader er);
  }

  public interface IImageReader<out TImage> : IImageReader
      where TImage : IImage {
    IImage IImageReader.ReadImage(byte[] srcBytes,
                             Endianness endianness = Endianness.LittleEndian)
      => this.ReadImage(srcBytes, endianness);

    new TImage ReadImage(byte[] srcBytes,
                Endianness endianness = Endianness.LittleEndian) {
      using var er = new EndianBinaryReader(srcBytes, endianness);
      return this.ReadImage(er);
    }

    IImage IImageReader.ReadImage(IEndianBinaryReader er) => this.ReadImage(er);

    new TImage ReadImage(IEndianBinaryReader er);
  }
}