using System;
using System.IO;

using fin.image;
using fin.image.io;
using fin.math;

namespace f3dzex2.image {
  public enum N64ImageFormat : byte {
    // Note: "1 bit per pixel" is not a Fast3D format.
    _1BPP = 0x00,
    ARGB1555 = 0x10,
    RGBA8888 = 0x18,
    CI4 = 0x40,
    CI8 = 0x48,
    LA4 = 0x60,
    LA8 = 0x68,
    LA16 = 0x70,
    I4i = 0x80,
    I4ii = 0x90,
    I8 = 0x88,
  }

  public enum N64ColorFormat {
    RGBA = 0,
    YUV = 1,
    CI = 2,
    LA = 3,
    L = 4,
  }

  public enum BitsPerPixel {
    _4BPP = 0,
    _8BPP = 1,
    _16BPP = 2,
    _32BPP = 3,
  }

  public class N64ImageParser {
    public static void SplitN64ImageFormat(byte imageFormat,
                                           out N64ColorFormat colorFormat,
                                           out BitsPerPixel bitsPerPixel) {
      colorFormat =
          (N64ColorFormat) BitLogic.ExtractFromRight(imageFormat, 5, 3);
      bitsPerPixel =
          (BitsPerPixel) BitLogic.ExtractFromRight(imageFormat, 3, 2);
    }

    public IImage Parse(N64ImageFormat format,
                        byte[] data,
                        int width,
                        int height,
                        ushort[] palette,
                        bool isPaletteRGBA16) {
      SplitN64ImageFormat((byte) format, out var colorFormat, out var bitSize);
      return Parse(colorFormat,
                   bitSize,
                   data,
                   width,
                   height,
                   palette,
                   isPaletteRGBA16);
    }

    public IImage Parse(N64ColorFormat colorFormat,
                        BitsPerPixel bitsPerPixel,
                        byte[] data,
                        int width,
                        int height,
                        ushort[] palette,
                        bool isPaletteRGBA16) {
      switch (colorFormat) {
        case N64ColorFormat.RGBA: {
          switch (bitsPerPixel) {
            case BitsPerPixel._16BPP:
              return PixelImageReader.New(width,
                                          height,
                                          new Argb1555PixelReader(),
                                          Endianness.BigEndian)
                                     .Read(data);
            default:
              throw new ArgumentOutOfRangeException(
                  nameof(bitsPerPixel),
                  bitsPerPixel,
                  null);
          }
        }
        case N64ColorFormat.LA: {
          switch (bitsPerPixel) {
            case BitsPerPixel._8BPP:
              return PixelImageReader.New(width,
                                          height,
                                          new La8PixelReader(),
                                          Endianness.BigEndian)
                                     .Read(data);
            case BitsPerPixel._16BPP:
              return PixelImageReader.New(width,
                                          height,
                                          new La16PixelReader(),
                                          Endianness.BigEndian)
                                     .Read(data);
            default:
              throw new ArgumentOutOfRangeException(nameof(bitsPerPixel), bitsPerPixel, null);
          }
        }
        default:
          throw new ArgumentOutOfRangeException(nameof(colorFormat),
                                                colorFormat,
                                                null);
      }
    }
  }
}