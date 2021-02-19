// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Sound.STRM
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.G3D_Binary_File_Format;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.Sound
{
  public class STRM
  {
    public const string Signature = "STRM";
    public FileHeader.HeaderInfo Header;
    public STRM.HeadSection Head;
    public STRM.DataSection Data;

    public STRM(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      bool OK;
      this.Header = new FileHeader.HeaderInfo(er, nameof (STRM), out OK);
      if (!OK)
      {
        int num1 = (int) MessageBox.Show("Error 1");
      }
      else
      {
        this.Head = new STRM.HeadSection(er, out OK);
        if (!OK)
        {
          int num2 = (int) MessageBox.Show("Error 2");
        }
        else
        {
          this.Data = new STRM.DataSection(er, out OK);
          if (!OK)
          {
            int num3 = (int) MessageBox.Show("Error 2");
          }
        }
      }
      er.Close();
    }

    public WAV ToWave()
    {
      this.Data.Data = this.Head.NrChannel != (byte) 2 ? this.GetChannelMono(this.Data.Data, this.Head.nBlock, this.Head.nBlockLength, this.Head.nLastBlockLen, (int) this.Head.nWaveType) : this.GetChannelsStereo(this.Data.Data, this.Head.nBlock, this.Head.nBlockLength, this.Head.nLastBlockLen, (int) this.Head.nWaveType);
      return this.Head.nWaveType == (byte) 0 ? new WAV(this.Data.Data, (uint) this.Head.nSampleRate, (ushort) 8, (ushort) this.Head.NrChannel) : new WAV(this.Data.Data, (uint) this.Head.nSampleRate, (ushort) 16, (ushort) this.Head.NrChannel);
    }

    public IntPtr ToArrayPtr(out int Size)
    {
      IntPtr num = this.Head.NrChannel != (byte) 2 ? Marshal.AllocHGlobal(Size = (((int) this.Head.nBlock - 1) * (int) this.Head.nBlockSample + (int) this.Head.nLastBlockSample) * 2) : Marshal.AllocHGlobal(Size = (((int) this.Head.nBlock - 2) * (int) this.Head.nBlockSample + (int) this.Head.nLastBlockSample * 2) * 2);
      BackgroundWorker backgroundWorker = new BackgroundWorker();
      backgroundWorker.DoWork += new DoWorkEventHandler(this.b_DoWork);
      backgroundWorker.RunWorkerAsync((object) num);
      return num;
    }

    public void b_DoWork(object sender, DoWorkEventArgs e)
    {
      IntPtr num1 = (IntPtr) e.Argument;
      byte[] data = this.Data.Data;
      uint nBlock = this.Head.nBlock;
      uint nBlockLength = this.Head.nBlockLength;
      uint nLastBlockLen = this.Head.nLastBlockLen;
      int nWaveType = (int) this.Head.nWaveType;
      int nrChannel = (int) this.Head.NrChannel;
      int offset = 0;
      if (nrChannel == 2)
        return;
      int num2;
      for (num2 = 0; (long) num2 < (long) (nBlock - 1U); ++num2)
        Marshal.Copy(this.GetBlock(nWaveType, nBlockLength, data, ref offset), 0, num1 + num2 * (int) this.Head.nBlockSample * 2, (int) this.Head.nBlockSample * 2);
      Marshal.Copy(this.GetBlock(nWaveType, nBlockLength, data, ref offset), 0, num1 + num2 * (int) this.Head.nBlockSample * 2, (int) this.Head.nLastBlockSample * 2);
    }

    private byte[] GetChannelMono(
      byte[] Data,
      uint nBlock,
      uint nBlockLen,
      uint nLastBlockLen,
      int WaveType)
    {
      List<byte> byteList = new List<byte>();
      int offset = 0;
      for (int index = 0; (long) index < (long) (nBlock - 1U); ++index)
        byteList.AddRange((IEnumerable<byte>) this.GetBlock(WaveType, nBlockLen, Data, ref offset));
      byteList.AddRange((IEnumerable<byte>) this.GetBlock(WaveType, nLastBlockLen, Data, ref offset));
      return byteList.ToArray();
    }

    private byte[] GetChannelsStereo(
      byte[] Data,
      uint nBlock,
      uint nBlockLen,
      uint nLastBlockLen,
      int WaveType)
    {
      List<byte> byteList1 = new List<byte>();
      List<byte> byteList2 = new List<byte>();
      int offset = 0;
      for (int index = 0; (long) index < (long) (nBlock - 1U); ++index)
      {
        byteList1.AddRange((IEnumerable<byte>) this.GetBlock(WaveType, nBlockLen, Data, ref offset));
        byteList2.AddRange((IEnumerable<byte>) this.GetBlock(WaveType, nBlockLen, Data, ref offset));
      }
      byteList1.AddRange((IEnumerable<byte>) this.GetBlock(WaveType, nLastBlockLen, Data, ref offset));
      byteList2.AddRange((IEnumerable<byte>) this.GetBlock(WaveType, nLastBlockLen, Data, ref offset));
      return this.CombineStereo(byteList1.ToArray(), byteList2.ToArray(), WaveType);
    }

    private unsafe byte[] GetBlock(int WaveType, uint nBlockLen, byte[] Data, ref int offset)
    {
      byte[] destination;
      switch (WaveType)
      {
        case 1:
          destination = ((IEnumerable<byte>) Data).ToList<byte>().GetRange(offset, (int) nBlockLen).ToArray();
          break;
        case 2:
          byte* buf = stackalloc byte[(int) nBlockLen];
          byte* outbuffer = stackalloc byte[((int) nBlockLen - 4) * 4];
          Marshal.Copy(Data, offset, (IntPtr) (void*) buf, (int) nBlockLen);
          MKDS_Course_Modifier.Converters.Sound.ConvertImaAdpcm(buf, (int) nBlockLen, outbuffer);
          destination = new byte[((int) nBlockLen - 4) * 4];
          Marshal.Copy((IntPtr) (void*) outbuffer, destination, 0, ((int) nBlockLen - 4) * 4);
          break;
        default:
          destination = MKDS_Course_Modifier.Converters.Sound.SignedPCM8ToUnsigned(((IEnumerable<byte>) Data).ToList<byte>().GetRange(offset, (int) nBlockLen).ToArray());
          break;
      }
      offset += (int) nBlockLen;
      return destination;
    }

    private byte[] CombineStereo(byte[] Left, byte[] Right, int WaveType)
    {
      List<byte> byteList = new List<byte>();
      if (WaveType > 0)
      {
        for (int index = 0; index < Left.Length; index += 2)
        {
          byteList.Add(Left[index]);
          byteList.Add(Left[index + 1]);
          byteList.Add(Right[index]);
          byteList.Add(Right[index + 1]);
        }
      }
      else
      {
        for (int index = 0; index < Left.Length; ++index)
        {
          byteList.Add(Left[index]);
          byteList.Add(Right[index]);
        }
      }
      return byteList.ToArray();
    }

    public class HeadSection
    {
      public const string Signature = "HEAD";
      public DataBlockHeader Header;
      public byte nWaveType;
      public byte bLoop;
      public byte NrChannel;
      public byte Unknown;
      public ushort nSampleRate;
      public ushort nTime;
      public uint nLoopOffset;
      public uint nSample;
      public uint nDataOffset;
      public uint nBlock;
      public uint nBlockLength;
      public uint nBlockSample;
      public uint nLastBlockLen;
      public uint nLastBlockSample;

      public HeadSection(EndianBinaryReader er, out bool OK)
      {
        bool OK1;
        this.Header = new DataBlockHeader(er, "HEAD", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          long position = er.BaseStream.Position;
          this.nWaveType = er.ReadByte();
          this.bLoop = er.ReadByte();
          this.NrChannel = er.ReadByte();
          this.Unknown = er.ReadByte();
          this.nSampleRate = er.ReadUInt16();
          this.nTime = er.ReadUInt16();
          this.nLoopOffset = er.ReadUInt32();
          this.nSample = er.ReadUInt32();
          this.nDataOffset = er.ReadUInt32();
          this.nBlock = er.ReadUInt32();
          this.nBlockLength = er.ReadUInt32();
          this.nBlockSample = er.ReadUInt32();
          this.nLastBlockLen = er.ReadUInt32();
          this.nLastBlockSample = er.ReadUInt32();
          er.ReadBytes(32);
          OK = true;
        }
      }
    }

    public class DataSection
    {
      public const string Signature = "DATA";
      public DataBlockHeader Header;
      public byte[] Data;

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
          this.Data = er.ReadBytes((int) this.Header.size - 8);
          OK = true;
        }
      }
    }
  }
}
