using fin.io;
using fin.util.asserts;
using fin.util.image;

using System;
using System.Drawing;
using System.IO;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.PixelFormats;

using System.Drawing.Imaging;

using Color = System.Drawing.Color;
using Image = SixLabors.ImageSharp.Image;


namespace fin.image {
  public static class FinImage {
    public static IImage FromFile(IFile file) {
      using var stream = file.OpenRead();
      return FinImage.FromStream(stream);
    }

    public static readonly Configuration ImageSharpConfig;

    static FinImage() {
      ImageSharpConfig = Configuration.Default.Clone();
      ImageSharpConfig.PreferContiguousImageBuffers = true;
    }

    public static IImage FromStream(Stream stream) {
      var image = Image.Load(ImageSharpConfig, stream);

      var firstFrame = image.Frames[0];
      var pixelFormat = image.GetType().GenericTypeArguments[0];
      if (pixelFormat == typeof(Rgba32)) {
        return new Rgba32Image(Asserts.CastNonnull(image as Image<Rgba32>));
      } else if (pixelFormat == typeof(Rgb24)) {
        return new Rgb24Image(Asserts.CastNonnull(image as Image<Rgb24>));
      } else if (pixelFormat == typeof(L8)) {
        return new I8Image(Asserts.CastNonnull(image as Image<L8>));
      } else if (pixelFormat == typeof(La16)) {
        return new Ia16Image(Asserts.CastNonnull(image as Image<La16>));
      }

      throw new ArgumentOutOfRangeException(
          nameof(pixelFormat),
          pixelFormat,
          null);
    }

    public static unsafe IImage FromBitmap(Bitmap bitmap) {
      var width = bitmap.Width;
      var height = bitmap.Height;

      var image = new Rgba32Image(width, height);
      BitmapUtil.InvokeAsLocked(bitmap,
                                bmpData => {
                                  var ptr = (byte*) bmpData.Scan0;

                                  image.Mutate((_, setHandler) => {
                                    for (var y = 0; y < height; ++y) {
                                      for (var x = 0; x < width; ++x) {
                                        var index = 4 * (y * width + x);
                                        var b = ptr[index];
                                        var g = ptr[index + 1];
                                        var r = ptr[index + 2];
                                        var a = ptr[index + 3];

                                        setHandler(x, y, r, g, b, a);
                                      }
                                    }
                                  });
                                });
      return image;
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

    public static IImageEncoder ConvertFinImageFormatToImageSharpEncoder(
        LocalImageFormat imageFormat)
      => imageFormat switch {
          LocalImageFormat.BMP  => new BmpEncoder(),
          LocalImageFormat.PNG  => new PngEncoder(),
          LocalImageFormat.JPEG => new JpegEncoder(),
          LocalImageFormat.GIF  => new GifEncoder(),
          LocalImageFormat.TGA  => new TgaEncoder(),
          _ => throw new ArgumentOutOfRangeException(
              nameof(imageFormat),
              imageFormat,
              null)
      };

    public static unsafe Bitmap ConvertToBitmap(IImage image) {
      var width = image.Width;
      var height = image.Height;

      var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
      BitmapUtil.InvokeAsLocked(bitmap,
                                bmpData => {
                                  var ptr = (byte*) bmpData.Scan0;

                                  image.Access(getHandler => {
                                    for (var y = 0; y < height; ++y) {
                                      for (var x = 0; x < width; ++x) {
                                        getHandler(
                                            x,
                                            y,
                                            out var r,
                                            out var g,
                                            out var b,
                                            out var a);

                                        var index = 4 * (y * width + x);
                                        ptr[index] = b;
                                        ptr[index + 1] = g;
                                        ptr[index + 2] = r;
                                        ptr[index + 3] = a;
                                      }
                                    }
                                  });
                                });
      return bitmap;
    }

    public delegate void GetHandler<TPixel>(int x,
                                            int y,
                                            out TPixel pixel)
        where TPixel : unmanaged, IPixel<TPixel>;

    public delegate void AccessHandler<TPixel>(GetHandler<TPixel> getHandler)
        where TPixel : unmanaged, IPixel<TPixel>;

    public static unsafe void Access<TPixel>(Image<TPixel> image,
                                             AccessHandler<TPixel>
                                                 accessHandler)
        where TPixel : unmanaged, IPixel<TPixel> {
      var frame = Asserts.CastNonnull(image.Frames[0]);
      Asserts.True(frame.DangerousTryGetSinglePixelMemory(out var memory));

      using var memoryHandle = memory.Pin();

      var ptr = (TPixel*) memoryHandle.Pointer;

      void GetHandler(int x, int y, out TPixel pixel)
        => pixel = ptr[y * frame.Width + x];

      accessHandler(GetHandler);
    }


    public delegate void SetHandler<TPixel>(int x,
                                            int y,
                                            TPixel pixel)
        where TPixel : unmanaged, IPixel<TPixel>;

    public delegate void MutateHandler<TPixel>(GetHandler<TPixel> getHandler,
                                               SetHandler<TPixel> setHandler)
        where TPixel : unmanaged, IPixel<TPixel>;

    public static unsafe void Mutate<TPixel>(Image<TPixel> image,
                                             MutateHandler<TPixel>
                                                 mutateHandler)
        where TPixel : unmanaged, IPixel<TPixel> {
      var frame = Asserts.CastNonnull(image.Frames[0]);
      Asserts.True(frame.DangerousTryGetSinglePixelMemory(out var memory));

      using var memoryHandle = memory.Pin();

      var ptr = (TPixel*) memoryHandle.Pointer;

      void GetHandler(int x, int y, out TPixel pixel)
        => pixel = ptr[y * frame.Width + x];

      void SetHandler(int x, int y, TPixel pixel)
        => ptr[y * frame.Width + x] = pixel;

      mutateHandler(GetHandler, SetHandler);
    }
  }

