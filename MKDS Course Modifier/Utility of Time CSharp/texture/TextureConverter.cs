using System;
using System.Collections.Generic;

using Microsoft.VisualBasic.CompilerServices;

namespace UoT {
  /// <summary>
  ///   Helper class that converts OoT's various texture formats to RGBA32.
  ///
  ///   Based on the spec at:
  ///   https://wiki.cloudmodding.com/oot/Textures
  /// </summary>
  public class TextureConverter {
    public static ITextureConverter GetConverter(
        ColorFormat colorFormat,
        BitSize bitSize) {
      switch (colorFormat) {
        case ColorFormat.RGBA:
          switch (bitSize) {
            case BitSize.S_32B:
              // TODO: Implement OGLTexImg = N64TexImg
              throw new NotImplementedException();
            case BitSize.S_16B:
              return new Rgba16();
            default:
              throw new NotSupportedException();
          }

        case ColorFormat.CI:
          switch (bitSize) {
            case BitSize.S_8B:
              return new Ci8();
            case BitSize.S_4B:
              return new Ci4();
            default:
              throw new NotSupportedException();
          }

        case ColorFormat.IA:
          switch (bitSize) {
            case BitSize.S_16B:
              return new Ia16();
            case BitSize.S_8B:
              return new Ia8();
            case BitSize.S_4B:
              return new Ia4();
            default:
              throw new NotSupportedException();
          }

        case ColorFormat.I:
          switch (bitSize) {
            case BitSize.S_8B:
              return new I8();
            case BitSize.S_4B:
              return new I4();
            default:
              throw new NotSupportedException();
          }

        default:
          throw new NotSupportedException();
      }
    }


    public interface ITextureConverter {
      void Convert(
          uint Width,
          uint Height,
          uint LineSize,
          IList<byte> SourceImg,
          int sourceOffset,
          ref byte[] DestImg,
          Color4UByte[] Palette);
    }

    private class Rgba16 : ITextureConverter {
      private readonly double FACTOR = 255.0 / 31;

      public void Convert(
          uint Width,
          uint Height,
          uint LineSize,
          IList<byte> SourceImg,
          int SourceTexPos,
          ref byte[] DestImg,
          Color4UByte[] _) {
        var DestTexPos = 0;

        ushort RGBA5551 = 0;
        DestImg = new byte[(int) (Width * Height * 8L + 1)];
        for (var i = 0; i < Height; ++i) {
          for (int j = 0; j < Width; ++j) {
            RGBA5551 = IoUtil.ReadUInt16(SourceImg, (uint) SourceTexPos);
            DestImg[DestTexPos] =
                (byte) Math.Round(IoUtil.ShiftR(RGBA5551, 11, 5) * FACTOR);
            DestImg[DestTexPos + 1] =
                (byte) Math.Round(IoUtil.ShiftR(RGBA5551, 6, 5) * FACTOR);
            DestImg[DestTexPos + 2] =
                (byte) Math.Round(IoUtil.ShiftR(RGBA5551, 1, 5) * FACTOR);
            if (Conversions.ToBoolean(RGBA5551 & 1))
              DestImg[DestTexPos + 3] = 255;
            else
              DestImg[DestTexPos + 3] = 0;
            SourceTexPos += 2;
            DestTexPos += 4;
          }

          SourceTexPos = (int) (SourceTexPos + (LineSize * 4L - Width));
        }
      }
    }


    // TODO: Where is YUV??


    private class Ci8 : ITextureConverter {
      private byte[] PaletteIndex = new byte[2];

      public void Convert(
          uint Width,
          uint Height,
          uint LineSize,
          IList<byte> SourceImg,
          int SourceTexPos,
          ref byte[] DestImg,
          Color4UByte[] Palette) {
        var DestTexPos = 0;

        DestImg = new byte[(int) (Width * Height * 8L + 1)];
        for (var i = 0; i < Height; ++i) {
          for (var j = 0; j < Width; ++j) {
            PaletteIndex[0] = SourceImg[SourceTexPos];
            DestImg[DestTexPos] = Palette[PaletteIndex[0]].r;
            DestImg[DestTexPos + 1] = Palette[PaletteIndex[0]].g;
            DestImg[DestTexPos + 2] = Palette[PaletteIndex[0]].b;
            DestImg[DestTexPos + 3] = Palette[PaletteIndex[0]].a;
            SourceTexPos += 1;
            DestTexPos += 4;
          }

          SourceTexPos = (int) (SourceTexPos + (LineSize * 8L - Width));
        }
      }
    }

