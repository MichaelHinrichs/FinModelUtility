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

using fin.image;

using gx;

using schema;


namespace bmd.GCN {
  public partial class BTI {
    public BTI.BTIHeader Header;
    public byte[] Data;

    public BTI(byte[] data) {
      using var er =
          new EndianBinaryReader(new MemoryStream(data),
                                 Endianness.BigEndian);
      this.Header = er.ReadNew<BTIHeader>();
      er.Position = (long) this.Header.DataOffset;
      this.Data = er.ReadBytes(this.Header.GetCompressedBufferSize());
    }

    public IImage ToBitmap() {
      ImageDataFormat imageDataFormat = (ImageDataFormat) null;
      switch ((byte) this.Header.Format) {
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
      byte[] numArray = imageDataFormat.ConvertFrom(
          this.Data, (int) this.Header.Width, (int) this.Header.Height,
          (ProgressChangedEventHandler) null);

      var width = this.Header.Width;
      var height = this.Header.Height;

      var bitmap = new Rgba32Image(width, height);
      bitmap.Mutate((_, setHandler) => {
        for (var y = 0; y < height; ++y) {
          for (var x = 0; x < width; ++x) {
            var i = 4 * (y * width + x);

            var b = numArray[i + 0];
            var g = numArray[i + 1];
            var r = numArray[i + 2];
            var a = numArray[i + 3];

            setHandler(x, y, r, g, b, a);
          }
        }
      });

      return bitmap;
    }

    public enum TextureFormat : byte {
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

    public enum PaletteFormat : byte {
      PAL_A8_I8,
      PAL_R5_G6_B5,
      PAL_A3_RGB5,
    }

    [BinarySchema]
    public partial class BTIHeader : IBiSerializable {
      public BTI.TextureFormat Format;
      public byte Unknown1;
      public ushort Width;
      public ushort Height;
      public GX_WRAP_TAG WrapS;
      public GX_WRAP_TAG WrapT;
      public byte Unknown2;
      public BTI.PaletteFormat PaletteFormat;
      public ushort NrPaletteEntries;
      public uint PaletteOffset;
      public uint Unknown3;
      public GX_TEXTURE_FILTER MinFilter;
      public GX_TEXTURE_FILTER MagFilter;
      public ushort Unknown4;
      public byte NrMipMap;
      public byte Unknown5;
      public ushort Unknown6;
      public uint DataOffset;

      public int GetCompressedBufferSize() {
        int num1 = (int) this.Width + (8 - (int) this.Width % 8) % 8;
        int num2 = (int) this.Width + (4 - (int) this.Width % 4) % 4;
        int num3 = (int) this.Height + (8 - (int) this.Height % 8) % 8;
        int num4 = (int) this.Height + (4 - (int) this.Height % 4) % 4;
        switch (this.Format) {
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