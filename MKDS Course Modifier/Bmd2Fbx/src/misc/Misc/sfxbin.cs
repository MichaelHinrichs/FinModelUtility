// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Misc.sfxbin
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Sound;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MKDS_Course_Modifier.Misc
{
  public class sfxbin
  {
    public sfxbin.WaveEntry[] Waves;

    public sfxbin(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      sfxbin.WaveEntry waveEntry = new sfxbin.WaveEntry(er);
      int length = (int) waveEntry.Offset / 16;
      this.Waves = new sfxbin.WaveEntry[length];
      this.Waves[0] = waveEntry;
      for (int newSize = 1; newSize < length; ++newSize)
      {
        this.Waves[newSize] = new sfxbin.WaveEntry(er);
        if (this.Waves[newSize].Offset == 0U)
        {
          Array.Resize<sfxbin.WaveEntry>(ref this.Waves, newSize);
          break;
        }
      }
      er.Close();
    }

    public class WaveEntry
    {
      public uint Unknown;
      public uint Offset;
      public uint Length;
      public byte[] Data;

      public WaveEntry(EndianBinaryReader er)
      {
        this.Unknown = er.ReadUInt32();
        this.Offset = er.ReadUInt32();
        this.Length = er.ReadUInt32();
        int num = (int) er.ReadUInt32();
        if (this.Offset == 0U)
          return;
        long position = er.BaseStream.Position;
        er.BaseStream.Position = (long) this.Offset;
        this.Data = er.ReadBytes((int) this.Length * 4);
        er.BaseStream.Position = position;
      }

      public unsafe WAV ToWave()
      {
        byte* buf = stackalloc byte[this.Data.Length];
        byte* outbuffer = stackalloc byte[(this.Data.Length - 4) * 4];
        Marshal.Copy(this.Data, 0, (IntPtr) (void*) buf, this.Data.Length);
        MKDS_Course_Modifier.Converters.Sound.ConvertImaAdpcm(buf, this.Data.Length, outbuffer);
        byte[] numArray = new byte[(this.Data.Length - 4) * 4];
        Marshal.Copy((IntPtr) (void*) outbuffer, numArray, 0, (this.Data.Length - 4) * 4);
        return new WAV(numArray, this.Unknown / 7U, (ushort) 16, (ushort) 1);
      }
    }
  }
}
