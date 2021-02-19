// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.IO.NitroFS.EntryNameTableFileEntry
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.IO;
using System.Text;

namespace MKDS_Course_Modifier.IO.NitroFS
{
  public class EntryNameTableFileEntry : EntryNameTableEntry
  {
    public string entryName;

    public EntryNameTableFileEntry(string Name)
    {
      this.entryNameLength = (byte) Name.Length;
      this.entryName = Name;
    }

    public EntryNameTableFileEntry(EndianBinaryReader er)
      : base(er)
    {
      this.entryName = er.ReadString(Encoding.ASCII, (int) this.entryNameLength);
    }

    public override void Write(EndianBinaryWriter er)
    {
      base.Write(er);
      er.Write(this.entryName, Encoding.ASCII, false);
    }
  }
}