  public abstract class BImage<TPixel> : fin.image.IImage<TPixel>
      where TPixel : unmanaged, IPixel<TPixel> {
    ~BImage() => this.ReleaseUnmanagedResources_();

    public void Dispose() {
      this.ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() => this.Impl.Dispose();

    protected abstract Image<TPixel> Impl { get; }

    public int Width => this.Impl.Width;
    public int Height => this.Impl.Height;

    public abstract void Access(IImage.AccessHandler accessHandler);
    public abstract bool HasAlphaChannel { get; }

    public Bitmap AsBitmap() => FinImage.ConvertToBitmap(this);

    public void ExportToStream(Stream stream, LocalImageFormat imageFormat)
      => this.Impl.Save(
          stream,
          FinImage.ConvertFinImageFormatToImageSharpEncoder(imageFormat));

    public FinImageLock<TPixel> Lock() => new(Impl);
  }

  public class Rgba32Image : BImage<Rgba32> {
    public Rgba32Image(int width, int height) : this(
        new Image<Rgba32>(FinImage.ImageSharpConfig, width, height)) { }

    internal Rgba32Image(Image<Rgba32> impl) {
      this.Impl = impl;
    }

    protected override Image<Rgba32> Impl { get; }

    public override void Access(IImage.AccessHandler accessHandler)
      => FinImage.Access(this.Impl,
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
      => FinImage.Mutate(this.Impl,
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

  public class Rgb24Image : IImage {
    private readonly Image<Rgb24> impl_;

    public Rgb24Image(int width, int height) : this(
        new Image<Rgb24>(FinImage.ImageSharpConfig, width, height)) { }

    internal Rgb24Image(Image<Rgb24> impl) {
      this.impl_ = impl;
    }

    ~Rgb24Image() => this.ReleaseUnmanagedResources_();

    public void Dispose() {
      this.ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() => this.impl_.Dispose();

    public int Width => this.impl_.Width;
    public int Height => this.impl_.Height;

    public void Access(IImage.AccessHandler accessHandler) {
      var frame = this.impl_.Frames[0];

      void GetHandler(
          int x,
          int y,
          out byte r,
          out byte g,
          out byte b,
          out byte a) {
        var pixel = frame[x, y];
        r = pixel.R;
        g = pixel.G;
        b = pixel.B;
        a = 255;
      }

      accessHandler(GetHandler);
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

    public void Mutate(MutateHandler mutateHandler) {
      var frame = this.impl_.Frames[0];

      void GetHandler(
          int x,
          int y,
          out byte r,
          out byte g,
          out byte b) {
        var pixel = frame[x, y];
        r = pixel.R;
        g = pixel.G;
        b = pixel.B;
      }

      void SetHandler(int x, int y, byte r, byte g, byte b) {
        var pixel = frame[x, y];
        pixel.R = r;
        pixel.G = g;
        pixel.B = b;
        frame[x, y] = pixel;
      }

      mutateHandler(GetHandler, SetHandler);
    }

    public bool HasAlphaChannel => false;
    public Bitmap AsBitmap() => FinImage.ConvertToBitmap(this);

    public void GetRgb24Bytes(Span<byte> bytes)
      => this.impl_.CopyPixelDataTo(bytes);

    public void ExportToStream(Stream stream, LocalImageFormat imageFormat)
      => this.impl_.Save(
          stream,
          FinImage.ConvertFinImageFormatToImageSharpEncoder(imageFormat));
  }

  public class Ia16Image : IImage {
    private readonly Image<La16> impl_;

    public Ia16Image(int width, int height) : this(
        new Image<La16>(FinImage.ImageSharpConfig, width, height)) { }

    internal Ia16Image(Image<La16> impl) {
      this.impl_ = impl;
    }

    ~Ia16Image() => this.ReleaseUnmanagedResources_();

    public void Dispose() {
      this.ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() => this.impl_.Dispose();

    public int Width => this.impl_.Width;
    public int Height => this.impl_.Height;

    public void Access(IImage.AccessHandler accessHandler) {
      var frame = this.impl_.Frames[0];

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
      var frame = this.impl_.Frames[0];

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

    public bool HasAlphaChannel => true;
    public Bitmap AsBitmap() => FinImage.ConvertToBitmap(this);

    public void GetIa16Bytes(Span<byte> bytes)
      => this.impl_.CopyPixelDataTo(bytes);

    public void ExportToStream(Stream stream, LocalImageFormat imageFormat)
      => this.impl_.Save(
          stream,
          FinImage.ConvertFinImageFormatToImageSharpEncoder(imageFormat));
  }

  public class I8Image : IImage {
    private readonly Image<L8> impl_;

    public I8Image(int width, int height) :
        this(new Image<L8>(FinImage.ImageSharpConfig, width, height)) { }

    internal I8Image(Image<L8> impl) {
      this.impl_ = impl;
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
      => FinImage.Access(this.impl_,
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
      => FinImage.Mutate(this.impl_,
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

    public bool HasAlphaChannel => false;
    public Bitmap AsBitmap() => FinImage.ConvertToBitmap(this);

    public void GetI8Bytes(Span<byte> bytes)
      => this.impl_.CopyPixelDataTo(bytes);

    public void ExportToStream(Stream stream, LocalImageFormat imageFormat)
      => this.impl_.Save(
          stream,
          FinImage.ConvertFinImageFormatToImageSharpEncoder(imageFormat));
  }
}