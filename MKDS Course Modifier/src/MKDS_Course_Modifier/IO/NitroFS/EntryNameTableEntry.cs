// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.IO.NitroFS.EntryNameTableEntry
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.IO;

namespace MKDS_Course_Modifier.IO.NitroFS
{
  public class EntryNameTableEntry
  {
    public byte entryNameLength;

    protected EntryNameTableEntry()
    {
    }

    public EntryNameTableEntry(EndianBinaryReader er)
    {
      this.entryNameLength = er.ReadByte();
    }

    public virtual void Write(EndianBinaryWriter er)
    {
      er.Write(this.entryNameLength);
    }
  }
}
