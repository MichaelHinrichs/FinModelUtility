// Decompiled with JetBrains decompiler
// Type: WaveLib.WaveFormat
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.Runtime.InteropServices;

namespace WaveLib
{
  [StructLayout(LayoutKind.Sequential)]
  public class WaveFormat
  {
    public short wFormatTag;
    public short nChannels;
    public int nSamplesPerSec;
    public int nAvgBytesPerSec;
    public short nBlockAlign;
    public short wBitsPerSample;
    public short cbSize;

    public WaveFormat(int rate, int bits, int channels)
    {
      this.wFormatTag = (short) 1;
      this.nChannels = (short) channels;
      this.nSamplesPerSec = rate;
      this.wBitsPerSample = (short) bits;
      this.cbSize = (short) 0;
      this.nBlockAlign = (short) (channels * (bits / 8));
      this.nAvgBytesPerSec = this.nSamplesPerSec * (int) this.nBlockAlign;
    }
  }
}
