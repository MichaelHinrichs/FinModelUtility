// Decompiled with JetBrains decompiler
// Type: WaveLib.WaveNative
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.Runtime.InteropServices;

namespace WaveLib
{
  internal class WaveNative
  {
    public const int MMSYSERR_NOERROR = 0;
    public const int MM_WOM_OPEN = 955;
    public const int MM_WOM_CLOSE = 956;
    public const int MM_WOM_DONE = 957;
    public const int CALLBACK_FUNCTION = 196608;
    public const int TIME_MS = 1;
    public const int TIME_SAMPLES = 2;
    public const int TIME_BYTES = 4;
    private const string mmdll = "winmm.dll";

    [DllImport("winmm.dll")]
    public static extern int waveOutGetNumDevs();

    [DllImport("winmm.dll")]
    public static extern int waveOutPrepareHeader(
      IntPtr hWaveOut,
      ref WaveNative.WaveHdr lpWaveOutHdr,
      int uSize);

    [DllImport("winmm.dll")]
    public static extern int waveOutUnprepareHeader(
      IntPtr hWaveOut,
      ref WaveNative.WaveHdr lpWaveOutHdr,
      int uSize);

    [DllImport("winmm.dll")]
    public static extern int waveOutWrite(
      IntPtr hWaveOut,
      ref WaveNative.WaveHdr lpWaveOutHdr,
      int uSize);

    [DllImport("winmm.dll")]
    public static extern int waveOutOpen(
      out IntPtr hWaveOut,
      int uDeviceID,
      WaveFormat lpFormat,
      WaveNative.WaveDelegate dwCallback,
      int dwInstance,
      int dwFlags);

    [DllImport("winmm.dll")]
    public static extern int waveOutReset(IntPtr hWaveOut);

    [DllImport("winmm.dll")]
    public static extern int waveOutClose(IntPtr hWaveOut);

    [DllImport("winmm.dll")]
    public static extern int waveOutPause(IntPtr hWaveOut);

    [DllImport("winmm.dll")]
    public static extern int waveOutRestart(IntPtr hWaveOut);

    [DllImport("winmm.dll")]
    public static extern int waveOutGetPosition(IntPtr hWaveOut, out int lpInfo, int uSize);

    [DllImport("winmm.dll")]
    public static extern int waveOutSetVolume(IntPtr hWaveOut, int dwVolume);

    [DllImport("winmm.dll")]
    public static extern int waveOutGetVolume(IntPtr hWaveOut, out int dwVolume);

    public delegate void WaveDelegate(
      IntPtr hdrvr,
      int uMsg,
      int dwUser,
      ref WaveNative.WaveHdr wavhdr,
      int dwParam2);

    public struct WaveHdr
    {
      public IntPtr lpData;
      public int dwBufferLength;
      public int dwBytesRecorded;
      public IntPtr dwUser;
      public int dwFlags;
      public int dwLoops;
      public IntPtr lpNext;
      public int reserved;
    }
  }
}
