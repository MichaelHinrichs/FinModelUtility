﻿using fin.image.formats;

using schema.binary;

using SixLabors.ImageSharp.PixelFormats;

namespace fin.image.io.pixel {
  /// <summary>
  ///   Helper class for reading 4-bit luminance pixels.
  /// </summary>
  public class L4PixelReader : IPixelReader<L8> {
    public IImage<L8> CreateImage(int width, int height)
      => new L8Image(PixelFormat.L4, width, height);

    public unsafe void Decode(IBinaryReader br,
                              L8* scan0,
                              int offset) {
      var value = br.ReadByte();

      var upper = (byte) ((value >> 4) * 17);
      var lower = (byte) ((value & 0xF) * 17);

      scan0[offset + 0] = new L8(upper);
      scan0[offset + 1] = new L8(lower);
    }

    public int PixelsPerRead => 2;
  }
}