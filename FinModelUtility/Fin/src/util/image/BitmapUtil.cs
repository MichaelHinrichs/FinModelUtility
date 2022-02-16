using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

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

    private static unsafe BitmapTransparencyType IsTransparentImpl_(BitmapData bmpData) {
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
        srcOffset += bmpData.Stride;
      }

      return hasTransparency
                 ? BitmapTransparencyType.MASK
                 : BitmapTransparencyType.OPAQUE;
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
  }
}