// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Sound.SWAV
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.G3D_Binary_File_Format;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.Sound
{
  public class SWAV
  {
    public const string Signature = "SWAV";
    public FileHeader.HeaderInfo Header;
    public SWAV.DataSection Data;

    public SWAV(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      bool OK;
      this.Header = new FileHeader.HeaderInfo(er, nameof (SWAV), out OK);
      if (!OK)
      {
        int num1 = (int) MessageBox.Show("Error 1");
      }
      else
      {
        this.Data = new SWAV.DataSection(er, out OK);
        if (!OK)
        {
          int num2 = (int) MessageBox.Show("Error 2");
        }
      }
      er.Close();
    }

    public SWAV(SWAV.SWAVInfo Info, byte[] Data)
    {
      this.Data = new SWAV.DataSection(Info, Data);
    }

    public byte[] Write()
    {
      MemoryStream memoryStream = new MemoryStream();
      EndianBinaryWriter er = new EndianBinaryWriter((Stream) memoryStream, Endianness.LittleEndian);
      this.Header.fileSize = 16U + this.Data.Header.size;
      this.Header.Write(er);
      this.Data.Write(er);
      byte[] array = memoryStream.ToArray();
      er.Close();
      return array;
    }

    public unsafe WAV ToWave()
    {
      if (this.Data.Info.nWaveType == (byte) 0)
        return new WAV(MKDS_Course_Modifier.Converters.Sound.SignedPCM8ToUnsigned(this.Data.Data), (uint) this.Data.Info.nSampleRate, (ushort) 8, (ushort) 1);
      if (this.Data.Info.nWaveType == (byte) 1)
        return new WAV(this.Data.Data, (uint) this.Data.Info.nSampleRate, (ushort) 16, (ushort) 1);
      byte[] Data = new byte[(this.Data.Data.Length - 4) * 4];
      MKDS_Course_Modifier.Converters.Sound.ConvertImaAdpcm((byte*) (void*) Marshal.UnsafeAddrOfPinnedArrayElement((Array) this.Data.Data, 0), this.Data.Data.Length, (byte*) (void*) Marshal.UnsafeAddrOfPinnedArrayElement((Array) Data, 0));
      return new WAV(Data, (uint) this.Data.Info.nSampleRate, (ushort) 16, (ushort) 1);
    }

    public class SWAVInfo
    {
      public byte nWaveType;
      public byte bLoop;
      public ushort nSampleRate;
      public ushort nTime;
      public ushort nLoopOffset;
      public uint nNonLoopLen;

      public SWAVInfo(EndianBinaryReader er)
      {
        this.nWaveType = er.ReadByte();
        this.bLoop = er.ReadByte();
        this.nSampleRate = er.ReadUInt16();
        this.nTime = er.ReadUInt16();
        this.nLoopOffset = er.ReadUInt16();
        this.nNonLoopLen = er.ReadUInt32();
      }

      public SWAVInfo()
      {
      }

      public void Write(EndianBinaryWriter er)
      {
        er.Write(this.nWaveType);
        er.Write(this.bLoop);
        er.Write(this.nSampleRate);
        er.Write(this.nTime);
        er.Write(this.nLoopOffset);
        er.Write(this.nNonLoopLen);
      }
    }

    public class DataSection
    {
      public const string Signature = "DATA";
      public DataBlockHeader Header;
      public SWAV.SWAVInfo Info;
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
          long position = er.BaseStream.Position;
          this.Info = new SWAV.SWAVInfo(er);
          if (this.Info.nWaveType == (byte) 2)
          {
            er.BaseStream.Position = position + 12L;
            this.Data = er.ReadBytes((int) ((long) ((int) this.Info.nLoopOffset * 4) + (long) (this.Info.nNonLoopLen * 4U) - 4L));
          }
          else
            this.Data = er.ReadBytes((int) (er.BaseStream.Length - er.BaseStream.Position));
          OK = true;
        }
      }

      public DataSection(SWAV.SWAVInfo Info, byte[] Data)
      {
        this.Header = new DataBlockHeader("DATA", (uint) (20 + Data.Length));
        this.Info = Info;
        this.Data = Data;
      }

      public void Write(EndianBinaryWriter er)
      {
        this.Header.Write(er, (int) this.Header.size);
        this.Info.Write(er);
        er.Write(this.Data, 0, this.Data.Length);
      }
    }
  }
}
