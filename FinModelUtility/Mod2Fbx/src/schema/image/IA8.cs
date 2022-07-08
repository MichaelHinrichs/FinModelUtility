using System.Collections.Generic;

using fin.model;
using fin.model.impl;

using mod.util;

namespace mod.schema.image {
  public class IA8 : BImageFormat {
    public IA8(IList<byte> rawData, int imageWidth, int imageHeight) : base(
        rawData,
        imageWidth,
        imageHeight,
        4,
        4,
        16) { }

    protected override IColor[] DecodeBlock(IList<byte> block, int position) {
      var bl = new IColor[4 * 4];
      var s = new VectorReader(block, position, Endianness.Big);

      for (var j = 0; j < 4; ++j) {
        for (var i = 0; i < 4; ++i) {
          var intensity = s.ReadU8();
          var alpha = s.ReadU8();

          bl[j * 4 + i] = ColorImpl.FromRgbaBytes(intensity, intensity, intensity, alpha); 
        }
      }
      
      return bl;
    }
  }
}