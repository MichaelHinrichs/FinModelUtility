using System.Collections.Generic;

using fin.model;
using fin.model.impl;

using mod.util;

namespace mod.gcn.image {
  public class Rgba32 : BImageFormat {
    private const int BLOCK_WIDTH = 4;
    private const int BLOCK_HEIGHT = 4;

    public Rgba32(IList<byte> rawData, int imageWidth, int imageHeight) : base(
        rawData,
        imageWidth,
        imageHeight,
        Rgba32.BLOCK_WIDTH,
        Rgba32.BLOCK_HEIGHT,
        32) {}

    protected override IColor[] DecodeBlock(IList<byte> block, int position) {
      var bl = new IColor[Rgba32.BLOCK_WIDTH * Rgba32.BLOCK_HEIGHT];
      var s = new VectorReader(block, position, Endianness.Big);

      var colors = new (byte, byte, byte, byte)[Rgba32.BLOCK_WIDTH *
                                                Rgba32.BLOCK_HEIGHT];
      for (var i = 0; i < 16; ++i) {
        var a = s.ReadU8();
        var r = s.ReadU8();
        colors[i] = (r, 0, 0, a);
      }

      for (var i = 0; i < 16; ++i) {
        var g = s.ReadU8();
        var b = s.ReadU8();

        var (r, _, _, a) = colors[i];
        colors[i] = (r, g, b, a);
      }

      for (var j = 0; j < Rgba32.BLOCK_HEIGHT; ++j) {
        for (var i = 0; i < Rgba32.BLOCK_WIDTH; ++i) {
          var index = j * Rgba32.BLOCK_WIDTH + i;
          var (r, g, b, a) = colors[index];
          bl[index] = ColorImpl.FromRgbaBytes(r, g, b, a);
        }
      }

      return bl;
    }
  }
}