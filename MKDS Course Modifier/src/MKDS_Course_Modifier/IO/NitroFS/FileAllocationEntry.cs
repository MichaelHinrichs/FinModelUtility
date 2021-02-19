// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.IO.NitroFS.FileAllocationEntry
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.IO;

namespace MKDS_Course_Modifier.IO.NitroFS
{
  public class FileAllocationEntry
  {
    public uint fileTop;
    public uint fileBottom;

    public FileAllocationEntry(uint Offset, uint Size)
    {
      this.fileTop = Offset;
      this.fileBottom = Offset + Size;
    }

    public FileAllocationEntry(EndianBinaryReader er)
    {
      this.fileTop = er.ReadUInt32();
      this.fileBottom = er.ReadUInt32();
    }

    public void Write(EndianBinaryWriter er)
    {
      er.Write(this.fileTop);
      er.Write(this.fileBottom);
    }

    public uint fileSize
    {
      get
      {
        return this.fileBottom - this.fileTop;
      }
    }
  }
}
