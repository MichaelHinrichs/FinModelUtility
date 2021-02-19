// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Sound.SWAR
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.G3D_Binary_File_Format;
using System;
using System.IO;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.Sound
{
  public class SWAR
  {
    public const string Signature = "SWAR";
    public FileHeader.HeaderInfo Header;
    public SWAR.DataSection Data;

    public SWAR(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      bool OK;
      this.Header = new FileHeader.HeaderInfo(er, nameof (SWAR), out OK);
      if (!OK)
      {
        int num1 = (int) MessageBox.Show("Error 1");
      }
      else
      {
        this.Data = new SWAR.DataSection(er, out OK);
        if (!OK)
        {
          int num2 = (int) MessageBox.Show("Error 2");
        }
      }
      er.Close();
    }

    private SWAV ToWave(int idx)
    {
      return new SWAV(this.Data.SamplesInfo[idx], this.Data.SampleData[idx]);
    }

    public SWAV this[int i]
    {
      get
      {
        return this.ToWave(i);
      }
    }

    public class DataSection
    {
      public const string Signature = "DATA";
      public DataBlockHeader Header;
      public uint nSample;
      public uint[] Offsets;
      public SWAV.SWAVInfo[] SamplesInfo;
      public byte[][] SampleData;

      public DataSection(EndianBinaryReader er, out bool OK)
      {
        bool OK1;
        this.Header = new DataBlockHeader(er, "DATA", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          er.ReadBytes(32);
          this.nSample = er.ReadUInt32();
          this.Offsets = er.ReadUInt32s((int) this.nSample);
          this.SamplesInfo = new SWAV.SWAVInfo[(IntPtr) this.nSample];
          this.SampleData = new byte[(IntPtr) this.nSample][];
          for (int index = 0; (long) index < (long) this.nSample; ++index)
          {
            this.SamplesInfo[index] = new SWAV.SWAVInfo(er);
            this.SampleData[index] = index >= this.Offsets.Length - 1 ? er.ReadBytes((int) (er.BaseStream.Length - (long) this.Offsets[index]) - 12) : er.ReadBytes((int) this.Offsets[index + 1] - (int) this.Offsets[index] - 12);
          }
          OK = true;
        }
      }
    }
  }
}
