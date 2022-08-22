using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using fin.io;
using fin.util.image;


namespace fin.image {
  using SystemImageFormat = System.Drawing.Imaging.ImageFormat;

  public static class FinImage {
    public static IImage FromFile(IFile file) {
      using var stream = file.OpenRead();
      return FinImage.FromStream(stream);
    }

    public static IImage FromStream(Stream stream) {
      var image = Bitmap.FromStream(stream) as Bitmap;
      var pixelFormat = image.PixelFormat;

      switch (pixelFormat) {
        case PixelFormat.Format32bppArgb:
          return new Rgba32Image(image);
        case PixelFormat.Format24bppRgb:
          return new Rgb24Image(image);
        default:
          throw new ArgumentOutOfRangeException(
              nameof(pixelFormat), pixelFormat, null);
      }
    }

    public static SystemImageFormat ConvertFinImageFormatToSystem(
        LocalImageFormat imageFormat)
      => imageFormat switch {
          LocalImageFormat.BMP  => SystemImageFormat.Bmp,
          LocalImageFormat.PNG  => SystemImageFormat.Png,
          LocalImageFormat.JPEG => SystemImageFormat.Jpeg,
          LocalImageFormat.GIF  => SystemImageFormat.Gif,
          _ => throw new ArgumentOutOfRangeException(
                   nameof(imageFormat), imageFormat, null)
      };
  }

  public class Rgba32Image : IImage {
    private readonly Bitmap impl_;

    public Rgba32Image(int width, int height) : this(
        new Bitmap(width, height, PixelFormat.Format32bppArgb)) { }

    internal Rgba32Image(Bitmap bitmap) {
      this.impl_ = new Bitmap(bitmap);
    }

    ~Rgba32Image() => this.ReleaseUnmanagedResources_();

    public void Dispose() {
      this.ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() => this.impl_.Dispose();

    public int Width => this.impl_.Width;
    public int Height => this.impl_.Height;

    public delegate void GetHandler(int x,
                                    int y,
                                    out byte r,
                                    out byte g,
                                    out byte b,
                                    out byte a);

    public delegate void SetHandler(int x,
                                    int y,
                                    byte r,
                                    byte g,
                                    byte b,
                                    byte a);

    public delegate void MutateHandler(GetHandler getHandler,
                                       SetHandler setHandler);

    public unsafe void Mutate(MutateHandler mutateHandler) {
      BitmapUtil.InvokeAsLocked(this.impl_, bmpData => {
        var ptr = (byte*) bmpData.Scan0;

        GetHandler getHandler =
            (int x, int y, out byte r, out byte g, out byte b, out byte a) => {
              var index = 4 * (y * bmpData.Width + x);
              b = ptr[index];
              g = ptr[index + 1];
              r = ptr[index + 2];
              a = ptr[index + 3];
            };

        SetHandler setHandler = (x, y, r, g, b, a) => {
          var index = 4 * (y * bmpData.Width + x);
          ptr[index] = b;
          ptr[index + 1] = g;
          ptr[index + 2] = r;
          ptr[index + 3] = a;
        };

        mutateHandler(getHandler, setHandler);
      });
    }

    public void ExportToStream(Stream stream, LocalImageFormat imageFormat)
      => this.impl_.Save(
          stream, FinImage.ConvertFinImageFormatToSystem(imageFormat));
  }

  public class Rgb24Image : IImage {
    private readonly Bitmap impl_;

    public Rgb24Image(int width, int height) : this(
        new Bitmap(width, height, PixelFormat.Format24bppRgb)) { }

    internal Rgb24Image(Bitmap bitmap) {
      this.impl_ = new Bitmap(bitmap);
    }

    ~Rgb24Image() => this.ReleaseUnmanagedResources_();

