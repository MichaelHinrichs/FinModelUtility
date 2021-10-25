using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using fin.util.color;
using fin.util.image;

namespace zar.format.cmb {
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


    public unsafe Bitmap DecodeImage(byte[] input, Texture texture) {
      // M-1: Note: I don't think HiLo8 exist for .cmb

      if (texture.isEtc1) {
        return this.Etc1Decompress_(input, texture);
      }

      var format = this.CollapseFormat_(texture.imageFormat);
      var width = texture.width;
      var height = texture.height;

      var output = new Bitmap(width, height, PixelFormat.Format32bppArgb);

      BitmapUtil.InvokeAsLocked(
          output,
          bitmapData => {
            var o = (byte*) bitmapData.Scan0.ToPointer();

            using var er =
                new EndianBinaryReader(new MemoryStream(input),
                                       Endianness.LittleEndian);

            for (var ty = 0; ty < height; ty += 8) {
              for (var tx = 0; tx < width; tx += 8) {
                for (var px = 0; px < 64; ++px) {
                  var x = this.swizzleLut_[px] & 7;
                  var y = (this.swizzleLut_[px] - x) >> 3;

                  var OOffs = (tx + x + (height - 1 - (ty + y)) * width) * 4;

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
                      throw new NotImplementedException();
                      break;
                    }
                    case GlTextureFormat.LA8: {
                      a = er.ReadByte();
                      b = g = r = er.ReadByte();
                      break;
                    }
                    case GlTextureFormat.HiLo8: {
                      throw new NotImplementedException();
                      break;
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
                      break;
                    }
                    case GlTextureFormat.L4: {
                      throw new NotImplementedException();
                      break;
                    }
                    case GlTextureFormat.A4: {
                      throw new NotImplementedException();
                      break;
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

                  o[OOffs + 0] = b;
                  o[OOffs + 1] = g;
                  o[OOffs + 2] = r;
                  o[OOffs + 3] = a;
                }
              }
            }
          });

      return output;
    }

    private int[] xt_ = {0, 4, 0, 4};
    private int[] yt_ = {0, 0, 4, 4};

    private unsafe Bitmap Etc1Decompress_(byte[] data, Texture texture) {
      using var er =
          new EndianBinaryReader(new MemoryStream(data),
                                 Endianness.LittleEndian);

      var alpha = this.CollapseFormat_(texture.imageFormat) ==
                  GlTextureFormat.ETC1a4;

      var width = texture.width;
      var height = texture.height;

      var output = new Bitmap(width, height, PixelFormat.Format32bppArgb);

      BitmapUtil.InvokeAsLocked(
          output,
          bitmapData => {
            var o = (byte*) bitmapData.Scan0.ToPointer();

            for (var ty = 0; ty < height; ty += 8) {
              for (var tx = 0; tx < width; tx += 8) {
                for (var t = 0; t < 4; ++t) {
                  var alphaBlock = 0xffffffffffffffff;

                  if (alpha) {
                    // ReSharper disable once AccessToDisposedClosure
                    alphaBlock = er.ReadUInt64();
                  }

                  // ReSharper disable once AccessToDisposedClosure
                  var col = er.ReadUInt64();
                  var colorBlock = this.Swap64_(col);
                  var tile = this.Etc1Tile_(colorBlock);
                  var tileOffset = 0;

                  for (var py = this.yt_[t]; py < this.yt_[t] + 4; ++py) {
                    for (var px = this.xt_[t]; px < this.xt_[t] + 4; ++px) {
                      var oOffs = ((height - 1 - (ty + py)) * width + tx + px) *
                                  4;

                      var r = tile[tileOffset + 0];
                      var g = tile[tileOffset + 1];
                      var b = tile[tileOffset + 2];

                      var alphaShift = ((px & 3) * 4 + (py & 3)) << 2;
                      var a = (alphaBlock >> alphaShift) & 0xf;
                      a = (a << 4) | a;

                      o[oOffs + 0] = b;
                      o[oOffs + 1] = g;
                      o[oOffs + 2] = r;
                      o[oOffs + 3] = (byte) a;

                      tileOffset += 4;
                    }
                  }
                }
              }
            }
          });

      return output;
    }

    private ulong Swap64_(ulong value) {
      value = ((value & 0xffffffff00000000) >> 32) |
              ((value & 0x00000000ffffffff) << 32);
      value = ((value & 0xffff0000ffff0000) >> 16) |
              ((value & 0x0000ffff0000ffff) << 16);
      value = ((value & 0xff00ff00ff00ff00) >> 8) |
              ((value & 0x00ff00ff00ff00ff) << 8);
      return value;
    }

