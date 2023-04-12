using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using fin.image;
using fin.image.io;


namespace Quad64.src.Scripts {
  class TextureFormats {
    public static IImage decodeTexture(byte format,
                                       byte[] data,
                                       int width,
                                       int height,
                                       ushort[] palette,
                                       bool isPaletteRGBA16) {
      switch (format) {
        case 0x10:
          return PixelImageReader.New(width,
                                      height,
                                      new Argb1555PixelReader(),
                                      Endianness.BigEndian)
                                 .Read(data);
        case 0x68:
          return PixelImageReader.New(width,
                                      height,
                                      new La8PixelReader(),
                                      Endianness.BigEndian)
                                 .Read(data);
        case 0x70:
          return PixelImageReader.New(width,
                                      height,
                                      new La16PixelReader(),
                                      Endianness.BigEndian)
                                 .Read(data);
      }

      switch (format) {
        default:
        case 0x00: // Note: "1 bit per pixel" is not a Fast3D format.
          return decode1BPP(data, width, height).ToImage();
        case 0x18:
          return decodeRGBA32(data, width, height).ToImage();
        case 0x40:
          return decodeCI4(data, width, height, palette, isPaletteRGBA16).ToImage();
        case 0x48:
          return decodeCI8(data, width, height, palette, isPaletteRGBA16).ToImage();
        case 0x60:
          return decodeIA4(data, width, height).ToImage();
        case 0x80:
        case 0x90:
          return decodeI4(data, width, height).ToImage();
        case 0x88:
          return decodeI8(data, width, height).ToImage();
      }
    }


    public static Bitmap decode1BPP(byte[] data, int width, int height) {
      Bitmap tex = new Bitmap(width, height);
      if (data.Length >= (width * height) / 8) // Sanity Check
      {
        int len = (width * height) / 8;
        for (int i = 0; i < len; ++i) {
          for (int x = 0; x < 8; x++) {
            byte intensity = (byte) ((data[i] >> (7 - x)) & 1);
            if (intensity > 0)
              intensity = 0xFF;
            int alpha = intensity;
            int pos = (i * 8) + x;
            tex.SetPixel(pos % width, pos / width,
                         Color.FromArgb(alpha, intensity, intensity,
                                        intensity));
          }
        }
      }
      tex.Tag = new string[] {
          "Format: 1BPP", "Width: " + width,
          "Height: " + height
      };
      return tex;
    }

    public static Bitmap decodeRGBA32(byte[] data, int width, int height) {
      Console.WriteLine("Texture size = (" + width + "x" + height + ")");
      Console.WriteLine("data.Length = (" + data.Length + ")");
      Bitmap tex = new Bitmap(width, height);

      if (data.Length >= width * height * 4) // Sanity Check
      {
        BitmapData bitmapData = tex.LockBits(new Rectangle(0, 0, width, height),
                                             ImageLockMode.ReadWrite,
                                             tex.PixelFormat);

        int len = width * height;
        for (int i = 0; i < len; i++) {
          // Swap red and blue values
          byte temp_red = data[(i * 4) + 0];
          data[(i * 4) + 0] = data[(i * 4) + 2];
          data[(i * 4) + 2] = temp_red;
        }
        Marshal.Copy(data, 0, bitmapData.Scan0, data.Length);
        tex.UnlockBits(bitmapData);
      }
      tex.Tag = new string[] {
          "Format: RGBA32", "Width: " + width,
          "Height: " + height
      };
      return tex;
    }

    public static Bitmap decodeIA16(byte[] data, int width, int height) {
      Bitmap tex = new Bitmap(width, height);
      if (data.Length >= width * height * 2) // Sanity Check
      {
        BitmapData bitmapData = tex.LockBits(new Rectangle(0, 0, width, height),
                                             ImageLockMode.ReadWrite,
                                             tex.PixelFormat);
        byte[] pixels = new byte[width * height * 4];

        int len = width * height;
        for (int i = 0; i < len; i++) {
          pixels[(i * 4) + 2] = data[i * 2]; // Red
          pixels[(i * 4) + 1] = data[i * 2]; // Green
          pixels[(i * 4) + 0] = data[i * 2]; // Blue
          pixels[(i * 4) + 3] = data[(i * 2) + 1]; // Alpha
        }
        Marshal.Copy(pixels, 0, bitmapData.Scan0, pixels.Length);
        tex.UnlockBits(bitmapData);
      }
      tex.Tag = new string[] {
          "Format: IA16", "Width: " + width,
          "Height: " + height
      };
      return tex;
    }

