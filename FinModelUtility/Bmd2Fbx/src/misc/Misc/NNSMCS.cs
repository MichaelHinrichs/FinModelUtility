// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Misc.NNSMCS
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Text;

namespace MKDS_Course_Modifier.Misc
{
  public class NNSMCS
  {
    private NamedPipeClientStream McsStream1;
    private FileStream McsStream2;
    private FileStream McsStream3;
    private NNSMCS.NNSMcsStreamInfo Stream1Info;
    private IntPtr handle1;

    [DllImport("nnsmcs.dll", EntryPoint = "NNS_McsOpenStream")]
    private static extern IntPtr NNSMcsPFOpenStream(ushort channel, uint flags);

    [DllImport("nnsmcs.dll", EntryPoint = "NNS_McsOpenStreamEx")]
    private static extern IntPtr NNSMcsPFOpenStreamEx(
      ushort channel,
      uint flags,
      ref NNSMCS.NNSMcsStreamInfo pStreamInfo);

    [DllImport("kernel32.dll")]
    private static extern bool PeekNamedPipe(
      IntPtr hNamedPipe,
      IntPtr lpBuffer,
      uint nBufferSize,
      IntPtr lpBytesRead,
      IntPtr lpTotalBytesAvail,
      IntPtr lpBytesLeftThisMessage);

    public NNSMCS()
    {
      this.OpenStreams();
      this.InitalizeNitroViewer();
    }

    private void OpenStreams()
    {
      this.Stream1Info = new NNSMCS.NNSMcsStreamInfo();
      this.Stream1Info.structBytes = 8U;
      this.handle1 = NNSMCS.NNSMcsPFOpenStreamEx((ushort) 19781, 0U, ref this.Stream1Info);
      this.McsStream1 = new NamedPipeClientStream(PipeDirection.InOut, false, true, new SafePipeHandle(this.handle1, true));
      this.McsStream1.Write(new byte[2], 0, 2);
      this.McsStream1.Read(new byte[4], 0, 4);
    }