    private class Ci4 : ITextureConverter {
      private byte[] PaletteIndex = new byte[2];

      public void Convert(
          uint Width,
          uint Height,
          uint LineSize,
          IList<byte> SourceImg,
          int SourceTexPos,
          ref byte[] DestImg,
          Color4UByte[] Palette) {
        var DestTexPos = 0;

        DestImg = new byte[(int) (Width * Height * 8L + 1)];
        for (int i = 0, loopTo = (int) (Height - 1L); i <= loopTo; i++) {
          for (int j = 0, loopTo1 = (int) (Width / 2L - 1L);
               j <= loopTo1;
               j++) {
            PaletteIndex[0] = (byte) ((SourceImg[SourceTexPos] & 0xF0) >> 4);
            PaletteIndex[1] = (byte) (SourceImg[SourceTexPos] & 0xF);

            DestImg[DestTexPos] = Palette[PaletteIndex[0]].r;
            DestImg[DestTexPos + 1] = Palette[PaletteIndex[0]].g;
            DestImg[DestTexPos + 2] = Palette[PaletteIndex[0]].b;
            DestImg[DestTexPos + 3] = Palette[PaletteIndex[0]].a;

            DestImg[DestTexPos + 4] = Palette[PaletteIndex[1]].r;
            DestImg[DestTexPos + 5] = Palette[PaletteIndex[1]].g;
            DestImg[DestTexPos + 6] = Palette[PaletteIndex[1]].b;
            DestImg[DestTexPos + 7] = Palette[PaletteIndex[1]].a;

            SourceTexPos += 1;
            DestTexPos += 8;
          }

          SourceTexPos = (int) (SourceTexPos + (LineSize * 8L - Width / 2L));
        }
      }
    }


    private class I8 : ITextureConverter {
      private const byte FACTOR = 17;

      public void Convert(
          uint Width,
          uint Height,
          uint LineSize,
          IList<byte> SourceImg,
          int SourceTexPos,
          ref byte[] DestImg,
          Color4UByte[] _) {
        var DestTexPos = 0;

        DestImg = new byte[(int) (Width * Height * 8L + 1)];
        for (int i = 0, loopTo = (int) (Height - 1L); i <= loopTo; i++) {
          for (int j = 0, loopTo1 = (int) (Width - 1L); j <= loopTo1; j++) {
            var intensity = SourceImg[SourceTexPos];
            DestImg[DestTexPos] = intensity;
            DestImg[DestTexPos + 1] = intensity;
            DestImg[DestTexPos + 2] = intensity;
            DestImg[DestTexPos + 3] = 255;

            ++SourceTexPos;
            DestTexPos += 4;
          }

          SourceTexPos = (int) (SourceTexPos + (LineSize * 8L - Width));
        }
      }
    }

    private class I4 : ITextureConverter {
      private const byte FACTOR = 17;

      public void Convert(
          uint Width,
          uint Height,
          uint LineSize,
          IList<byte> SourceImg,
          int SourceTexPos,
          ref byte[] DestImg,
          Color4UByte[] _) {
        var DestTexPos = 0;

        DestImg = new byte[(int) (Width * Height * 8L + 1)];
        for (int i = 0, loopTo = (int) (Height - 1L); i <= loopTo; i++) {
          for (int j = 0, loopTo1 = (int) (Width / 2L - 1L);
               j <= loopTo1;
               j++) {
            BitMath.Split(SourceImg[SourceTexPos],
                          out var upper4,
                          out var lower4);

            var upperIntensity = (byte) (upper4 * FACTOR);
            DestImg[DestTexPos] = upperIntensity;
            DestImg[DestTexPos + 1] = upperIntensity;
            DestImg[DestTexPos + 2] = upperIntensity;
            DestImg[DestTexPos + 3] = 255;

            var lowerIntensity = (byte) (lower4 * FACTOR);
            DestImg[DestTexPos + 4] = lowerIntensity;
            DestImg[DestTexPos + 5] = lowerIntensity;
            DestImg[DestTexPos + 6] = lowerIntensity;
            DestImg[DestTexPos + 7] = 255;

            ++SourceTexPos;
            DestTexPos += 8;
          }

          SourceTexPos = (int) (SourceTexPos + (LineSize * 8L - Width / 2L));
        }
      }
    }


    private class Ia16 : ITextureConverter {
      private const byte FACTOR = 17;

