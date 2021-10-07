using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace fin.util.image {
  public static class BitmapUtil {
    // Based on answers at:
    // https://stackoverflow.com/questions/9291641/how-to-detect-if-a-bitmap-has-alpha-channel-in-net
    public static bool IsTransparent(Bitmap bmp) {
      var bmpBounds = new Rectangle(0, 0, bmp.Width, bmp.Height);
      var bmpData = bmp.LockBits(bmpBounds, ImageLockMode.ReadOnly, bmp.PixelFormat);

      var isTransparent = false;
      byte[] bytes = new byte[bmpData.Height * bmpData.Stride];
      Marshal.Copy(bmpData.Scan0, bytes, 0, bytes.Length);
      for (var p = 3; p < bytes.Length; p += 4) {
        if (bytes[p] != 255) {
          isTransparent = true;
          break;
        }
      }

      bmp.UnlockBits(bmpData);

      return isTransparent;
    }
  }
}