    private void InitalizeNitroViewer()
    {
      this.McsStream1.Write(new byte[4]
      {
        (byte) 3,
        (byte) 0,
        (byte) 1,
        (byte) 0
      }, 0, 4);
      this.McsStream1.Write(new byte[2]
      {
        (byte) 1,
        (byte) 0
      }, 0, 2);
      this.McsStream1.Write(new byte[4]
      {
        (byte) 0,
        (byte) 5,
        (byte) 134,
        (byte) 3
      }, 0, 4);
      this.McsStream1.Write(new byte[12]
      {
        (byte) 1,
        (byte) 5,
        (byte) 0,
        (byte) 0,
        (byte) 154,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        byte.MaxValue,
        (byte) 127
      }, 0, 12);
      this.McsStream1.Write(new byte[6]
      {
        (byte) 2,
        (byte) 5,
        (byte) 5,
        (byte) 0,
        (byte) 10,
        (byte) 0
      }, 0, 6);
      this.McsStream1.Write(new byte[10]
      {
        (byte) 3,
        (byte) 5,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 0
      }, 0, 10);
      this.McsStream1.Write(new byte[4]
      {
        (byte) 4,
        (byte) 5,
        (byte) 1,
        (byte) 0
      }, 0, 4);
      this.McsStream1.Write(new byte[32]
      {
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 168,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 205,
        (byte) 100,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      }, 0, 32);
      this.McsStream1.Write(new byte[32]
      {
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 36,
        (byte) 4,
        (byte) 0,
        (byte) 0,
        (byte) 116,
        (byte) 15,
        (byte) 0,
        (byte) 0,
        (byte) 85,
        (byte) 21,
        (byte) 0,
        (byte) 0,
        (byte) 154,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        byte.MaxValue,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 16,
        (byte) 0,
        (byte) 0
      }, 0, 32);
      this.McsStream1.Write(new byte[4]
      {
        (byte) 15,
        (byte) 1,
        (byte) 0,
        (byte) 0
      }, 0, 4);
      this.McsStream1.Write(new byte[10]
      {
        (byte) 2,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 176,
        (byte) 244,
        (byte) 176,
        (byte) 244
      }, 0, 10);
      this.McsStream1.Write(new byte[6]
      {
        (byte) 3,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        byte.MaxValue,
        (byte) 127
      }, 0, 6);
      this.McsStream1.Write(new byte[10]
      {
        (byte) 2,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 77,
        (byte) 11,
        (byte) 173,
        (byte) 244,
        (byte) 0,
        (byte) 0
      }, 0, 10);
      this.McsStream1.Write(new byte[6]
      {
        (byte) 3,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        byte.MaxValue,
        (byte) 127
      }, 0, 6);
      this.McsStream1.Write(new byte[10]
      {
        (byte) 2,
        (byte) 1,
        (byte) 2,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 173,
        (byte) 244,
        (byte) 77,
        (byte) 11
      }, 0, 10);
      this.McsStream1.Write(new byte[6]
      {
        (byte) 3,
        (byte) 1,
        (byte) 2,
        (byte) 0,
        byte.MaxValue,
        (byte) 127
      }, 0, 6);
      this.McsStream1.Write(new byte[10]
      {
        (byte) 2,
        (byte) 1,
        (byte) 3,
        (byte) 0,
        (byte) 176,
        (byte) 244,
        (byte) 176,
        (byte) 244,
        (byte) 0,
        (byte) 0
      }, 0, 10);
      this.McsStream1.Write(new byte[6]
      {
        (byte) 3,
        (byte) 1,
        (byte) 3,
        (byte) 0,
        byte.MaxValue,
        (byte) 127
      }, 0, 6);
      List<byte> byteList1 = new List<byte>();
      byteList1.Add((byte) 4);
      byteList1.Add((byte) 1);
      byteList1.Add((byte) 0);
      byteList1.Add((byte) 0);
      for (int index = 0; index < 128; ++index)
        byteList1.Add((byte) (index * 2));
      this.McsStream1.Write(byteList1.ToArray(), 0, 132);
      this.McsStream1.Write(new byte[4]
      {
        (byte) 5,
        (byte) 1,
        (byte) 0,
        (byte) 0
      }, 0, 4);
      List<byte> byteList2 = new List<byte>();
      byteList2.Add((byte) 6);
      byteList2.Add((byte) 1);
      for (int index = 0; index < 32; ++index)
      {
        byteList2.Add(byte.MaxValue);
        byteList2.Add((byte) 127);
      }
      this.McsStream1.Write(byteList2.ToArray(), 0, 66);
      this.McsStream1.Write(new byte[6]
      {
        (byte) 7,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0
      }, 0, 6);
      this.McsStream1.Write(new byte[4]
      {
        (byte) 8,
        (byte) 1,
        (byte) 1,
        (byte) 0
      }, 0, 4);
      this.McsStream1.Write(new byte[4]
      {
        (byte) 16,
        (byte) 1,
        (byte) 1,
        (byte) 0
      }, 0, 4);
      byte[] buffer = new byte[8];
      buffer[0] = (byte) 17;
      buffer[1] = (byte) 1;
      this.McsStream1.Write(buffer, 0, 8);
      this.McsStream1.Write(new byte[12]
      {
        (byte) 9,
        (byte) 1,
        byte.MaxValue,
        (byte) 127,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        byte.MaxValue,
        (byte) 127,
        (byte) 0,
        (byte) 0
      }, 0, 12);
      List<byte> byteList3 = new List<byte>();
      byteList3.Add((byte) 10);
      byteList3.Add((byte) 1);
      byteList3.Add((byte) 0);
      byteList3.Add((byte) 0);
      for (int index = 0; index < 32; ++index)
        byteList3.Add((byte) (index * 4));
      this.McsStream1.Write(byteList3.ToArray(), 0, 36);
      this.SetEdgeMarking(false);
      this.McsStream1.Write(new byte[18]
      {
        (byte) 12,
        (byte) 1,
        byte.MaxValue,
        (byte) 127,
        byte.MaxValue,
        (byte) 127,
        byte.MaxValue,
        (byte) 127,
        byte.MaxValue,
        (byte) 127,
        byte.MaxValue,
        (byte) 127,
        byte.MaxValue,
        (byte) 127,
        byte.MaxValue,
        (byte) 127,
        byte.MaxValue,
        (byte) 127
      }, 0, 18);
      this.SetAntialiasing(true);
      this.McsStream1.Write(new byte[12]
      {
        (byte) 14,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 31,
        (byte) 0,
        (byte) 63,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        byte.MaxValue,
        (byte) 127
      }, 0, 12);
      this.McsStream1.Write(new byte[4]
      {
        (byte) 3,
        (byte) 0,
        (byte) 0,
        (byte) 0
      }, 0, 4);
      this.McsStream1.Write(new byte[8]
      {
        (byte) 2,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 67,
        (byte) 78,
        (byte) 89,
        (byte) 83
      }, 0, 8);
      this.McsStream1.Read(new byte[8], 0, 8);
    }