      public void Convert(
          uint Width,
          uint Height,
          uint LineSize,
          IList<byte> SourceImg,
          int SourceTexPos,
          ref byte[] DestImg,
          Color4UByte[] _) {
        var DestTexPos = 0;

        DestImg = new byte[(int) (Width * Height * 8L + 1)];
        for (int i = 0, loopTo = (int) (Height - 1L); i <= loopTo; i++) {
          for (int j = 0, loopTo1 = (int) (Width - 1L); j <= loopTo1; j++) {
            var IAIntensity = SourceImg[SourceTexPos];
            var IAAlpha = SourceImg[SourceTexPos + 1];
            DestImg[DestTexPos] = IAIntensity;
            DestImg[DestTexPos + 1] = IAIntensity;
            DestImg[DestTexPos + 2] = IAIntensity;
            DestImg[DestTexPos + 3] = IAAlpha;
            SourceTexPos += 2;
            DestTexPos += 4;
          }

          SourceTexPos = (int) (SourceTexPos + (LineSize * 4L - Width));
        }
      }
    }

    private class Ia8 : ITextureConverter {
      private const byte FACTOR = 17;

      public void Convert(
          uint Width,
          uint Height,
          uint LineSize,
          IList<byte> SourceImg,
          int SourceTexPos,
          ref byte[] DestImg,
          Color4UByte[] _) {
        var DestTexPos = 0;

        DestImg = new byte[(int) (Width * Height * 8L + 1)];
        for (int i = 0, loopTo = (int) (Height - 1L); i <= loopTo; i++) {
          for (int j = 0, loopTo1 = (int) (Width - 1L); j <= loopTo1; j++) {
            BitMath.Split(SourceImg[SourceTexPos],
                          out var upper4,
                          out var lower4);
            var intensity = (byte) (upper4 * FACTOR);
            var alpha = (byte) (lower4 * FACTOR);

            DestImg[DestTexPos] = intensity;
            DestImg[DestTexPos + 1] = intensity;
            DestImg[DestTexPos + 2] = intensity;
            DestImg[DestTexPos + 3] = alpha;

            ++SourceTexPos;
            DestTexPos += 4;
          }

          SourceTexPos = (int) (SourceTexPos + (LineSize * 8L - Width));
        }
      }
    }

    private class Ia4 : ITextureConverter {
      private readonly double FACTOR = 255.0 / 7;

      public void Convert(
          uint Width,
          uint Height,
          uint LineSize,
          IList<byte> SourceImg,
          int SourceTexPos,
          ref byte[] DestImg,
          Color4UByte[] _) {
        var DestTexPos = 0;

        DestImg = new byte[(int) (Width * Height * 8L + 1)];
        for (int i = 0, loopTo = (int) (Height - 1L); i <= loopTo; i++) {
          for (int j = 0, loopTo1 = (int) (Width / 2L - 1L);
               j <= loopTo1;
               j++) {
            BitMath.Split(SourceImg[SourceTexPos],
                          out var upper4,
                          out var lower4);

            byte IAAlpha;
            if (Conversions.ToBoolean(upper4 & 1))
              IAAlpha = 255;
            else
              IAAlpha = 0;
            var upperIntensity =
                (byte) Math.Round(IoUtil.ShiftR(upper4, 1, 3) * FACTOR);
            DestImg[DestTexPos] = upperIntensity;
            DestImg[DestTexPos + 1] = upperIntensity;
            DestImg[DestTexPos + 2] = upperIntensity;
            DestImg[DestTexPos + 3] = IAAlpha;

            var lowerIntensity =
                (byte) Math.Round(IoUtil.ShiftR(lower4, 1, 3) * FACTOR);
            if (Conversions.ToBoolean(lower4 & 1))
              IAAlpha = 255;
            else
              IAAlpha = 0;
            DestImg[DestTexPos + 4] = lowerIntensity;
            DestImg[DestTexPos + 5] = lowerIntensity;
            DestImg[DestTexPos + 6] = lowerIntensity;
            DestImg[DestTexPos + 7] = IAAlpha;

            ++SourceTexPos;
            DestTexPos += 8;
          }

          SourceTexPos = (int) (SourceTexPos + (LineSize * 8L - Width / 2L));
        }
      }
    }
  }

  // TODO: Move somewhere else.
  public static class BitMath {
    public static void Split(byte value, out byte upper4, out byte lower4) {
      upper4 = (byte) (value >> 4);
      lower4 = (byte) ((byte) (value << 4) >> 4);
    }
  }
}