    public void Dispose() {
      this.ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() => this.impl_.Dispose();

    public int Width => this.impl_.Width;
    public int Height => this.impl_.Height;

    public delegate void GetHandler(int x,
                                    int y,
                                    out byte r,
                                    out byte g,
                                    out byte b);

    public delegate void SetHandler(int x,
                                    int y,
                                    byte r,
                                    byte g,
                                    byte b);

    public delegate void MutateHandler(GetHandler getHandler,
                                       SetHandler setHandler);

    public unsafe void Mutate(MutateHandler mutateHandler) {
      BitmapUtil.InvokeAsLocked(this.impl_, bmpData => {
        var ptr = (byte*) bmpData.Scan0;

        GetHandler getHandler =
            (int x, int y, out byte r, out byte g, out byte b) => {
              var index = 3 * (y * bmpData.Width + x);
              b = ptr[index];
              g = ptr[index + 1];
              r = ptr[index + 2];
            };

        SetHandler setHandler = (x, y, r, g, b) => {
          var index = 3 * (y * bmpData.Width + x);
          ptr[index] = b;
          ptr[index + 1] = g;
          ptr[index + 2] = r;
        };

        mutateHandler(getHandler, setHandler);
      });
    }

    public void ExportToStream(Stream stream, LocalImageFormat imageFormat)
      => this.impl_.Save(
          stream, FinImage.ConvertFinImageFormatToSystem(imageFormat));
  }

  public class Ia16Image : IImage {
    private readonly Rgba32Image impl_;

    public Ia16Image(int width, int height) : this(
        new Bitmap(width, height, PixelFormat.Format32bppArgb)) { }

    internal Ia16Image(Bitmap bitmap) {
      this.impl_ = new Rgba32Image(bitmap);
    }

    ~Ia16Image() => this.ReleaseUnmanagedResources_();

    public void Dispose() {
      this.ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() => this.impl_.Dispose();

    public int Width => this.impl_.Width;
    public int Height => this.impl_.Height;

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

    public unsafe void Mutate(MutateHandler mutateHandler) {
      this.impl_.Mutate((rgbaGetHandler, rgbaSetHandler) => {
        GetHandler iaGetHandler =
            (int x, int y, out byte intensity, out byte alpha) =>
                rgbaGetHandler(x, y, out intensity, out _, out _, out alpha);

        SetHandler iaSetHandler = (x, y, intensity, alpha) =>
            rgbaSetHandler(x, y, intensity, intensity, intensity, alpha);

        mutateHandler(iaGetHandler, iaSetHandler);
      });
    }

    public void ExportToStream(Stream stream, LocalImageFormat imageFormat)
      => this.impl_.ExportToStream(stream, imageFormat);
  }

  public class I8Image : IImage {
    private readonly Rgb24Image impl_;

    public I8Image(int width, int height) : this(
        new Bitmap(width, height, PixelFormat.Format24bppRgb)) { }

    internal I8Image(Bitmap bitmap) {
      this.impl_ = new Rgb24Image(bitmap);
    }

    ~I8Image() => this.ReleaseUnmanagedResources_();

    public void Dispose() {
      this.ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() => this.impl_.Dispose();

    public int Width => this.impl_.Width;
    public int Height => this.impl_.Height;

    public delegate void GetHandler(int x,
                                    int y,
                                    out byte intensity);

    public delegate void SetHandler(int x,
                                    int y,
                                    byte intensity);

    public delegate void MutateHandler(GetHandler getHandler,
                                       SetHandler setHandler);

    public unsafe void Mutate(MutateHandler mutateHandler) {
      this.impl_.Mutate((rgbGetHandler, rgbSetHandler) => {
        GetHandler iGetHandler = (int x, int y, out byte intensity) =>
            rgbGetHandler(x, y, out intensity, out _, out _);

        SetHandler iSetHandler = (x, y, intensity) =>
            rgbSetHandler(x, y, intensity, intensity, intensity);

        mutateHandler(iGetHandler, iSetHandler);
      });
    }

    public void ExportToStream(Stream stream, LocalImageFormat imageFormat)
      => this.impl_.ExportToStream(stream, imageFormat);
  }
}