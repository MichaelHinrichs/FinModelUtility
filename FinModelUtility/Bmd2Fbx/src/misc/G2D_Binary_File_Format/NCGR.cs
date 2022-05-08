// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.G2D_Binary_File_Format.NCGR
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using bmd.Converters;
using bmd.G3D_Binary_File_Format;
using System.Drawing;
using System.IO;

namespace bmd.G2D_Binary_File_Format
{
  public class NCGR
  {
    public const string Signature = "RGCN";
    public FileHeader.HeaderInfo Header;
    public NCGR.Characterdata CharacterData;
    public NCGR.CharacterposInfoBlock CharacterPosInfoBlock;

    public NCGR(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      bool OK;
      this.Header = new FileHeader.HeaderInfo(er, "RGCN", out OK);
      if (!OK)
      {
        // TODO: Message box
        //int num1 = (int) MessageBox.Show("Error 0");
      }
      else
      {
        this.CharacterData = new NCGR.Characterdata(er, out OK);
        if (!OK)
        {
          // TODO: Message box
          //int num2 = (int) MessageBox.Show("Error 1");
        }
        else if (this.Header.dataBlocks == (ushort) 2)
        {
          this.CharacterPosInfoBlock = new NCGR.CharacterposInfoBlock(er, out OK);
          if (!OK)
          {
            // TODO: Message box
            //int num3 = (int) MessageBox.Show("Error 2");
          }
        }
      }
      er.Close();
    }

    public NCGR(byte[] data, int width, int height, Graphic.GXTexFmt fmt)
    {
      this.Header = new FileHeader.HeaderInfo("RGCN", (ushort) 1);
      this.CharacterData = new NCGR.Characterdata(data, width, height, fmt);
    }

    public byte[] Write()
    {
      MemoryStream memoryStream = new MemoryStream();
      EndianBinaryWriter er = new EndianBinaryWriter((Stream) memoryStream, Endianness.LittleEndian);
      this.Header.dataBlocks = (ushort) 1;
      this.Header.Write(er);
      this.CharacterData.Write(er);
      er.BaseStream.Position = 8L;
      er.WriteUInt32((uint) er.BaseStream.Length);
      er.BaseStream.Position = 20L;
      er.WriteUInt32((uint) ((ulong) er.BaseStream.Length - 16UL));
      byte[] array = memoryStream.ToArray();
      er.Close();
      return array;
    }

    public class Characterdata
    {
      public const string Signature = "RAHC";
      public DataBlockHeader Header;
      public ushort W;
      public ushort H;
      public Graphic.GXTexFmt pixelFmt;
      public Graphic.GXOBJVRamModeChar mapingType;
      public Graphic.NNSG2dCharacterFmt characterFmt;
      public uint szByte;
      public uint pRawData;
      public byte[] Data;

      public Characterdata(EndianBinaryReader er, out bool OK)
      {
        bool OK1;
        this.Header = new DataBlockHeader(er, "RAHC", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.H = er.ReadUInt16();
          this.W = er.ReadUInt16();
          this.pixelFmt = (Graphic.GXTexFmt) er.ReadUInt32();
          this.mapingType = (Graphic.GXOBJVRamModeChar) er.ReadUInt32();
          this.characterFmt = (Graphic.NNSG2dCharacterFmt) er.ReadUInt32();
          this.szByte = er.ReadUInt32();
          this.pRawData = er.ReadUInt32();
          this.Data = er.ReadBytes((int) this.szByte);
          OK = true;
        }
      }

      public Characterdata(byte[] data, int width, int height, Graphic.GXTexFmt fmt)
      {
        this.Header = new DataBlockHeader("RAHC", 0U);
        this.W = (ushort) (width / 8);
        this.H = (ushort) (height / 8);
        this.pixelFmt = fmt;
        this.mapingType = Graphic.GXOBJVRamModeChar.GX_OBJVRAMMODE_CHAR_2D;
        this.characterFmt = Graphic.NNSG2dCharacterFmt.NNS_G2D_CHARACTER_FMT_CHAR;
        this.szByte = (uint) data.Length;
        this.pRawData = 24U;
        this.Data = data;
      }

