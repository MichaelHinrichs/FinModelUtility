// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.G2D_Binary_File_Format.NSCR
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Converters;
using MKDS_Course_Modifier.G3D_Binary_File_Format;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.G2D_Binary_File_Format
{
  public class NSCR
  {
    public const string Signature = "RCSN";
    public FileHeader.HeaderInfo Header;
    public NSCR.Screendata ScreenData;

    public NSCR(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      bool OK;
      this.Header = new FileHeader.HeaderInfo(er, "RCSN", out OK);
      if (!OK)
      {
        int num1 = (int) MessageBox.Show("Error 0");
      }
      else
      {
        this.ScreenData = new NSCR.Screendata(er, out OK);
        if (!OK)
        {
          int num2 = (int) MessageBox.Show("Error 1");
        }
      }
      er.Close();
    }

    public NSCR(byte[] data, int width, int height, Graphic.NNSG2dColorMode mode)
    {
      this.Header = new FileHeader.HeaderInfo("RCSN", (ushort) 1);
      this.ScreenData = new NSCR.Screendata(data, width, height, mode);
    }

    public byte[] Write()
    {
      MemoryStream memoryStream = new MemoryStream();
      EndianBinaryWriter er = new EndianBinaryWriter((Stream) memoryStream, Endianness.LittleEndian);
      this.Header.Write(er);
      this.ScreenData.Write(er);
      er.BaseStream.Position = 8L;
      er.Write((uint) er.BaseStream.Length);
      er.BaseStream.Position = 20L;
      er.Write((uint) ((ulong) er.BaseStream.Length - 16UL));
      byte[] array = memoryStream.ToArray();
      er.Close();
      return array;
    }

    public class Screendata
    {
      public const string Signature = "NRCS";
      public DataBlockHeader Header;
      public ushort screenWidth;
      public ushort screenHeight;
      public Graphic.NNSG2dColorMode colorMode;
      public Graphic.NNSG2dScreenFormat screenFormat;
      public uint szByte;
      public byte[] Data;

      public Screendata(EndianBinaryReader er, out bool OK)
      {
        bool OK1;
        this.Header = new DataBlockHeader(er, "NRCS", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.screenWidth = er.ReadUInt16();
          this.screenHeight = er.ReadUInt16();
          this.colorMode = (Graphic.NNSG2dColorMode) er.ReadUInt16();
          this.screenFormat = (Graphic.NNSG2dScreenFormat) er.ReadUInt16();
          this.szByte = er.ReadUInt32();
          this.Data = er.ReadBytes((int) this.szByte);
          OK = true;
        }
      }

      public Screendata(byte[] data, int width, int height, Graphic.NNSG2dColorMode mode)
      {
        this.Header = new DataBlockHeader("NRCS", 0U);
        this.screenWidth = (ushort) width;
        this.screenHeight = (ushort) height;
        this.colorMode = mode;
        this.screenFormat = Graphic.NNSG2dScreenFormat.NNS_G2D_SCREENFORMAT_TEXT;
        this.szByte = (uint) data.Length;
        this.Data = data;
      }

      public Bitmap ToBitmap(NCGR Image, NCLR Palette)
      {
        return Graphic.ConvertData(Image.CharacterData.Data, (int) Image.CharacterData.W * 8, (int) Image.CharacterData.H * 8, Palette.PaletteData.Data, this.Data, (int) this.screenWidth, (int) this.screenHeight, Image.CharacterData.pixelFmt, Image.CharacterData.characterFmt);
      }

      public void Write(EndianBinaryWriter er)
      {
        this.Header.Write(er, 0);
        er.Write(this.screenWidth);
        er.Write(this.screenHeight);
        er.Write((ushort) this.colorMode);
        er.Write((ushort) this.screenFormat);
        er.Write(this.szByte);
        er.Write(this.Data, 0, this.Data.Length);
      }
    }
  }
}
