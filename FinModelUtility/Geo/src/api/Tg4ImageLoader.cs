using BCnEncoder.Decoder;
using BCnEncoder.Shared;

using fin.image;
using fin.io;

using SixLabors.ImageSharp.PixelFormats;

namespace geo.api {
  public class Tg4ImageFileBundle {
    public required IFileHierarchyFile Tg4hFile { get; init; }
    public required IFileHierarchyFile Tg4dFile { get; init; }
  }

  public class Tg4ImageLoader {
    public unsafe IImage LoadImage(Tg4ImageFileBundle bundle) {
      var headerFile = bundle.Tg4hFile;
      using var headerEr =
          new EndianBinaryReader(headerFile.Impl.OpenRead(),
                                 Endianness.LittleEndian);
      headerEr.Position = 0x20;
      var width = headerEr.ReadUInt16();
      var height = headerEr.ReadUInt16();
      var format = headerEr.ReadStringNTAtOffset(0x4b);

      var dataFile = bundle.Tg4dFile;
      var bytes = dataFile.Impl.ReadAllBytes();

      var compressionFormat = format switch {
          "DXT1c"   => CompressionFormat.Bc1,
          "DXT1a"   => CompressionFormat.Bc1WithAlpha,
          "DXT5"    => CompressionFormat.Bc3,
          "DXT5_NM" => CompressionFormat.Bc3,
      };

      var loadedDxt = new BcDecoder().DecodeRaw(bytes,
                                                width,
                                                height,
                                                compressionFormat);

      var rgbaImage = new Rgba32Image(width, height);
      using var imageLock = rgbaImage.Lock();
      var ptr = imageLock.pixelScan0;

      for (var y = 0; y < height; y++) {
        for (var x = 0; x < width; ++x) {
          var i = y * width + x;

          var src = loadedDxt[i];
          ptr[i] = new Rgba32(src.r, src.g, src.b, src.a);
        }
      }

      return rgbaImage;
    }
  }
}