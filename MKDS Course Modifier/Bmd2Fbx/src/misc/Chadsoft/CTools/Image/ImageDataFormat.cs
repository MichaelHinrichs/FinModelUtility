// Decompiled with JetBrains decompiler
// Type: Chadsoft.CTools.Image.ImageDataFormat
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.ComponentModel;

namespace Chadsoft.CTools.Image
{
  public sealed class ImageDataFormat
  {
    private static ImageDataFormat _i4;
    private static ImageDataFormat _i8;
    private static ImageDataFormat _ia4;
    private static ImageDataFormat _ia8;
    private static ImageDataFormat _rgb565;
    private static ImageDataFormat _rgb5a3;
    private static ImageDataFormat _rgba32;
    private static ImageDataFormat _c4;
    private static ImageDataFormat _c8;
    private static ImageDataFormat _c14x2;
    private static ImageDataFormat _cmpr;
    private ConvertBlockDelegate _convertTo;
    private ConvertBlockDelegate _convertFrom;

    public static ImageDataFormat I4
    {
      get
      {
        if (ImageDataFormat._i4 == null)
          ImageDataFormat._i4 = new ImageDataFormat(nameof (I4), nameof (I4), 4, 0, 8, 8, 32, false, false, false, false, 0, 0, new ConvertBlockDelegate(ImageDataFormat.ConvertBlockToI4), new ConvertBlockDelegate(ImageDataFormat.ConvertBlockFromI4));
        return ImageDataFormat._i4;
      }
    }

    public static ImageDataFormat I8
    {
      get
      {
        if (ImageDataFormat._i8 == null)
          ImageDataFormat._i8 = new ImageDataFormat(nameof (I8), nameof (I8), 8, 0, 8, 4, 32, false, false, false, false, 0, 0, new ConvertBlockDelegate(ImageDataFormat.ConvertBlockToI8), new ConvertBlockDelegate(ImageDataFormat.ConvertBlockFromI8));
        return ImageDataFormat._i8;
      }
    }

    public static ImageDataFormat IA4
    {
      get
      {
        if (ImageDataFormat._ia4 == null)
          ImageDataFormat._ia4 = new ImageDataFormat(nameof (IA4), nameof (IA4), 8, 4, 8, 4, 32, false, false, false, false, 0, 0, new ConvertBlockDelegate(ImageDataFormat.ConvertBlockToIa4), new ConvertBlockDelegate(ImageDataFormat.ConvertBlockFromIa4));
        return ImageDataFormat._ia4;
      }
    }

    public static ImageDataFormat IA8
    {
      get
      {
        if (ImageDataFormat._ia8 == null)
          ImageDataFormat._ia8 = new ImageDataFormat(nameof (IA8), nameof (IA8), 16, 8, 4, 4, 32, false, false, false, false, 0, 0, new ConvertBlockDelegate(ImageDataFormat.ConvertBlockToIa8), new ConvertBlockDelegate(ImageDataFormat.ConvertBlockFromIa8));
        return ImageDataFormat._ia8;
      }
    }

    public static ImageDataFormat RGB565
    {
      get
      {
        if (ImageDataFormat._rgb565 == null)
          ImageDataFormat._rgb565 = new ImageDataFormat(nameof (RGB565), nameof (RGB565), 16, 0, 4, 4, 32, true, false, false, false, 0, 0, new ConvertBlockDelegate(ImageDataFormat.ConvertBlockToRgb565), new ConvertBlockDelegate(ImageDataFormat.ConvertBlockFromRgb565));
        return ImageDataFormat._rgb565;
      }
    }

    public static ImageDataFormat RGB5A3
    {
      get
      {
        if (ImageDataFormat._rgb5a3 == null)
          ImageDataFormat._rgb5a3 = new ImageDataFormat(nameof (RGB5A3), nameof (RGB5A3), 16, 3, 4, 4, 32, true, false, false, false, 0, 0, new ConvertBlockDelegate(ImageDataFormat.ConvertBlockToRgb5a3), new ConvertBlockDelegate(ImageDataFormat.ConvertBlockFromRgb5a3));
        return ImageDataFormat._rgb5a3;
      }
    }