    private byte[] Etc1Tile_(ulong block) {
      var blockLow = (uint) (block >> 32);
      var blockHigh = (uint) (block >> 0);

      var flip = (blockHigh & 0x1000000) != 0;
      var diff = (blockHigh & 0x2000000) != 0;

      var r1 = 0l;
      var g1 = 0l;
      var b1 = 0l;
      var r2 = 0l;
      var g2 = 0l;
      var b2 = 0l;

      if (diff) {
        b1 = (blockHigh & 0x0000f8) >> 0;
        g1 = (blockHigh & 0x00f800) >> 8;
        r1 = (blockHigh & 0xf80000) >> 16;

        // CAST AS SBYTE IS THE ISSUE
        b2 = (this.CastSByte_(b1 >> 3) +
              (this.CastSByte_((blockHigh & 0x000007) << 5) >> 5));
        g2 = (this.CastSByte_(g1 >> 3) +
              (this.CastSByte_((blockHigh & 0x000700) >> 3) >> 5));
        r2 = (this.CastSByte_(r1 >> 3) +
              (this.CastSByte_((blockHigh & 0x070000) >> 11) >> 5));

        b1 |= b1 >> 5;
        g1 |= g1 >> 5;
        r1 |= r1 >> 5;

        b2 = (b2 << 3) | (b2 >> 2);
        g2 = (g2 << 3) | (g2 >> 2);
        r2 = (r2 << 3) | (r2 >> 2);
      } else {
        b1 = (blockHigh & 0x0000f0) >> 0;
        g1 = (blockHigh & 0x00f000) >> 8;
        r1 = (blockHigh & 0xf00000) >> 16;

        b2 = (blockHigh & 0x00000f) << 4;
        g2 = (blockHigh & 0x000f00) >> 4;
        r2 = (blockHigh & 0x0f0000) >> 12;

        b1 |= b1 >> 4;
        g1 |= g1 >> 4;
        r1 |= r1 >> 4;

        b2 |= b2 >> 4;
        g2 |= g2 >> 4;
        r2 |= r2 >> 4;
      }

      var table1 = (blockHigh >> 29) & 7;
      var table2 = (blockHigh >> 26) & 7;

      var output = new byte[4 * 4 * 4];

      if (!flip) {
        for (var y = 0; y < 4; ++y) {
          for (var x = 0; x < 2; ++x) {
            var color1 =
                this.Etc1Pixel_(r1, g1, b1, x + 0, y, blockLow, table1);
            var color2 =
                this.Etc1Pixel_(r2, g2, b2, x + 2, y, blockLow, table2);

            var offset1 = (y * 4 + x) * 4;
            output[offset1 + 0] = color1[2];
            output[offset1 + 1] = color1[1];
            output[offset1 + 2] = color1[0];

            var offset2 = (y * 4 + x + 2) * 4;
            output[offset2 + 0] = color2[2];
            output[offset2 + 1] = color2[1];
            output[offset2 + 2] = color2[0];
          }
        }
      } else {
        for (var y = 0; y < 2; ++y) {
          for (var x = 0; x < 4; ++x) {
            var color1 =
                this.Etc1Pixel_(r1, g1, b1, x, y + 0, blockLow, table1);
            var color2 =
                this.Etc1Pixel_(r2, g2, b2, x, y + 2, blockLow, table2);

            var offset1 = (y * 4 + x) * 4;
            output[offset1 + 0] = color1[2];
            output[offset1 + 1] = color1[1];
            output[offset1 + 2] = color1[0];

            var offset2 = ((y + 2) * 4 + x) * 4;
            output[offset2 + 0] = color2[2];
            output[offset2 + 1] = color2[1];
            output[offset2 + 2] = color2[0];
          }
        }
      }

      return output;
    }

    private long CastSByte_(long value) {
      if (value < -127) {
        return (value + 255);
      }
      if (value > 127) {
        return (value - 255);
      }
      return value;
    }

    private byte[] Etc1Pixel_(
        long r,
        long g,
        long b,
        int x,
        int y,
        ulong block,
        ulong table) {
      var Index = x * 4 + y;
      var MSB = block << 1;

      int pixel;
      if (Index < 8) {
        pixel = this.etc1Lut_[table][((block >> (Index + 24)) & 1) +
                                     ((MSB >> (Index + 8)) & 2)];
      } else {
        pixel = this.etc1Lut_[table][((block >> (Index + 8)) & 1) +
                                     ((MSB >> (Index - 8)) & 2)];
      }

      return new[] {
          this.Saturate_(r + pixel),
          this.Saturate_(g + pixel),
          this.Saturate_(b + pixel)
      };
    }

    private byte Saturate_(long value)
      => (byte) Math.Max(0, Math.Min(value, 255));
  }
}