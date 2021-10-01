using System.Collections.Generic;

using fin.math;
using fin.model;
using fin.model.impl;

using mod.util;

namespace mod.gcn.image {
  public class Rgb5A3 : BImageFormat {
    public Rgb5A3(IList<byte> rawData, int imageWidth, int imageHeight) : base(
        rawData,
        imageWidth,
        imageHeight,
        4,
        4,
        32,
        16) {}

    protected override IColor[] DecodeBlock(IList<byte> block, int position) {
      var bl = new IColor[4 * 4];
      var s = new VectorReader(block, position, Endianness.Big);

      for (var j = 0; j < 4; ++j) {
        for (var i = 0; i < 4; ++i) {
          var pix = s.ReadU16();

          var alphaFlag = BitLogic.ExtractFromRight(pix, 15, 1);

          byte r, g, b, a;
          if (alphaFlag == 1) {
            a = 255;
            r = ColorUtil.ExtractScaled(pix, 10, 5);
            g = ColorUtil.ExtractScaled(pix, 5, 5);
            b = ColorUtil.ExtractScaled(pix,0, 5);
          } else {
            a = ColorUtil.ExtractScaled(pix, 12, 3);

            r = ColorUtil.ExtractScaled(pix, 8, 4, 17);
            g = ColorUtil.ExtractScaled(pix, 4, 4, 17);
            b = ColorUtil.ExtractScaled(pix, 0, 4, 17);
          }

          bl[j * 4 + i] = ColorImpl.FromBytes(r, g, b, a);
        }
      }

      return bl;
    }
  }
}