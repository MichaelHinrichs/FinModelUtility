using System.Drawing;

using fin.model;
using fin.model.impl;
using fin.util.asserts;
using fin.util.color;
using fin.util.image;

using schema;


namespace modl.schema.res.texr {
  public class Texr : IBiSerializable {
    private enum TexrMode {
      BW1,
      BW2
    }

    public string FileName { get; private set; }

    public List<BwTexture> Textures { get; } = new();

    public void Read(EndianBinaryReader er) {
      er.AssertStringEndian("TEXR");

      var texrLength = er.ReadUInt32();
      var expectedTexrEnd = er.Position + texrLength;

      this.FileName = er.ReadString(er.ReadInt32());

      var textureSectionName = er.ReadStringEndian(4);

      var mode = textureSectionName switch {
          "XBTF" => TexrMode.BW1,
          "GBTF" => TexrMode.BW2,
          _      => throw new NotSupportedException(),
      };

      var btfLength = er.ReadUInt32();
      var expectedBtfEnd = er.Position + btfLength;

      Asserts.Equal(expectedTexrEnd, expectedBtfEnd);

      this.Textures.Clear();
      var textureCount = er.ReadUInt32();
      for (var i = 0; i < textureCount; ++i) {
        switch (mode) {
          case TexrMode.BW1: {
            er.AssertStringEndian("TEXT");
            var textureLength = er.ReadUInt32();
            var expectedTextureEnd = er.Position + textureLength;

            var textureName = er.ReadString(0x10);

            var width = er.ReadUInt32();
            var height = er.ReadUInt32();

            var unknowns0 = er.ReadUInt32s(2);

            var textureType = er.ReadString(8);
            var drawType = er.ReadString(8);

            var unknowns1 = er.ReadUInt32s(8);

            var unknowns2 = er.ReadUInt32s(1);

            var image = textureType switch {
                "A8R8G8B8" => this.ReadA8R8G8B8_(er, width, height),
                "DXT1"     => this.ReadDxt1_(er, width, height),
                "P8"       => this.ReadP8_(er, width, height),
                _          => throw new NotImplementedException(),
            };

            image.Save(
                $@"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\battalion_wars_2\_test_\{textureType}_{textureName}.png");

            this.Textures.Add(new BwTexture(textureName, image));

            er.Position = expectedTextureEnd;
            Asserts.Equal(expectedTextureEnd, er.Position);
            break;
          }
          case TexrMode.BW2: {
            er.AssertStringEndian("GTXD");
            var textureLength = er.ReadUInt32();
            var expectedTextureEnd = er.Position + textureLength;

            var textureName = er.ReadString(0x20);

            var endiannessType = er.Endianness;
            er.Endianness = Endianness.BigEndian;

            var width = er.ReadUInt32();
            var height = er.ReadUInt32();

            var unknowns0 = er.ReadUInt32s(2);

            var rawTextureType = er.ReadString(8)
                                   .Replace("\0", "")
                                   .ToCharArray();
            Array.Reverse(rawTextureType);
            var textureType = new string(rawTextureType);
            var drawType = er.ReadString(8);

            var unknown = er.ReadChars(48);

            er.Endianness = endiannessType;

            var image = textureType switch {
                "A8R8G8B8" => this.ReadA8R8G8B8_(er, width, height),
                "DXT1"     => this.ReadDxt1_(er, width, height),
                "P8"       => this.ReadP8_(er, width, height),
                "P4"       => this.ReadP4_(er, width, height),
                "IA8"      => this.ReadIA8_(er, width, height),
                "IA4"      => this.ReadIA4_(er, width, height),
                "I8"       => this.ReadI8_(er, width, height),
                "I4"       => this.ReadI4_(er, width, height),
                _          => throw new NotImplementedException(),
            };

            this.Textures.Add(new BwTexture(textureName, image));

            er.Position = expectedTextureEnd;
            Asserts.Equal(expectedTextureEnd, er.Position);
            break;
          }
        }
      }

      Asserts.Equal(expectedTexrEnd, er.Position);
    }

