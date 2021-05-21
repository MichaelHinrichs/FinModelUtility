// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Misc.Riff.RIFF`1
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.Misc.Riff
{
  public class RIFF<T> where T : RIFFData, new()
  {
    public RIFF<T>.RIFFHeader Header;
    public T Data;

    public RIFF(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      this.Data = new T();
      bool OK;
      this.Header = new RIFF<T>.RIFFHeader(er, this.Data.GetSignature(), out OK);
      if (!OK)
      {
        int num = (int) MessageBox.Show("Error 1");
      }
      else
        this.Data.Read(er);
      er.Close();
    }

    public class RIFFHeader
    {
      public string Signature;
      public uint FileSize;
      public string DataSignature;

      public RIFFHeader(EndianBinaryReader er, string Signature, out bool OK)
      {
        this.Signature = er.ReadString(Encoding.ASCII, 4);
        if (this.Signature != nameof (RIFF<T>))
        {
          OK = false;
        }
        else
        {
          this.FileSize = er.ReadUInt32();
          this.DataSignature = er.ReadString(Encoding.ASCII, 4);
          if (this.DataSignature != Signature)
            OK = false;
          else
            OK = true;
        }
      }
    }
  }
}
