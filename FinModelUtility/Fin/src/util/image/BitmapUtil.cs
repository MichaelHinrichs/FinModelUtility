using System;
using System.Drawing;
using System.Drawing.Imaging;


namespace fin.util.image {
  public static class BitmapUtil {
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
  }
}