// Decompiled with JetBrains decompiler
// Type: WaveLib.WaveOutBuffer
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace WaveLib
{
  internal class WaveOutBuffer : IDisposable
  {
    private AutoResetEvent m_PlayEvent = new AutoResetEvent(false);
    public WaveOutBuffer NextBuffer;
    private IntPtr m_WaveOut;
    private WaveNative.WaveHdr m_Header;
    private byte[] m_HeaderData;
    private GCHandle m_HeaderHandle;
    private GCHandle m_HeaderDataHandle;
    private bool m_Playing;

    internal static void WaveOutProc(
      IntPtr hdrvr,
      int uMsg,
      int dwUser,
      ref WaveNative.WaveHdr wavhdr,
      int dwParam2)
    {
      if (uMsg != 957)
        return;
      try
      {
        ((WaveOutBuffer) ((GCHandle) wavhdr.dwUser).Target).OnCompleted();
      }
      catch
      {
      }
    }

    public WaveOutBuffer(IntPtr waveOutHandle, int size)
    {
      this.m_WaveOut = waveOutHandle;
      this.m_HeaderHandle = GCHandle.Alloc((object) this.m_Header, GCHandleType.Pinned);
      this.m_Header.dwUser = (IntPtr) GCHandle.Alloc((object) this);
      this.m_HeaderData = new byte[size];
      this.m_HeaderDataHandle = GCHandle.Alloc((object) this.m_HeaderData, GCHandleType.Pinned);
      this.m_Header.lpData = this.m_HeaderDataHandle.AddrOfPinnedObject();
      this.m_Header.dwBufferLength = size;
      WaveOutHelper.Try(WaveNative.waveOutPrepareHeader(this.m_WaveOut, ref this.m_Header, Marshal.SizeOf((object) this.m_Header)));
    }

    ~WaveOutBuffer()
    {
      this.Dispose();
    }

    public void Dispose()
    {
      if (this.m_Header.lpData != IntPtr.Zero)
      {
        WaveNative.waveOutUnprepareHeader(this.m_WaveOut, ref this.m_Header, Marshal.SizeOf((object) this.m_Header));
        this.m_HeaderHandle.Free();
        this.m_Header.lpData = IntPtr.Zero;
      }
      this.m_PlayEvent.Close();
      if (this.m_HeaderDataHandle.IsAllocated)
        this.m_HeaderDataHandle.Free();
      GC.SuppressFinalize((object) this);
    }

    public int Size
    {
      get
      {
        return this.m_Header.dwBufferLength;
      }
    }

    public IntPtr Data
    {
      get
      {
        return this.m_Header.lpData;
      }
    }

    public bool Play()
    {
      lock (this)
      {
        this.m_PlayEvent.Reset();
        this.m_Playing = WaveNative.waveOutWrite(this.m_WaveOut, ref this.m_Header, Marshal.SizeOf((object) this.m_Header)) == 0;
        return this.m_Playing;
      }
    }

    public void WaitFor()
    {
      if (this.m_Playing)
        this.m_Playing = this.m_PlayEvent.WaitOne();
      else
        Thread.Sleep(0);
    }

    public void OnCompleted()
    {
      this.m_PlayEvent.Set();
      this.m_Playing = false;
    }
  }
}
