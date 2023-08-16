using System.Collections.Generic;

using fin.color;
using fin.image;
using fin.image.formats;

using SixLabors.ImageSharp.PixelFormats;

namespace fin.src.image.formats {
  public class IndexedImage8 : BIndexedImage {
    private readonly IImage<L8> impl_;

    public IndexedImage8(PixelFormat pixelFormat,
                         IImage<L8> impl,
                         IReadOnlyList<IColor> palette) : base(
        pixelFormat,
        impl,
        palette) {
      this.impl_ = impl;
    }

    public override unsafe void Access(IImage.AccessHandler accessHandler) {
      using var bytes = this.impl_.Lock();
      var ptr = bytes.pixelScan0;

      void InternalGetHandler(
          int x,
          int y,
          out byte r,
          out byte g,
          out byte b,
          out byte a) {
        var index = ptr[y * this.Width + x];
        var color = this.Palette[index.PackedValue];
        r = color.Rb;
        g = color.Gb;
        b = color.Bb;
        a = color.Ab;
      }

      accessHandler(InternalGetHandler);
    }
  }
}