    public static Bitmap decodeIA8(byte[] data, int width, int height) {
      Bitmap tex = new Bitmap(width, height);
      if (data.Length >= width * height) // Sanity Check
      {
        BitmapData bitmapData = tex.LockBits(new Rectangle(0, 0, width, height),
                                             ImageLockMode.ReadWrite,
                                             tex.PixelFormat);
        byte[] pixels = new byte[width * height * 4];

        int len = width * height;
        for (int i = 0; i < len; i++) {
          byte intensity = (byte) (((data[i] >> 4) & 0xF) * 16);
          pixels[(i * 4) + 2] = intensity; // Red
          pixels[(i * 4) + 1] = intensity; // Green
          pixels[(i * 4) + 0] = intensity; // Blue
          pixels[(i * 4) + 3] = (byte) ((data[i] & 0xF) * 16); // Alpha
        }
        Marshal.Copy(pixels, 0, bitmapData.Scan0, pixels.Length);
        tex.UnlockBits(bitmapData);
      }
      tex.Tag = new string[] {
          "Format: IA8", "Width: " + width,
          "Height: " + height
      };
      return tex;
    }

    public static Bitmap decodeIA4(byte[] data, int width, int height) {
      Bitmap tex = new Bitmap(width, height);

      if (data.Length >= (width * height) / 2) // Sanity Check
      {
        BitmapData bitmapData = tex.LockBits(new Rectangle(0, 0, width, height),
                                             ImageLockMode.ReadWrite,
                                             tex.PixelFormat);
        byte[] pixels = new byte[width * height * 4];

        int len = (width * height) / 2;
        for (int i = 0; i < len; i++) {
          byte twoPixels = data[i];

          byte intensity = (byte) ((twoPixels >> 5) * 32);
          pixels[(i * 8) + 2] = intensity; // Red
          pixels[(i * 8) + 1] = intensity; // Green
          pixels[(i * 8) + 0] = intensity; // Blue
          pixels[(i * 8) + 3] =
              (byte) (((twoPixels >> 4) & 0x1) * 255); // Alpha

          intensity = (byte) (((twoPixels >> 1) & 0x7) * 32);
          pixels[(i * 8) + 6] = intensity; // Red
          pixels[(i * 8) + 5] = intensity; // Green
          pixels[(i * 8) + 4] = intensity; // Blue
          pixels[(i * 8) + 7] = (byte) ((twoPixels & 0x1) * 255); // Alpha
        }
        Marshal.Copy(pixels, 0, bitmapData.Scan0, pixels.Length);
        tex.UnlockBits(bitmapData);
        tex.Tag = new string[] {
            "Format: IA4", "Width: " + width,
            "Height: " + height
        };
      }
      return tex;
    }

    public static Bitmap decodeI8(byte[] data, int width, int height) {
      Bitmap tex = new Bitmap(width, height);

      if (data.Length >= width * height) // Sanity Check
      {
        BitmapData bitmapData = tex.LockBits(new Rectangle(0, 0, width, height),
                                             ImageLockMode.ReadWrite,
                                             tex.PixelFormat);
        byte[] pixels = new byte[width * height * 4];

        int len = width * height;
        for (int i = 0; i < len; i++) {
          byte intensity = data[i];
          pixels[(i * 4) + 2] = intensity; // Red
          pixels[(i * 4) + 1] = intensity; // Green
          pixels[(i * 4) + 0] = intensity; // Blue
          pixels[(i * 4) + 3] = 0xFF; // Alpha
        }
        Marshal.Copy(pixels, 0, bitmapData.Scan0, pixels.Length);
        tex.UnlockBits(bitmapData);

        tex.Tag = new string[] {
            "Format: I8", "Width: " + width,
            "Height: " + height
        };
      }
      return tex;
    }

