// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Misc.TEX
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Converters;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;

namespace MKDS_Course_Modifier.Misc
{
  public class TEX
  {
    public const string Signature = "tex\0";
    public TEX.TEXHeader Header;
    public byte[] PaletteData;
    public byte[] TextureData;
    public byte[] Texture4x4Data;

    public TEX(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      bool OK;
      this.Header = new TEX.TEXHeader(er, out OK);
      if (!OK)
      {
        int num = (int) MessageBox.Show("Error 1");
      }
      else
      {
        er.BaseStream.Position = (long) this.Header.PaletteDataOffset;
        this.PaletteData = er.ReadBytes((int) this.Header.PaletteDataSize * (int) this.Header.NrPalettes);
        er.BaseStream.Position = (long) this.Header.TextureDataOffset;
        this.TextureData = er.ReadBytes((int) this.Header.TextureDataSize);
        er.BaseStream.Position = (long) this.Header.Texture4x4DataOffset;
        this.Texture4x4Data = er.ReadBytes((int) this.Header.Texture4x4DataSize);
      }
      er.Close();
    }

    public Bitmap GetBitmap(int PaletteNr)
    {
      return Graphic.ConvertData(this.TextureData, this.PaletteData, this.Texture4x4Data, PaletteNr, (int) this.Header.Width, (int) this.Header.Height, this.Header.TextureFormat, Graphic.NNSG2dCharacterFmt.NNS_G2D_CHARACTER_FMT_BMP, false, true);
    }

    public class TEXHeader
    {
      public string Type;
      public ushort Unknown1;
      public Graphic.GXTexFmt TextureFormat;
      public ushort Width;
      public ushort Height;
      public uint PaletteDataOffset;
      public uint PaletteDataSize;
      public uint NrPalettes;
      public uint TextureDataOffset;
      public uint TextureDataSize;
      public uint Texture4x4DataOffset;
      public uint Texture4x4DataSize;

      public TEXHeader(EndianBinaryReader er, out bool OK)
      {
        this.Type = er.ReadString(Encoding.ASCII, 4);
        if (this.Type != "tex\0")
        {
          OK = false;
        }
        else
        {
          this.Unknown1 = er.ReadUInt16();
          this.TextureFormat = (Graphic.GXTexFmt) er.ReadUInt16();
          this.Width = (ushort) (8U << (int) er.ReadUInt16());
          this.Height = (ushort) (8U << (int) er.ReadUInt16());
          this.PaletteDataOffset = er.ReadUInt32();
          this.PaletteDataSize = er.ReadUInt32();
          this.NrPalettes = er.ReadUInt32();
          this.TextureDataOffset = er.ReadUInt32();
          this.TextureDataSize = er.ReadUInt32();
          this.Texture4x4DataOffset = er.ReadUInt32();
          this.Texture4x4DataSize = er.ReadUInt32();
          OK = true;
        }
      }
    }
  }
}
