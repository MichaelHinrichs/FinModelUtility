// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.IO.NitroFS.EntryNameTableEndOfDirectoryEntry
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.IO;

namespace MKDS_Course_Modifier.IO.NitroFS
{
  public class EntryNameTableEndOfDirectoryEntry : EntryNameTableEntry
  {
    public EntryNameTableEndOfDirectoryEntry()
    {
    }

    public EntryNameTableEndOfDirectoryEntry(EndianBinaryReader er)
      : base(er)
    {
    }

    public override void Write(EndianBinaryWriter er)
    {
      base.Write(er);
    }
  }
}