      public Bitmap ToBitmap(NCLR Palette, int PalNr)
      {
        int Width = 0;
        int Height = 0;
        switch (this.mapingType)
        {
          case Graphic.GXOBJVRamModeChar.GX_OBJVRAMMODE_CHAR_2D:
            Width = (int) this.W * 8;
            Height = (int) this.H * 8;
            break;
          case Graphic.GXOBJVRamModeChar.GX_OBJVRAMMODE_CHAR_1D_32K:
            Width = 32;
            Height = this.Data.Length * (this.pixelFmt == Graphic.GXTexFmt.GX_TEXFMT_PLTT16 ? 2 : 1) / 32;
            break;
          case Graphic.GXOBJVRamModeChar.GX_OBJVRAMMODE_CHAR_1D_64K:
            Width = 64;
            Height = this.Data.Length * (this.pixelFmt == Graphic.GXTexFmt.GX_TEXFMT_PLTT16 ? 2 : 1) / 64;
            break;
          case Graphic.GXOBJVRamModeChar.GX_OBJVRAMMODE_CHAR_1D_128K:
            Width = 128;
            Height = this.Data.Length * (this.pixelFmt == Graphic.GXTexFmt.GX_TEXFMT_PLTT16 ? 2 : 1) / 128;
            break;
          case Graphic.GXOBJVRamModeChar.GX_OBJVRAMMODE_CHAR_1D_256K:
            Width = 256;
            Height = this.Data.Length * (this.pixelFmt == Graphic.GXTexFmt.GX_TEXFMT_PLTT16 ? 2 : 1) / 256;
            break;
        }
        return Graphic.ConvertData(this.Data, Palette.PaletteData.Data, PalNr, Width, Height, this.pixelFmt, this.characterFmt, true, false);
      }

      public Bitmap ToBitmap(byte[] Palette, int PalNr)
      {
        int Width = 0;
        int Height = 0;
        switch (this.mapingType)
        {
          case Graphic.GXOBJVRamModeChar.GX_OBJVRAMMODE_CHAR_2D:
            Width = (int) this.W * 8;
            Height = (int) this.H * 8;
            break;
          case Graphic.GXOBJVRamModeChar.GX_OBJVRAMMODE_CHAR_1D_32K:
            Width = 32;
            Height = this.Data.Length * (this.pixelFmt == Graphic.GXTexFmt.GX_TEXFMT_PLTT16 ? 2 : 1) / 32;
            break;
          case Graphic.GXOBJVRamModeChar.GX_OBJVRAMMODE_CHAR_1D_64K:
            Width = 64;
            Height = this.Data.Length * (this.pixelFmt == Graphic.GXTexFmt.GX_TEXFMT_PLTT16 ? 2 : 1) / 64;
            break;
          case Graphic.GXOBJVRamModeChar.GX_OBJVRAMMODE_CHAR_1D_128K:
            Width = 128;
            Height = this.Data.Length * (this.pixelFmt == Graphic.GXTexFmt.GX_TEXFMT_PLTT16 ? 2 : 1) / 128;
            break;
          case Graphic.GXOBJVRamModeChar.GX_OBJVRAMMODE_CHAR_1D_256K:
            Width = 256;
            Height = this.Data.Length * (this.pixelFmt == Graphic.GXTexFmt.GX_TEXFMT_PLTT16 ? 2 : 1) / 256;
            break;
        }
        return Graphic.ConvertData(this.Data, Palette, PalNr, Width, Height, this.pixelFmt, this.characterFmt, true, false);
      }

      public void Write(EndianBinaryWriter er)
      {
        this.Header.Write(er, 0);
        er.WriteUInt16(this.H);
        er.WriteUInt16(this.W);
        er.WriteUInt32((uint) this.pixelFmt);
        er.WriteUInt32((uint) this.mapingType);
        er.WriteUInt32((uint) this.characterFmt);
        er.WriteUInt32((uint) this.Data.Length);
        er.WriteUInt32(this.pRawData);
        er.WriteBytes(this.Data, 0, this.Data.Length);
      }
    }

    public class CharacterposInfoBlock
    {
      public const string Signature = "SOPC";
      public DataBlockHeader Header;
      public ushort srcPosX;
      public ushort srcPosY;
      public ushort srcW;
      public ushort srcH;

      public CharacterposInfoBlock(EndianBinaryReader er, out bool OK)
      {
        bool OK1;
        this.Header = new DataBlockHeader(er, "SOPC", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.srcPosX = er.ReadUInt16();
          this.srcPosY = er.ReadUInt16();
          this.srcW = er.ReadUInt16();
          this.srcH = er.ReadUInt16();
          OK = true;
        }
      }
    }
  }
}