    public void Write(EndianBinaryWriter ew) =>
        throw new NotImplementedException();

    private Image
        ReadA8R8G8B8_(EndianBinaryReader er,
                      uint width,
                      uint height) {
      er.AssertStringEndian("MIP ");

      var mipSize = width * height * 4;
      er.AssertUInt32(mipSize);

      var endianness = er.Endianness;
      er.Endianness = Endianness.BigEndian;

      var image = new Bitmap((int) width, (int) height);
      BitmapUtil.InvokeAsLocked(image, bmpData => {
        unsafe {
          var ptr = (byte*) bmpData.Scan0;

          var blockWidth = 4;
          var blockHeight = 4;

          for (var blockY = 0; blockY < height / blockHeight; blockY++) {
            var availableBlockHeight =
                Math.Min(blockHeight, height - blockHeight * blockY);

            for (var blockX = 0; blockX < width / blockWidth; blockX++) {
              var availableBlockWidth =
                  Math.Min(blockWidth, width - blockWidth * blockX);

              for (var iy = 0; iy < availableBlockHeight; ++iy) {
                var imgY = blockY * blockHeight + iy;
                for (var ix = 0; ix < availableBlockWidth; ++ix) {
                  var imgX = blockX * blockWidth + ix;

                  var a = er.ReadByte();
                  var r = er.ReadByte();

                  var index = 4 * (imgY * width + imgX);
                  ptr[index + 2] = r;
                  ptr[index + 3] = a;
                }
              }

              for (var iy = 0; iy < availableBlockHeight; ++iy) {
                var imgY = blockY * blockHeight + iy;
                for (var ix = 0; ix < availableBlockWidth; ++ix) {
                  var imgX = blockX * blockWidth + ix;

                  var g = er.ReadByte();
                  var b = er.ReadByte();

                  var index = 4 * (imgY * width + imgX);
                  ptr[index + 0] = b;
                  ptr[index + 1] = g;
                }
              }
            }
          }
        }
      });

      er.Endianness = endianness;

      return image;
    }

