using System.Collections.Generic;

using fin.model;
using fin.model.impl;

using mod.util;

namespace mod.gcn.image {
  public class I8 : BImageFormat {
    public I8(IList<byte> rawData, int imageWidth, int imageHeight) : base(
        rawData,
        imageWidth,
        imageHeight,
        8,
        4,
        32,
        8) { }

    protected override IColor[] DecodeBlock(IList<byte> block, int position) {
      var bl = new IColor[8 * 4];
      var s = new VectorReader(block, position, Endianness.Big);

      for (var j = 0; j < 4; ++j) {
        for (var i = 0; i < 8; ++i) {
          var pix = s.ReadU8();

          bl[j * 8 + i] = ColorImpl.FromRgbaBytes(pix, pix, pix, 255);
        }
      }
      
      return bl;
    }
  }
}