﻿// Decompiled with JetBrains decompiler
// Type: Chadsoft.CTools.Image.ImageDataFormat
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.ComponentModel;

using fin.color;
using fin.util.color;

using Microsoft.Toolkit.HighPerformance;

using SixLabors.ImageSharp.PixelFormats;

using static j3d.GCN.BMD.SHP1Section.Batch.Packet.Primitive;

namespace Chadsoft.CTools.Image {
  public sealed class ImageDataFormat {
    private static ImageDataFormat _cmpr;
    private ConvertBlockDelegate _convertTo;
    private ConvertBlockDelegate _convertFrom;

    public static ImageDataFormat Cmpr {
      get {
        if (ImageDataFormat._cmpr == null)
          ImageDataFormat._cmpr = new ImageDataFormat(
              "CMPR",
              "CMPR",
              4,
              1,
              8,
              8,
              32,
              true,
              true,
              true,
              false,
              0,
              0,
              new ConvertBlockDelegate(ImageDataFormat.ConvertBlockToCmpr),
              new ConvertBlockDelegate(ImageDataFormat.ConvertBlockFromCmpr));
        return ImageDataFormat._cmpr;
      }
    }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public int BitsPerPixel { get; private set; }

    public int AlphaDepth { get; private set; }

    public int BlockWidth { get; private set; }

    public int BlockHeight { get; private set; }

    public int BlockStride { get; private set; }

    public bool HasColour { get; private set; }

    public bool IsCompressed { get; private set; }

    public bool LossyCompression { get; private set; }

    public bool Palette { get; private set; }

    public int PaletteSize { get; private set; }

    public int PaletteBitsPerEntry { get; private set; }

    public ImageDataFormat(
        string name,
        string description,
        int bitsPerPixel,
        int alphaDepth,
        int blockWidth,
        int blockHeight,
        int blockStride,
        bool hasColour,
        bool isCompressed,
        bool lossyCompression,
        bool palette,
        int paletteSize,
        int paletteBitsPerEntry,
        ConvertBlockDelegate convertTo,
        ConvertBlockDelegate convertFrom) {
      this.Name = name;
      this.Description = description;
      this.BitsPerPixel = bitsPerPixel;
      this.AlphaDepth = alphaDepth;
      this.BlockWidth = blockWidth;
      this.BlockHeight = blockHeight;
      this.BlockStride = blockStride;
      this.HasColour = hasColour;
      this.IsCompressed = isCompressed;
      this.Palette = palette;
      this.PaletteSize = paletteSize;
      this.PaletteBitsPerEntry = paletteBitsPerEntry;
      this._convertTo = convertTo;
      this._convertFrom = convertFrom;
    }

    public byte[] ConvertFrom(
        byte[] data,
        int width,
        int height,
        ProgressChangedEventHandler progress) {
      int num1 =
          Math.Max(width * height / this.BlockHeight / this.BlockWidth / 100,
                   1024);
      byte[] numArray1 = new byte[width * height << 2];
      byte[] block = new byte[this.BlockStride];
      int num2 = 0;
      int num3 = 0;
      for (; num2 < height; num2 += this.BlockHeight) {
        int num4 = 0;
        while (num4 < width) {
          Array.Copy((Array) data,
                     num3 * block.Length,
                     (Array) block,
                     0,
                     block.Length);
          byte[] numArray2 = this._convertFrom(block);
          for (int index = 0;
               index < Math.Min(this.BlockHeight, height - num2);
               ++index)
            Array.Copy((Array) numArray2,
                       index * this.BlockWidth << 2,
                       (Array) numArray1,
                       (index + num2) * width + num4 << 2,
                       Math.Min(this.BlockWidth, width - num4) << 2);
          if (num3 % num1 == 0 && progress != null)
            progress((object) this,
                     new ProgressChangedEventArgs(
                         (num4 + num2 * width * 100) / (numArray1.Length / 4),
                         (object) null));
          num4 += this.BlockWidth;
          ++num3;
        }
      }

      return numArray1;
    }

    private static byte[] ConvertBlockFromCmpr(byte[] block) {
      byte[] numArray1 = new byte[256];
      byte[][] numArray2 = new byte[4][];
      int index = 0;
      int num1 = 0;
      int num2 = 0;

      for (; index < block.Length / 8; ++index) {
        numArray2[index] =
            ImageDataFormat.ConvertBlockFromQuaterCmpr(block, index << 3);
        Array.Copy((Array) numArray2[index],
                   0,
                   (Array) numArray1,
                   num1 + num2,
                   16);
        Array.Copy((Array) numArray2[index],
                   16,
                   (Array) numArray1,
                   num1 + num2 + 32,
                   16);
        Array.Copy((Array) numArray2[index],
                   32,
                   (Array) numArray1,
                   num1 + num2 + 64,
                   16);
        Array.Copy((Array) numArray2[index],
                   48,
                   (Array) numArray1,
                   num1 + num2 + 96,
                   16);
        num1 = 16 - num1;
        if (num1 == 0)
          num2 = 128;
      }

      return numArray1;
    }

