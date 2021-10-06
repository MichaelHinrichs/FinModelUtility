// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Sound.WAV
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.G3D_Binary_File_Format;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.Sound
{
  public class WAV
  {
    public const string Signature = "RIFF";
    public DataBlockHeader Header;
    public WAV.WaveData Wave;

    public WAV(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      bool OK;
      this.Header = new DataBlockHeader(er, "RIFF", out OK);
      if (!OK)
      {
        int num1 = (int) MessageBox.Show("Error 1");
      }
      else
      {
        this.Wave = new WAV.WaveData(er, out OK);
        if (!OK)
        {
          int num2 = (int) MessageBox.Show("Error 2");
        }
      }
      er.Close();
    }

    public byte[] Write()
    {
      MemoryStream memoryStream = new MemoryStream();
      EndianBinaryWriter er = new EndianBinaryWriter((Stream) memoryStream, Endianness.LittleEndian);
      this.Header.Write(er, (int) this.Header.size);
      this.Wave.Write(er);
      byte[] array = memoryStream.ToArray();
      er.Close();
      return array;
    }

    public WAV(byte[] Data, uint SampleRate, ushort BitsPerSample, ushort NrChannel)
    {
      this.Header = new DataBlockHeader("RIFF", (uint) (36 + Data.Length));
      this.Wave = new WAV.WaveData(Data, SampleRate, BitsPerSample, NrChannel);
    }

    public class WaveData
    {
      public const string Signature = "WAVE";
      public string Type;
      public WAV.WaveData.FMTBlock FMT;
      public WAV.WaveData.DATABlock DATA;

      public WaveData(EndianBinaryReader er, out bool OK)
      {
        this.Type = er.ReadString(Encoding.ASCII, 4);
        if (this.Type != "WAVE")
        {
          OK = false;
        }
        else
        {
          bool OK1;
          this.FMT = new WAV.WaveData.FMTBlock(er, out OK1);
          if (!OK1)
          {
            OK = false;
          }
          else
          {
            this.DATA = new WAV.WaveData.DATABlock(er, out OK1);
            if (!OK1)
              OK = false;
            else
              OK = true;
          }
        }
      }

      public void Write(EndianBinaryWriter er)
      {
        er.Write("WAVE", Encoding.ASCII, false);
        this.FMT.Write(er);
        this.DATA.Write(er);
      }

      public WaveData(byte[] Data, uint SampleRate, ushort BitsPerSample, ushort NrChannel)
      {
        this.Type = "WAVE";
        this.FMT = new WAV.WaveData.FMTBlock(SampleRate, BitsPerSample, NrChannel);
        this.DATA = new WAV.WaveData.DATABlock(Data);
      }

      public class FMTBlock
      {
        public const string Signature = "fmt ";
        public DataBlockHeader Header;
        public WAV.WaveData.FMTBlock.WaveFormat AudioFormat;
        public ushort NrChannel;
        public uint SampleRate;
        public uint ByteRate;
        public ushort BlockAlign;
        public ushort BitsPerSample;

        public FMTBlock(EndianBinaryReader er, out bool OK)
        {
          bool OK1;
          this.Header = new DataBlockHeader(er, "fmt ", out OK1);
          if (!OK1)
          {
            OK = false;
          }
          else
          {
            this.AudioFormat = (WAV.WaveData.FMTBlock.WaveFormat) er.ReadUInt16();
            this.NrChannel = er.ReadUInt16();
            this.SampleRate = er.ReadUInt32();
            this.ByteRate = er.ReadUInt32();
            this.BlockAlign = er.ReadUInt16();
            this.BitsPerSample = er.ReadUInt16();
            OK = true;
          }
        }

        public void Write(EndianBinaryWriter er)
        {
          this.Header.Write(er, (int) this.Header.size);
          er.Write((ushort) this.AudioFormat);
          er.Write(this.NrChannel);
          er.Write(this.SampleRate);
          er.Write(this.ByteRate);
          er.Write(this.BlockAlign);
          er.Write(this.BitsPerSample);
        }

        public FMTBlock(uint SampleRate, ushort BitsPerSample, ushort NrChannel)
        {
          this.Header = new DataBlockHeader("fmt ", 16U);
          this.AudioFormat = WAV.WaveData.FMTBlock.WaveFormat.WAVE_FORMAT_PCM;
          this.NrChannel = NrChannel;
          this.SampleRate = SampleRate;
          this.BitsPerSample = BitsPerSample;
          this.ByteRate = SampleRate * (uint) BitsPerSample * (uint) NrChannel / 8U;
          this.BlockAlign = (ushort) ((int) NrChannel * (int) BitsPerSample / 8);
        }

        public enum WaveFormat : ushort
        {
          WAVE_FORMAT_PCM = 1,
          IBM_FORMAT_ADPCM = 2,
          IBM_FORMAT_ALAW = 6,
          IBM_FORMAT_MULAW = 7,
          WAVE_FORMAT_EXTENSIBLE = 65534, // 0xFFFE
        }
      }

      public class DATABlock
      {
        public const string Signature = "data";
        public DataBlockHeader Header;
        public byte[] Data;

        public DATABlock(EndianBinaryReader er, out bool OK)
        {
          bool OK1;
          this.Header = new DataBlockHeader(er, "data", out OK1);
          if (!OK1)
          {
            OK = false;
          }
          else
          {
            this.Data = er.ReadBytes((int) this.Header.size);
            OK = true;
          }
        }

        public DATABlock(byte[] Data)
        {
          this.Header = new DataBlockHeader("data", (uint) Data.Length);
          this.Data = Data;
        }

        public void Write(EndianBinaryWriter er)
        {
          this.Header.Write(er, this.Data.Length);
          er.Write(this.Data, 0, this.Data.Length);
        }
      }
    }
  }
}
