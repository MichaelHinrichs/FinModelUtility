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

  /// <summary>
  ///   I.e. bits per pixel.
  /// </summary>
  public enum BitsPerTexel {
    _4BPT = 0,
    _8BPT = 1,
    _16BPT = 2,
    _32BPT = 3,
  }

  public class N64ImageParser {
    public static void SplitN64ImageFormat(byte imageFormat,
                                           out N64ColorFormat colorFormat,
                                           out BitsPerTexel bitsPerTexel) {
      colorFormat =
          (N64ColorFormat) BitLogic.ExtractFromRight(imageFormat, 5, 3);
      bitsPerTexel =
          (BitsPerTexel) BitLogic.ExtractFromRight(imageFormat, 3, 2);
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
                        BitsPerTexel bitsPerTexel,
                        byte[] data,
                        int width,
                        int height,
                        ushort[] palette,
                        bool isPaletteRGBA16) {
      switch (colorFormat) {
        case N64ColorFormat.RGBA: {
          switch (bitsPerTexel) {
            case BitsPerTexel._16BPT:
              return PixelImageReader.New(width,
                                          height,
                                          new Argb1555PixelReader(),
                                          Endianness.BigEndian)
                                     .Read(data);
            default:
              throw new ArgumentOutOfRangeException(
                  nameof(bitsPerTexel),
                  bitsPerTexel,
                  null);
          }
        }
        case N64ColorFormat.LA: {
          switch (bitsPerTexel) {
            case BitsPerTexel._8BPT:
              return PixelImageReader.New(width,
                                          height,
                                          new La8PixelReader(),
                                          Endianness.BigEndian)
                                     .Read(data);
            case BitsPerTexel._16BPT:
              return PixelImageReader.New(width,
                                          height,
                                          new La16PixelReader(),
                                          Endianness.BigEndian)
                                     .Read(data);
            default:
              throw new ArgumentOutOfRangeException(nameof(bitsPerTexel), bitsPerTexel, null);
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