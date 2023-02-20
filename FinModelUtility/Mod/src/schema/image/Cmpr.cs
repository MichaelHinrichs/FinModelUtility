using System;
using System.Collections.Generic;

using fin.color;
using fin.util.color;

using mod.util;

using SixLabors.ImageSharp.PixelFormats;

namespace mod.schema.image {
  public class Cmpr : BImageFormat {
    public Cmpr(IList<byte> rawData, int imageWidth, int imageHeight) : base(
        rawData,
        imageWidth,
        imageHeight,
        8,
        8,
        4) { }

    protected override IColor[] DecodeBlock(IList<byte> block, int position) {
      var bl = new IColor[8 * 8];
      var s = new VectorReader(block, position, Endianness.Big);

      for (var j = 0; j < 2; ++j) {
        for (var i = 0; i < 2; ++i) {
          var subblock = this.DecodeCmprSubblock_(block, position);
          position += 8;

          for (var r = 0; r < 4; ++r) {
            for (var c = 0; c < 4; ++c) {
              var x = i * 4 + c;
              var y = j * 4 + r;

              bl[y * 8 + x] = subblock[r * 4 + c];
            }
          }
        }
      }

      return bl;
    }

    private IColor[] DecodeCmprSubblock_(IList<byte> block, int position) {
      var reader = new VectorReader(block, position, Endianness.Big);

      Span<Rgba32> palette = stackalloc Rgba32[4];
      DecodeCmprPalette_(palette, reader.ReadU16(), reader.ReadU16());

      var sb = new IColor[4 * 4];
      for (var j = 0; j < 4; ++j) {
        var indices = reader.ReadU8();
        for (var i = 0; i < 4; ++i) {
          var index = (indices >> (2 * (3 - i))) & 0b11;
          sb[j * 4 + i] = FinColor.FromRgba(palette[index]);
        }
      }

      return sb;
    }

    private static void DecodeCmprPalette_(Span<Rgba32> palette,
                                           ushort color1Value,
                                           ushort color2Value) {
      ColorUtil.SplitRgb565(color1Value, out var r1, out var g1, out var b1);
      ColorUtil.SplitRgb565(color2Value, out var r2, out var g2, out var b2);

      palette[0] = new Rgba32(r1, g1, b1, 255);
      palette[1] = new Rgba32(r2, g2, b2, 255);

      if (color1Value > color2Value) {
        // 3rd color in palette is 1/3 from 1st to 2nd.
        palette[2] = new Rgba32(
            (byte) ((((int) r1 << 1) + (int) r2) / 3),
            (byte) ((((int) g1 << 1) + (int) g2) / 3),
            (byte) ((((int) b1 << 1) + (int) b2) / 3),
            byte.MaxValue);
        // 4th color in palette is 2/3 from 1st to 2nd.
        palette[3] = new Rgba32(
            (byte) (((int) r1 + ((int) r2 << 1)) / 3),
            (byte) (((int) g1 + ((int) g2 << 1)) / 3),
            (byte) (((int) b1 + ((int) b2 << 1)) / 3),
            byte.MaxValue);
      } else {
        // 3rd color in palette is halfway between 1st and 2nd.
        palette[2] = new Rgba32(
            (byte) (((int) r1 + (int) r2) >> 1),
            (byte) (((int) g1 + (int) g2) >> 1),
            (byte) (((int) b1 + (int) b2) >> 1),
            byte.MaxValue);
        // 4th color in palette is transparency.
        palette[3] = new Rgba32(0, 0, 0, 0);
      }
    }
  }
}