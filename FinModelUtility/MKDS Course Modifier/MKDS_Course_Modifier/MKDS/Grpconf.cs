// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.MKDS.Grpconf
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.IO;

namespace MKDS_Course_Modifier.MKDS
{
  public class Grpconf
  {
    public Grpconf.GrpconfEntry[] Objects;

    public Grpconf(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      this.Objects = new Grpconf.GrpconfEntry[file.Length / 16];
      for (int index = 0; index < file.Length / 16; ++index)
        this.Objects[index] = new Grpconf.GrpconfEntry(er);
      er.Close();
    }

    public class GrpconfEntry
    {
      public ushort ObjectID;
      public Grpconf.GrpconfEntry.Model Has3DModel;
      public ushort Unknown1;
      public ushort Unknown2;
      public ushort Unknown3;
      public ushort Unknown4;
      public ushort Unknown5;
      public ushort Unknown6;

      public GrpconfEntry(EndianBinaryReader er)
      {
        this.ObjectID = er.ReadUInt16();
        this.Has3DModel = (Grpconf.GrpconfEntry.Model) er.ReadUInt16();
        this.Unknown1 = er.ReadUInt16();
        this.Unknown2 = er.ReadUInt16();
        this.Unknown3 = er.ReadUInt16();
        this.Unknown4 = er.ReadUInt16();
        this.Unknown5 = er.ReadUInt16();
        this.Unknown6 = er.ReadUInt16();
      }

      public override string ToString()
      {
        ObjectDb.Object @object = MKDS_Const.ObjectDatabase.GetObject(this.ObjectID);
        return @object != null ? @object.ToString() : BitConverter.ToString(BitConverter.GetBytes(this.ObjectID)).Replace("-", "");
      }

      public enum Model : ushort
      {
        None,
        _2D,
        _3D,
      }
    }
  }
}
