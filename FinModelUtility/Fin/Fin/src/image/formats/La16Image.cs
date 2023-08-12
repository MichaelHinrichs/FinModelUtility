using System;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace fin.image.formats {
  public class La16Image : BImage<La16> {
    public La16Image(PixelFormat format, int width, int height) : this(
        format,
        new Image<La16>(FinImage.ImageSharpConfig, width, height)) { }

    internal La16Image(PixelFormat format, Image<La16> impl) : base(
        format) {
      this.Impl = impl;
    }

    protected override Image<La16> Impl { get; }

    public override void Access(IImage.AccessHandler accessHandler) {
      var frame = this.Impl.Frames[0];

      void GetHandler(
          int x,
          int y,
          out byte r,
          out byte g,
          out byte b,
          out byte a) {
        var pixel = frame[x, y];
        r = g = b = pixel.L;
        a = pixel.A;
      }

      accessHandler(GetHandler);
    }

    public delegate void GetHandler(int x,
                                    int y,
                                    out byte intensity,
                                    out byte alpha);

    public delegate void SetHandler(int x,
                                    int y,
                                    byte intensity,
                                    byte alpha);

    public delegate void MutateHandler(GetHandler getHandler,
                                       SetHandler setHandler);

    public void Mutate(MutateHandler mutateHandler) {
      var frame = this.Impl.Frames[0];

      void GetHandler(
          int x,
          int y,
          out byte i,
          out byte a) {
        var pixel = frame[x, y];
        i = pixel.L;
        a = pixel.A;
      }

      void SetHandler(int x, int y, byte i, byte a) {
        var pixel = frame[x, y];
        pixel.L = i;
        pixel.A = a;
        frame[x, y] = pixel;
      }

      mutateHandler(GetHandler, SetHandler);
    }

    public override bool HasAlphaChannel => true;

    public void GetIa16Bytes(Span<byte> bytes)
      => this.Impl.CopyPixelDataTo(bytes);
  }
}