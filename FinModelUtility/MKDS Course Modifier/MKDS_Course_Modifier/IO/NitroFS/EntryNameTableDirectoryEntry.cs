// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.IO.NitroFS.EntryNameTableDirectoryEntry
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.IO;
using System.Text;

namespace MKDS_Course_Modifier.IO.NitroFS
{
  public class EntryNameTableDirectoryEntry : EntryNameTableEntry
  {
    public string entryName;
    public ushort directoryID;

    public EntryNameTableDirectoryEntry(string Name, ushort DirectoryID)
    {
      this.entryNameLength = (byte) (Name.Length | 128);
      this.entryName = Name;
      this.directoryID = DirectoryID;
    }

    public EntryNameTableDirectoryEntry(EndianBinaryReader er)
      : base(er)
    {
      this.entryName = er.ReadString(Encoding.ASCII, (int) this.entryNameLength & (int) sbyte.MaxValue);
      this.directoryID = er.ReadUInt16();
    }

    public override void Write(EndianBinaryWriter er)
    {
      base.Write(er);
      er.Write(this.entryName, Encoding.ASCII, false);
      er.Write(this.directoryID);
    }
  }
}
