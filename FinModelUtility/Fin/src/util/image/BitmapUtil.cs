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

    private static BitmapTransparencyType IsTransparentImpl_(BitmapData bmpData) {
      var hasTransparency = false;
      byte[] bytes = new byte[bmpData.Height * bmpData.Stride];
      Marshal.Copy(bmpData.Scan0, bytes, 0, bytes.Length);
      for (var p = 3; p < bytes.Length; p += 4) {
        var alpha = bytes[p];
        hasTransparency |= alpha < 255;
        if (alpha > 0 && alpha < 255) {
          return BitmapTransparencyType.TRANSPARENT;
        }
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