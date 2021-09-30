// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.G3D_Binary_File_Format.AnmHeader
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.IO;
using System.Text;

namespace bmd.G3D_Binary_File_Format
{
  public class AnmHeader
  {
    public string category0;
    public byte revision;
    public string category1;

    public AnmHeader(
      EndianBinaryReader er,
      AnmHeader.Category0 category0,
      AnmHeader.Category1 category1,
      out bool OK)
    {
      this.category0 = er.ReadString(Encoding.ASCII, 1);
      if (this.category0 != category0.ToString())
      {
        OK = false;
      }
      else
      {
        this.revision = er.ReadByte();
        this.category1 = er.ReadString(Encoding.ASCII, 2);
        if (this.category1 != category1.ToString())
          OK = false;
        else
          OK = true;
      }
    }

    public enum Category0
    {
      M,
      J,
      V,
    }

    public enum Category1
    {
      AC,
      AV,
      AM,
      PT,
      AT,
    }
  }
}
