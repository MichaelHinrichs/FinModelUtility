using System.Collections.Generic;

using fin.model;
using fin.model.impl;
using fin.util.color;

using mod.util;

namespace mod.schema.image {
  public class Cmpr : BImageFormat {
    public Cmpr(IList<byte> rawData, int imageWidth, int imageHeight) : base(
        rawData,
        imageWidth,
        imageHeight,
        8,
        8,
        4) {}

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
      var palette = this.DecodeCmprPalette_(reader.ReadU16(), reader.ReadU16());

      var sb = new IColor[4 * 4];
      for (var j = 0; j < 4; ++j) {
        var indices = reader.ReadU8();
        for (var i = 0; i < 4; ++i) {
          var index = (indices >> (2 * (3 - i))) & 0b11;
          sb[j * 4 + i] = palette[index];
        }
      }

      return sb;
    }

    private IColor[] DecodeCmprPalette_(ushort color1Value, ushort color2Value) {
      var palette = new IColor[4];

      var color1 = ColorUtil.ParseRgb565(color1Value);
      var color2 = ColorUtil.ParseRgb565(color2Value);

      IColor color3, color4;
      if (color1Value > color2Value) {
        color3 = ColorUtil.Interpolate(color1, color2, 1d / 3);
        color4 = ColorUtil.Interpolate(color1, color2, 2d / 3);
      } else {
        color3 = ColorUtil.Interpolate(color1, color2, .5);
        color4 = ColorImpl.FromRgbaBytes(0, 0, 0, 0);
      }

      palette[0] = color1;
      palette[1] = color2;
      palette[2] = color3;
      palette[3] = color4;

      return palette;
    }
  }
}