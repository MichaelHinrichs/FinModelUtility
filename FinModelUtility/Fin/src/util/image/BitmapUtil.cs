using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace fin.util.image {
  public static class BitmapUtil {
    // Based on answers at:
    // https://stackoverflow.com/questions/9291641/how-to-detect-if-a-bitmap-has-alpha-channel-in-net
    public static bool IsTransparent(Bitmap bmp)
      => BitmapUtil.InvokeAsLocked(bmp, BitmapUtil.IsTransparentImpl_);

    private static bool IsTransparentImpl_(BitmapData bmpData) {
      var isTransparent = false;
      byte[] bytes = new byte[bmpData.Height * bmpData.Stride];
      Marshal.Copy(bmpData.Scan0, bytes, 0, bytes.Length);
      for (var p = 3; p < bytes.Length; p += 4) {
        if (bytes[p] != 255) {
          return true;
        }
      }

      return false;
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

    public static void InvokeAsLocked<T>(Bitmap bmp, T data, Action<BitmapData, T> handler) {
      var bmpBounds = new Rectangle(0, 0, bmp.Width, bmp.Height);
      var bmpData =
          bmp.LockBits(bmpBounds, ImageLockMode.ReadOnly, bmp.PixelFormat);

      handler(bmpData, data);

      bmp.UnlockBits(bmpData);
    }
  }
}