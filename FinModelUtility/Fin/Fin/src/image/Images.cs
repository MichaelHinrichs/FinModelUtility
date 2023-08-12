﻿using fin.io;
using fin.util.asserts;

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

using System.Threading.Tasks;

using FastBitmapLib;

using fin.color;
using fin.image.formats;

using Color = System.Drawing.Color;
using Image = SixLabors.ImageSharp.Image;


namespace fin.image {
  public static class FinImage {
    public static IImage FromFile(IReadOnlyGenericFile file) {
      using var stream = file.OpenRead();
      return FinImage.FromStream(stream);
    }

    public static async Task<IImage> FromFileAsync(IReadOnlyGenericFile file) {
      await using var stream = file.OpenRead();
      return await FinImage.FromStreamAsync(stream);
    }


    public static readonly Configuration ImageSharpConfig;

    static FinImage() {
      ImageSharpConfig = Configuration.Default.Clone();
      ImageSharpConfig.PreferContiguousImageBuffers = true;
    }

    public static IImage FromStream(Stream stream) {
      var imageTask = FromStreamAsync(stream);
      imageTask.Wait();
      return imageTask.Result;
    }

    public static async Task<IImage> FromStreamAsync(Stream stream) {
      var image = Image.Load(ImageSharpConfig, stream);

      var pixelFormat = image.GetType().GenericTypeArguments[0];
      if (pixelFormat == typeof(Rgba32)) {
        return new Rgba32Image(PixelFormat.RGBA8888,
                               Asserts.CastNonnull(image as Image<Rgba32>));
      } else if (pixelFormat == typeof(Rgb24)) {
        return new Rgb24Image(PixelFormat.RGB888,
                              Asserts.CastNonnull(image as Image<Rgb24>));
      } else if (pixelFormat == typeof(L8)) {
        return new L8Image(PixelFormat.L8,
                           Asserts.CastNonnull(image as Image<L8>));
      } else if (pixelFormat == typeof(La16)) {
        return new La16Image(PixelFormat.LA88,
                             Asserts.CastNonnull(image as Image<La16>));
      }

      throw new ArgumentOutOfRangeException(
          nameof(pixelFormat),
          pixelFormat,
          null);
    }

    public static IImage Create1x1FromColor(Color color)
      => CreateFromColor(color, 1, 1);

    public static unsafe IImage CreateFromColor(
        Color color,
        int width,
        int height) {
      var bmp = new Rgba32Image(PixelFormat.RGBA8888, width, height);

      using var imageLock = bmp.Lock();
      new Span<Rgba32>(imageLock.pixelScan0, width * height).Fill(
          new Rgba32(color.R, color.G, color.B, color.A));

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

      var bitmap = new Bitmap(width,
                              height,
                              System.Drawing.Imaging.PixelFormat
                                    .Format32bppArgb);
      using var fastBitmap = bitmap.FastLock();
      var dstPtr = (int*) fastBitmap.Scan0;

      switch (image) {
        case Rgba32Image rgba32Image: {
          using var imageLock = rgba32Image.Lock();
          var srcPtr = imageLock.pixelScan0;
          for (var y = 0; y < height; ++y) {
            for (var x = 0; x < width; ++x) {
              var index = y * width + x;
              var rgba = srcPtr[index];
              dstPtr[index] =
                  FinColor.MergeBgra(rgba.R, rgba.G, rgba.B, rgba.A);
            }
          }

          break;
        }
        case Rgb24Image rgb24Image: {
          using var imageLock = rgb24Image.Lock();
          var srcPtr = imageLock.pixelScan0;
          for (var y = 0; y < height; ++y) {
            for (var x = 0; x < width; ++x) {
              var index = y * width + x;
              var rgb = srcPtr[index];
              dstPtr[index] = FinColor.MergeBgra(rgb.R, rgb.G, rgb.B, 255);
            }
          }

          break;
        }
        case L8Image i8Image: {
          using var imageLock = i8Image.Lock();
          var srcPtr = imageLock.pixelScan0;
          for (var y = 0; y < height; ++y) {
            for (var x = 0; x < width; ++x) {
              var index = y * width + x;
              var i = srcPtr[index].PackedValue;
              dstPtr[index] = FinColor.MergeBgra(i, i, i, 255);
            }
          }

          break;
        }
        default: {
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

                var index = y * width + x;
                dstPtr[index] = FinColor.MergeBgra(r, g, b, a);
              }
            }
          });
          break;
        }
      }

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
}