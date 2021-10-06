// Decompiled with JetBrains decompiler
// Type: WaveLib.WaveOutPlayer
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace WaveLib
{
  public class WaveOutPlayer : IDisposable
  {
    private WaveNative.WaveDelegate m_BufferProc = new WaveNative.WaveDelegate(WaveOutBuffer.WaveOutProc);
    private IntPtr m_WaveOut;
    private WaveOutBuffer m_Buffers;
    private WaveOutBuffer m_CurrentBuffer;
    private Thread m_Thread;
    private BufferFillEventHandler m_FillProc;
    private bool m_Finished;
    private byte m_zero;

    public static int DeviceCount
    {
      get
      {
        return WaveNative.waveOutGetNumDevs();
      }
    }

    public WaveOutPlayer(
      int device,
      WaveFormat format,
      int bufferSize,
      int bufferCount,
      BufferFillEventHandler fillProc)
    {
      this.m_zero = format.wBitsPerSample == (short) 8 ? (byte) 128 : (byte) 0;
      this.m_FillProc = fillProc;
      WaveOutHelper.Try(WaveNative.waveOutOpen(out this.m_WaveOut, device, format, this.m_BufferProc, 0, 196608));
      this.AllocateBuffers(bufferSize, bufferCount);
      this.m_Thread = new Thread(new ThreadStart(this.ThreadProc));
      this.m_Thread.Start();
    }

    ~WaveOutPlayer()
    {
      this.Dispose();
    }

    public void Dispose()
    {
      if (this.m_Thread == null)
        return;
      try
      {
        this.m_Finished = true;
        if (this.m_WaveOut != IntPtr.Zero)
          WaveNative.waveOutReset(this.m_WaveOut);
        this.m_Thread.Join();
        this.m_FillProc = (BufferFillEventHandler) null;
        this.FreeBuffers();
        if (this.m_WaveOut != IntPtr.Zero)
          WaveNative.waveOutClose(this.m_WaveOut);
      }
      finally
      {
        this.m_Thread = (Thread) null;
        this.m_WaveOut = IntPtr.Zero;
      }
    }

    private void ThreadProc()
    {
      while (!this.m_Finished)
      {
        this.Advance();
        if (this.m_FillProc != null && !this.m_Finished)
        {
          this.m_FillProc(this.m_CurrentBuffer.Data, this.m_CurrentBuffer.Size);
        }
        else
        {
          byte zero = this.m_zero;
          byte[] source = new byte[this.m_CurrentBuffer.Size];
          for (int index = 0; index < source.Length; ++index)
            source[index] = zero;
          Marshal.Copy(source, 0, this.m_CurrentBuffer.Data, source.Length);
        }
        this.m_CurrentBuffer.Play();
      }
      this.WaitForAllBuffers();
    }

    private void AllocateBuffers(int bufferSize, int bufferCount)
    {
      this.FreeBuffers();
      if (bufferCount <= 0)
        return;
      this.m_Buffers = new WaveOutBuffer(this.m_WaveOut, bufferSize);
      WaveOutBuffer waveOutBuffer1 = this.m_Buffers;
      try
      {
        for (int index = 1; index < bufferCount; ++index)
        {
          WaveOutBuffer waveOutBuffer2 = new WaveOutBuffer(this.m_WaveOut, bufferSize);
          waveOutBuffer1.NextBuffer = waveOutBuffer2;
          waveOutBuffer1 = waveOutBuffer2;
        }
      }
      finally
      {
        waveOutBuffer1.NextBuffer = this.m_Buffers;
      }
    }

    private void FreeBuffers()
    {
      this.m_CurrentBuffer = (WaveOutBuffer) null;
      if (this.m_Buffers == null)
        return;
      WaveOutBuffer buffers = this.m_Buffers;
      this.m_Buffers = (WaveOutBuffer) null;
      WaveOutBuffer waveOutBuffer = buffers;
      do
      {
        WaveOutBuffer nextBuffer = waveOutBuffer.NextBuffer;
        waveOutBuffer.Dispose();
        waveOutBuffer = nextBuffer;
      }
      while (waveOutBuffer != buffers);
    }

    private void Advance()
    {
      this.m_CurrentBuffer = this.m_CurrentBuffer == null ? this.m_Buffers : this.m_CurrentBuffer.NextBuffer;
      this.m_CurrentBuffer.WaitFor();
    }

    private void WaitForAllBuffers()
    {
      for (WaveOutBuffer waveOutBuffer = this.m_Buffers; waveOutBuffer.NextBuffer != this.m_Buffers; waveOutBuffer = waveOutBuffer.NextBuffer)
        waveOutBuffer.WaitFor();
    }
  }
}
