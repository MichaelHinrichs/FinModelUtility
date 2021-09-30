// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.GCN.BTI
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using Chadsoft.CTools.Image;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace MKDS_Course_Modifier.GCN
{
  public class BTI
  {
    public BTI.BTIHeader Header;
    public byte[] Data;

    public BTI(byte[] data)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(data), Endianness.BigEndian);
      this.Header = new BTI.BTIHeader(er);
      er.BaseStream.Position = (long) this.Header.DataOffset;
      this.Data = er.ReadBytes(this.Header.GetCompressedBufferSize());
      er.Close();
    }

    public Bitmap ToBitmap()
    {
      ImageDataFormat imageDataFormat = (ImageDataFormat) null;
      switch ((byte) this.Header.Format)
      {
        case 0:
          imageDataFormat = ImageDataFormat.I4;
          break;
        case 1:
          imageDataFormat = ImageDataFormat.I8;
          break;
        case 2:
          imageDataFormat = ImageDataFormat.IA4;
          break;
        case 3:
          imageDataFormat = ImageDataFormat.IA8;
          break;
        case 4:
          imageDataFormat = ImageDataFormat.RGB565;
          break;
        case 5:
          imageDataFormat = ImageDataFormat.RGB5A3;
          break;
        case 6:
          imageDataFormat = ImageDataFormat.Rgba32;
          break;
        case 14:
          imageDataFormat = ImageDataFormat.Cmpr;
          break;
      }
      byte[] numArray = imageDataFormat.ConvertFrom(this.Data, (int) this.Header.Width, (int) this.Header.Height, (ProgressChangedEventHandler) null);
      Bitmap bitmap = new Bitmap((int) this.Header.Width, (int) this.Header.Height);
      BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, (int) this.Header.Width, (int) this.Header.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
      for (int ofs = 0; ofs < numArray.Length; ++ofs)
        Marshal.WriteByte(bitmapdata.Scan0, ofs, numArray[ofs]);
      bitmap.UnlockBits(bitmapdata);
      return bitmap;
    }

    public enum TextureFormat
    {
      I4 = 0,
      I8 = 1,
      A4_I4 = 2,
      A8_I8 = 3,
      R5_G6_B5 = 4,
      A3_RGB5 = 5,
      ARGB8 = 6,
      INDEX4 = 8,
      INDEX8 = 9,
      INDEX14_X2 = 10, // 0x0000000A
      S3TC1 = 14, // 0x0000000E
    }

    public enum PaletteFormat
    {
      PAL_A8_I8,
      PAL_R5_G6_B5,
      PAL_A3_RGB5,
    }

    public enum GX_WRAP_TAG
    {
      GX_CLAMP,
      GX_REPEAT,
      GX_MIRROR,
      GX_MAXTEXWRAPMODE,
    }

    public enum GX_TEXTURE_FILTER
    {
      GX_NEAR,
      GX_LINEAR,
      GX_NEAR_MIP_NEAR,
      GX_LIN_MIP_NEAR,
      GX_NEAR_MIP_LIN,
      GX_LIN_MIP_LIN,
      GX_NEAR2,
      GX_NEAR3,
    }

    public class BTIHeader
    {
      public BTI.TextureFormat Format;
      public byte Unknown1;
      public ushort Width;
      public ushort Height;
      public BTI.GX_WRAP_TAG WrapS;
      public BTI.GX_WRAP_TAG WrapT;
      public byte Unknown2;
      public BTI.PaletteFormat PaletteFormat;
      public ushort NrPaletteEntries;
      public uint PaletteOffset;
      public uint Unknown3;
      public BTI.GX_TEXTURE_FILTER MinFilter;
      public BTI.GX_TEXTURE_FILTER MagFilter;
      public ushort Unknown4;
      public byte NrMipMap;
      public byte Unknown5;
      public ushort Unknown6;
      public uint DataOffset;

      public BTIHeader(EndianBinaryReader er)
      {
        this.Format = (BTI.TextureFormat) er.ReadByte();
        this.Unknown1 = er.ReadByte();
        this.Width = er.ReadUInt16();
        this.Height = er.ReadUInt16();
        this.WrapS = (BTI.GX_WRAP_TAG) er.ReadByte();
        this.WrapT = (BTI.GX_WRAP_TAG) er.ReadByte();
        this.Unknown2 = er.ReadByte();
        this.PaletteFormat = (BTI.PaletteFormat) er.ReadByte();
        this.NrPaletteEntries = er.ReadUInt16();
        this.PaletteOffset = er.ReadUInt32();
        this.Unknown3 = er.ReadUInt32();
        this.MinFilter = (BTI.GX_TEXTURE_FILTER) er.ReadByte();
        this.MagFilter = (BTI.GX_TEXTURE_FILTER) er.ReadByte();
        this.Unknown4 = er.ReadUInt16();
        this.NrMipMap = er.ReadByte();
        this.Unknown5 = er.ReadByte();
        this.Unknown6 = er.ReadUInt16();
        this.DataOffset = er.ReadUInt32();
      }

      public int GetGlWrapModeS()
      {
        return this.GetGlWrapMode(this.WrapS);
      }

      public int GetGlWrapModeT()
      {
        return this.GetGlWrapMode(this.WrapT);
      }

      private int GetGlWrapMode(BTI.GX_WRAP_TAG id)
      {
        switch (id)
        {
          case BTI.GX_WRAP_TAG.GX_CLAMP:
            return 33071;
          case BTI.GX_WRAP_TAG.GX_REPEAT:
            return 10497;
          case BTI.GX_WRAP_TAG.GX_MIRROR:
            return 33648;
          case BTI.GX_WRAP_TAG.GX_MAXTEXWRAPMODE:
            return 10496;
          default:
            return -1;
        }
      }

      public int GetGlFilterModeMin()
      {
        return this.GetGlFilterMode(this.MinFilter);
      }

      public int GetGlFilterModeMag()
      {
        return this.GetGlFilterMode(this.MagFilter);
      }

      private int GetGlFilterMode(BTI.GX_TEXTURE_FILTER id)
      {
        switch (id)
        {
          case BTI.GX_TEXTURE_FILTER.GX_NEAR:
          case BTI.GX_TEXTURE_FILTER.GX_NEAR2:
          case BTI.GX_TEXTURE_FILTER.GX_NEAR3:
            return 9728;
          case BTI.GX_TEXTURE_FILTER.GX_LINEAR:
            return 9729;
          case BTI.GX_TEXTURE_FILTER.GX_NEAR_MIP_NEAR:
            return 9984;
          case BTI.GX_TEXTURE_FILTER.GX_LIN_MIP_NEAR:
            return 9985;
          case BTI.GX_TEXTURE_FILTER.GX_NEAR_MIP_LIN:
            return 9986;
          case BTI.GX_TEXTURE_FILTER.GX_LIN_MIP_LIN:
            return 9987;
          default:
            return -1;
        }
      }

      public int GetCompressedBufferSize()
      {
        int num1 = (int) this.Width + (8 - (int) this.Width % 8) % 8;
        int num2 = (int) this.Width + (4 - (int) this.Width % 4) % 4;
        int num3 = (int) this.Height + (8 - (int) this.Height % 8) % 8;
        int num4 = (int) this.Height + (4 - (int) this.Height % 4) % 4;
        switch (this.Format)
        {
          case BTI.TextureFormat.I4:
            return num1 * num3 / 2;
          case BTI.TextureFormat.I8:
            return num1 * num4;
          case BTI.TextureFormat.A4_I4:
            return num1 * num4;
          case BTI.TextureFormat.A8_I8:
            return num2 * num4 * 2;
          case BTI.TextureFormat.R5_G6_B5:
            return num2 * num4 * 2;
          case BTI.TextureFormat.A3_RGB5:
            return num2 * num4 * 2;
          case BTI.TextureFormat.ARGB8:
            return num2 * num4 * 4;
          case BTI.TextureFormat.INDEX4:
            return num1 * num3 / 2;
          case BTI.TextureFormat.INDEX8:
            return num1 * num4;
          case BTI.TextureFormat.INDEX14_X2:
            return num2 * num4 * 2;
          case BTI.TextureFormat.S3TC1:
            return num2 * num4 / 2;
          default:
            return -1;
        }
      }
    }
  }
}
