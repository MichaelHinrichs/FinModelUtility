// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.G3D_Binary_File_Format.DataBlockHeader
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.IO;
using System.Text;

namespace bmd.G3D_Binary_File_Format
{
  public class DataBlockHeader
  {
    public string kind;
    public uint size;

    public DataBlockHeader(EndianBinaryReader er, string Signature, out bool OK)
    {
      this.kind = er.ReadString(Encoding.ASCII, 4);
      if (this.kind != Signature)
      {
        OK = false;
      }
      else
      {
        this.size = er.ReadUInt32();
        OK = true;
      }
    }

    public DataBlockHeader(string kind, uint size)
    {
      this.kind = kind;
      this.size = size;
    }

    public void Write(EndianBinaryWriter er, int Size)
    {
      er.WriteString(this.kind);
      er.WriteInt32(Size);
    }
  }
}
