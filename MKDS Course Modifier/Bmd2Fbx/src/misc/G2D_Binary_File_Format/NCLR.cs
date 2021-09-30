// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.G2D_Binary_File_Format.NCLR
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Converters;
using MKDS_Course_Modifier.G3D_Binary_File_Format;
using System.Drawing;
using System.IO;

namespace MKDS_Course_Modifier.G2D_Binary_File_Format
{
  public class NCLR
  {
    public const string Signature1 = "RLCN";
    public const string Signature2 = "RPCN";
    public FileHeader.HeaderInfo Header;
    public NCLR.Palettedata PaletteData;
    public NCLR.PalettecompressData PaletteCompressData;

    public NCLR(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      bool OK;
      this.Header = new FileHeader.HeaderInfo(er, "RLCN", out OK);
      if (!OK)
      {
        er.BaseStream.Position = 0L;
        this.Header = new FileHeader.HeaderInfo(er, "RPCN", out OK);
      }
      if (!OK)
      {
        // TODO: Message box
        //int num1 = (int) MessageBox.Show("Error 0");
      }
      else
      {
        this.PaletteData = new NCLR.Palettedata(er, out OK);
        if (!OK)
        {
          // TODO: Message box
          //int num2 = (int) MessageBox.Show("Error 1");
        }
      }
      er.Close();
    }

    public NCLR(byte[] Palette, Graphic.GXTexFmt mode)
    {
      this.Header = new FileHeader.HeaderInfo("RLCN", (ushort) 1);
      this.PaletteData = new NCLR.Palettedata(Palette, mode);
    }

    public byte[] Write()
    {
      MemoryStream memoryStream = new MemoryStream();
      EndianBinaryWriter er = new EndianBinaryWriter((Stream) memoryStream, Endianness.LittleEndian);
      this.Header.Write(er);
      this.PaletteData.Write(er);
      er.BaseStream.Position = 8L;
      er.Write((uint) er.BaseStream.Length);
      byte[] array = memoryStream.ToArray();
      er.Close();
      return array;
    }

    public class Palettedata
    {
      public const string Signature = "TTLP";
      public DataBlockHeader Header;
      public Graphic.GXTexFmt fmt;
      public bool bExtendedPlt;
      public uint szByte;
      public uint pRawData;
      public byte[] Data;

      public Palettedata(EndianBinaryReader er, out bool OK)
      {
        bool OK1;
        this.Header = new DataBlockHeader(er, "TTLP", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.fmt = (Graphic.GXTexFmt) er.ReadUInt32();
          this.bExtendedPlt = er.ReadUInt32() == 1U;
          this.szByte = er.ReadUInt32();
          this.pRawData = er.ReadUInt32();
          this.Data = er.ReadBytes((int) this.Header.size - 24);
          OK = true;
        }
      }

      public Palettedata(byte[] Palette, Graphic.GXTexFmt mode)
      {
        this.Header = new DataBlockHeader("TTLP", (uint) (24 + Palette.Length));
        this.fmt = mode;
        this.bExtendedPlt = false;
        this.szByte = (uint) Palette.Length;
        this.pRawData = 16U;
        this.Data = Palette;
      }

      public void Write(EndianBinaryWriter er)
      {
        this.Header.Write(er, 24 + this.Data.Length);
        er.Write((int) this.fmt);
        er.Write(0);
        er.Write(this.Data.Length);
        er.Write(16);
        er.Write(this.Data, 0, this.Data.Length);
      }

      public Color[] ToColorArray()
      {
        return Graphic.ConvertABGR1555(this.Data);
      }
    }

    public class PalettecompressData
    {
      public const string Signature = "PMCP";
      public DataBlockHeader Header;
      public ushort numPalette;
      public ushort pad16;
      public uint pPlttIdxTbl;
      public ushort[] Data;

      public PalettecompressData(EndianBinaryReader er, out bool OK)
      {
        bool OK1;
        this.Header = new DataBlockHeader(er, "PMCP", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          OK = true;
          this.numPalette = er.ReadUInt16();
          this.pad16 = er.ReadUInt16();
          this.pPlttIdxTbl = er.ReadUInt32();
          this.Data = new ushort[(int) this.numPalette];
          for (int index = 0; index < (int) this.numPalette; ++index)
            this.Data[index] = er.ReadUInt16();
        }
      }
    }
  }
}
