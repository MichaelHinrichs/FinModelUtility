using fin.color;
using fin.data;
using fin.image;
using fin.image.io;
using fin.image.io.tile;
using fin.io.image.tile;
using fin.util.color;

using schema.binary;

using SixLabors.ImageSharp.PixelFormats;


namespace modl.schema.res.texr {
  public interface ITexr : IBinaryDeserializable {
    IImage Image { get; }
  }

  public abstract class BTexr {
    protected IImage
        ReadA8R8G8B8_(IEndianBinaryReader er,
                      uint width,
                      uint height) {
      SectionHeaderUtil.AssertNameAndSize(
          er,
          "MIP ",
          width * height * 4);

      var image =
          new Rgba32Image(PixelFormat.RGBA8888, (int) width, (int) height);
      image.Mutate((_, setHandler) => {
        var blockWidth = 4;
        var blockHeight = 4;

        var blockGrid = new Grid<(byte a, byte r)>(blockWidth, blockHeight);

        for (var blockY = 0; blockY < height / blockHeight; blockY++) {
          var availableBlockHeight =
              Math.Min(blockHeight, height - blockHeight * blockY);

          for (var blockX = 0; blockX < width / blockWidth; blockX++) {
            var availableBlockWidth =
                Math.Min(blockWidth, width - blockWidth * blockX);

            for (var iy = 0; iy < availableBlockHeight; ++iy) {
              for (var ix = 0; ix < availableBlockWidth; ++ix) {
                var a = er.ReadByte();
                var r = er.ReadByte();

                blockGrid[ix, iy] = (a, r);
              }
            }

            for (var iy = 0; iy < availableBlockHeight; ++iy) {
              var imgY = blockY * blockHeight + iy;
              for (var ix = 0; ix < availableBlockWidth; ++ix) {
                var imgX = blockX * blockWidth + ix;

                var (a, r) = blockGrid[ix, iy];
                var g = er.ReadByte();
                var b = er.ReadByte();

                setHandler(imgX, imgY, r, g, b, a);
              }
            }
          }
        }
      });

      return image;
    }

    protected IImage
        ReadDxt1_(IEndianBinaryReader er, uint width, uint height) {
      // TODO: Trim this little bit off?
      width = (uint) (MathF.Ceiling(width / 8f) * 8);
      height = (uint) (MathF.Ceiling(height / 8f) * 8);

      var mipSize = width * height >> 1;
      SectionHeaderUtil.AssertNameAndSize(er, "MIP ", mipSize);

      return TiledImageReader
             .New((int) width, (int) height, new CmprTileReader())
             .Read(er);
    }

    protected IImage ReadP8_(IEndianBinaryReader er, uint width, uint height) {
      SectionHeaderUtil.AssertNameAndSize(er, "PAL ", 512);

      var palette = er.ReadUInt16s(256)
                      .Select(value => ColorUtil.ParseRgb5A3(value))
                      .ToArray();

      SectionHeaderUtil.AssertNameAndSize(er, "MIP ", width * height);

      var indexImage = TiledImageReader
                       .New((int) width, (int) height, 8, 4, new L8PixelReader())
                       .Read(er);

      return new IndexedImage8(PixelFormat.P8, indexImage, palette);
    }

    protected IImage ReadP4_(IEndianBinaryReader er, uint width, uint height) {
      // TODO: This method seems incorrect...

      SectionHeaderUtil.AssertNameAndSize(er, "PAL ", 32);

      var palette = er.ReadUInt16s(16)
                      .Select(value => {
                        var r = ColorUtil.ExtractScaled(value, 0, 4);
                        var g = ColorUtil.ExtractScaled(value, 4, 4);
                        var b = ColorUtil.ExtractScaled(value, 8, 4);
                        var a = ColorUtil.ExtractScaled(value, 12, 4);

                        // TODO: Is this correct??? Textures don't look quite right
                        return FinColor.FromRgbaBytes(r, g, b, a);
                      })
                      .ToArray();

      SectionHeaderUtil.AssertNameAndSize(er, "MIP ", width * height / 2);

      var indexImage = TiledImageReader
                       .New((int) width, (int) height, 8, 8, new L4PixelReader())
                       .Read(er);

      return new IndexedImage8(PixelFormat.P4, indexImage, palette);
    }

    protected IImage ReadIA8_(IEndianBinaryReader er, uint width, uint height) {
      SectionHeaderUtil.AssertNameAndSize(er, "MIP ", 2 * width * height);
      return TiledImageReader
             .New((int) width, (int) height, 4, 4, new La16PixelReader())
             .Read(er);
    }

    protected IImage ReadIA4_(IEndianBinaryReader er, uint width, uint height) {
      SectionHeaderUtil.AssertNameAndSize(er, "MIP ", width * height);
      return TiledImageReader
             .New((int) width, (int) height, 8, 4, new La8PixelReader())
             .Read(er);
    }

    protected IImage ReadI8_(IEndianBinaryReader er, uint width, uint height) {
      SectionHeaderUtil.AssertNameAndSize(er, "MIP ", width * height);
      return TiledImageReader
             .New((int) width, (int) height, 8, 4, new L8PixelReader())
             .Read(er.ReadBytes(width * height));
    }

    protected IImage ReadI4_(IEndianBinaryReader er, uint width, uint height) {
      SectionHeaderUtil.AssertNameAndSize(er, "MIP ", width * height / 2);
      return TiledImageReader
             .New((int) width, (int) height, 8, 8, new L8PixelReader())
             .Read(er);
    }
  }
}