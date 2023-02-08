using BCnEncoder.Decoder;
using BCnEncoder.Shared;

using fin.image;
using fin.io;

using SixLabors.ImageSharp.PixelFormats;

using IImage = fin.image.IImage;

namespace xmod.api {
  public class TexImageLoader {
    public IImage LoadImage(IFile texFile) {
      using var er = new EndianBinaryReader(texFile.OpenRead());
      var width = er.ReadUInt16();
      var height = er.ReadUInt16();
      var dxtType = er.ReadUInt16();

      ColorRgba32[] loadedDxt;

      IImage image;
      switch (dxtType) {
        // DXT1
        case 22: {
            var expectedLength = width * height / 16 * (2 + 2 + 4);

            er.Position = 0xe;
            var bytes = er.ReadBytes(expectedLength);

            loadedDxt =
                new BcDecoder().DecodeRaw(bytes,
                                          width,
                                          height,
                                          CompressionFormat.Bc1);
            break;
          }
        // DXT3
        case 14: {
            var expectedLength = width * height / 16 * (8 + 2 + 2 + 4);

            er.Position = 0xe;
            var bytes = er.ReadBytes(expectedLength);

            loadedDxt =
                new BcDecoder().DecodeRaw(bytes,
                                          width,
                                          height,
                                          CompressionFormat.Bc2);
            break;
          }
        // DXT5
        case 26: {
            var expectedLength = width * height / 16 * (8 + 2 + 2 + 4);

            er.Position = 0xe;
            var bytes = er.ReadBytes(expectedLength);

            loadedDxt =
                new BcDecoder().DecodeRaw(bytes,
                                          width,
                                          height,
                                          CompressionFormat.Bc3);
            break;
          }
        default:
          throw new NotImplementedException();
      }

      unsafe {
        var rgbaImage = new Rgba32Image(width, height);
        image = rgbaImage;
        using var imageLock = rgbaImage.Lock();
        var ptr = imageLock.pixelScan0;

        for (var y = 0; y < height; y++) {
          for (var x = 0; x < width; ++x) {
            var i = y * width + x;

            var src = loadedDxt[i];
            ptr[i] = new Rgba32(src.r, src.g, src.b, src.a);
          }
        }
      }

      return image;
    }
  }
}