    public static ImageDataFormat Rgba32
    {
      get
      {
        if (ImageDataFormat._rgba32 == null)
          ImageDataFormat._rgba32 = new ImageDataFormat("RGBA32", "RGBA32", 32, 8, 4, 4, 64, true, false, false, false, 0, 0, new ConvertBlockDelegate(ImageDataFormat.ConvertBlockToRgba32), new ConvertBlockDelegate(ImageDataFormat.ConvertBlockFromRgba32));
        return ImageDataFormat._rgba32;
      }
    }

    public static ImageDataFormat Cmpr
    {
      get
      {
        if (ImageDataFormat._cmpr == null)
          ImageDataFormat._cmpr = new ImageDataFormat("CMPR", "CMPR", 4, 1, 8, 8, 32, true, true, true, false, 0, 0, new ConvertBlockDelegate(ImageDataFormat.ConvertBlockToCmpr), new ConvertBlockDelegate(ImageDataFormat.ConvertBlockFromCmpr));
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
      ConvertBlockDelegate convertFrom)
    {
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

    public int RoundWidth(int width)
    {
      return width + (this.BlockWidth - width % this.BlockWidth) % this.BlockWidth;
    }

    public int RoundHeight(int height)
    {
      return height + (this.BlockHeight - height % this.BlockHeight) % this.BlockHeight;
    }

    public byte[] ConvertFrom(
      byte[] data,
      int width,
      int height,
      ProgressChangedEventHandler progress)
    {
      int num1 = Math.Max(width * height / this.BlockHeight / this.BlockWidth / 100, 1024);
      byte[] numArray1 = new byte[width * height << 2];
      byte[] block = new byte[this.BlockStride];
      int num2 = 0;
      int num3 = 0;
      for (; num2 < height; num2 += this.BlockHeight)
      {
        int num4 = 0;
        while (num4 < width)
        {
          Array.Copy((Array) data, num3 * block.Length, (Array) block, 0, block.Length);
          byte[] numArray2 = this._convertFrom(block);
          for (int index = 0; index < Math.Min(this.BlockHeight, height - num2); ++index)
            Array.Copy((Array) numArray2, index * this.BlockWidth << 2, (Array) numArray1, (index + num2) * width + num4 << 2, Math.Min(this.BlockWidth, width - num4) << 2);
          if (num3 % num1 == 0 && progress != null)
            progress((object) this, new ProgressChangedEventArgs((num4 + num2 * width * 100) / (numArray1.Length / 4), (object) null));
          num4 += this.BlockWidth;
          ++num3;
        }
      }
      return numArray1;
    }

    public byte[] ConvertTo(
      byte[] data,
      int width,
      int height,
      ProgressChangedEventHandler progress)
    {
      int num1 = Math.Max(width * height / this.BlockHeight / this.BlockWidth / 100, 1024);
      byte[] numArray = new byte[this.RoundWidth(width) / this.BlockWidth * this.RoundHeight(height) / this.BlockHeight * this.BlockStride];
      byte[] block = new byte[this.BlockWidth * this.BlockHeight << 2];
      int num2 = 0;
      int num3 = 0;
      for (; num2 < height; num2 += this.BlockHeight)
      {
        int num4 = 0;
        while (num4 < width)
        {
          Array.Clear((Array) block, 0, block.Length);
          for (int index = 0; index < Math.Min(this.BlockHeight, height - num2); ++index)
            Array.Copy((Array) data, (num2 + index) * width + num4 << 2, (Array) block, index * this.BlockWidth << 2, Math.Min(this.BlockWidth, width - num4) << 2);
          this._convertTo(block).CopyTo((Array) numArray, num3 * this.BlockStride);
          if (num3 % num1 == 0 && progress != null)
            progress((object) this, new ProgressChangedEventArgs((num4 + num2 * width * 100) / (width * height), (object) null));
          num4 += this.BlockWidth;
          ++num3;
        }
      }
      return numArray;
    }

    private static byte[] ConvertBlockFromI4(byte[] block)
    {
      byte[] numArray = new byte[256];
      for (int index = 0; index < block.Length; ++index)
      {
        numArray[index * 8] = numArray[index * 8 + 1] = numArray[index * 8 + 2] = numArray[index * 8 + 3] = (byte) (((int) block[index] >> 4) * 17);
        numArray[index * 8 + 4] = numArray[index * 8 + 5] = numArray[index * 8 + 6] = numArray[index * 8 + 7] = (byte) (((int) block[index] & 15) * 17);
      }
      return numArray;
    }

    private static byte[] ConvertBlockFromI8(byte[] block)
    {
      byte[] numArray = new byte[128];
      for (int index = 0; index < block.Length; ++index)
        numArray[index * 4] = numArray[index * 4 + 1] = numArray[index * 4 + 2] = numArray[index * 4 + 3] = block[index];
      return numArray;
    }

    private static byte[] ConvertBlockFromIa4(byte[] block)
    {
      byte[] numArray = new byte[128];
      for (int index = 0; index < block.Length; ++index)
      {
        numArray[index * 4] = numArray[index * 4 + 1] = numArray[index * 4 + 2] = (byte) (((int) block[index] & 15) * 17);
        numArray[index * 4 + 3] = (byte) (((int) block[index] >> 4) * 17);
      }
      return numArray;
    }

    private static byte[] ConvertBlockFromIa8(byte[] block)
    {
      byte[] numArray = new byte[64];
      for (int index = 0; index < block.Length / 2; ++index)
      {
        numArray[index * 4] = numArray[index * 4 + 1] = numArray[index * 4 + 2] = block[index * 2 + 1];
        numArray[index * 4 + 3] = block[index * 2];
      }
      return numArray;
    }

    private static byte[] ConvertBlockFromRgb565(byte[] block)
    {
      byte[] numArray = new byte[64];
      for (int index = 0; index < block.Length / 2; ++index)
      {
        numArray[index * 4] = (byte) ((int) block[index * 2 + 1] << 3 & 248 | (int) block[index * 2 + 1] >> 2 & 7);
        numArray[index * 4 + 1] = (byte) ((int) block[index * 2] << 5 & 224 | (int) block[index * 2 + 1] >> 3 & 28 | (int) block[index * 2] >> 1 & 3);
        numArray[index * 4 + 2] = (byte) ((int) block[index * 2] & 248 | (int) block[index * 2] >> 5);
        numArray[index * 4 + 3] = byte.MaxValue;
      }
      return numArray;
    }

    private static byte[] ConvertBlockFromRgb5a3(byte[] block)
    {
      byte[] numArray = new byte[64];
      for (int index = 0; index < block.Length / 2; ++index)
      {
        if (((int) block[index * 2] & 128) == 0)
        {
          numArray[index * 4] = (byte) ((int) block[index * 2 + 1] << 4 & 240 | (int) block[index * 2 + 1] & 15);
          numArray[index * 4 + 1] = (byte) ((int) block[index * 2 + 1] & 240 | (int) block[index * 2 + 1] >> 4 & 15);
          numArray[index * 4 + 2] = (byte) ((int) block[index * 2] << 4 & 240 | (int) block[index * 2] & 15);
          numArray[index * 4 + 3] = (byte) ((int) block[index * 2] << 1 & 224 | (int) block[index * 2] >> 2 & 28 | (int) block[index * 2] >> 5 & 3);
        }
        else
        {
          numArray[index * 4] = (byte) ((int) block[index * 2 + 1] << 3 & 248 | (int) block[index * 2 + 1] >> 2 & 7);
          numArray[index * 4 + 1] = (byte) ((int) block[index * 2] << 6 & 192 | (int) block[index * 2 + 1] >> 2 & 56 | (int) block[index * 2] & 6 | (int) block[index * 2 + 1] >> 7 & 1);
          numArray[index * 4 + 2] = (byte) ((int) block[index * 2] << 1 & 248 | (int) block[index * 2] >> 4 & 7);
          numArray[index * 4 + 3] = byte.MaxValue;
        }
      }
      return numArray;
    }

    private static byte[] ConvertBlockFromRgba32(byte[] block)
    {
      byte[] numArray = new byte[64];
      for (int index = 0; index < block.Length / 4; ++index)
      {
        numArray[index * 4] = block[index * 2 + 33];
        numArray[index * 4 + 1] = block[index * 2 + 32];
        numArray[index * 4 + 2] = block[index * 2 + 1];
        numArray[index * 4 + 3] = block[index * 2];
      }
      return numArray;
    }

    private static byte[] ConvertBlockFromCmpr(byte[] block)
    {
      byte[] numArray1 = new byte[256];
      byte[][] numArray2 = new byte[4][];
      int index = 0;
      int num1 = 0;
      int num2 = 0;
      for (; index < block.Length / 8; ++index)
      {
        numArray2[index] = ImageDataFormat.ConvertBlockFromQuaterCmpr(block, index << 3);
        Array.Copy((Array) numArray2[index], 0, (Array) numArray1, num1 + num2, 16);
        Array.Copy((Array) numArray2[index], 16, (Array) numArray1, num1 + num2 + 32, 16);
        Array.Copy((Array) numArray2[index], 32, (Array) numArray1, num1 + num2 + 64, 16);
        Array.Copy((Array) numArray2[index], 48, (Array) numArray1, num1 + num2 + 96, 16);
        num1 = 16 - num1;
        if (num1 == 0)
          num2 = 128;
      }
      return numArray1;
    }

    private static byte[] ConvertBlockFromQuaterCmpr(byte[] block, int offset)
    {
      byte[] numArray1 = new byte[64];
      byte[][] numArray2 = new byte[4][]
      {
        new byte[4]
        {
          (byte) ((int) block[offset + 1] << 3 & 248),
          (byte) ((int) block[offset] << 5 & 224 | (int) block[offset + 1] >> 3 & 28),
          (byte) ((uint) block[offset] & 248U),
          byte.MaxValue
        },
        new byte[4]
        {
          (byte) ((int) block[offset + 3] << 3 & 248),
          (byte) ((int) block[offset + 2] << 5 & 224 | (int) block[offset + 3] >> 3 & 28),
          (byte) ((uint) block[offset + 2] & 248U),
          byte.MaxValue
        },
        null,
        null
      };
      if ((int) block[offset] > (int) block[offset + 2] || (int) block[offset] == (int) block[offset + 2] && (int) block[offset + 1] > (int) block[offset + 3])
      {
        numArray2[2] = new byte[4]
        {
          (byte) ((((int) numArray2[0][0] << 1) + (int) numArray2[1][0]) / 3),
          (byte) ((((int) numArray2[0][1] << 1) + (int) numArray2[1][1]) / 3),
          (byte) ((((int) numArray2[0][2] << 1) + (int) numArray2[1][2]) / 3),
          byte.MaxValue
        };
        numArray2[3] = new byte[4]
        {
          (byte) (((int) numArray2[0][0] + ((int) numArray2[1][0] << 1)) / 3),
          (byte) (((int) numArray2[0][1] + ((int) numArray2[1][1] << 1)) / 3),
          (byte) (((int) numArray2[0][2] + ((int) numArray2[1][2] << 1)) / 3),
          byte.MaxValue
        };
      }
      else
      {
        numArray2[2] = new byte[4]
        {
          (byte) ((int) numArray2[0][0] + (int) numArray2[1][0] >> 1),
          (byte) ((int) numArray2[0][1] + (int) numArray2[1][1] >> 1),
          (byte) ((int) numArray2[0][2] + (int) numArray2[1][2] >> 1),
          byte.MaxValue
        };
        numArray2[3] = new byte[4];
      }
      for (int index = 0; index < 4; ++index)
      {
        numArray2[(int) block[offset + index + 4] >> 6].CopyTo((Array) numArray1, index * 16);
        numArray2[(int) block[offset + index + 4] >> 4 & 3].CopyTo((Array) numArray1, index * 16 + 4);
        numArray2[(int) block[offset + index + 4] >> 2 & 3].CopyTo((Array) numArray1, index * 16 + 8);
        numArray2[(int) block[offset + index + 4] & 3].CopyTo((Array) numArray1, index * 16 + 12);
      }
      return numArray1;
    }

    private static byte[] ConvertBlockToI4(byte[] block)
    {
      byte[] numArray = new byte[32];
      for (int index = 0; index < numArray.Length; ++index)
        numArray[index] = (byte) (((int) block[index * 8] * 11 + (int) block[index * 8 + 1] * 59 + (int) block[index * 8 + 2] * 30) / 1700 << 4 | ((int) block[index * 8 + 4] * 11 + (int) block[index * 8 + 5] * 59 + (int) block[index * 8 + 6] * 30) / 1700);
      return numArray;
    }

    private static byte[] ConvertBlockToI8(byte[] block)
    {
      byte[] numArray = new byte[32];
      for (int index = 0; index < numArray.Length; ++index)
        numArray[index] = (byte) (((int) block[index * 4] * 11 + (int) block[index * 4 + 1] * 59 + (int) block[index * 4 + 2] * 30) / 100);
      return numArray;
    }

    private static byte[] ConvertBlockToIa4(byte[] block)
    {
      byte[] numArray = new byte[32];
      for (int index = 0; index < numArray.Length; ++index)
        numArray[index] = (byte) (((int) block[index * 4] * 11 + (int) block[index * 4 + 1] * 59 + (int) block[index * 4 + 2] * 30) / 1700 | (int) block[index * 4 + 3] / 17 << 4);
      return numArray;
    }

    private static byte[] ConvertBlockToIa8(byte[] block)
    {
      byte[] numArray = new byte[32];
      for (int index = 0; index < numArray.Length / 2; ++index)
      {
        numArray[index * 2 + 1] = (byte) (((int) block[index * 4] * 11 + (int) block[index * 4 + 1] * 59 + (int) block[index * 4 + 2] * 30) / 100);
        numArray[index * 2] = block[index * 4 + 3];
      }
      return numArray;
    }

    private static byte[] ConvertBlockToRgb565(byte[] block)
    {
      byte[] numArray = new byte[32];
      for (int index = 0; index < numArray.Length / 2; ++index)
      {
        numArray[index * 2] = (byte) ((int) block[index * 4 + 2] & 248 | (int) block[index * 4 + 1] >> 5);
        numArray[index * 2 + 1] = (byte) ((int) block[index * 4] >> 3 | (int) block[index * 4 + 1] << 3 & 224);
      }
      return numArray;
    }

    private static byte[] ConvertBlockToRgb5a3(byte[] block)
    {
      byte[] numArray = new byte[32];
      for (int index = 0; index < numArray.Length / 2; ++index)
      {
        if (block[index * 4 + 3] < (byte) 224)
        {
          numArray[index * 2] = (byte) ((int) block[index * 4 + 3] >> 1 & 112 | (int) block[index * 4 + 2] >> 4);
          numArray[index * 2 + 1] = (byte) ((int) block[index * 4 + 1] & 240 | (int) block[index * 4] >> 4);
        }
        else
        {
          numArray[index * 2] = (byte) (128 | (int) block[index * 4 + 2] >> 1 & 124 | (int) block[index * 4 + 1] >> 6);
          numArray[index * 2 + 1] = (byte) ((int) block[index * 4] >> 3 | (int) block[index * 4 + 1] << 2 & 224);
        }
      }
      return numArray;
    }

    private static byte[] ConvertBlockToRgba32(byte[] block)
    {
      byte[] numArray = new byte[64];
      for (int index = 0; index < numArray.Length / 4; ++index)
      {
        numArray[index * 2 + 33] = block[index * 4];
        numArray[index * 2 + 32] = block[index * 4 + 1];
        numArray[index * 2 + 1] = block[index * 4 + 2];
        numArray[index * 2] = block[index * 4 + 3];
      }
      return numArray;
    }

    private static byte[] ConvertBlockToCmpr(byte[] block)
    {
      byte[] numArray = new byte[32];
      byte[] block1 = new byte[64];
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      for (; num1 < block.Length / 64; ++num1)
      {
        Array.Copy((Array) block, num2 + num3, (Array) block1, 0, 16);
        Array.Copy((Array) block, num2 + num3 + 32, (Array) block1, 16, 16);
        Array.Copy((Array) block, num2 + num3 + 64, (Array) block1, 32, 16);
        Array.Copy((Array) block, num2 + num3 + 96, (Array) block1, 48, 16);
        num2 = 16 - num2;
        if (num2 == 0)
          num3 = 128;
        ImageDataFormat.ConvertBlockToQuaterCmpr(block1).CopyTo((Array) numArray, num1 << 3);
      }
      return numArray;
    }

    private static byte[] ConvertBlockToQuaterCmpr(byte[] block)
    {
      int num1;
      int num2 = num1 = -1;
      int num3 = num1;
      int num4 = num1;
      bool flag = false;
      byte[] numArray = new byte[8];
      for (int index1 = 0; index1 < 15; ++index1)
      {
        if (block[index1 * 4 + 3] < (byte) 16)
        {
          flag = true;
        }
        else
        {
          for (int index2 = index1 + 1; index2 < 16; ++index2)
          {
            int num5 = ImageDataFormat.Distance(block, index1 * 4, block, index2 * 4);
            if (num5 > num4)
            {
              num4 = num5;
              num3 = index1;
              num2 = index2;
            }
          }
        }
      }
      byte[][] palette;
      if (num4 == -1)
      {
        palette = new byte[4][]
        {
          new byte[4]{ (byte) 0, (byte) 0, (byte) 0, byte.MaxValue },
          new byte[4]
          {
            byte.MaxValue,
            byte.MaxValue,
            byte.MaxValue,
            byte.MaxValue
          },
          null,
          null
        };
      }
      else
      {
        palette = new byte[4][]
        {
          new byte[4],
          new byte[4],
          null,
          null
        };
        Array.Copy((Array) block, num3 * 4, (Array) palette[0], 0, 3);
        palette[0][3] = byte.MaxValue;
        Array.Copy((Array) block, num2 * 4, (Array) palette[1], 0, 3);
        palette[1][3] = byte.MaxValue;
        if ((int) palette[0][0] >> 3 == (int) palette[1][0] >> 3 && (int) palette[0][1] >> 2 == (int) palette[1][1] >> 2 && (int) palette[0][2] >> 3 == (int) palette[1][2] >> 3)
        {
          int num5 = (int) palette[0][0] >> 3 != 0 || (int) palette[0][1] >> 2 != 0 ? 1 : ((int) palette[0][2] >> 3 != 0 ? 1 : 0);
          palette[1][0] = num5 != 0 ? (palette[1][1] = palette[1][2] = (byte) 0) : (palette[1][1] = palette[1][2] = byte.MaxValue);
        }
      }
      numArray[0] = (byte) ((int) palette[0][2] & 248 | (int) palette[0][1] >> 5);
      numArray[1] = (byte) ((int) palette[0][1] << 3 & 224 | (int) palette[0][0] >> 3);
      numArray[2] = (byte) ((int) palette[1][2] & 248 | (int) palette[1][1] >> 5);
      numArray[3] = (byte) ((int) palette[1][1] << 3 & 224 | (int) palette[1][0] >> 3);
      if (((int) numArray[0] > (int) numArray[2] ? 1 : ((int) numArray[0] != (int) numArray[2] ? 0 : ((int) numArray[1] >= (int) numArray[3] ? 1 : 0))) == (flag ? 1 : 0))
      {
        Array.Copy((Array) numArray, 0, (Array) numArray, 4, 2);
        Array.Copy((Array) numArray, 2, (Array) numArray, 0, 2);
        Array.Copy((Array) numArray, 4, (Array) numArray, 2, 2);
        palette[2] = palette[0];
        palette[0] = palette[1];
        palette[1] = palette[2];
      }
      if (!flag)
      {
        palette[2] = new byte[4]
        {
          (byte) ((((int) palette[0][0] << 1) + (int) palette[1][0]) / 3),
          (byte) ((((int) palette[0][1] << 1) + (int) palette[1][1]) / 3),
          (byte) ((((int) palette[0][2] << 1) + (int) palette[1][2]) / 3),
          byte.MaxValue
        };
        palette[3] = new byte[4]
        {
          (byte) (((int) palette[0][0] + ((int) palette[1][0] << 1)) / 3),
          (byte) (((int) palette[0][1] + ((int) palette[1][1] << 1)) / 3),
          (byte) (((int) palette[0][2] + ((int) palette[1][2] << 1)) / 3),
          byte.MaxValue
        };
      }
      else
      {
        palette[2] = new byte[4]
        {
          (byte) ((int) palette[0][0] + (int) palette[1][0] >> 1),
          (byte) ((int) palette[0][1] + (int) palette[1][1] >> 1),
          (byte) ((int) palette[0][2] + (int) palette[1][2] >> 1),
          byte.MaxValue
        };
        palette[3] = new byte[4];
      }
      for (int index = 0; index < block.Length >> 4; ++index)
        numArray[4 + index] = (byte) (ImageDataFormat.LeastDistance(palette, block, index * 16) << 6 | ImageDataFormat.LeastDistance(palette, block, index * 16 + 4) << 4 | ImageDataFormat.LeastDistance(palette, block, index * 16 + 8) << 2 | ImageDataFormat.LeastDistance(palette, block, index * 16 + 12));
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
