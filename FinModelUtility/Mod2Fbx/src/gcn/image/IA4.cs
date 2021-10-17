using System.Collections.Generic;

using fin.model;
using fin.model.impl;
using fin.util.color;

using mod.util;

namespace mod.gcn.image {
  public class IA4 : BImageFormat {
    public IA4(IList<byte> rawData, int imageWidth, int imageHeight) : base(
        rawData,
        imageWidth,
        imageHeight,
        8,
        4,
        8) { }

    protected override IColor[] DecodeBlock(IList<byte> block, int position) {
      var bl = new IColor[8 * 4];
      var s = new VectorReader(block, position, Endianness.Big);

      for (var j = 0; j < 4; ++j) {
        for (var i = 0; i < 8; ++i) {
          var pixes = s.ReadU8();

          var intensity = ColorUtil.ExtractScaled(pixes, 4, 4, 17);
          var alpha = ColorUtil.ExtractScaled(pixes, 0, 4, 17);

          bl[j * 8 + i] = ColorImpl.FromRgbaBytes(intensity, intensity, intensity, alpha);
        }
      }
      
      return bl;
    }
  }
}