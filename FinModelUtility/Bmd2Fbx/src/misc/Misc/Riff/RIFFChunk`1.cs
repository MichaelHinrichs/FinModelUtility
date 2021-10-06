// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Misc.Riff.RIFFChunk`1
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.IO;
using System.Text;

namespace MKDS_Course_Modifier.Misc.Riff
{
  public class RIFFChunk<T> where T : RIFFChunkData, new()
  {
    public RIFFChunk<T>.RIFFChunkHeader Header;
    public T Data;

    public RIFFChunk(EndianBinaryReader er, out bool OK)
    {
      this.Data = new T();
      bool OK1;
      this.Header = new RIFFChunk<T>.RIFFChunkHeader(er, this.Data.GetSignature(), out OK1);
      if (!OK1)
      {
        OK = false;
      }
      else
      {
        this.Data.Read(er);
        OK = true;
      }
    }

    public class RIFFChunkHeader
    {
      public string DataSignature;
      public uint SectionSize;

      public RIFFChunkHeader(EndianBinaryReader er, string Signature, out bool OK)
      {
        this.DataSignature = er.ReadString(Encoding.ASCII, 4);
        if (this.DataSignature != Signature)
        {
          OK = false;
        }
        else
        {
          this.SectionSize = er.ReadUInt32();
          OK = true;
        }
      }
    }
  }
}
