using System;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace fin.image.formats {
  public class L8Image : BImage<L8> {
    public L8Image(PixelFormat format, int width, int height) : this(
        format,
        new Image<L8>(FinImage.ImageSharpConfig, width, height)) { }

    internal L8Image(PixelFormat format, Image<L8> impl) : base(
        format) {
      this.Impl = impl;
    }

    protected override Image<L8> Impl { get; }

    public override void Access(IImage.AccessHandler accessHandler)
      => FinImage.Access(
          this.Impl,
          getHandler => {
            void InternalGetHandler(
                int x,
                int y,
                out byte r,
                out byte g,
                out byte b,
                out byte a) {
              getHandler(x, y, out var pixel);
              r = g = b = pixel.PackedValue;
              a = 255;
            }

            accessHandler(InternalGetHandler);
          });

    public delegate void GetHandler(int x,
                                    int y,
                                    out byte intensity);

    public delegate void SetHandler(int x,
                                    int y,
                                    byte intensity);

    public delegate void MutateHandler(GetHandler getHandler,
                                       SetHandler setHandler);

    public void Mutate(MutateHandler mutateHandler)
      => FinImage.Mutate(
          this.Impl,
          (getHandler, setHandler) => {
            void InternalGetHandler(
                int x,
                int y,
                out byte i) {
              getHandler(x, y, out var pixel);
              i = pixel.PackedValue;
            }

            void InternalSetHandler(
                int x,
                int y,
                byte i) {
              var pixel = new L8(i);
              setHandler(x, y, pixel);
            }

            mutateHandler(InternalGetHandler,
                          InternalSetHandler);
          });

    public override bool HasAlphaChannel => false;

    public void GetI8Bytes(Span<byte> bytes)
      => this.Impl.CopyPixelDataTo(bytes);
  }
}