    public static Bitmap decodeI4(byte[] data, int width, int height) {
      Bitmap tex = new Bitmap(width, height);

      if (data.Length >= (width * height) / 2) // Sanity Check
      {
        BitmapData bitmapData = tex.LockBits(new Rectangle(0, 0, width, height),
                                             ImageLockMode.ReadWrite,
                                             tex.PixelFormat);
        byte[] pixels = new byte[width * height * 4];

        int len = (width * height) / 2;
        for (int i = 0; i < len; i++) {
          byte twoPixels = data[i];

          byte intensity = (byte) ((twoPixels >> 4) * 16);
          pixels[(i * 8) + 2] = intensity; // Red
          pixels[(i * 8) + 1] = intensity; // Green
          pixels[(i * 8) + 0] = intensity; // Blue
          pixels[(i * 8) + 3] = 0xFF; // Alpha

          intensity = (byte) ((twoPixels & 0xF) * 16);
          pixels[(i * 8) + 6] = intensity; // Red
          pixels[(i * 8) + 5] = intensity; // Green
          pixels[(i * 8) + 4] = intensity; // Blue
          pixels[(i * 8) + 7] = 0xFF; // Alpha
        }
        Marshal.Copy(pixels, 0, bitmapData.Scan0, pixels.Length);
        tex.UnlockBits(bitmapData);
      }
      tex.Tag = new string[] {
          "Format: I4", "Width: " + width,
          "Height: " + height
      };
      return tex;
    }

    public static Bitmap decodeCI4(byte[] data,
                                   int width,
                                   int height,
                                   ushort[] palette,
                                   bool isPaletteRGBA16) {
      Bitmap tex = new Bitmap(width, height);

      if (data.Length >= (width * height) / 2) // Sanity Check
      {
        BitmapData bitmapData = tex.LockBits(new Rectangle(0, 0, width, height),
                                             ImageLockMode.ReadWrite,
                                             tex.PixelFormat);
        byte[] pixels = new byte[width * height * 4];

        int len = (width * height) / 2;
        for (int i = 0; i < len; i++) {
          ushort pixel = palette[(data[i] >> 4) & 0xF];
          pixels[(i * 8) + 2] = (byte) (((pixel >> 11) & 0x1F) * 8); // Red
          pixels[(i * 8) + 1] = (byte) (((pixel >> 6) & 0x1F) * 8); // Green
          pixels[(i * 8) + 0] = (byte) (((pixel >> 1) & 0x1F) * 8); // Blue
          pixels[(i * 8) + 3] =
              (pixel & 1) > 0 ? (byte) 0xFF : (byte) 0x00; // Alpha

          pixel = palette[(data[i]) & 0xF];
          pixels[(i * 8) + 6] = (byte) (((pixel >> 11) & 0x1F) * 8); // Red
          pixels[(i * 8) + 5] = (byte) (((pixel >> 6) & 0x1F) * 8); // Green
          pixels[(i * 8) + 4] = (byte) (((pixel >> 1) & 0x1F) * 8); // Blue
          pixels[(i * 8) + 7] =
              (pixel & 1) > 0 ? (byte) 0xFF : (byte) 0x00; // Alpha
        }
        Marshal.Copy(pixels, 0, bitmapData.Scan0, pixels.Length);
        tex.UnlockBits(bitmapData);
      }
      tex.Tag = new string[] {
          "Format: CI4", "Width: " + width,
          "Height: " + height
      };
      return tex;
    }

    public static Bitmap decodeCI8(byte[] data,
                                   int width,
                                   int height,
                                   ushort[] palette,
                                   bool isPaletteRGBA16) {
      Bitmap tex = new Bitmap(width, height);

      if (data.Length >= width * height) // Sanity Check
      {
        BitmapData bitmapData = tex.LockBits(new Rectangle(0, 0, width, height),
                                             ImageLockMode.ReadWrite,
                                             tex.PixelFormat);
        byte[] pixels = new byte[width * height * 4];

        int len = width * height;
        for (int i = 0; i < len; i++) {
          ushort pixel = palette[data[i]];
          pixels[(i * 4) + 2] = (byte) (((pixel >> 11) & 0x1F) * 8); // Red
          pixels[(i * 4) + 1] = (byte) (((pixel >> 6) & 0x1F) * 8); // Green
          pixels[(i * 4) + 0] = (byte) (((pixel >> 1) & 0x1F) * 8); // Blue
          pixels[(i * 4) + 3] =
              (pixel & 1) > 0 ? (byte) 0xFF : (byte) 0x00; // (Transparency)
          //tex.SetPixel(i % width, i / width, Color.FromArgb(alpha, red, green, blue));
        }
        Marshal.Copy(pixels, 0, bitmapData.Scan0, pixels.Length);
        tex.UnlockBits(bitmapData);
      }
      tex.Tag = new string[] {
          "Format: CI8", "Width: " + width,
          "Height: " + height
      };
      return tex;
    }
  }
}