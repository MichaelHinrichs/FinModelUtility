// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.IO.NitroFS.DirectoryTableEntry
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.IO;

namespace MKDS_Course_Modifier.IO.NitroFS
{
  public class DirectoryTableEntry
  {
    public uint dirEntryStart;
    public ushort dirEntryFileID;
    public ushort dirParentID;

    public DirectoryTableEntry()
    {
    }

    public DirectoryTableEntry(EndianBinaryReader er)
    {
      this.dirEntryStart = er.ReadUInt32();
      this.dirEntryFileID = er.ReadUInt16();
      this.dirParentID = er.ReadUInt16();
    }

    public void Write(EndianBinaryWriter er)
    {
      er.Write(this.dirEntryStart);
      er.Write(this.dirEntryFileID);
      er.Write(this.dirParentID);
    }
  }
}
