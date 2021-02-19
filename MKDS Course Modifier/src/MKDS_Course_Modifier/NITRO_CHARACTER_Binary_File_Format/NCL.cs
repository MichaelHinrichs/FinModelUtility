// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.NITRO_CHARACTER_Binary_File_Format.NCL
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Converters;
using MKDS_Course_Modifier.G3D_Binary_File_Format;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.NITRO_CHARACTER_Binary_File_Format
{
  public class NCL
  {
    public const string Signature = "NCCL";
    public FileHeader.HeaderInfo Header;
    public NCL.Palettedata PaletteData;

    public NCL(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      bool OK;
      this.Header = new FileHeader.HeaderInfo(er, "NCCL", out OK);
      if (!OK)
      {
        int num1 = (int) MessageBox.Show("Error 0");
      }
      else
      {
        this.PaletteData = new NCL.Palettedata(er, out OK);
        if (!OK)
        {
          int num2 = (int) MessageBox.Show("Error 1");
        }
      }
      er.Close();
    }

    public class Palettedata
    {
      public const string Signature = "PALT";
      public DataBlockHeader Header;
      public uint HeaderSize;
      public uint pRawData;
      public byte[] Data;

      public Palettedata(EndianBinaryReader er, out bool OK)
      {
        bool OK1;
        this.Header = new DataBlockHeader(er, "PALT", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.HeaderSize = er.ReadUInt32();
          this.pRawData = er.ReadUInt32();
          this.Data = er.ReadBytes((int) this.Header.size - 16);
          OK = true;
        }
      }

      public Color[] ToColorArray()
      {
        return Graphic.ConvertABGR1555(this.Data);
      }
    }
  }
}
