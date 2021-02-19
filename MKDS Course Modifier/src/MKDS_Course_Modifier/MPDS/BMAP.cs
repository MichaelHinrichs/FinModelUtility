// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.MPDS.BMAP
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.IO;

namespace MKDS_Course_Modifier.MPDS
{
  public class BMAP
  {
    public uint NrFiles;
    public BMAP.FileEntry[] Files;

    public BMAP(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      this.NrFiles = er.ReadUInt32();
      if ((int) er.ReadUInt32() == (int) this.NrFiles * 8)
      {
        er.BaseStream.Position -= 4L;
        this.Files = new BMAP.FileEntry[this.NrFiles];
        for (int index = 0; (long) index < (long) this.NrFiles; ++index)
          this.Files[index] = new BMAP.FileEntry(er, true);
        for (int index = 0; (long) index < (long) this.NrFiles; ++index)
        {
          er.BaseStream.Position = (long) (this.Files[index].Offset + 4U);
          this.Files[index].Data = er.ReadBytes((int) this.Files[index].Size);
        }
      }
      else
      {
        er.BaseStream.Position -= 4L;
        this.Files = new BMAP.FileEntry[this.NrFiles];
        for (int index = 0; (long) index < (long) this.NrFiles; ++index)
          this.Files[index] = new BMAP.FileEntry(er, false);
        for (int index = 0; (long) index < (long) this.NrFiles; ++index)
        {
          er.BaseStream.Position = (long) this.Files[index].Offset;
          this.Files[index].Data = (long) index == (long) (this.NrFiles - 1U) ? er.ReadBytes((int) er.BaseStream.Length - 2 - (int) this.Files[index].Offset) : er.ReadBytes((int) this.Files[index + 1].Offset - 2 - (int) this.Files[index].Offset);
        }
      }
      er.Close();
    }

    public class FileEntry
    {
      public uint Offset;
      public uint Size;
      public byte[] Data;

      public FileEntry(EndianBinaryReader er, bool HasSize)
      {
        this.Offset = er.ReadUInt32();
        if (!HasSize)
          return;
        this.Size = er.ReadUInt32();
      }
    }
  }
}
