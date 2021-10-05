using System.Collections.Generic;

using fin.math;
using fin.model;
using fin.model.impl;

using mod.util;

namespace mod.gcn.image {
  public class I4 : BImageFormat {
    public I4(IList<byte> rawData, int imageWidth, int imageHeight) : base(
        rawData,
        imageWidth,
        imageHeight,
        8,
        8,
        32,
        4) { }

    protected override IColor[] DecodeBlock(IList<byte> block, int position) {
      var bl = new IColor[8 * 8];
      var s = new VectorReader(block, position, Endianness.Big);

      for (var j = 0; j < 8; ++j) {
        for (var i = 0; i < 4; ++i) {
          var pixes = s.ReadU8();

          var pix1 = ColorUtil.ExtractScaled(pixes, 4, 4, 17);
          var pix2 = ColorUtil.ExtractScaled(pixes, 0, 4, 17);

          bl[j * 8 + 2 * i] = ColorImpl.FromBytes(pix1, pix1, pix1, 255);
          bl[j * 8 + 2 * i + 1] = ColorImpl.FromBytes(pix2, pix2, pix2, 255);
        }
      }
      
      return bl;
    }
  }
}