    public void SendFile(string Path)
    {
      this.McsStream1.Write(new byte[4]
      {
        (byte) 3,
        (byte) 0,
        (byte) 1,
        (byte) 0
      }, 0, 4);
      List<byte> byteList = new List<byte>();
      byteList.Add((byte) 0);
      byteList.Add((byte) 4);
      byteList.Add((byte) 0);
      byteList.Add((byte) 0);
      byte[] numArray = File.ReadAllBytes(Path);
      byteList.AddRange((IEnumerable<byte>) BitConverter.GetBytes((uint) numArray.Length));
      if (Path.Length > 256)
        throw new PathTooLongException("The maximum path length is 256. This path is: " + (object) Path.Length);
      byteList.AddRange((IEnumerable<byte>) Encoding.ASCII.GetBytes(Path));
      byteList.AddRange((IEnumerable<byte>) new byte[264 - byteList.Count]);
      this.McsStream1.Write(byteList.ToArray(), 0, 264);
      this.McsStream1.Write(new byte[104]
      {
        (byte) 24,
        (byte) 2,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 16,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 16,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 16,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 16,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 32,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 32,
        (byte) 0,
        (byte) 0
      }, 0, 104);
      this.McsStream1.Write(new byte[4]
      {
        (byte) 3,
        (byte) 0,
        (byte) 0,
        (byte) 0
      }, 0, 4);
      this.McsStream1.Write(new byte[8]
      {
        (byte) 2,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 67,
        (byte) 78,
        (byte) 89,
        (byte) 83
      }, 0, 8);
    }

    public void SetEdgeMarking(bool Enable)
    {
      this.SendData((byte) 11, (byte) 1, Enable ? (byte) 1 : (byte) 0, (byte) 0);
    }

    public void SetAntialiasing(bool Enable)
    {
      this.SendData((byte) 13, (byte) 1, Enable ? (byte) 1 : (byte) 0, (byte) 0);
    }

    private void SendData(params byte[] Data)
    {
      this.McsStream1.Write(Data, 0, Data.Length);
    }

    public void Close()
    {
      this.McsStream1.Close();
    }

    private enum NNS_MCS_DEVICE_TYPE : uint
    {
      NNS_MCS_DEVICE_TYPE_UNKNOWN,
      NNS_MCS_DEVICE_TYPE_NITRO_DEBUGGER,
      NNS_MCS_DEVICE_TYPE_NITRO_UIC,
      NNS_MCS_DEVICE_TYPE_ENSATA,
    }

    private struct NNSMcsStreamInfo
    {
      public uint structBytes;
      public NNSMCS.NNS_MCS_DEVICE_TYPE deviceType;
    }
  }
}
