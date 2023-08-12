using System;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace fin.image.formats {
  public class Rgba32Image : BImage<Rgba32> {
    public Rgba32Image(PixelFormat format, int width, int height) : this(
        format,
        new Image<Rgba32>(FinImage.ImageSharpConfig, width, height)) { }

    internal Rgba32Image(PixelFormat format, Image<Rgba32> impl) : base(
        format) {
      this.Impl = impl;
    }

    protected override Image<Rgba32> Impl { get; }

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
              r = pixel.R;
              g = pixel.G;
              b = pixel.B;
              a = pixel.A;
            }

            accessHandler(InternalGetHandler);
          });

    public delegate void SetHandler(int x,
                                    int y,
                                    byte r,
                                    byte g,
                                    byte b,
                                    byte a);

    public delegate void MutateHandler(IImage.Rgba32GetHandler getHandler,
                                       SetHandler setHandler);

    public void Mutate(MutateHandler mutateHandler)
      => FinImage.Mutate(
          this.Impl,
          (getHandler, setHandler) => {
            void InternalGetHandler(
                int x,
                int y,
                out byte r,
                out byte g,
                out byte b,
                out byte a) {
              getHandler(x, y, out var pixel);
              r = pixel.R;
              g = pixel.G;
              b = pixel.B;
              a = pixel.A;
            }

            void InternalSetHandler(
                int x,
                int y,
                byte r,
                byte g,
                byte b,
                byte a) {
              var pixel = new Rgba32(r, g, b, a);
              setHandler(x, y, pixel);
            }

            mutateHandler(InternalGetHandler,
                          InternalSetHandler);
          });

    public override bool HasAlphaChannel => true;

    public void GetRgba32Bytes(Span<byte> bytes)
      => this.Impl.CopyPixelDataTo(bytes);
  }
}