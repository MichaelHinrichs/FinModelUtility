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
        case PixelFormat.Format4bppIndexed:
          return new Indexed4Image(image);
        case PixelFormat.Format8bppIndexed:
          return new Indexed8Image(image);
        default:
          throw new ArgumentOutOfRangeException(
              nameof(pixelFormat), pixelFormat, null);
      }
    }

    public static IImage Create1x1FromColor(Color color)
      => CreateFromColor(color, 1, 1);

    public static IImage CreateFromColor(Color color, int width, int height) {
      var bmp = new Rgba32Image(width, height);
      bmp.Mutate((_, setHandler) => {
        for (var y = 0; y < height; ++y) {
          for (var x = 0; x < width; ++x) {
            setHandler(x, y, color.R, color.G, color.B, color.A);
          }
        }
      });
      return bmp;
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
      this.impl_ = bitmap;
    }

    ~Rgba32Image() => this.ReleaseUnmanagedResources_();

    public void Dispose() {
      this.ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() => this.impl_.Dispose();

    public int Width => this.impl_.Width;
    public int Height => this.impl_.Height;

    public unsafe void Access(IImage.AccessHandler accessHandler) {
      BitmapUtil.InvokeAsLocked(this.impl_, bmpData => {
        var ptr = (byte*) bmpData.Scan0;

        void GetHandler(int x,
                        int y,
                        out byte r,
                        out byte g,
                        out byte b,
                        out byte a) {
          var index = 4 * (y * bmpData.Width + x);
          b = ptr[index];
          g = ptr[index + 1];
          r = ptr[index + 2];
          a = ptr[index + 3];
        }

        accessHandler(GetHandler);
      });
    }

    public delegate void SetHandler(int x,
                                    int y,
                                    byte r,
                                    byte g,
                                    byte b,
                                    byte a);

    public delegate void MutateHandler(IImage.Rgba32GetHandler getHandler,
                                       SetHandler setHandler);

    public unsafe void Mutate(MutateHandler mutateHandler) {
      BitmapUtil.InvokeAsLocked(this.impl_, bmpData => {
        var ptr = (byte*) bmpData.Scan0;

        void GetHandler(int x,
                        int y,
                        out byte r,
                        out byte g,
                        out byte b,
                        out byte a) {
          var index = 4 * (y * bmpData.Width + x);
          b = ptr[index];
          g = ptr[index + 1];
          r = ptr[index + 2];
          a = ptr[index + 3];
        }

        void SetHandler(int x, int y, byte r, byte g, byte b, byte a) {
          var index = 4 * (y * bmpData.Width + x);
          ptr[index] = b;
          ptr[index + 1] = g;
          ptr[index + 2] = r;
          ptr[index + 3] = a;
        }

        mutateHandler(GetHandler, SetHandler);
      });
    }

    public Bitmap AsBitmap() => this.impl_;

    public void ExportToStream(Stream stream, LocalImageFormat imageFormat)
      => this.impl_.Save(
          stream, FinImage.ConvertFinImageFormatToSystem(imageFormat));
  }

  public class Rgb24Image : IImage {
    private readonly Bitmap impl_;

    public Rgb24Image(int width, int height) : this(
        new Bitmap(width, height, PixelFormat.Format24bppRgb)) { }

    internal Rgb24Image(Bitmap bitmap) {
      this.impl_ = bitmap;
    }

    ~Rgb24Image() => this.ReleaseUnmanagedResources_();

    public void Dispose() {
      this.ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() => this.impl_.Dispose();

    public int Width => this.impl_.Width;
    public int Height => this.impl_.Height;

    public unsafe void Access(IImage.AccessHandler accessHandler) {
      BitmapUtil.InvokeAsLocked(this.impl_, bmpData => {
        var ptr = (byte*) bmpData.Scan0;

        void GetHandler(int x,
                        int y,
                        out byte r,
                        out byte g,
                        out byte b,
                        out byte a) {
          var index = 3 * (y * bmpData.Width + x);
          b = ptr[index];
          g = ptr[index + 1];
          r = ptr[index + 2];
          a = 255;
        }

        accessHandler(GetHandler);
      });
    }

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

        void GetHandler(int x, int y, out byte r, out byte g, out byte b) {
          var index = 3 * (y * bmpData.Width + x);
          b = ptr[index];
          g = ptr[index + 1];
          r = ptr[index + 2];
        }

        void SetHandler(int x, int y, byte r, byte g, byte b) {
          var index = 3 * (y * bmpData.Width + x);
          ptr[index] = b;
          ptr[index + 1] = g;
          ptr[index + 2] = r;
        }

        mutateHandler(GetHandler, SetHandler);
      });
    }

    public Bitmap AsBitmap() => this.impl_;

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

    public void Access(IImage.AccessHandler accessHandler)
      => this.impl_.Access(accessHandler);

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
      this.impl_.Mutate((rgbaGetHandler, rgbaSetHandler) => {
        void IaGetHandler(int x, int y, out byte intensity, out byte alpha)
          => rgbaGetHandler(x, y, out intensity, out _, out _, out alpha);

        void IaSetHandler(int x, int y, byte intensity, byte alpha)
          => rgbaSetHandler(x, y, intensity, intensity, intensity, alpha);

        mutateHandler(IaGetHandler, IaSetHandler);
      });
    }

    public Bitmap AsBitmap() => this.impl_.AsBitmap();

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

    public void Access(IImage.AccessHandler accessHandler)
      => this.impl_.Access(accessHandler);

    public delegate void GetHandler(int x,
                                    int y,
                                    out byte intensity);

    public delegate void SetHandler(int x,
                                    int y,
                                    byte intensity);

    public delegate void MutateHandler(GetHandler getHandler,
                                       SetHandler setHandler);

    public void Mutate(MutateHandler mutateHandler) {
      this.impl_.Mutate((rgbGetHandler, rgbSetHandler) => {
        void IGetHandler(int x, int y, out byte intensity)
          => rgbGetHandler(x, y, out intensity, out _, out _);

        void ISetHandler(int x, int y, byte intensity)
          => rgbSetHandler(x, y, intensity, intensity, intensity);

        mutateHandler(IGetHandler, ISetHandler);
      });
    }

    public Bitmap AsBitmap() => this.impl_.AsBitmap();

    public void ExportToStream(Stream stream, LocalImageFormat imageFormat)
      => this.impl_.ExportToStream(stream, imageFormat);
  }

  public class Indexed4Image : IImage {
    private readonly Bitmap impl_;

    public Indexed4Image(int width, int height) : this(
        new Bitmap(width, height, PixelFormat.Format4bppIndexed)) { }

    internal Indexed4Image(Bitmap bitmap) {
      this.impl_ = bitmap;
    }

    ~Indexed4Image() => this.ReleaseUnmanagedResources_();

    public void Dispose() {
      this.ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() => this.impl_.Dispose();

    public int Width => this.impl_.Width;
    public int Height => this.impl_.Height;

    public unsafe void Access(IImage.AccessHandler accessHandler) {
      var palette = this.impl_.Palette.Entries;

      BitmapUtil.InvokeAsLocked(this.impl_, bmpData => {
        var ptr = (byte*) bmpData.Scan0;

        void GetHandler(int x,
                        int y,
                        out byte r,
                        out byte g,
                        out byte b,
                        out byte a) {
          var index = y * bmpData.Width + x;

          var fullI = index;
          var isUpper = fullI % 2 == 1;
          var fullColorIndex = ptr[fullI / 2];

          var colorIndex =
              isUpper ? fullColorIndex & 0xF : fullColorIndex >> 4;
          var color = palette[colorIndex];

          r = color.R;
          g = color.G;
          b = color.B;
          a = color.A;
        }

        accessHandler(GetHandler);
      });
    }

    public Bitmap AsBitmap() => this.impl_;

    public void ExportToStream(Stream stream, LocalImageFormat imageFormat)
      => this.impl_.Save(
          stream, FinImage.ConvertFinImageFormatToSystem(imageFormat));
  }

  public class Indexed8Image : IImage {
    private readonly Bitmap impl_;

    public Indexed8Image(int width, int height) : this(
        new Bitmap(width, height, PixelFormat.Format4bppIndexed)) { }

    internal Indexed8Image(Bitmap bitmap) {
      this.impl_ = bitmap;
    }

    ~Indexed8Image() => this.ReleaseUnmanagedResources_();

    public void Dispose() {
      this.ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() => this.impl_.Dispose();

    public int Width => this.impl_.Width;
    public int Height => this.impl_.Height;

    public unsafe void Access(IImage.AccessHandler accessHandler) {
      var palette = this.impl_.Palette.Entries;

      BitmapUtil.InvokeAsLocked(this.impl_, bmpData => {
        var ptr = (byte*) bmpData.Scan0;

        void GetHandler(int x,
                        int y,
                        out byte r,
                        out byte g,
                        out byte b,
                        out byte a) {
          var index = y * bmpData.Width + x;
          var colorIndex = ptr[index];
          var color = palette[colorIndex];

          r = color.R;
          g = color.G;
          b = color.B;
          a = color.A;
        }

        accessHandler(GetHandler);
      });
    }

    public Bitmap AsBitmap() => this.impl_;

    public void ExportToStream(Stream stream, LocalImageFormat imageFormat)
      => this.impl_.Save(
          stream, FinImage.ConvertFinImageFormatToSystem(imageFormat));
  }
}