    private static unsafe byte[] ConvertBlockFromQuaterCmpr(byte[] block, int offset) {
      Span<byte> numArray1 = stackalloc byte[64];
      Span<Rgba32> palette = stackalloc Rgba32[4];

      for (var p = 0; p < 2; ++p) {
        ColorUtil.SplitRgb565((ushort) (block[offset + 2 * p] << 8 | block[offset + 2 * p + 1]), out var b, out var g, out var r);
        palette[p] = new Rgba32(r, g, b);
      }

      if (block[offset] > block[offset + 2] ||
          block[offset] == block[offset + 2] &&
          block[offset + 1] > block[offset + 3]) {
        palette[2] = new Rgba32(
            (byte) ((((int) palette[0].R << 1) + (int) palette[1].R) / 3),
            (byte) ((((int) palette[0].G << 1) + (int) palette[1].G) / 3),
            (byte) ((((int) palette[0].B << 1) + (int) palette[1].B) / 3),
            byte.MaxValue);
        palette[3] = new Rgba32(
            (byte) (((int) palette[0].R + ((int) palette[1].R << 1)) / 3),
            (byte) (((int) palette[0].G + ((int) palette[1].G << 1)) / 3),
            (byte) (((int) palette[0].B + ((int) palette[1].B << 1)) / 3),
            byte.MaxValue);
      } else {
        palette[2] = new Rgba32(
            (byte) ((int) palette[0].R + (int) palette[1].R >> 1)
            ,
            (byte) ((int) palette[0].G + (int) palette[1].G >> 1),
            (byte) ((int) palette[0].B + (int) palette[1].B >> 1),
            byte.MaxValue);
        palette[3] = new Rgba32(0, 0, 0, 0);
      }

      for (int r = 0; r < 4; ++r) {
        var currentRow = block[offset + r + 4];
        for (var c = 0; c < 4; ++c) {
          var paletteIndex = (currentRow >> ((3 - c) * 2)) & 3;
          var paletteColor = palette[paletteIndex];

          var i = r * 16 + 4 * c;
          numArray1[i + 0] = paletteColor.R;
          numArray1[i + 1] = paletteColor.G;
          numArray1[i + 2] = paletteColor.B;
          numArray1[i + 3] = paletteColor.A;
        }
      }

      return numArray1.ToArray();
    }

    private static byte[] ConvertBlockToCmpr(byte[] block) {
      byte[] numArray = new byte[32];
      byte[] block1 = new byte[64];
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      for (; num1 < block.Length / 64; ++num1) {
        Array.Copy((Array) block, num2 + num3, (Array) block1, 0, 16);
        Array.Copy((Array) block, num2 + num3 + 32, (Array) block1, 16, 16);
        Array.Copy((Array) block, num2 + num3 + 64, (Array) block1, 32, 16);
        Array.Copy((Array) block, num2 + num3 + 96, (Array) block1, 48, 16);
        num2 = 16 - num2;
        if (num2 == 0)
          num3 = 128;
        ImageDataFormat.ConvertBlockToQuaterCmpr(block1)
                       .CopyTo((Array) numArray, num1 << 3);
      }

      return numArray;
    }

