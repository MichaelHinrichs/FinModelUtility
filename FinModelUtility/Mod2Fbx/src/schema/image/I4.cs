using System.Collections.Generic;

using fin.color;
using fin.model;
using fin.model.impl;
using fin.util.color;

using mod.util;

namespace mod.schema.image {
  public class I4 : BImageFormat {
    public I4(IList<byte> rawData, int imageWidth, int imageHeight) : base(
        rawData,
        imageWidth,
        imageHeight,
        8,
        8,
        4) { }

    protected override IColor[] DecodeBlock(IList<byte> block, int position) {
      var bl = new IColor[8 * 8];
      var s = new VectorReader(block, position, Endianness.Big);

      for (var j = 0; j < 8; ++j) {
        for (var i = 0; i < 4; ++i) {
          var pixes = s.ReadU8();

          var intensity1 = ColorUtil.ExtractScaled(pixes, 4, 4, 17);
          var intensity2 = ColorUtil.ExtractScaled(pixes, 0, 4, 17);

          bl[j * 8 + 2 * i] = FinColor.FromIntensityByte(intensity1);
          bl[j * 8 + 2 * i + 1] = FinColor.FromIntensityByte(intensity2);
        }
      }
      
      return bl;
    }
  }
}