    private Image ReadDxt1_(EndianBinaryReader er, uint width, uint height) {
      er.AssertStringEndian("MIP ");

      // TODO: Trim this little bit off?
      width = (uint) (MathF.Ceiling(width / 8f) * 8);
      height = (uint) (MathF.Ceiling(height / 8f) * 8);

      var mipSize = width * height / 2;
      er.AssertUInt32(mipSize);

      var endianness = er.Endianness;
      er.Endianness = Endianness.BigEndian;

      var tileWidth = 4;
      var tileHeight = 4;

      IColor[] colors = new IColor[4];

      var image = new Bitmap((int) width, (int) height);
      BitmapUtil.InvokeAsLocked(image, bmpData => {
        var x = 0;
        var y = 0;

        unsafe {
          var ptr = (byte*) bmpData.Scan0;
          for (var ii = 0; ii < mipSize / 8; ++ii) {
            var color0Value = er.ReadUInt16();
            var color1Value = er.ReadUInt16();
            var pixelMask = er.ReadUInt32();

            var color0 = colors[0] = ColorUtil.ParseRgb565(color0Value);
            var color1 = colors[1] = ColorUtil.ParseRgb565(color1Value);

            if (color0Value > color1Value) {
              colors[2] = ColorImpl.FromRgbBytes(
                  (byte) ((2 * color0.Rb + color1.Rb) / 3f),
                  (byte) ((2 * color0.Gb + color1.Gb) / 3f),
                  (byte) ((2 * color0.Bb + color1.Bb) / 3f));
              colors[3] = ColorImpl.FromRgbBytes(
                  (byte) ((2 * color1.Rb + color0.Rb) / 3f),
                  (byte) ((2 * color1.Gb + color0.Gb) / 3f),
                  (byte) ((2 * color1.Bb + color0.Bb) / 3f));
            } else {
              colors[2] = ColorImpl.FromRgbBytes(
                  (byte) ((color0.Rb + color1.Rb) / 2f),
                  (byte) ((color0.Gb + color1.Gb) / 2f),
                  (byte) ((color0.Bb + color1.Bb) / 2f));
              colors[3] = ColorImpl.FromRgbaBytes(
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

                var imageIndex = 4 * (arrayY * width + arrayX);
                ptr[imageIndex + 0] = color.Bb;
                ptr[imageIndex + 1] = color.Gb;
                ptr[imageIndex + 2] = color.Rb;
                ptr[imageIndex + 3] = color.Ab;
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
        }
      });

      er.Endianness = endianness;

      return image;
    }

    private Image ReadP8_(EndianBinaryReader er, uint width, uint height) {
      er.AssertStringEndian("PAL ");
      er.AssertUInt32(512);

      var endianness = er.Endianness;
      er.Endianness = Endianness.BigEndian;

      var palette = er.ReadUInt16s(256)
                      .Select(value => ColorUtil.ParseRgb5A3(value))
                      .ToArray();

      er.Endianness = endianness;

      er.AssertStringEndian("MIP ");
      var mipSize = width * height;
      er.AssertUInt32(mipSize);

      er.Endianness = Endianness.BigEndian;

      var image = new Bitmap((int) width, (int) height);
      BitmapUtil.InvokeAsLocked(image, bmpData => {
        var blockWidth = 8;
        var blockHeight = 4;

        var blockCountX = width / blockWidth;
        var blockCountY = height / blockHeight;

        unsafe {
          var ptr = (byte*) bmpData.Scan0;
          for (var blockY = 0; blockY < blockCountY; ++blockY) {
            for (var blockX = 0; blockX < blockCountX; ++blockX) {
              for (var yInBlock = 0; yInBlock < blockHeight; ++yInBlock) {
                var y = blockY * blockHeight + yInBlock;
                for (var xInBlock = 0; xInBlock < blockWidth; ++xInBlock) {
                  var x = blockX * blockWidth + xInBlock;

                  var paletteIndex = er.ReadByte();
                  var color = palette[paletteIndex];

                  var imageIndex = 4 * (y * width + x);
                  ptr[imageIndex + 0] = color.Bb;
                  ptr[imageIndex + 1] = color.Gb;
                  ptr[imageIndex + 2] = color.Rb;
                  ptr[imageIndex + 3] = color.Ab;
                }
              }
            }
          }
        }
      });

      er.Endianness = endianness;

      return image;
    }

    private Image ReadP4_(EndianBinaryReader er, uint width, uint height) {
      // TODO: This method seems incorrect...

      er.AssertStringEndian("PAL ");
      er.AssertUInt32(32);

      var endianness = er.Endianness;
      er.Endianness = Endianness.BigEndian;

      var paletteShorts = er.ReadUInt16s(16);

      var palette = paletteShorts
                    .Select(value => {
                      var r = ColorUtil.ExtractScaled(value, 0, 4);
                      var g = ColorUtil.ExtractScaled(value, 4, 4);
                      var b = ColorUtil.ExtractScaled(value, 8, 4);
                      var a = ColorUtil.ExtractScaled(value, 12, 4);

                      // TODO: Is this correct??? Textures don't look quite right
                      return ColorImpl.FromRgbaBytes(
                          (byte) r, (byte) g, (byte) b, (byte) a);
                    })
                    .ToArray();

      er.Endianness = endianness;

      er.AssertStringEndian("MIP ");
      var mipSize = width * height;
      er.AssertUInt32(mipSize / 2);

      er.Endianness = Endianness.BigEndian;

      var image = new Bitmap((int) width, (int) height);
      BitmapUtil.InvokeAsLocked(image, bmpData => {
        var blockWidth = 8;
        var blockHeight = 8;

        var blockCountX = width / blockWidth;
        var blockCountY = height / blockHeight;

        unsafe {
          var ptr = (byte*) bmpData.Scan0;
          for (var blockY = 0; blockY < blockCountY; ++blockY) {
            for (var blockX = 0; blockX < blockCountX; ++blockX) {
              for (var yInBlock = 0; yInBlock < blockHeight; ++yInBlock) {
                var y = blockY * blockHeight + yInBlock;
                for (var xInBlock = 0; xInBlock < blockWidth; xInBlock += 2) {
                  var x = blockX * blockWidth + xInBlock;

                  var paletteIndex = er.ReadByte();

                  var upperPaletteIndex = paletteIndex >> 4;
                  var color0 = palette[upperPaletteIndex];

                  var imageIndex0 = 4 * (y * width + x);
                  ptr[imageIndex0 + 0] = color0.Bb;
                  ptr[imageIndex0 + 1] = color0.Gb;
                  ptr[imageIndex0 + 2] = color0.Rb;
                  ptr[imageIndex0 + 3] = color0.Ab;

                  var lowerPaletteIndex = paletteIndex & 0xf;
                  var color1 = palette[lowerPaletteIndex];

                  var imageIndex1 = 4 * (y * width + x + 1);
                  ptr[imageIndex1 + 0] = color1.Bb;
                  ptr[imageIndex1 + 1] = color1.Gb;
                  ptr[imageIndex1 + 2] = color1.Rb;
                  ptr[imageIndex1 + 3] = color1.Ab;
                }
              }
            }
          }
        }
      });

      er.Endianness = endianness;

      return image;
    }

    private Image ReadIA8_(EndianBinaryReader er, uint width, uint height) {
      er.AssertStringEndian("MIP ");
      var mipSize = 2 * width * height;
      er.AssertUInt32(mipSize);

      er.Endianness = Endianness.BigEndian;

      var image = new Bitmap((int) width, (int) height);
      BitmapUtil.InvokeAsLocked(image, bmpData => {
        unsafe {
          var blockWidth = 4;
          var blockHeight = 4;

          var blockCountX = width / blockWidth;
          var blockCountY = height / blockHeight;

          var ptr = (byte*) bmpData.Scan0;
          for (var blockY = 0; blockY < blockCountY; ++blockY) {
            for (var blockX = 0; blockX < blockCountX; ++blockX) {
              for (var yInBlock = 0; yInBlock < blockHeight; ++yInBlock) {
                var y = blockY * blockHeight + yInBlock;
                for (var xInBlock = 0; xInBlock < blockWidth; ++xInBlock) {
                  var x = blockX * blockWidth + xInBlock;

                  var intensity = er.ReadByte();
                  var alpha = er.ReadByte();

                  var i = 4 * (y * width + x);

                  ptr[i + 0] = intensity;
                  ptr[i + 1] = intensity;
                  ptr[i + 2] = intensity;
                  ptr[i + 3] = alpha;
                }
              }
            }
          }
        }
      });

      er.Endianness = Endianness.LittleEndian;

      return image;
    }

    private Image ReadIA4_(EndianBinaryReader er, uint width, uint height) {
      er.AssertStringEndian("MIP ");
      var mipSize = width * height;
      er.AssertUInt32(mipSize);

      er.Endianness = Endianness.BigEndian;

      var image = new Bitmap((int) width, (int) height);
      BitmapUtil.InvokeAsLocked(image, bmpData => {
        unsafe {
          var blockWidth = 8;
          var blockHeight = 4;

          var blockCountX = width / blockWidth;
          var blockCountY = height / blockHeight;

          var ptr = (byte*) bmpData.Scan0;
          for (var blockY = 0; blockY < blockCountY; ++blockY) {
            for (var blockX = 0; blockX < blockCountX; ++blockX) {
              for (var yInBlock = 0; yInBlock < blockHeight; ++yInBlock) {
                var y = blockY * blockHeight + yInBlock;
                for (var xInBlock = 0; xInBlock < blockWidth; ++xInBlock) {
                  var x = blockX * blockWidth + xInBlock;

                  var color = er.ReadByte();

                  var intensity = ColorUtil.ExtractScaled(color, 0, 4);
                  var alpha = ColorUtil.ExtractScaled(color, 4, 4);

                  var i = 4 * (y * width + x);

                  ptr[i + 0] = intensity;
                  ptr[i + 1] = intensity;
                  ptr[i + 2] = intensity;
                  ptr[i + 3] = alpha;
                }
              }
            }
          }
        }
      });

      er.Endianness = Endianness.LittleEndian;

      return image;
    }

    private Image ReadI8_(EndianBinaryReader er, uint width, uint height) {
      er.AssertStringEndian("MIP ");
      var mipSize = width * height;
      er.AssertUInt32(mipSize);

      er.Endianness = Endianness.BigEndian;

      var image = new Bitmap((int)width, (int)height);
      BitmapUtil.InvokeAsLocked(image, bmpData => {
        unsafe {
          var blockWidth = 8;
          var blockHeight = 4;

          var blockCountX = width / blockWidth;
          var blockCountY = height / blockHeight;

          var ptr = (byte*)bmpData.Scan0;
          for (var blockY = 0; blockY < blockCountY; ++blockY) {
            for (var blockX = 0; blockX < blockCountX; ++blockX) {
              for (var yInBlock = 0; yInBlock < blockHeight; ++yInBlock) {
                var y = blockY * blockHeight + yInBlock;
                for (var xInBlock = 0; xInBlock < blockWidth; ++xInBlock) {
                  var x = blockX * blockWidth + xInBlock;

                  var intensity = er.ReadByte();

                  var i = 4 * (y * width + x);

                  ptr[i + 0] = intensity;
                  ptr[i + 1] = intensity;
                  ptr[i + 2] = intensity;
                  ptr[i + 3] = 255;
                }
              }
            }
          }
        }
      });

      er.Endianness = Endianness.LittleEndian;

      return image;
    }

    private Image ReadI4_(EndianBinaryReader er, uint width, uint height) {
      er.AssertStringEndian("MIP ");
      var mipSize = width * height / 2;
      er.AssertUInt32(mipSize);

      er.Endianness = Endianness.BigEndian;

      var image = new Bitmap((int) width, (int) height);
      BitmapUtil.InvokeAsLocked(image, bmpData => {
        unsafe {
          var blockWidth = 8;
          var blockHeight = 8;

          var blockCountX = width / blockWidth;
          var blockCountY = height / blockHeight;

          var ptr = (byte*) bmpData.Scan0;
          for (var blockY = 0; blockY < blockCountY; ++blockY) {
            for (var blockX = 0; blockX < blockCountX; ++blockX) {
              for (var yInBlock = 0; yInBlock < blockHeight; ++yInBlock) {
                var y = blockY * blockHeight + yInBlock;
                for (var xInBlock = 0; xInBlock < blockWidth; xInBlock += 2) {
                  var x = blockX * blockWidth + xInBlock;

                  var color = er.ReadByte();

                  var intensity1 = ColorUtil.ExtractScaled(color, 4, 4);
                  var intensity2 = ColorUtil.ExtractScaled(color, 0, 4);

                  var i = 4 * (y * width + x);

                  ptr[i + 0] = intensity1;
                  ptr[i + 1] = intensity1;
                  ptr[i + 2] = intensity1;
                  ptr[i + 3] = 255;

                  ptr[i + 4] = intensity2;
                  ptr[i + 5] = intensity2;
                  ptr[i + 6] = intensity2;
                  ptr[i + 7] = 255;
                }
              }
            }
          }
        }
      });

      er.Endianness = Endianness.LittleEndian;

      return image;
    }
  }

  public record BwTexture(string Name, Image Image);
}