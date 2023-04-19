using fin.color;
using fin.data;
using fin.image;
using fin.util.color;

using schema.binary;

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
          er, "MIP ", width * height * 4);

      var image = new Rgba32Image(PixelFormat.RGBA8888, (int) width, (int) height);
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

    protected IImage ReadDxt1_(IEndianBinaryReader er, uint width, uint height) {
      // TODO: Trim this little bit off?
      width = (uint) (MathF.Ceiling(width / 8f) * 8);
      height = (uint) (MathF.Ceiling(height / 8f) * 8);

      var mipSize = width * height / 2;
      SectionHeaderUtil.AssertNameAndSize(er, "MIP ", mipSize);

      var tileWidth = 4;
      var tileHeight = 4;

      IColor[] colors = new IColor[4];

      var image = new Rgba32Image(PixelFormat.DXT1, (int) width, (int) height);
      image.Mutate((_, setHandler) => {
        var x = 0;
        var y = 0;

        for (var ii = 0; ii < mipSize / 8; ++ii) {
          var color0Value = er.ReadUInt16();
          var color1Value = er.ReadUInt16();
          var pixelMask = er.ReadUInt32();

          var color0 = colors[0] = ColorUtil.ParseRgb565(color0Value);
          var color1 = colors[1] = ColorUtil.ParseRgb565(color1Value);

          if (color0Value > color1Value) {
            colors[2] = FinColor.FromRgbBytes(
                (byte) ((2 * color0.Rb + color1.Rb) / 3f),
                (byte) ((2 * color0.Gb + color1.Gb) / 3f),
                (byte) ((2 * color0.Bb + color1.Bb) / 3f));
            colors[3] = FinColor.FromRgbBytes(
                (byte) ((2 * color1.Rb + color0.Rb) / 3f),
                (byte) ((2 * color1.Gb + color0.Gb) / 3f),
                (byte) ((2 * color1.Bb + color0.Bb) / 3f));
          } else {
            colors[2] = FinColor.FromRgbBytes(
                (byte) ((color0.Rb + color1.Rb) / 2f),
                (byte) ((color0.Gb + color1.Gb) / 2f),
                (byte) ((color0.Bb + color1.Bb) / 2f));
            colors[3] = FinColor.FromRgbaBytes(
                0, 0, 0, 0);
          }

          var ii2 = ii % 4;
          var iiX = (ii2 % 2) * 4;
          var iiY = (ii2 / 2) * 4;

          for (var yInTile = 0; yInTile < tileHeight; ++yInTile) {
            var arrayY = y + iiY + yInTile;
            for (var xInTile = 0; xInTile < tileWidth; ++xInTile) {
              var arrayX = x + iiX + xInTile;

              var colorIndex =
                  (pixelMask >> ((15 - (yInTile * 4 + xInTile)) * 2)) &
                  0b11;
              var color = colors[colorIndex];

              setHandler(arrayX,
                         arrayY,
                         color.Rb,
                         color.Gb,
                         color.Bb,
                         color.Ab);
            }
          }

          if (ii2 == 3) {
            x += 8;
            if (x >= width) {
              x = 0;
              y += 8;
            }
          }
        }
      });

      return image;
    }

    protected IImage ReadP8_(IEndianBinaryReader er, uint width, uint height) {
      SectionHeaderUtil.AssertNameAndSize(er, "PAL ", 512);

      var palette = er.ReadUInt16s(256)
                      .Select(value => ColorUtil.ParseRgb5A3(value))
                      .ToArray();

      SectionHeaderUtil.AssertNameAndSize(er, "MIP ", width * height);

      var image = new Rgba32Image(PixelFormat.P8, (int) width, (int) height);
      image.Mutate((_, setHandler) => {
        var blockWidth = 8;
        var blockHeight = 4;

        var blockCountX = width / blockWidth;
        var blockCountY = height / blockHeight;

        for (var blockY = 0; blockY < blockCountY; ++blockY) {
          for (var blockX = 0; blockX < blockCountX; ++blockX) {
            for (var yInBlock = 0; yInBlock < blockHeight; ++yInBlock) {
              var y = blockY * blockHeight + yInBlock;
              for (var xInBlock = 0; xInBlock < blockWidth; ++xInBlock) {
                var x = blockX * blockWidth + xInBlock;

                var paletteIndex = er.ReadByte();
                var color = palette[paletteIndex];

                setHandler(x, y, color.Rb, color.Gb, color.Bb, color.Ab);
              }
            }
          }
        }
      });

      return image;
    }

    protected IImage ReadP4_(IEndianBinaryReader er, uint width, uint height) {
      // TODO: This method seems incorrect...

      SectionHeaderUtil.AssertNameAndSize(er, "PAL ", 32);

      var paletteShorts = er.ReadUInt16s(16);

      var palette = paletteShorts
                    .Select(value => {
                      var r = ColorUtil.ExtractScaled(value, 0, 4);
                      var g = ColorUtil.ExtractScaled(value, 4, 4);
                      var b = ColorUtil.ExtractScaled(value, 8, 4);
                      var a = ColorUtil.ExtractScaled(value, 12, 4);

                      // TODO: Is this correct??? Textures don't look quite right
                      return FinColor.FromRgbaBytes(
                          (byte) r, (byte) g, (byte) b, (byte) a);
                    })
                    .ToArray();

      SectionHeaderUtil.AssertNameAndSize(er, "MIP ", width * height / 2);

      var image = new Rgba32Image(PixelFormat.P4, (int) width, (int) height);
      image.Mutate((_, setHandler) => {
        var blockWidth = 8;
        var blockHeight = 8;

        var blockCountX = width / blockWidth;
        var blockCountY = height / blockHeight;

        for (var blockY = 0; blockY < blockCountY; ++blockY) {
          for (var blockX = 0; blockX < blockCountX; ++blockX) {
            for (var yInBlock = 0; yInBlock < blockHeight; ++yInBlock) {
              var y = blockY * blockHeight + yInBlock;
              for (var xInBlock = 0; xInBlock < blockWidth; xInBlock += 2) {
                var x = blockX * blockWidth + xInBlock;

                var paletteIndex = er.ReadByte();

                var upperPaletteIndex = paletteIndex >> 4;
                var color0 = palette[upperPaletteIndex];
                setHandler(x, y,
                           color0.Rb,
                           color0.Gb,
                           color0.Bb,
                           color0.Ab);

                var lowerPaletteIndex = paletteIndex & 0xf;
                var color1 = palette[lowerPaletteIndex];
                setHandler(x + 1, y,
                           color1.Rb,
                           color1.Gb, 
                           color1.Bb, 
                           color1.Ab);
              }
            }
          }
        }
      });

      return image;
    }

    protected IImage ReadIA8_(IEndianBinaryReader er, uint width, uint height) {
      SectionHeaderUtil.AssertNameAndSize(er, "MIP ", 2 * width * height);

      var image = new La16Image(PixelFormat.LA88, (int) width, (int) height);
      image.Mutate((_, setHandler) => {
        var blockWidth = 4;
        var blockHeight = 4;

        var blockCountX = width / blockWidth;
        var blockCountY = height / blockHeight;

        for (var blockY = 0; blockY < blockCountY; ++blockY) {
          for (var blockX = 0; blockX < blockCountX; ++blockX) {
            for (var yInBlock = 0; yInBlock < blockHeight; ++yInBlock) {
              var y = blockY * blockHeight + yInBlock;
              for (var xInBlock = 0; xInBlock < blockWidth; ++xInBlock) {
                var x = blockX * blockWidth + xInBlock;

                var intensity = er.ReadByte();
                var alpha = er.ReadByte();

                setHandler(x, y, intensity, alpha);
              }
            }
          }
        }
      });

      return image;
    }

    protected IImage ReadIA4_(IEndianBinaryReader er, uint width, uint height) {
      SectionHeaderUtil.AssertNameAndSize(er, "MIP ", width * height);

      var image = new La16Image(PixelFormat.LA44, (int) width, (int) height);
      image.Mutate((_, setHandler) => {
        var blockWidth = 8;
        var blockHeight = 4;

        var blockCountX = width / blockWidth;
        var blockCountY = height / blockHeight;

        for (var blockY = 0; blockY < blockCountY; ++blockY) {
          for (var blockX = 0; blockX < blockCountX; ++blockX) {
            for (var yInBlock = 0; yInBlock < blockHeight; ++yInBlock) {
              var y = blockY * blockHeight + yInBlock;
              for (var xInBlock = 0; xInBlock < blockWidth; ++xInBlock) {
                var x = blockX * blockWidth + xInBlock;

                var color = er.ReadByte();

                var intensity = ColorUtil.ExtractScaled(color, 0, 4);
                var alpha = ColorUtil.ExtractScaled(color, 4, 4);

                setHandler(x, y, intensity, alpha);
              }
            }
          }
        }
      });

      return image;
    }

    protected IImage ReadI8_(IEndianBinaryReader er, uint width, uint height) {
      SectionHeaderUtil.AssertNameAndSize(er, "MIP ", width * height);

      var image = new L8Image(PixelFormat.L8, (int) width, (int) height);
      image.Mutate((_, setHandler) => {
        var blockWidth = 8;
        var blockHeight = 4;

        var blockCountX = width / blockWidth;
        var blockCountY = height / blockHeight;

        for (var blockY = 0; blockY < blockCountY; ++blockY) {
          for (var blockX = 0; blockX < blockCountX; ++blockX) {
            for (var yInBlock = 0; yInBlock < blockHeight; ++yInBlock) {
              var y = blockY * blockHeight + yInBlock;
              for (var xInBlock = 0; xInBlock < blockWidth; ++xInBlock) {
                var x = blockX * blockWidth + xInBlock;

                var intensity = er.ReadByte();
                setHandler(x, y, intensity);
              }
            }
          }
        }
      });

      return image;
    }

    protected IImage ReadI4_(IEndianBinaryReader er, uint width, uint height) {
      SectionHeaderUtil.AssertNameAndSize(er, "MIP ", width * height / 2);

      var image = new L8Image(PixelFormat.L4, (int) width, (int) height);
      image.Mutate((_, setHandler) => {
        var blockWidth = 8;
        var blockHeight = 8;

        var blockCountX = width / blockWidth;
        var blockCountY = height / blockHeight;

        for (var blockY = 0; blockY < blockCountY; ++blockY) {
          for (var blockX = 0; blockX < blockCountX; ++blockX) {
            for (var yInBlock = 0; yInBlock < blockHeight; ++yInBlock) {
              var y = blockY * blockHeight + yInBlock;
              for (var xInBlock = 0; xInBlock < blockWidth; xInBlock += 2) {
                var x = blockX * blockWidth + xInBlock;

                var color = er.ReadByte();

                var intensity1 = ColorUtil.ExtractScaled(color, 4, 4);
                var intensity2 = ColorUtil.ExtractScaled(color, 0, 4);

                setHandler(x, y, intensity1);
                setHandler(x + 1, y, intensity2);
              }
            }
          }
        }
      });

      return image;
    }
  }
}