// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.G3D_Binary_File_Format.FileHeader
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.IO;
using System.Text;

namespace bmd.G3D_Binary_File_Format
{
  public class FileHeader
  {
    public FileHeader.HeaderInfo info;
    public uint[] offset;

    public FileHeader(EndianBinaryReader er, string Signature, out bool OK)
    {
      bool OK1;
      this.info = new FileHeader.HeaderInfo(er, Signature, out OK1);
      if (!OK1)
      {
        OK = false;
      }
      else
      {
        this.offset = new uint[(int) this.info.dataBlocks];
        for (int index = 0; index < (int) this.info.dataBlocks; ++index)
          this.offset[index] = er.ReadUInt32();
        OK = true;
      }
    }

    public FileHeader(string Signature, ushort NrDatablocks)
    {
      this.info = new FileHeader.HeaderInfo(Signature, NrDatablocks);
      this.offset = new uint[(int) NrDatablocks];
      this.offset[0] = (uint) (16 + (int) NrDatablocks * 4);
    }

    public void Write(EndianBinaryWriter er)
    {
      this.info.Write(er);
      er.Write(new uint[(int) this.info.dataBlocks], 0, (int) this.info.dataBlocks);
    }

    public uint this[int i]
    {
      get
      {
        return this.offset[i];
      }
      set
      {
        this.offset[i] = value;
      }
    }

    public class HeaderInfo
    {
      public ushort byteOrder = 65279;
      public ushort version = 1;
      public ushort headerSize = 16;
      public string signature;
      public uint fileSize;
      public ushort dataBlocks;

      public HeaderInfo(EndianBinaryReader er, string Signature, out bool OK)
      {
        this.signature = er.ReadString(Encoding.ASCII, 4);
        if (this.signature != Signature)
        {
          OK = false;
        }
        else
        {
          this.byteOrder = er.ReadUInt16();
          this.version = er.ReadUInt16();
          this.fileSize = er.ReadUInt32();
          this.headerSize = er.ReadUInt16();
          this.dataBlocks = er.ReadUInt16();
          OK = true;
        }
      }

      public HeaderInfo(string Signature, ushort DataBlocks)
      {
        this.signature = Signature;
        this.dataBlocks = DataBlocks;
      }

      public void Write(EndianBinaryWriter er)
      {
        er.Write(this.signature, Encoding.ASCII, false);
        er.Write(this.byteOrder);
        er.Write(this.version);
        er.Write(0);
        er.Write(this.headerSize);
        er.Write(this.dataBlocks);
      }
    }
  }
}
