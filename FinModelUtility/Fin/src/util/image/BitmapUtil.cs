using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

using fin.model;
using fin.util.asserts;


namespace fin.util.image {
  public enum BitmapTransparencyType {
    OPAQUE,
    MASK,
    TRANSPARENT,
  }

  public static class BitmapUtil {
    // Based on answers at:
    // https://stackoverflow.com/questions/9291641/how-to-detect-if-a-bitmap-has-alpha-channel-in-net
    public static BitmapTransparencyType GetTransparencyType(Bitmap bmp)
      => BitmapUtil.InvokeAsLocked(bmp, BitmapUtil.IsTransparentImpl_);

    private static unsafe BitmapTransparencyType IsTransparentImpl_(
        BitmapData bmpData) {
      if ((bmpData.PixelFormat & PixelFormat.Alpha) == 0) {
        return BitmapTransparencyType.OPAQUE;
      }

      var hasTransparency = false;

      var src = (byte*) bmpData.Scan0.ToPointer();
      var srcOffset = 0;

      var height = bmpData.Height;
      var stride = bmpData.Stride;

      for (var y = 0; y < height; ++y) {
        for (var p = 3; p < stride; p += 4) {
          var alpha = src[srcOffset + p];
          hasTransparency |= alpha < 255;
          if (alpha > 0 && alpha < 255) {
            return BitmapTransparencyType.TRANSPARENT;
          }
        }
        srcOffset += stride;
      }

      return hasTransparency
                 ? BitmapTransparencyType.MASK
                 : BitmapTransparencyType.OPAQUE;
    }

    public static Bitmap Create1x1WithColor(Color color) {
      var bmp = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
      bmp.SetPixel(0, 0, color);
      return bmp;
    }

    public static T InvokeAsLocked<T>(Bitmap bmp, Func<BitmapData, T> handler) {
      var bmpBounds = new Rectangle(0, 0, bmp.Width, bmp.Height);
      var bmpData =
          bmp.LockBits(bmpBounds, ImageLockMode.ReadOnly, bmp.PixelFormat);

      var output = handler(bmpData);

      bmp.UnlockBits(bmpData);

      return output;
    }

    public static void InvokeAsLocked(Bitmap bmp, Action<BitmapData> handler) {
      var bmpBounds = new Rectangle(0, 0, bmp.Width, bmp.Height);
      var bmpData =
          bmp.LockBits(bmpBounds, ImageLockMode.ReadOnly, bmp.PixelFormat);

      handler(bmpData);

      bmp.UnlockBits(bmpData);
    }

    public static void InvokeAsLocked<T>(
        Bitmap bmp,
        T data,
        Action<BitmapData, T> handler) {
      var bmpBounds = new Rectangle(0, 0, bmp.Width, bmp.Height);
      var bmpData =
          bmp.LockBits(bmpBounds, ImageLockMode.ReadOnly, bmp.PixelFormat);

      handler(bmpData, data);

      bmp.UnlockBits(bmpData);
    }

    public static void InvokeAsLocked(Bitmap[] bmps,
                                      Action<BitmapData[]> handler) {
      var bmpDatas = bmps.Select(bmp => {
                           var bmpBounds = new Rectangle(0, 0, bmp.Width,
                             bmp.Height);
                           var bmpData =
                               bmp.LockBits(bmpBounds, ImageLockMode.ReadOnly,
                                            bmp.PixelFormat);

                           return bmpData;
                         })
                         .ToArray();

      handler(bmpDatas);

      for (var b = 0; b < bmps.Length; ++b) {
        var bmp = bmps[b];
        var bmpData = bmpDatas[b];
        bmp.UnlockBits(bmpData);
      }
    }

    public static unsafe void ProcessImage(Bitmap src,
                                           Bitmap dst,
                                           ConvertPixel converter) {
      Asserts.Equal(src.Width, dst.Width);
      Asserts.Equal(src.Height, dst.Height);

      BitmapUtil.InvokeAsLocked(new[] {src, dst}, bmpDatas => {
        var srcData = bmpDatas[0];
        var dstData = bmpDatas[1];

        var width = src.Width;
        var height = src.Height;

        var colorPalette = src.Palette.Entries;

        for (var y = 0; y < height; ++y) {
          for (var x = 0; x < width; ++x) {
            var i = y * width + x;
            BitmapUtil.GetPixel(src,
                                srcData,
                                colorPalette,
                                i, 
                                out var inR, 
                                out var inG,
                                out var inB,
                                out var inA);

            converter(inR, inG, inB, inA, out var outR, out var outG,
                      out var outB, out var outA);

            BitmapUtil.SetPixel(dstData, i, outR, outG, outB, outA);
          }
        }
      });
    }

    public delegate void ConvertPixel(byte inR,
                                      byte inG,
                                      byte inB,
                                      byte inA,
                                      out byte outR,
                                      out byte outG,
                                      out byte outB,
                                      out byte outA);

    public static unsafe void GetPixel(
        Bitmap bmp,
        BitmapData bmpData,
        Color[] colorPalette,
        int index,
        out byte r,
        out byte g,
        out byte b,
        out byte a) {
      var ptr = (byte*) bmpData.Scan0;
      switch (bmpData.PixelFormat) {
        case PixelFormat.Format24bppRgb: {
          var i = 3 * index;
          b = ptr[i];
          g = ptr[i + 1];
          r = ptr[i + 2];
          a = 255;
          break;
        }
        case PixelFormat.Format32bppArgb: {
          var i = 4 * index;
          b = ptr[i];
          g = ptr[i + 1];
          r = ptr[i + 2];
          a = ptr[i + 3];
          break;
        }
        case PixelFormat.Format4bppIndexed: {
          var fullI = index;
          var isUpper = fullI % 2 == 1;
          var fullColorIndex = ptr[fullI / 2];

          var colorIndex =
              isUpper ? fullColorIndex & 0xF : fullColorIndex >> 4;
          var color = colorPalette[colorIndex];

          r = color.R;
          g = color.G;
          b = color.B;
          a = color.A;
          break;
        }
        case PixelFormat.Format8bppIndexed: {
          var i = index;

          var colorIndex = ptr[i];
          var color = colorPalette[colorIndex];

          r = color.R;
          g = color.G;
          b = color.B;
          a = color.A;
          break;
        }
        default: throw new ArgumentOutOfRangeException();
      }
    }

    public static unsafe void SetPixel(BitmapData bmpData,
                                       int index,
                                       byte r,
                                       byte g,
                                       byte b,
                                       byte a) {
      var ptr = (byte*) bmpData.Scan0;
      switch (bmpData.PixelFormat) {
        case PixelFormat.Format24bppRgb: {
          var i = 3 * index;
          ptr[i] = b;
          ptr[i + 1] = g;
          ptr[i + 2] = r;
          break;
        }
        case PixelFormat.Format32bppArgb: {
          var i = 4 * index;
          ptr[i] = b;
          ptr[i + 1] = g;
          ptr[i + 2] = r;
          ptr[i + 3] = a;
          break;
        }
        default: throw new ArgumentOutOfRangeException();
      }
    }
  }
}