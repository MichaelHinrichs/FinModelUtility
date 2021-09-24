// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Converters.Graphic
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.G2D_Binary_File_Format;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

namespace MKDS_Course_Modifier.Converters
{
  public class Graphic
  {
    private static int[][] shiftList = new int[9][]
    {
      new int[2],
      new int[2]{ 1, (int) byte.MaxValue },
      new int[2]{ 3, 85 },
      new int[2]{ 7, 36 },
      new int[2]{ 15, 17 },
      new int[2]{ 31, 8 },
      new int[2]{ 63, 4 },
      new int[2]{ (int) sbyte.MaxValue, 2 },
      new int[2]{ (int) byte.MaxValue, 1 }
    };

    public static Bitmap ConvertData(
      byte[] Data,
      byte[] Palette,
      int PaletteNr,
      int Width,
      int Height,
      Graphic.GXTexFmt Type,
      Graphic.NNSG2dCharacterFmt CharacterType,
      bool cut = true,
      bool firstTransparent = false)
    {
      return Graphic.ConvertData(Data, Palette, new byte[0], PaletteNr, Width, Height, Type, CharacterType, firstTransparent, cut);
    }

    public static Bitmap ConvertData(
      byte[] Data,
      byte[] Palette,
      byte[] Tex4x4,
      int PaletteNr,
      int Width,
      int Height,
      Graphic.GXTexFmt Type,
      Graphic.NNSG2dCharacterFmt CharacterType,
      bool firstTransparent = false,
      bool cut = true)
    {
      Bitmap bitmap = (Bitmap) null;
      int index1 = 0;
      switch (Type)
      {
        case Graphic.GXTexFmt.GX_TEXFMT_A3I5:
          Color[] colorArray1 = Graphic.ConvertABGR1555(Palette);
          bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
          BitmapData bitmapdata1 = bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
          int num1 = 0;
          int num2 = 0;
          foreach (byte num3 in Data)
          {
            Color color = Color.FromArgb((((int) num3 >> 5 << 2) + ((int) num3 >> 5 >> 1)) * 8, colorArray1[(int) num3 & 31]);
            Marshal.WriteInt32(bitmapdata1.Scan0, num2 * bitmapdata1.Stride + num1++ * 4, color.ToArgb());
            if (num1 >= Width)
            {
              ++num2;
              num1 = 0;
            }
          }
          bitmap.UnlockBits(bitmapdata1);
          break;
        case Graphic.GXTexFmt.GX_TEXFMT_PLTT4:
          Color[] colorArray2 = Graphic.ConvertABGR1555(Palette);
          if (firstTransparent)
            colorArray2[0] = Color.FromArgb(0, (int) byte.MaxValue, 0, 0);
          if (CharacterType == Graphic.NNSG2dCharacterFmt.NNS_G2D_CHARACTER_FMT_CHAR)
          {
            bitmap = new Bitmap(Data.Length / 16 * 8, 8, PixelFormat.Format32bppArgb);
            BitmapData bitmapdata2 = bitmap.LockBits(new Rectangle(0, 0, Data.Length / 32 * 8, 8), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            for (int index2 = 0; index2 < Data.Length / 16; ++index2)
            {
              for (int index3 = 0; index3 < 8; ++index3)
              {
                for (int index4 = 0; index4 < 2; ++index4)
                {
                  byte num3 = Data[index1];
                  int num4 = index3;
                  int num5 = index2 * 8 + index4 * 4 + 3;
                  Marshal.WriteInt32(bitmapdata2.Scan0, num4 * bitmapdata2.Stride + num5 * 4, colorArray2[((int) num3 & 3) + 4 * PaletteNr].ToArgb());
                  int num6 = index2 * 8 + index4 * 4 + 2;
                  Marshal.WriteInt32(bitmapdata2.Scan0, num4 * bitmapdata2.Stride + num6 * 4, colorArray2[((int) num3 >> 2 & 3) + 4 * PaletteNr].ToArgb());
                  int num7 = index2 * 8 + index4 * 4 + 1;
                  Marshal.WriteInt32(bitmapdata2.Scan0, num4 * bitmapdata2.Stride + num7 * 4, colorArray2[((int) num3 >> 4 & 3) + 4 * PaletteNr].ToArgb());
                  int num8 = index2 * 8 + index4 * 4;
                  Marshal.WriteInt32(bitmapdata2.Scan0, num4 * bitmapdata2.Stride + num8 * 4, colorArray2[((int) num3 >> 6 & 3) + 4 * PaletteNr].ToArgb());
                  ++index1;
                }
              }
            }
            bitmap.UnlockBits(bitmapdata2);
            break;
          }
          bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
          BitmapData bitmapdata3 = bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
          int num9 = 0;
          int num10 = 0;
          foreach (byte num3 in Data)
          {
            IntPtr scan0_1 = bitmapdata3.Scan0;
            int num4 = num10 * bitmapdata3.Stride;
            int num5 = num9;
            int num6 = num5 + 1;
            int num7 = num5 * 4;
            int ofs1 = num4 + num7;
            int argb1 = colorArray2[((int) num3 & 3) + 4 * PaletteNr].ToArgb();
            Marshal.WriteInt32(scan0_1, ofs1, argb1);
            IntPtr scan0_2 = bitmapdata3.Scan0;
            int num8 = num10 * bitmapdata3.Stride;
            int num11 = num6;
            int num12 = num11 + 1;
            int num13 = num11 * 4;
            int ofs2 = num8 + num13;
            int argb2 = colorArray2[((int) num3 >> 2 & 3) + 4 * PaletteNr].ToArgb();
            Marshal.WriteInt32(scan0_2, ofs2, argb2);
            IntPtr scan0_3 = bitmapdata3.Scan0;
            int num14 = num10 * bitmapdata3.Stride;
            int num15 = num12;
            int num16 = num15 + 1;
            int num17 = num15 * 4;
            int ofs3 = num14 + num17;
            int argb3 = colorArray2[((int) num3 >> 4 & 3) + 4 * PaletteNr].ToArgb();
            Marshal.WriteInt32(scan0_3, ofs3, argb3);
            IntPtr scan0_4 = bitmapdata3.Scan0;
            int num18 = num10 * bitmapdata3.Stride;
            int num19 = num16;
            num9 = num19 + 1;
            int num20 = num19 * 4;
            int ofs4 = num18 + num20;
            int argb4 = colorArray2[((int) num3 >> 6 & 3) + 4 * PaletteNr].ToArgb();
            Marshal.WriteInt32(scan0_4, ofs4, argb4);
            if (num9 >= Width)
            {
              ++num10;
              num9 = 0;
            }
          }
          bitmap.UnlockBits(bitmapdata3);
          break;
        case Graphic.GXTexFmt.GX_TEXFMT_PLTT16:
          Color[] colorArray3 = Graphic.ConvertABGR1555(Palette);
          if (firstTransparent)
            colorArray3[0] = Color.FromArgb(0, (int) byte.MaxValue, 0, 0);
          if (CharacterType == Graphic.NNSG2dCharacterFmt.NNS_G2D_CHARACTER_FMT_CHAR)
          {
            bitmap = new Bitmap(Data.Length / 32 * 8, 8, PixelFormat.Format32bppArgb);
            BitmapData bitmapdata2 = bitmap.LockBits(new Rectangle(0, 0, Data.Length / 32 * 8, 8), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            for (int index2 = 0; index2 < Data.Length / 32; ++index2)
            {
              for (int index3 = 0; index3 < 8; ++index3)
              {
                for (int index4 = 0; index4 < 4; ++index4)
                {
                  byte num3 = Data[index1];
                  int num4 = index3;
                  int num5 = index2 * 8 + index4 * 2 + 1;
                  Marshal.WriteInt32(bitmapdata2.Scan0, num4 * bitmapdata2.Stride + num5 * 4, colorArray3[(int) num3 / 16 + 16 * PaletteNr].ToArgb());
                  int num6 = index2 * 8 + index4 * 2;
                  Marshal.WriteInt32(bitmapdata2.Scan0, num4 * bitmapdata2.Stride + num6 * 4, colorArray3[(int) num3 % 16 + 16 * PaletteNr].ToArgb());
                  ++index1;
                }
              }
            }
            bitmap.UnlockBits(bitmapdata2);
            break;
          }
          bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
          BitmapData bitmapdata4 = bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
          int num21 = 0;
          int num22 = 0;
          foreach (byte num3 in Data)
          {
            IntPtr scan0_1 = bitmapdata4.Scan0;
            int num4 = num22 * bitmapdata4.Stride;
            int num5 = num21;
            int num6 = num5 + 1;
            int num7 = num5 * 4;
            int ofs1 = num4 + num7;
            int argb1 = colorArray3[(int) num3 % 16 + 16 * PaletteNr].ToArgb();
            Marshal.WriteInt32(scan0_1, ofs1, argb1);
            IntPtr scan0_2 = bitmapdata4.Scan0;
            int num8 = num22 * bitmapdata4.Stride;
            int num11 = num6;
            num21 = num11 + 1;
            int num12 = num11 * 4;
            int ofs2 = num8 + num12;
            int argb2 = colorArray3[(int) num3 / 16 + 16 * PaletteNr].ToArgb();
            Marshal.WriteInt32(scan0_2, ofs2, argb2);
            if (num21 >= Width)
            {
              ++num22;
              num21 = 0;
            }
          }
          bitmap.UnlockBits(bitmapdata4);
          break;
        case Graphic.GXTexFmt.GX_TEXFMT_PLTT256:
          Color[] colorArray4 = Graphic.ConvertABGR1555(Palette);
          if (firstTransparent)
            colorArray4[0] = Color.FromArgb(0, (int) byte.MaxValue, 0, 0);
          if (CharacterType == Graphic.NNSG2dCharacterFmt.NNS_G2D_CHARACTER_FMT_CHAR)
          {
            bitmap = new Bitmap(Data.Length / 64 * 8, 8, PixelFormat.Format32bppArgb);
            BitmapData bitmapdata2 = bitmap.LockBits(new Rectangle(0, 0, Data.Length / 64 * 8, 8), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            for (int index2 = 0; index2 < Data.Length / 64; ++index2)
            {
              for (int index3 = 0; index3 < 8; ++index3)
              {
                for (int index4 = 0; index4 < 8; ++index4)
                {
                  byte num3 = Data[index1];
                  int num4 = index3;
                  int num5 = index2 * 8 + index4;
                  Marshal.WriteInt32(bitmapdata2.Scan0, num4 * bitmapdata2.Stride + num5 * 4, colorArray4[(int) num3 + 256 * PaletteNr].ToArgb());
                  ++index1;
                }
              }
            }
            bitmap.UnlockBits(bitmapdata2);
            break;
          }
          bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
          BitmapData bitmapdata5 = bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
          int num23 = 0;
          int num24 = 0;
          foreach (byte num3 in Data)
          {
            Marshal.WriteInt32(bitmapdata5.Scan0, num24 * bitmapdata5.Stride + num23++ * 4, colorArray4[(int) num3 + 256 * PaletteNr].ToArgb());
            if (num23 >= Width)
            {
              ++num24;
              num23 = 0;
            }
          }
          bitmap.UnlockBits(bitmapdata5);
          break;
        case Graphic.GXTexFmt.GX_TEXFMT_COMP4x4:
          Color[] colorArray5 = Graphic.ConvertABGR1555(Palette);
          bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
          BitmapData bitmapdata6 = bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
          int num25 = 0;
          int num26 = 0;
          for (int index2 = 0; index2 < Data.Length / 4; ++index2)
          {
            int num3 = Bytes.Read4BytesAsInt32(Data, index2 * 4);
            short num4 = Bytes.Read2BytesAsInt16(Tex4x4, index2 * 2);
            int index3 = ((int) num4 & 16383) << 1;
            bool flag1 = ((int) num4 >> 14 & 1) == 1;
            bool flag2 = ((int) num4 >> 15 & 1) == 1;
            int num5 = 0;
            for (int index4 = 0; index4 < 4; ++index4)
            {
              for (int index5 = 0; index5 < 4; ++index5)
              {
                int num6 = num3 >> num5 * 2 & 3;
                Color color = new Color();
                if (!flag1 && flag2)
                  color = colorArray5[index3 + num6];
                else if (!flag1 && !flag2)
                  color = num6 != 3 ? colorArray5[index3 + num6] : Color.Transparent;
                else if (flag1 && flag2)
                {
                  switch (num6)
                  {
                    case 0:
                    case 1:
                      color = colorArray5[index3 + num6];
                      break;
                    case 2:
                      color = Color.FromArgb((5 * (int) colorArray5[index3].R + 3 * (int) colorArray5[index3 + 1].R) / 8, (5 * (int) colorArray5[index3].G + 3 * (int) colorArray5[index3 + 1].G) / 8, (5 * (int) colorArray5[index3].B + 3 * (int) colorArray5[index3 + 1].B) / 8);
                      break;
                    case 3:
                      color = Color.FromArgb((3 * (int) colorArray5[index3].R + 5 * (int) colorArray5[index3 + 1].R) / 8, (3 * (int) colorArray5[index3].G + 5 * (int) colorArray5[index3 + 1].G) / 8, (3 * (int) colorArray5[index3].B + 5 * (int) colorArray5[index3 + 1].B) / 8);
                      break;
                  }
                }
                else if (flag1 && !flag2)
                {
                  switch (num6)
                  {
                    case 0:
                    case 1:
                      color = colorArray5[index3 + num6];
                      break;
                    case 2:
                      color = Color.FromArgb(((int) colorArray5[index3].R + (int) colorArray5[index3 + 1].R) / 2, ((int) colorArray5[index3].G + (int) colorArray5[index3 + 1].G) / 2, ((int) colorArray5[index3].B + (int) colorArray5[index3 + 1].B) / 2);
                      break;
                    case 3:
                      color = Color.Transparent;
                      break;
                  }
                }
                Marshal.WriteInt32(bitmapdata6.Scan0, num26 * bitmapdata6.Stride + num25++ * 4, color.ToArgb());
                ++num5;
              }
              num25 -= 4;
              ++num26;
            }
            num26 -= 4;
            num25 += 4;
            if (num25 >= Width)
            {
              num26 += 4;
              num25 = 0;
            }
          }
          bitmap.UnlockBits(bitmapdata6);
          break;
        case Graphic.GXTexFmt.GX_TEXFMT_A5I3:
          Color[] colorArray6 = Graphic.ConvertABGR1555(Palette);
          bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
          BitmapData bitmapdata7 = bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
          int num27 = 0;
          int num28 = 0;
          foreach (byte num3 in Data)
          {
            Color color = Color.FromArgb(((int) num3 >> 3) * 8, colorArray6[(int) num3 & 7]);
            Marshal.WriteInt32(bitmapdata7.Scan0, num28 * bitmapdata7.Stride + num27++ * 4, color.ToArgb());
            if (num27 >= Width)
            {
              ++num28;
              num27 = 0;
            }
          }
          bitmap.UnlockBits(bitmapdata7);
          break;
        case Graphic.GXTexFmt.GX_TEXFMT_DIRECT:
          bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
          BitmapData bitmapdata8 = bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
          Color[] colorArray7 = Graphic.ConvertABGR1555(Data);
          int num29 = 0;
          int num30 = 0;
          for (int index2 = 0; index2 < colorArray7.Length; ++index2)
          {
            if ((int) Data[index2 * 2 + 1] >> 7 == 0)
              colorArray7[index2] = Color.FromArgb(0, colorArray7[index2]);
            Marshal.WriteInt32(bitmapdata8.Scan0, num30 * bitmapdata8.Stride + num29++ * 4, colorArray7[index2].ToArgb());
            if (num29 >= Width)
            {
              ++num30;
              num29 = 0;
            }
          }
          bitmap.UnlockBits(bitmapdata8);
          break;
      }
      if (CharacterType == Graphic.NNSG2dCharacterFmt.NNS_G2D_CHARACTER_FMT_CHAR && cut)
        bitmap = Graphic.CutImage((Image) bitmap, Width, 1);
      return bitmap;
    }

    /*public static void ConvertBitmap(
      Bitmap b,
      out byte[] Data,
      out byte[] Palette,
      Graphic.GXTexFmt Type,
      Graphic.NNSG2dCharacterFmt CharacterType,
      out bool firstTransparent,
      bool forceFirstTransparent = false)
    {
      Graphic.ConvertBitmap(b, out Data, out Palette, out byte[] _, Type, CharacterType, out firstTransparent, forceFirstTransparent);
    }*/

    private static byte[] GetA3I5Data(Bitmap b, Color[] pal)
    {
      List<byte> byteList = new List<byte>();
      BitmapData bitmapdata = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
      for (int index = 0; index < b.Width * b.Height; ++index)
      {
        Color a = Color.FromArgb(Marshal.ReadInt32(bitmapdata.Scan0, index * 4));
        int num1 = Graphic.NearestColor(a, pal);
        int num2 = Graphic.map((int) a.A, 0, (int) byte.MaxValue, 0, 7);
        byteList.Add((byte) (num2 << 5 | num1));
      }
      b.UnlockBits(bitmapdata);
      return byteList.ToArray();
    }

    private static byte[] GetPLTT4Data(Bitmap b, Color[] pal, bool firstTransparent)
    {
      List<byte> byteList = new List<byte>();
      BitmapData bitmapdata = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
      for (int index = 0; index < b.Width * b.Height; index += 4)
      {
        Color a1 = Color.FromArgb(Marshal.ReadInt32(bitmapdata.Scan0, index * 4));
        Color a2 = Color.FromArgb(Marshal.ReadInt32(bitmapdata.Scan0, index * 4 + 4));
        Color a3 = Color.FromArgb(Marshal.ReadInt32(bitmapdata.Scan0, index * 4 + 8));
        Color a4 = Color.FromArgb(Marshal.ReadInt32(bitmapdata.Scan0, index * 4 + 12));
        int num1 = a1.A >= (byte) 127 ? Graphic.NearestColor(a1, ((IEnumerable<Color>) pal).ToList<Color>().GetRange(firstTransparent ? 1 : 0, firstTransparent ? pal.Length - 1 : pal.Length).ToArray()) + (firstTransparent ? 1 : 0) : 0;
        int num2 = a1.A >= (byte) 127 ? Graphic.NearestColor(a2, ((IEnumerable<Color>) pal).ToList<Color>().GetRange(firstTransparent ? 1 : 0, firstTransparent ? pal.Length - 1 : pal.Length).ToArray()) + (firstTransparent ? 1 : 0) : 0;
        int num3 = a1.A >= (byte) 127 ? Graphic.NearestColor(a3, ((IEnumerable<Color>) pal).ToList<Color>().GetRange(firstTransparent ? 1 : 0, firstTransparent ? pal.Length - 1 : pal.Length).ToArray()) + (firstTransparent ? 1 : 0) : 0;
        int num4 = a1.A >= (byte) 127 ? Graphic.NearestColor(a4, ((IEnumerable<Color>) pal).ToList<Color>().GetRange(firstTransparent ? 1 : 0, firstTransparent ? pal.Length - 1 : pal.Length).ToArray()) + (firstTransparent ? 1 : 0) : 0;
        byteList.Add((byte) (num4 << 6 | num3 << 4 | num2 << 2 | num1));
      }
      b.UnlockBits(bitmapdata);
      return byteList.ToArray();
    }

    private static byte[] GetPLTT16Data(
      Bitmap b,
      Graphic.NNSG2dCharacterFmt CharacterType,
      Color[] pal,
      bool firstTransparent)
    {
      if (CharacterType == Graphic.NNSG2dCharacterFmt.NNS_G2D_CHARACTER_FMT_CHAR)
      {
        List<byte> byteList = new List<byte>();
        BitmapData bitmapdata = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        for (int index1 = 0; index1 < bitmapdata.Height / 8; ++index1)
        {
          for (int index2 = 0; index2 < bitmapdata.Width / 8; ++index2)
          {
            Color[,] colorArray = new Color[8, 8];
            int num1;
            for (int index3 = 0; index3 < 8; ++index3)
            {
              for (int index4 = 0; index4 < 8; index4 = num1 + 1)
              {
                Color a1 = Color.FromArgb(Marshal.ReadInt32(bitmapdata.Scan0, (index1 * 8 + index3) * bitmapdata.Stride + (index2 * 8 + index4) * 4));
                num1 = index4 + 1;
                Color a2 = Color.FromArgb(Marshal.ReadInt32(bitmapdata.Scan0, (index1 * 8 + index3) * bitmapdata.Stride + (index2 * 8 + num1) * 4));
                int num2 = a1.A >= (byte) 127 ? Graphic.NearestColor(a1, ((IEnumerable<Color>) pal).ToList<Color>().GetRange(firstTransparent ? 1 : 0, firstTransparent ? pal.Length - 1 : pal.Length).ToArray()) + (firstTransparent ? 1 : 0) : 0;
                int num3 = a2.A >= (byte) 127 ? Graphic.NearestColor(a2, ((IEnumerable<Color>) pal).ToList<Color>().GetRange(firstTransparent ? 1 : 0, firstTransparent ? pal.Length - 1 : pal.Length).ToArray()) + (firstTransparent ? 1 : 0) : 0;
                byteList.Add((byte) (num3 << 4 | num2));
              }
            }
          }
        }
        b.UnlockBits(bitmapdata);
        return byteList.ToArray();
      }
      List<byte> byteList1 = new List<byte>();
      BitmapData bitmapdata1 = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
      for (int index = 0; index < b.Width * b.Height; index += 2)
      {
        Color a1 = Color.FromArgb(Marshal.ReadInt32(bitmapdata1.Scan0, index * 4));
        Color a2 = Color.FromArgb(Marshal.ReadInt32(bitmapdata1.Scan0, index * 4 + 4));
        int num1 = a1.A >= (byte) 127 ? Graphic.NearestColor(a1, ((IEnumerable<Color>) pal).ToList<Color>().GetRange(firstTransparent ? 1 : 0, firstTransparent ? pal.Length - 1 : pal.Length).ToArray()) + (firstTransparent ? 1 : 0) : 0;
        int num2 = a2.A >= (byte) 127 ? Graphic.NearestColor(a2, ((IEnumerable<Color>) pal).ToList<Color>().GetRange(firstTransparent ? 1 : 0, firstTransparent ? pal.Length - 1 : pal.Length).ToArray()) + (firstTransparent ? 1 : 0) : 0;
        byteList1.Add((byte) (num2 << 4 | num1));
      }
      b.UnlockBits(bitmapdata1);
      return byteList1.ToArray();
    }

    private static byte[] GetPLTT256Data(
      Bitmap b,
      Graphic.NNSG2dCharacterFmt CharacterType,
      Color[] pal,
      bool firstTransparent)
    {
      if (CharacterType == Graphic.NNSG2dCharacterFmt.NNS_G2D_CHARACTER_FMT_CHAR)
      {
        List<byte> byteList = new List<byte>();
        BitmapData bitmapdata = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        for (int index1 = 0; index1 < bitmapdata.Height / 8; ++index1)
        {
          for (int index2 = 0; index2 < bitmapdata.Width / 8; ++index2)
          {
            Color[,] colorArray = new Color[8, 8];
            for (int index3 = 0; index3 < 8; ++index3)
            {
              for (int index4 = 0; index4 < 8; ++index4)
              {
                Color a = Color.FromArgb(Marshal.ReadInt32(bitmapdata.Scan0, (index1 * 8 + index3) * bitmapdata.Stride + (index2 * 8 + index4) * 4));
                int num = a.A >= (byte) 127 ? Graphic.NearestColor(a, ((IEnumerable<Color>) pal).ToList<Color>().GetRange(firstTransparent ? 1 : 0, firstTransparent ? pal.Length - 1 : pal.Length).ToArray()) + (firstTransparent ? 1 : 0) : 0;
                byteList.Add((byte) num);
              }
            }
          }
        }
        b.UnlockBits(bitmapdata);
        return byteList.ToArray();
      }
      List<byte> byteList1 = new List<byte>();
      BitmapData bitmapdata1 = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
      for (int index = 0; index < b.Width * b.Height; ++index)
      {
        Color a = Color.FromArgb(Marshal.ReadInt32(bitmapdata1.Scan0, index * 4));
        int num = a.A >= (byte) 127 ? Graphic.NearestColor(a, ((IEnumerable<Color>) pal).ToList<Color>().GetRange(firstTransparent ? 1 : 0, firstTransparent ? pal.Length - 1 : pal.Length).ToArray()) + (firstTransparent ? 1 : 0) : 0;
        byteList1.Add((byte) num);
      }
      b.UnlockBits(bitmapdata1);
      return byteList1.ToArray();
    }

    /*private static byte[] GetCOMP4x4Data(Bitmap b, out byte[] Palette, out byte[] PalIdxData)
    {
      EndianBinaryReader endianBinaryReader = new EndianBinaryReader((Stream) new MemoryStream(DevIl.GetDXT1Data(b)), Endianness.LittleEndian);
      List<uint> uintList1 = new List<uint>();
      List<ushort> ushortList = new List<ushort>();
      List<uint> uintList2 = new List<uint>();
      do
      {
        uint num1 = (uint) endianBinaryReader.ReadUInt16();
        uint num2 = (uint) endianBinaryReader.ReadUInt16();
        uint num3 = endianBinaryReader.ReadUInt32();
        uint num4 = num1 >> 11 & 31U;
        uint num5 = (num1 >> 5 & 63U) / 2U & 31U;
        uint num6 = num1 & 31U;
        uint num7 = num2 >> 11 & 31U;
        uint num8 = (num2 >> 5 & 63U) / 2U & 31U;
        uint num9 = (uint) ((((int) (num2 & 31U) << 10) + ((int) num8 << 5) + (int) num7 << 16) + (((int) num6 << 10) + ((int) num5 << 5) + (int) num4));
        ushort num10 = 16384;
        if (num1 > num2)
          num10 |= (ushort) 32768;
        bool flag = false;
        int index;
        for (index = 0; index < uintList1.Count; ++index)
        {
          if ((int) uintList1[index] == (int) num9)
          {
            flag = true;
            break;
          }
        }
        if (!flag)
          uintList1.Add(num9);
        if (index <= 16383)
        {
          ushort num11 = (ushort) ((uint) num10 | (uint) (ushort) (index & 16383));
          uintList2.Add(num3);
          ushortList.Add(num11);
        }
        else
          break;
      }
      while (endianBinaryReader.BaseStream.Position != endianBinaryReader.BaseStream.Length);
      endianBinaryReader.Close();
      List<byte> byteList1 = new List<byte>();
      List<byte> byteList2 = new List<byte>();
      List<byte> byteList3 = new List<byte>();
      foreach (uint num in uintList2)
        byteList1.AddRange((IEnumerable<byte>) BitConverter.GetBytes(num));
      foreach (ushort num in ushortList)
        byteList2.AddRange((IEnumerable<byte>) BitConverter.GetBytes(num));
      foreach (uint num in uintList1)
        byteList3.AddRange((IEnumerable<byte>) BitConverter.GetBytes(num));
      while (byteList3.Count % 32 != 0)
        byteList3.Add((byte) 0);
      Palette = byteList3.ToArray();
      PalIdxData = byteList2.ToArray();
      return byteList1.ToArray();
    }

    private static byte[] GetA5I3Data(Bitmap b, Color[] pal)
    {
      List<byte> byteList = new List<byte>();
      BitmapData bitmapdata = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
      for (int index = 0; index < b.Width * b.Height; ++index)
      {
        Color a = Color.FromArgb(Marshal.ReadInt32(bitmapdata.Scan0, index * 4));
        int num1 = Graphic.NearestColor(a, pal);
        int num2 = Graphic.map((int) a.A, 0, (int) byte.MaxValue, 0, 31);
        byteList.Add((byte) (num2 << 3 | num1));
      }
      b.UnlockBits(bitmapdata);
      return byteList.ToArray();
    }

    private static byte[] GetDIRECTData(Bitmap b, Color[] pal)
    {
      List<byte> byteList = new List<byte>();
      BitmapData bitmapdata = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
      for (int index = 0; index < b.Width * b.Height; ++index)
      {
        Color a = Color.FromArgb(Marshal.ReadInt32(bitmapdata.Scan0, index * 4));
        Graphic.NearestColor(a, pal);
        int num1 = Graphic.map((int) a.A, 0, (int) byte.MaxValue, 0, 1);
        ushort num2 = (ushort) ((uint) (ushort) Graphic.ToABGR1555(pal[index]) | (uint) (ushort) (num1 << 15));
        byteList.AddRange((IEnumerable<byte>) BitConverter.GetBytes(num2));
      }
      b.UnlockBits(bitmapdata);
      return byteList.ToArray();
    }

    /*public static void ConvertBitmap(
      Bitmap b,
      out byte[] Data,
      out byte[] Palette,
      out byte[] Tex4x4,
      Graphic.GXTexFmt Type,
      Graphic.NNSG2dCharacterFmt CharacterType,
      out bool firstTransparent,
      bool forceFirstTransparent = false)
    {
      Color[] colorArray = (Color[]) null;
      Tex4x4 = (byte[]) null;
      Data = (byte[]) null;
      firstTransparent = false;
      switch (Type)
      {
        case Graphic.GXTexFmt.GX_TEXFMT_NONE:
          Data = (byte[]) null;
          break;
        case Graphic.GXTexFmt.GX_TEXFMT_A3I5:
          Color[] palette1 = Graphic.GeneratePalette(b, 32, false, false);
          firstTransparent = false;
          colorArray = Graphic.RemoveUselessPaletteEntries(palette1);
          Data = Graphic.GetA3I5Data(b, colorArray);
          break;
        case Graphic.GXTexFmt.GX_TEXFMT_PLTT4:
          colorArray = Graphic.GeneratePalette(b, 4, true, false);
          firstTransparent = colorArray[0] == Color.Transparent;
          Data = Graphic.GetPLTT4Data(b, colorArray, firstTransparent);
          break;
        case Graphic.GXTexFmt.GX_TEXFMT_PLTT16:
          colorArray = Graphic.GeneratePalette(b, 16, true, forceFirstTransparent);
          firstTransparent = colorArray[0].A == (byte) 0;
          if (CharacterType == Graphic.NNSG2dCharacterFmt.NNS_G2D_CHARACTER_FMT_BMP)
            colorArray = Graphic.RemoveUselessPaletteEntries(colorArray);
          Data = Graphic.GetPLTT16Data(b, CharacterType, colorArray, firstTransparent);
          break;
        case Graphic.GXTexFmt.GX_TEXFMT_PLTT256:
          colorArray = Graphic.GeneratePalette(b, 256, true, false);
          firstTransparent = colorArray[0].A == (byte) 0;
          if (CharacterType == Graphic.NNSG2dCharacterFmt.NNS_G2D_CHARACTER_FMT_BMP)
            colorArray = Graphic.RemoveUselessPaletteEntries(colorArray);
          Data = Graphic.GetPLTT256Data(b, CharacterType, colorArray, firstTransparent);
          break;
        case Graphic.GXTexFmt.GX_TEXFMT_COMP4x4:
          Data = Graphic.GetCOMP4x4Data(b, out Palette, out Tex4x4);
          return;
        case Graphic.GXTexFmt.GX_TEXFMT_A5I3:
          Color[] palette2 = Graphic.GeneratePalette(b, 8, false, false);
          firstTransparent = false;
          colorArray = Graphic.RemoveUselessPaletteEntries(palette2);
          Data = Graphic.GetA5I3Data(b, colorArray);
          break;
        case Graphic.GXTexFmt.GX_TEXFMT_DIRECT:
          Color[] palette3 = Graphic.GeneratePalette(b, b.Width * b.Height, false, false);
          firstTransparent = false;
          Data = Graphic.GetDIRECTData(b, palette3);
          colorArray = new Color[0];
          break;
      }
      Palette = Graphic.ToABGR1555(colorArray);
    }

    public static void ConvertBitmap(
      Bitmap b,
      out byte[] Data,
      Color[] Palette,
      Graphic.GXTexFmt Type,
      Graphic.NNSG2dCharacterFmt CharacterType,
      bool forceFirstTransparent = false)
    {
      Data = (byte[]) null;
      switch (Type)
      {
        case Graphic.GXTexFmt.GX_TEXFMT_PLTT16:
          bool firstTransparent1 = forceFirstTransparent;
          Data = Graphic.GetPLTT16Data(b, CharacterType, Palette, firstTransparent1);
          break;
        case Graphic.GXTexFmt.GX_TEXFMT_PLTT256:
          bool firstTransparent2 = forceFirstTransparent;
          Data = Graphic.GetPLTT256Data(b, CharacterType, Palette, firstTransparent2);
          break;
      }
    }

    /*public static void ConvertBitmap(
      Bitmap b,
      Color[] Palette,
      out byte[] Tilemap,
      out byte[] Screendata,
      Graphic.GXTexFmt Format = Graphic.GXTexFmt.GX_TEXFMT_PLTT256,
      bool ForceFirstTransparent = false)
    {
      Graphic.Tiles8x8 tiles8x8 = new Graphic.Tiles8x8(b, true);
      bool forceFirstTransparent = ForceFirstTransparent;
      byte[] Data;
      Graphic.ConvertBitmap(tiles8x8.TileMap, out Data, Palette, Format, Graphic.NNSG2dCharacterFmt.NNS_G2D_CHARACTER_FMT_CHAR, forceFirstTransparent);
      List<byte> byteList = new List<byte>();
      for (int index1 = 0; index1 < b.Height / 8; ++index1)
      {
        for (int index2 = 0; index2 < b.Width / 8; ++index2)
        {
          bool FlipX;
          bool FlipY;
          int tileIdx = tiles8x8.GetTileIdx(index1 * (b.Width / 8) + index2, out FlipX, out FlipY);
          int num = 0 + (((FlipY ? 1 : 0) & 1) << 11) + (((FlipX ? 1 : 0) & 1) << 10) + (tileIdx & 1023);
          byteList.AddRange((IEnumerable<byte>) BitConverter.GetBytes((ushort) num));
        }
      }
      Tilemap = Data;
      Screendata = byteList.ToArray();
    }*/

    /*public static void ConvertBitmap(
      Bitmap b,
      out byte[] Palette,
      out byte[] Tilemap,
      out byte[] Screendata,
      Graphic.GXTexFmt Format = Graphic.GXTexFmt.GX_TEXFMT_PLTT256,
      bool Simplify = true)
    {
      Graphic.Tiles8x8 tiles8x8 = new Graphic.Tiles8x8(b, Simplify);
      byte[] Data;
      byte[] Palette1;
      Graphic.ConvertBitmap(tiles8x8.TileMap, out Data, out Palette1, Format, Graphic.NNSG2dCharacterFmt.NNS_G2D_CHARACTER_FMT_CHAR, out bool _, false);
      List<byte> byteList = new List<byte>();
      for (int index1 = 0; index1 < b.Height / 8; ++index1)
      {
        for (int index2 = 0; index2 < b.Width / 8; ++index2)
        {
          bool FlipX;
          bool FlipY;
          int tileIdx = tiles8x8.GetTileIdx(index1 * (b.Width / 8) + index2, out FlipX, out FlipY);
          int num = 0 + (((FlipY ? 1 : 0) & 1) << 11) + (((FlipX ? 1 : 0) & 1) << 10) + (tileIdx & 1023);
          byteList.AddRange((IEnumerable<byte>) BitConverter.GetBytes((ushort) num));
        }
      }
      Palette = Palette1;
      Tilemap = Data;
      Screendata = byteList.ToArray();
    }*/

    /*public static void ConvertBitmap(
      Bitmap a,
      Bitmap b,
      out byte[] Palette,
      out byte[] Tilemap,
      out byte[] ScreendataA,
      out byte[] ScreendataB,
      Graphic.GXTexFmt Format = Graphic.GXTexFmt.GX_TEXFMT_PLTT256)
    {
      Graphic.Tiles8x8 tiles8x8 = new Graphic.Tiles8x8(a, b);
      byte[] Data;
      byte[] Palette1;
      Graphic.ConvertBitmap(tiles8x8.TileMap, out Data, out Palette1, Graphic.GXTexFmt.GX_TEXFMT_PLTT256, Graphic.NNSG2dCharacterFmt.NNS_G2D_CHARACTER_FMT_CHAR, out bool _, false);
      List<byte> byteList1 = new List<byte>();
      bool FlipX;
      bool FlipY;
      for (int index1 = 0; index1 < b.Height / 8; ++index1)
      {
        for (int index2 = 0; index2 < b.Width / 8; ++index2)
        {
          int tileIdx = tiles8x8.GetTileIdx(index1 * (b.Width / 8) + index2, out FlipX, out FlipY);
          int num = 0 + (((FlipY ? 1 : 0) & 1) << 11) + (((FlipX ? 1 : 0) & 1) << 10) + (tileIdx & 1023);
          byteList1.AddRange((IEnumerable<byte>) BitConverter.GetBytes((ushort) num));
        }
      }
      List<byte> byteList2 = new List<byte>();
      for (int index1 = 0; index1 < b.Height / 8; ++index1)
      {
        for (int index2 = 0; index2 < b.Width / 8; ++index2)
        {
          int tileIdx = tiles8x8.GetTileIdx(tiles8x8.AllTiles.Length / 2 + index1 * (b.Width / 8) + index2, out FlipX, out FlipY);
          int num = 0 + (((FlipY ? 1 : 0) & 1) << 11) + (((FlipX ? 1 : 0) & 1) << 10) + (tileIdx & 1023);
          byteList2.AddRange((IEnumerable<byte>) BitConverter.GetBytes((ushort) num));
        }
      }
      Palette = Palette1;
      Tilemap = Data;
      ScreendataA = byteList1.ToArray();
      ScreendataB = byteList2.ToArray();
    }*/

    private static float TileDifference(Color[] a, Color[] b)
    {
      float num1 = 0.0f;
      float num2 = 0.0f;
      foreach (Color a1 in a)
      {
        foreach (Color b1 in b)
        {
          if ((double) Graphic.ColorDifference(a1, b1) < 5.0)
            ++num1;
          else
            ++num2;
        }
      }
      return (float) ((double) num2 / (a.Length > b.Length ? (double) a.Length : (double) b.Length) * 100.0);
    }

    private static int map(int x, int in_min, int in_max, int out_min, int out_max)
    {
      return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }

    /*public static Bitmap ConvertData(
      byte[] Data,
      int Width,
      int Height,
      byte[] Palette,
      byte[] ScreenData,
      int ScreenWidth,
      int ScreenHeight,
      Graphic.GXTexFmt Type,
      Graphic.NNSG2dCharacterFmt CharacterType)
    {
      Bitmap bitmap = new Bitmap(ScreenData.Length / 2 * 8, 8);
      List<Bitmap> bitmapList = new List<Bitmap>();
      List<BitmapData> bitmapDataList = new List<BitmapData>();
      for (int PaletteNr = 0; PaletteNr < Palette.Length / 2 / (Type == Graphic.GXTexFmt.GX_TEXFMT_PLTT16 ? 16 : 256); ++PaletteNr)
      {
        bitmapList.Add(Graphic.ConvertData(Data, Palette, PaletteNr, Width, Height, Type, CharacterType, false, false));
        bitmapDataList.Add(bitmapList[PaletteNr].LockBits(new Rectangle(0, 0, bitmapList[PaletteNr].Width, bitmapList[PaletteNr].Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb));
      }
      BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, ScreenData.Length / 2 * 8, 8), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
      for (int index1 = 0; index1 < ScreenData.Length / 2; ++index1)
      {
        ushort num1 = Bytes.Read2BytesAsUInt16(ScreenData, index1 * 2);
        int num2 = (int) num1 & 1023;
        bool flag1 = ((int) num1 >> 10 & 1) == 1;
        bool flag2 = ((int) num1 >> 11 & 1) == 1;
        int index2 = (int) num1 >> 12;
        if (num2 > bitmapList[0].Width / 8)
        {
          index2 = ((int) num1 >> 8 & 16) != 0 ? 1 : 0;
          num2 -= 576;
          if (num2 < 0)
            num2 = num2 + 576 - 256;
        }
        int num3 = flag2 ? 7 : 0;
        for (int index3 = 0; (flag2 ? (num3 >= 0 ? 1 : 0) : (num3 < 8 ? 1 : 0)) != 0 && index3 < 8; ++index3)
        {
          int num4 = flag1 ? 7 : 0;
          for (int index4 = 0; (flag1 ? (num4 >= 0 ? 1 : 0) : (num4 < 8 ? 1 : 0)) != 0 && index4 < 8; ++index4)
          {
            Marshal.WriteInt32(bitmapdata.Scan0, index3 * bitmapdata.Stride + index1 * 8 * 4 + index4 * 4, Marshal.ReadInt32(bitmapDataList[index2].Scan0, num3 * bitmapDataList[index2].Stride + num2 * 8 * 4 + num4 * 4));
            num4 += flag1 ? -1 : 1;
          }
          num3 += flag2 ? -1 : 1;
        }
      }
      bitmap.UnlockBits(bitmapdata);
      for (int index = 0; index < Palette.Length / 2 / (Type == Graphic.GXTexFmt.GX_TEXFMT_PLTT16 ? 16 : 256); ++index)
        bitmapList[index].UnlockBits(bitmapDataList[index]);
      return Graphic.CutImage((Image) bitmap, ScreenWidth, 1);
    }*/

    public static Bitmap ConvertData(
      byte[] Data,
      byte[] Palette,
      int Width,
      int Height,
      NCER.cellBankBlock.cellDataBank.cellData c,
      NCER.cellBankBlock.cellDataBank.CharacterDataMapingType MappingMode,
      Graphic.GXTexFmt Type,
      Graphic.NNSG2dCharacterFmt CharacterType)
    {
      Bitmap bitmap1 = new Bitmap(512, 256);
      List<Bitmap> bitmapList1 = new List<Bitmap>();
      List<Bitmap> bitmapList2 = new List<Bitmap>();
      for (int PaletteNr = 0; PaletteNr < Palette.Length / 2 / (Type == Graphic.GXTexFmt.GX_TEXFMT_PLTT16 ? 16 : 256); ++PaletteNr)
      {
        bitmapList1.Add(Graphic.ConvertData(Data, Palette, PaletteNr, Width, Height, Type, CharacterType, false, false));
        bitmapList2.Add(Graphic.ConvertData(Data, Palette, PaletteNr, Width, Height, Type, CharacterType, false, true));
      }
      using (Graphics graphics = Graphics.FromImage((Image) bitmap1))
      {
        int num1 = -256;
        foreach (NCER.cellBankBlock.cellDataBank.cellData.cellOAMAttrData cellOamAttrData in c.CellOAMAttrData)
        {
          Size size = cellOamAttrData.GetSize();
          int num2 = (int) cellOamAttrData.StartingCharacterName * 8 / (MappingMode == NCER.cellBankBlock.cellDataBank.CharacterDataMapingType.NNS_G2D_CHARACTERMAPING_2D ? Width : size.Width);
          int x1 = (int) cellOamAttrData.StartingCharacterName * 8 - num2 * (MappingMode == NCER.cellBankBlock.cellDataBank.CharacterDataMapingType.NNS_G2D_CHARACTERMAPING_2D ? Width : size.Width);
          int ycoord = (int) cellOamAttrData.YCoord;
          num1 = (int) cellOamAttrData.XCoord;
          int y = ycoord >> 7 != 1 ? ycoord + 128 : ycoord - 128;
          int xcoord = (int) cellOamAttrData.XCoord;
          int x2 = xcoord >> 8 != 1 ? xcoord + 256 : xcoord - 256;
          if (size.Width + x1 > (MappingMode == NCER.cellBankBlock.cellDataBank.CharacterDataMapingType.NNS_G2D_CHARACTERMAPING_2D ? Width : size.Width))
          {
            int x3 = x1;
            int num3 = num2;
            for (int index = 0; index < size.Width / (size.Width - x1); ++index)
            {
              graphics.DrawImage((Image) Graphic.CutImage((Image) (cellOamAttrData.OBJMode == (byte) 1 ? bitmapList2 : bitmapList1)[(int) cellOamAttrData.ColorParameter], MappingMode == NCER.cellBankBlock.cellDataBank.CharacterDataMapingType.NNS_G2D_CHARACTERMAPING_2D ? Width : size.Width, 1), new Rectangle(x2, y, size.Width - x1, size.Height), new Rectangle(x3, num3 * 8, size.Width - x1, size.Height), GraphicsUnit.Pixel);
              if (x3 + (size.Width - x1) == size.Width)
              {
                ++num3;
                x3 = 0;
              }
              x2 += size.Width - x1;
            }
          }
          else if (cellOamAttrData.FlipX && cellOamAttrData.FlipY)
            graphics.DrawImage((Image) Graphic.CutImage((Image) (cellOamAttrData.OBJMode == (byte) 1 ? bitmapList2 : bitmapList1)[(int) cellOamAttrData.ColorParameter], MappingMode == NCER.cellBankBlock.cellDataBank.CharacterDataMapingType.NNS_G2D_CHARACTERMAPING_2D ? Width : size.Width, 1), new Rectangle(x2 + size.Width, y + size.Height, -size.Width, -size.Height), new Rectangle(x1, num2 * 8, size.Width, size.Height), GraphicsUnit.Pixel);
          else if (cellOamAttrData.FlipX)
            graphics.DrawImage((Image) Graphic.CutImage((Image) (cellOamAttrData.OBJMode == (byte) 1 ? bitmapList2 : bitmapList1)[(int) cellOamAttrData.ColorParameter], MappingMode == NCER.cellBankBlock.cellDataBank.CharacterDataMapingType.NNS_G2D_CHARACTERMAPING_2D ? Width : size.Width, 1), new Rectangle(x2 + size.Width, y, -size.Width, size.Height), new Rectangle(x1, num2 * 8, size.Width, size.Height), GraphicsUnit.Pixel);
          else if (cellOamAttrData.FlipY)
            graphics.DrawImage((Image) Graphic.CutImage((Image) (cellOamAttrData.OBJMode == (byte) 1 ? bitmapList2 : bitmapList1)[(int) cellOamAttrData.ColorParameter], MappingMode == NCER.cellBankBlock.cellDataBank.CharacterDataMapingType.NNS_G2D_CHARACTERMAPING_2D ? Width : size.Width, 1), new Rectangle(x2, y + size.Height, size.Width, -size.Height), new Rectangle(x1, num2 * 8, size.Width, size.Height), GraphicsUnit.Pixel);
          else
            graphics.DrawImage((Image) Graphic.CutImage((Image) (cellOamAttrData.OBJMode == (byte) 1 ? bitmapList2 : bitmapList1)[(int) cellOamAttrData.ColorParameter], MappingMode == NCER.cellBankBlock.cellDataBank.CharacterDataMapingType.NNS_G2D_CHARACTERMAPING_2D ? Width : size.Width, 1), new Rectangle(x2, y, size.Width, size.Height), new Rectangle(x1, num2 * 8, size.Width, size.Height), GraphicsUnit.Pixel);
        }
      }
      if (c.boundingRect != null)
      {
        Bitmap bitmap2 = new Bitmap((int) c.boundingRect.maxX - (int) c.boundingRect.minX, (int) c.boundingRect.maxY - (int) c.boundingRect.minY);
        using (Graphics graphics = Graphics.FromImage((Image) bitmap2))
          graphics.DrawImage((Image) bitmap1, new Rectangle(0, 0, bitmap2.Width, bitmap2.Height), new Rectangle(256 - bitmap2.Width / 2, 128 - bitmap2.Height / 2, bitmap2.Width, bitmap2.Height), GraphicsUnit.Pixel);
        bitmap1 = bitmap2;
      }
      return bitmap1;
    }

    private static Bitmap CutImage(Image im, int width, int blockrows)
    {
      int height = im.Height / blockrows;
      int num1 = im.Width / height;
      int num2 = width / height;
      int num3 = num1 / num2;
      Bitmap bitmap = new Bitmap(num2 * height, num3 * height * blockrows);
      Graphics graphics = Graphics.FromImage((Image) bitmap);
      graphics.Clear(Color.Transparent);
      Rectangle srcRect = new Rectangle(0, 0, num2 * height, height);
      Rectangle destRect = new Rectangle(0, 0, num2 * height, height);
      for (int index1 = 0; index1 < blockrows; ++index1)
      {
        srcRect.Y = index1 * height;
        for (int index2 = 0; index2 < num3; ++index2)
        {
          srcRect.X = index2 * num2 * height;
          destRect.Y = index2 * height + index1 * num3 * height;
          graphics.DrawImage(im, destRect, srcRect, GraphicsUnit.Pixel);
        }
      }
      return bitmap;
    }

    public static Color[] ConvertABGR1555(byte[] Data)
    {
      List<Color> colorList = new List<Color>();
      bool flag = false;
      byte num1 = 0;
      foreach (byte num2 in Data)
      {
        if (flag)
        {
          colorList.Add(Color.FromArgb(Graphic.decodeColor((int) Bytes.Read2BytesAsInt16(new byte[2]
          {
            num1,
            num2
          }, 0))));
          flag = false;
        }
        else
        {
          num1 = num2;
          flag = true;
        }
      }
      return colorList.ToArray();
    }

    public static byte[] ToABGR1555(Color[] Data)
    {
      List<byte> byteList = new List<byte>();
      foreach (Color a in Data)
      {
        ushort abgR1555 = (ushort) Graphic.ToABGR1555(a);
        byteList.AddRange((IEnumerable<byte>) BitConverter.GetBytes(abgR1555));
      }
      return byteList.ToArray();
    }

    public static Color ConvertABGR1555(short Data)
    {
      return Color.FromArgb(Graphic.decodeColor((int) Data));
    }

    public static int ToABGR1555(Color a)
    {
      return Graphic.encodeColor(a.ToArgb());
    }

    public static int decodeColor(int value)
    {
      int[] numArray = new int[6]{ 3, 2, 1, 5, 5, 5 };
      int num1 = Color.FromArgb((int) byte.MaxValue, 0, 0, 0).ToArgb();
      int num2 = 0;
      int num3 = numArray.Length / 2;
      for (int index1 = 0; index1 < num3; ++index1)
      {
        int num4 = numArray[num3 - index1 - 1];
        int index2 = numArray[num3 * 2 - index1 - 1];
        int num5 = Graphic.shiftList[index2][1];
        int num6 = Graphic.shiftList[index2][0];
        int num7 = (value >> num2 & num6) * num5;
        switch (num4)
        {
          case 0:
            num1 |= num7 << 24;
            break;
          case 1:
            num1 |= num7 << 16;
            break;
          case 2:
            num1 |= num7 << 8;
            break;
          case 3:
            num1 |= num7;
            break;
          case 4:
            num1 = num7 << 16 | num7 << 8 | num7;
            break;
        }
        num2 += index2;
      }
      return num1;
    }

    public static int encodeColor(int value)
    {
      int[] numArray = new int[6]{ 3, 2, 1, 5, 5, 5 };
      int num1 = 0;
      int num2 = 0;
      int num3 = numArray.Length / 2;
      for (int index1 = 0; index1 < num3; ++index1)
      {
        int num4 = numArray[num3 - index1 - 1];
        int index2 = numArray[num3 * 2 - index1 - 1];
        int num5 = Graphic.shiftList[index2][1];
        int num6 = Graphic.shiftList[index2][0];
        int num7 = 0;
        switch (num4)
        {
          case 0:
            num7 = value >> 24 & (int) byte.MaxValue;
            break;
          case 1:
            num7 = value >> 16 & (int) byte.MaxValue;
            break;
          case 2:
            num7 = value >> 8 & (int) byte.MaxValue;
            break;
          case 3:
            num7 = value & (int) byte.MaxValue;
            break;
          case 4:
            num7 = value & (int) byte.MaxValue;
            break;
        }
        num1 |= (num7 / num5 & num6) << num2;
        num2 += index2;
      }
      return (int) (short) num1;
    }

    public static Color[] GeneratePalette(
      Bitmap b,
      int NrColors,
      bool firsttransparent = true,
      bool forceFirstTransparent = false)
    {
      BitmapData bitmapdata = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
      List<Color> source = new List<Color>();
      bool first = forceFirstTransparent;
      for (int index = 0; index < b.Width * b.Height; ++index)
      {
        source.Add(Color.FromArgb(Marshal.ReadInt32(bitmapdata.Scan0, index * 4)));
        if (source.Last<Color>().A < (byte) 127 && firsttransparent && !first)
          first = true;
      }
      b.UnlockBits(bitmapdata);
      return Graphic.GeneratePalette(source.ToArray(), NrColors, first);
    }

    public static Color[] GeneratePalette(Color[] b, int NrColors, bool first)
    {
      List<Color> list = new List<Color>((IEnumerable<Color>) b).Distinct<Color>().ToList<Color>();
      while (list.Count > NrColors - (first ? 1 : 0))
      {
        float num1 = float.MaxValue;
        int index1 = -1;
        int index2 = -1;
        for (int index3 = 0; index3 < list.Count; ++index3)
        {
          for (int index4 = index3 + 1; index4 < list.Count; ++index4)
          {
            float num2 = Graphic.ColorDifference(list[index3], list[index4]);
            if ((double) num2 < (double) num1)
            {
              num1 = num2;
              index1 = index3;
              index2 = index4;
              if ((double) num2 < 700.0)
                goto label_9;
            }
          }
        }
label_9:
        Color color = Graphic.ColorMean(list[index1], list[index2]);
        list.RemoveAt(index1);
        if (index2 > index1)
          --index2;
        list.RemoveAt(index2);
        list.Add(color);
      }
      if (first && list[0].A != (byte) 0)
        list.Insert(0, Color.Transparent);
      list.AddRange((IEnumerable<Color>) new Color[NrColors - list.Count]);
      return list.ToArray();
    }

    private static Color[] RemoveUselessPaletteEntries(Color[] Palette)
    {
      List<Color> colorList = new List<Color>();
      for (int index = 0; index < Palette.Length; ++index)
      {
        if (Palette[index] == new Color() && index != 0)
        {
          while (index % 8 != 0)
          {
            ++index;
            colorList.Add(new Color());
          }
          break;
        }
        colorList.Add(Palette[index]);
      }
      return colorList.ToArray();
    }

    public static int NearestColor(Color a, Color[] b)
    {
      float num1 = float.MaxValue;
      int num2 = -1;
      for (int index = 0; index < b.Length; ++index)
      {
        float num3 = Graphic.ColorDifference(a, b[index]);
        if ((double) num3 < (double) num1)
        {
          num1 = num3;
          num2 = index;
        }
        if ((double) num1 == 0.0)
          return num2;
      }
      return num2;
    }

    private static float ColorDifference(Color a, Color b)
    {
      int num1 = (int) a.R - (int) b.R;
      int num2 = (int) a.G - (int) b.G;
      int num3 = (int) a.B - (int) b.B;
      return (float) (num1 * num1 + num2 * num2 + num3 * num3);
    }

    private static Color ColorMean(Color a, Color b)
    {
      return Color.FromArgb(((int) a.R + (int) b.R) / 2, ((int) a.G + (int) b.G) / 2, ((int) a.B + (int) b.B) / 2);
    }

    public enum GXTexFmt
    {
      GX_TEXFMT_NONE,
      GX_TEXFMT_A3I5,
      GX_TEXFMT_PLTT4,
      GX_TEXFMT_PLTT16,
      GX_TEXFMT_PLTT256,
      GX_TEXFMT_COMP4x4,
      GX_TEXFMT_A5I3,
      GX_TEXFMT_DIRECT,
    }

    public enum NNSG2dCharacterFmt
    {
      NNS_G2D_CHARACTER_FMT_CHAR,
      NNS_G2D_CHARACTER_FMT_BMP,
      NNS_G2D_CHARACTER_FMT_MAX,
    }

    public enum GXOBJVRamModeChar
    {
      GX_OBJVRAMMODE_CHAR_2D = 0,
      GX_OBJVRAMMODE_CHAR_1D_32K = 16, // 0x00000010
      GX_OBJVRAMMODE_CHAR_1D_64K = 17, // 0x00000011
      GX_OBJVRAMMODE_CHAR_1D_128K = 18, // 0x00000012
      GX_OBJVRAMMODE_CHAR_1D_256K = 19, // 0x00000013
    }

    public enum NNSG2dColorMode
    {
      NNS_G2D_SCREENCOLORMODE_16x16,
      NNS_G2D_SCREENCOLORMODE_256x1,
      NNS_G2D_SCREENCOLORMODE_256x16,
    }

    public enum NNSG2dScreenFormat
    {
      NNS_G2D_SCREENFORMAT_TEXT,
      NNS_G2D_SCREENFORMAT_AFFINE,
      NNS_G2D_SCREENFORMAT_AFFINEEXT,
    }

    public class Tiles8x8
    {
      public Graphic.Tile8x8[] AllTiles { get; private set; }

      public Graphic.Tile8x8[] UniqueTiles { get; private set; }

      public Tiles8x8(Bitmap b, bool Simplify = true)
      {
        this.SetTiles(b, Simplify);
      }

      public Tiles8x8(Bitmap a, Bitmap b)
      {
        this.SetTiles(a, b);
      }

      private void SetTiles(Bitmap b, bool Simplify = true)
      {
        BitmapData bitmapdata = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        this.AllTiles = new Graphic.Tile8x8[bitmapdata.Width / 8 * (bitmapdata.Height / 8)];
        for (int index1 = 0; index1 < bitmapdata.Height / 8; ++index1)
        {
          for (int index2 = 0; index2 < bitmapdata.Width / 8; ++index2)
          {
            Color[,] Colors = new Color[8, 8];
            for (int index3 = 0; index3 < 8; ++index3)
            {
              for (int index4 = 0; index4 < 8; ++index4)
                Colors[index4, index3] = Color.FromArgb(Marshal.ReadInt32(bitmapdata.Scan0, (index1 * 8 + index3) * bitmapdata.Stride + (index2 * 8 + index4) * 4));
            }
            this.AllTiles[index1 * (bitmapdata.Width / 8) + index2] = new Graphic.Tile8x8(Colors);
          }
        }
        b.UnlockBits(bitmapdata);
        if (Simplify)
        {
          List<Graphic.Tile8x8> List = new List<Graphic.Tile8x8>();
          foreach (Graphic.Tile8x8 allTile in this.AllTiles)
          {
            if (!this.ContainsTile8x8(allTile, List))
              List.Add(allTile);
          }
          this.UniqueTiles = List.ToArray();
        }
        else
          this.UniqueTiles = this.AllTiles;
      }

      private void SetTiles(Bitmap a, Bitmap b)
      {
        BitmapData bitmapdata1 = a.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        BitmapData bitmapdata2 = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        this.AllTiles = new Graphic.Tile8x8[bitmapdata1.Width / 8 * (bitmapdata1.Height / 8) * 2];
        for (int index1 = 0; index1 < bitmapdata1.Height / 8; ++index1)
        {
          for (int index2 = 0; index2 < bitmapdata1.Width / 8; ++index2)
          {
            Color[,] Colors = new Color[8, 8];
            for (int index3 = 0; index3 < 8; ++index3)
            {
              for (int index4 = 0; index4 < 8; ++index4)
                Colors[index4, index3] = Color.FromArgb(Marshal.ReadInt32(bitmapdata1.Scan0, (index1 * 8 + index3) * bitmapdata1.Stride + (index2 * 8 + index4) * 4));
            }
            this.AllTiles[index1 * (bitmapdata1.Width / 8) + index2] = new Graphic.Tile8x8(Colors);
          }
        }
        for (int index1 = 0; index1 < bitmapdata1.Height / 8; ++index1)
        {
          for (int index2 = 0; index2 < bitmapdata1.Width / 8; ++index2)
          {
            Color[,] Colors = new Color[8, 8];
            for (int index3 = 0; index3 < 8; ++index3)
            {
              for (int index4 = 0; index4 < 8; ++index4)
                Colors[index4, index3] = Color.FromArgb(Marshal.ReadInt32(bitmapdata2.Scan0, (index1 * 8 + index3) * bitmapdata2.Stride + (index2 * 8 + index4) * 4));
            }
            this.AllTiles[this.AllTiles.Length / 2 + index1 * (bitmapdata1.Width / 8) + index2] = new Graphic.Tile8x8(Colors);
          }
        }
        a.UnlockBits(bitmapdata1);
        b.UnlockBits(bitmapdata2);
        List<Graphic.Tile8x8> List = new List<Graphic.Tile8x8>();
        foreach (Graphic.Tile8x8 allTile in this.AllTiles)
        {
          if (!this.ContainsTile8x8(allTile, List))
            List.Add(allTile);
        }
        this.UniqueTiles = List.ToArray();
      }

      private bool ContainsTile8x8(Graphic.Tile8x8 t, List<Graphic.Tile8x8> List)
      {
        foreach (Graphic.Tile8x8 tile8x8 in List)
        {
          if (tile8x8.Equals(t, out bool _, out bool _))
            return true;
        }
        return false;
      }

      private int IndexOfTile8x8(Graphic.Tile8x8 t, Graphic.Tile8x8[] tiles)
      {
        int num = 0;
        foreach (Graphic.Tile8x8 tile in tiles)
        {
          if (tile == t)
            return num;
          ++num;
        }
        return -1;
      }

      private int IndexOfTile8x8(
        Graphic.Tile8x8 t,
        Graphic.Tile8x8[] tiles,
        out bool FlipX,
        out bool FlipY)
      {
        int num = 0;
        foreach (Graphic.Tile8x8 tile in tiles)
        {
          bool FlipX1;
          bool FlipY1;
          if (tile.Equals(t, out FlipX1, out FlipY1))
          {
            FlipX = FlipX1;
            FlipY = FlipY1;
            return num;
          }
          ++num;
        }
        FlipX = false;
        FlipY = false;
        return -1;
      }

      public Bitmap TileMap
      {
        get
        {
          Bitmap bitmap = new Bitmap(8, this.UniqueTiles.Length * 8);
          using (Graphics graphics = Graphics.FromImage((Image) bitmap))
          {
            int y = 0;
            foreach (Graphic.Tile8x8 uniqueTile in this.UniqueTiles)
            {
              graphics.DrawImage((Image) uniqueTile.ToBitmap(), 0, y);
              y += 8;
            }
          }
          return bitmap;
        }
      }

      public int this[int index]
      {
        get
        {
          return this.AllTiles != this.UniqueTiles ? this.IndexOfTile8x8(this.AllTiles[index], this.UniqueTiles) : index;
        }
      }

      public int GetTileIdx(int index, out bool FlipX, out bool FlipY)
      {
        if (this.AllTiles != this.UniqueTiles)
          return this.IndexOfTile8x8(this.AllTiles[index], this.UniqueTiles, out FlipX, out FlipY);
        FlipX = false;
        FlipY = false;
        return index;
      }
    }

    public class Tile8x8
    {
      private Color[,] Colors;

      public Tile8x8(Color[,] Colors)
      {
        this.Colors = Colors;
      }

      public static bool operator ==(Graphic.Tile8x8 t1, Graphic.Tile8x8 t2)
      {
        for (int index1 = 0; index1 < 8; ++index1)
        {
          for (int index2 = 0; index2 < 8; ++index2)
          {
            if (t1[index1, index2] != t2[index1, index2])
              return false;
          }
        }
        return true;
      }

      public static bool operator !=(Graphic.Tile8x8 t1, Graphic.Tile8x8 t2)
      {
        for (int index1 = 0; index1 < 8; ++index1)
        {
          for (int index2 = 0; index2 < 8; ++index2)
          {
            if (t1[index1, index2] != t2[index1, index2])
              return true;
          }
        }
        return false;
      }

      public bool Equals(Graphic.Tile8x8 Tile, out bool FlipX, out bool FlipY)
      {
        FlipX = false;
        FlipY = false;
        for (int index1 = 0; index1 < 8; ++index1)
        {
          for (int index2 = 0; index2 < 8; ++index2)
          {
            if (this[index1, index2] != Tile[index1, index2])
            {
              for (int index3 = 0; index3 < 8; ++index3)
              {
                for (int index4 = 0; index4 < 8; ++index4)
                {
                  if (this[index3, index4] != Tile[7 - index3, index4])
                  {
                    for (int index5 = 0; index5 < 8; ++index5)
                    {
                      for (int index6 = 0; index6 < 8; ++index6)
                      {
                        if (this[index5, index6] != Tile[index5, 7 - index6])
                        {
                          for (int index7 = 0; index7 < 8; ++index7)
                          {
                            for (int index8 = 0; index8 < 8; ++index8)
                            {
                              if (this[index7, index8] != Tile[7 - index7, 7 - index8])
                                return false;
                            }
                          }
                          FlipX = true;
                          FlipY = true;
                          return true;
                        }
                      }
                    }
                    FlipY = true;
                    return true;
                  }
                }
              }
              FlipX = true;
              return true;
            }
          }
        }
        return true;
      }

      public Color this[int x, int y]
      {
        get
        {
          return this.Colors[x, y];
        }
        set
        {
          this.Colors[x, y] = value;
        }
      }

      public Bitmap ToBitmap()
      {
        Bitmap bitmap = new Bitmap(8, 8);
        BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, 8, 8), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
        for (int index1 = 0; index1 < 8; ++index1)
        {
          for (int index2 = 0; index2 < 8; ++index2)
            Marshal.WriteInt32(bitmapdata.Scan0, index1 * 4 + index2 * bitmapdata.Stride, this.Colors[index1, index2].ToArgb());
        }
        bitmap.UnlockBits(bitmapdata);
        return bitmap;
      }

      public byte[] GetData(Color[] Palette, Graphic.GXTexFmt Format, bool FirstTransparent)
      {
        if (Format != Graphic.GXTexFmt.GX_TEXFMT_PLTT16)
          return (byte[]) null;
        List<byte> byteList = new List<byte>();
        for (int index1 = 0; index1 < 8; ++index1)
        {
          for (int index2 = 0; index2 < 4; ++index2)
          {
            Color a1 = this[index2 * 2, index1];
            Color a2 = this[index2 * 2 + 1, index1];
            int num1 = a1.A >= (byte) 127 ? Graphic.NearestColor(a1, ((IEnumerable<Color>) Palette).ToList<Color>().GetRange(FirstTransparent ? 1 : 0, FirstTransparent ? Palette.Length - 1 : Palette.Length).ToArray()) + (FirstTransparent ? 1 : 0) : 0;
            int num2 = a2.A >= (byte) 127 ? Graphic.NearestColor(a2, ((IEnumerable<Color>) Palette).ToList<Color>().GetRange(FirstTransparent ? 1 : 0, FirstTransparent ? Palette.Length - 1 : Palette.Length).ToArray()) + (FirstTransparent ? 1 : 0) : 0;
            byteList.Add((byte) (num2 << 4 | num1));
          }
        }
        return byteList.ToArray();
      }
    }

    public class Tile4x4
    {
      private Color[,] Colors;
      public int PaletteType;

      public Tile4x4(Color[,] Colors)
      {
        this.Colors = Colors;
      }

      public Color this[int x, int y]
      {
        get
        {
          return this.Colors[x, y];
        }
        set
        {
          this.Colors[x, y] = value;
        }
      }

      public Bitmap ToBitmap()
      {
        Bitmap bitmap = new Bitmap(4, 4);
        BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, 4, 4), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
        for (int index1 = 0; index1 < 4; ++index1)
        {
          for (int index2 = 0; index2 < 4; ++index2)
            Marshal.WriteInt32(bitmapdata.Scan0, index1 * 4 + index2 * bitmapdata.Stride, this.Colors[index1, index2].ToArgb());
        }
        bitmap.UnlockBits(bitmapdata);
        return bitmap;
      }
    }
  }
}
