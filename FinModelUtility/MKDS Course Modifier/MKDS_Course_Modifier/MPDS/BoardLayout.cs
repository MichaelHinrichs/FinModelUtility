// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.MPDS.BoardLayout
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.IO;

namespace MKDS_Course_Modifier.MPDS
{
  public class BoardLayout
  {
    public uint NrObjects;
    public BoardLayout.ObjectEntry[] Entries;

    public BoardLayout(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      this.NrObjects = er.ReadUInt32();
      this.Entries = new BoardLayout.ObjectEntry[this.NrObjects];
      for (int index = 0; (long) index < (long) this.NrObjects; ++index)
        this.Entries[index] = new BoardLayout.ObjectEntry(er);
      er.Close();
    }

    public class ObjectEntry
    {
      public uint Unknown1;
      public byte Index;
      public byte ObjectID;
      public short X;
      public short Y;
      public short Z;
      public byte Unknown2;
      public byte Unknown3;
      public byte Unknown4;
      public byte Unknown5;

      public ObjectEntry(EndianBinaryReader er)
      {
        this.Unknown1 = er.ReadUInt32();
        this.Index = er.ReadByte();
        this.ObjectID = er.ReadByte();
        this.X = er.ReadInt16();
        this.Y = er.ReadInt16();
        this.Z = er.ReadInt16();
        this.Unknown2 = er.ReadByte();
        this.Unknown3 = er.ReadByte();
        this.Unknown4 = er.ReadByte();
        this.Unknown5 = er.ReadByte();
      }
    }
  }
}
