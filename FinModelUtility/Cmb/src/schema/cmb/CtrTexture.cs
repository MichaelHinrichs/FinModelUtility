using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using cmb.schema.cmb.texture;

using fin.image;
using fin.util.asserts;
using fin.util.color;
using fin.util.image;


namespace cmb.schema.cmb {
  public class CtrTexture {
    private int[] swizzleLut_ = {
        0, 1, 8, 9, 2, 3, 10, 11,
        16, 17, 24, 25, 18, 19, 26, 27,
        4, 5, 12, 13, 6, 7, 14, 15,
        20, 21, 28, 29, 22, 23, 30, 31,
        32, 33, 40, 41, 34, 35, 42, 43,
        48, 49, 56, 57, 50, 51, 58, 59,
        36, 37, 44, 45, 38, 39, 46, 47,
        52, 53, 60, 61, 54, 55, 62, 63
    };

    private int[][] etc1Lut_ = {
        new[] {2, 8, -2, -8},
        new[] {5, 17, -5, -17},
        new[] {9, 29, -9, -29},
        new[] {13, 42, -13, -42},
        new[] {18, 60, -18, -60},
        new[] {24, 80, -24, -80},
        new[] {33, 106, -33, -106},
        new[] {47, 183, -47, -183},
    };

    private GlTextureFormat CollapseFormat_(GlTextureFormat format) {
      var lowerFormat = (GlTextureFormat) ((int) format & 0xFFFF);

      if (lowerFormat == GlTextureFormat.ETC1) {
        format = GlTextureFormat.ETC1;
      } else if (lowerFormat == GlTextureFormat.ETC1a4) {
        format = GlTextureFormat.ETC1a4;
      }

      return format;
    }

    private int GetFmtBpp_(GlTextureFormat format)
      => this.CollapseFormat_(format) switch {
          GlTextureFormat.RGBA8    => 32,
          GlTextureFormat.RGB8     => 24,
          GlTextureFormat.RGBA5551 => 16,
          GlTextureFormat.RGB565   => 16,
          GlTextureFormat.RGBA4444 => 16,
          GlTextureFormat.LA8      => 16,
          GlTextureFormat.Gas      => 8,
          GlTextureFormat.HiLo8    => 8,
          GlTextureFormat.L8       => 8,
          GlTextureFormat.A8       => 8,
          GlTextureFormat.LA4      => 8,
          GlTextureFormat.Shadow   => 8,
          GlTextureFormat.L4       => 4,
          GlTextureFormat.A4       => 4,
          GlTextureFormat.ETC1     => 4,
          GlTextureFormat.ETC1a4   => 8,
          _ => throw new ArgumentOutOfRangeException(
                   nameof(format),
                   format,
                   null)
      };


    public IImage DecodeImage(
        byte[] input,
        Texture texture) {
      // M-1: Note: I don't think HiLo8 exist for .cmb

      if (texture.isEtc1) {
        return this.Etc1Decompress_(input, texture);
      }

      var format = this.CollapseFormat_(texture.imageFormat);
      var width = texture.width;
      var height = texture.height;

      var output = new Rgba32Image(width, height);
      output.Mutate((_, setHandler) => {
        using var er =
            new EndianBinaryReader(new MemoryStream(input),
                                   Endianness.LittleEndian);

        for (var ty = 0; ty < height; ty += 8) {
          for (var tx = 0; tx < width; tx += 8) {
            for (var px = 0; px < 64; ++px) {
              byte r, g, b, a;

              switch (format) {
                case GlTextureFormat.RGB8: {
                  a = 255;
                  b = er.ReadByte();
                  g = er.ReadByte();
                  r = er.ReadByte();
                  break;
                }
                case GlTextureFormat.RGBA8: {
                  a = er.ReadByte();
                  b = er.ReadByte();
                  g = er.ReadByte();
                  r = er.ReadByte();
                  break;
                }
                case GlTextureFormat.RGBA5551: {
                  var value = er.ReadUInt16();
                  ColorUtil.SplitRgb5A1(value, out r, out g, out b, out a);
                  break;
                }
                case GlTextureFormat.RGB565: {
                  var value = er.ReadUInt16();
                  a = 255;
                  ColorUtil.SplitRgb565(value, out r, out g, out b);
                  break;
                }
                case GlTextureFormat.RGBA4444: {
                  var value = er.ReadUInt16();
                  ColorUtil.SplitRgba4444(value,
                                          out r,
                                          out g,
                                          out b,
                                          out a);
                  break;
                }
                case GlTextureFormat.LA8: {
                  a = er.ReadByte();
                  b = g = r = er.ReadByte();
                  break;
                }
                case GlTextureFormat.HiLo8: {
                  throw new NotImplementedException();
                }
                case GlTextureFormat.A8: {
                  a = er.ReadByte();
                  b = 255;
                  g = 255;
                  r = 255;
                  break;
                }
                case GlTextureFormat.LA4: {
                  throw new NotImplementedException();
                }
                case GlTextureFormat.L4: {
                  throw new NotImplementedException();
                }
                case GlTextureFormat.A4: {
                  throw new NotImplementedException();
                }
                case GlTextureFormat.L8:
                case GlTextureFormat.Gas:
                case GlTextureFormat.Shadow: {
                  a = 255;
                  b = g = r = er.ReadByte();
                  break;
                }
                default: throw new ArgumentOutOfRangeException();
              }

              var x = this.swizzleLut_[px] & 7;
              var y = (this.swizzleLut_[px] - x) >> 3;
              setHandler(tx + x, ty + y, r, g, b, a);
            }
          }
        }
      });

      return output;
    }

    private IImage Etc1Decompress_(byte[] data, Texture texture) {
      using var er =
          new EndianBinaryReader(new MemoryStream(data),
                                 Endianness.LittleEndian);

      var width = texture.width;
      var height = texture.height;

      var bytes =
          Etc1.Decompress(er, width, height, texture.imageFormat, data.Length);

      // The expected size will not be exactly the same as the number of decompressed bytes if there are mipmaps.
      Asserts.True(width * height * 4 <= bytes.Length);

      var output = new Rgba32Image(width, height);
      output.Mutate((_, setHandler) => {
        for (var y = 0; y < height; ++y) {
          for (var x = 0; x < width; ++x) {
            var i = 4 * (y * width + x);

            var r = bytes[i + 0];
            var g = bytes[i + 1];
            var b = bytes[i + 2];
            var a = bytes[i + 3];

            setHandler(x, y, r, g, b, a);
          }
        }
      });

      return output;
    }
  }
}