    private static byte[] ConvertBlockToQuaterCmpr(byte[] block) {
      int num1;
      int num2 = num1 = -1;
      int num3 = num1;
      int num4 = num1;
      bool flag = false;
      byte[] numArray = new byte[8];
      for (int index1 = 0; index1 < 15; ++index1) {
        if (block[index1 * 4 + 3] < (byte) 16) {
          flag = true;
        } else {
          for (int index2 = index1 + 1; index2 < 16; ++index2) {
            int num5 =
                ImageDataFormat.Distance(block, index1 * 4, block, index2 * 4);
            if (num5 > num4) {
              num4 = num5;
              num3 = index1;
              num2 = index2;
            }
          }
        }
      }

      byte[][] palette;
      if (num4 == -1) {
        palette = new byte[4][] {
            new byte[4] { (byte) 0, (byte) 0, (byte) 0, byte.MaxValue },
            new byte[4] {
                byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue
            },
            null,
            null
        };
      } else {
        palette = new byte[4][] { new byte[4], new byte[4], null, null };
        Array.Copy((Array) block, num3 * 4, (Array) palette[0], 0, 3);
        palette[0][3] = byte.MaxValue;
        Array.Copy((Array) block, num2 * 4, (Array) palette[1], 0, 3);
        palette[1][3] = byte.MaxValue;
        if ((int) palette[0][0] >> 3 == (int) palette[1][0] >> 3 &&
            (int) palette[0][1] >> 2 == (int) palette[1][1] >> 2 &&
            (int) palette[0][2] >> 3 == (int) palette[1][2] >> 3) {
          int num5 =
              (int) palette[0][0] >> 3 != 0 || (int) palette[0][1] >> 2 != 0
                  ? 1
                  : ((int) palette[0][2] >> 3 != 0 ? 1 : 0);
          palette[1][0] = num5 != 0
              ? (palette[1][1] = palette[1][2] = (byte) 0)
              : (palette[1][1] = palette[1][2] = byte.MaxValue);
        }
      }

      numArray[0] =
          (byte) ((int) palette[0][2] & 248 | (int) palette[0][1] >> 5);
      numArray[1] =
          (byte) ((int) palette[0][1] << 3 & 224 | (int) palette[0][0] >> 3);
      numArray[2] =
          (byte) ((int) palette[1][2] & 248 | (int) palette[1][1] >> 5);
      numArray[3] =
          (byte) ((int) palette[1][1] << 3 & 224 | (int) palette[1][0] >> 3);
      if (((int) numArray[0] > (int) numArray[2]
              ? 1
              : ((int) numArray[0] != (int) numArray[2]
                  ? 0
                  : ((int) numArray[1] >= (int) numArray[3] ? 1 : 0))) ==
          (flag ? 1 : 0)) {
        Array.Copy((Array) numArray, 0, (Array) numArray, 4, 2);
        Array.Copy((Array) numArray, 2, (Array) numArray, 0, 2);
        Array.Copy((Array) numArray, 4, (Array) numArray, 2, 2);
        palette[2] = palette[0];
        palette[0] = palette[1];
        palette[1] = palette[2];
      }

      if (!flag) {
        palette[2] = new byte[4] {
            (byte) ((((int) palette[0][0] << 1) + (int) palette[1][0]) / 3),
            (byte) ((((int) palette[0][1] << 1) + (int) palette[1][1]) / 3),
            (byte) ((((int) palette[0][2] << 1) + (int) palette[1][2]) / 3),
            byte.MaxValue
        };
        palette[3] = new byte[4] {
            (byte) (((int) palette[0][0] + ((int) palette[1][0] << 1)) / 3),
            (byte) (((int) palette[0][1] + ((int) palette[1][1] << 1)) / 3),
            (byte) (((int) palette[0][2] + ((int) palette[1][2] << 1)) / 3),
            byte.MaxValue
        };
      } else {
        palette[2] = new byte[4] {
            (byte) ((int) palette[0][0] + (int) palette[1][0] >> 1),
            (byte) ((int) palette[0][1] + (int) palette[1][1] >> 1),
            (byte) ((int) palette[0][2] + (int) palette[1][2] >> 1),
            byte.MaxValue
        };
        palette[3] = new byte[4];
      }

      for (int index = 0; index < block.Length >> 4; ++index)
        numArray[4 + index] =
            (byte) (
                ImageDataFormat.LeastDistance(palette, block, index * 16) << 6 |
                ImageDataFormat.LeastDistance(palette, block, index * 16 + 4) <<
                4 | ImageDataFormat.LeastDistance(
                    palette,
                    block,
                    index * 16 + 8) << 2 |
                ImageDataFormat.LeastDistance(palette, block, index * 16 + 12));
      return numArray;
    }

    private static int LeastDistance(byte[][] palette, byte[] colour, int offset)
    {
      if (colour[offset + 3] < (byte) 8)
        return 3;
      int num1 = int.MaxValue;
      int num2 = 0;
      for (int index = 0; index < palette.Length && palette[index][3] == byte.MaxValue; ++index)
      {
        int num3 = ImageDataFormat.Distance(palette[index], 0, colour, offset);
        if (num3 < num1)
        {
          if (num3 == 0)
            return index;
          num1 = num3;
          num2 = index;
        }
      }
      return num2;
    }

    private static int Distance(byte[] colour1, int offset1, byte[] colour2, int offset2)
    {
      int num1 = 0;
      for (int index = 0; index < 3; ++index)
      {
        int num2 = (int) colour1[offset1 + index] - (int) colour2[offset2 + index];
        num1 += num2 * num2;
      }
      return num1;
    }
  }
}