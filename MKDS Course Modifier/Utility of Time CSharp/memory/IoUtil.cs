using System;
using System.Collections.Generic;

using UoT.util.array;

namespace UoT {
  public static class IoUtil {
    public static void SplitAddress(uint address, out byte bank, out uint offset) {
      bank = (byte) (address >> 24);
      offset = address << 8 >> 8;
    }

    public static uint MergeAddress(byte bank, uint offset) {
      return (uint) ((bank << 24) | (offset & 0x00ffffff));
    }


    // TODO: Rename params.
    /// <summary>
    ///   Gets multiple bits from the right-hand side of a value.
    /// </summary>
    public static uint ShiftR(uint v, int s, int w)
      => (uint)((v >> s) & ((1 << w) - 1));

    // TODO: What is this doing?
    // TODO: Rename params.
    public static uint ShiftL(uint v, int s, int w)
      => (uint)(v & ((1 << w) - 1) << s);

    // TODO: What is this doing?
    // TODO: Rename params.
    public static double Fixed2Float(double v, int b)
      => v * RDP_Defs.FIXED2FLOATRECIP[b - 1];


    private static readonly short[] int16Buffer_ = new short[1];

    public static short ReadInt16(IList<byte> buffer, uint offset) {
      var bytes = new byte[2];
      bytes[0] = buffer[(int)offset + 1];
      bytes[1] = buffer[(int)offset];

      Buffer.BlockCopy(bytes, 0, IoUtil.int16Buffer_, 0, 2);
      return IoUtil.int16Buffer_[0];
    }

    public static ushort ReadUInt16(IList<byte> buffer, uint offset)
      => (ushort)IoUtil.ReadUInt(buffer, offset, 2);

    public static uint ReadUInt24(IList<byte> buffer, uint offset)
      => IoUtil.ReadUInt(buffer, offset, 3);

    public static uint ReadUInt32(IList<byte> buffer, uint offset)
      => IoUtil.ReadUInt(buffer, offset, 4);

    private static uint ReadUInt(IList<byte> buffer, uint offset, int byteNum) {
      uint total = 0;
      for (var i = 0; i < byteNum; ++i) {
        var hexIndex = 2 * (byteNum - 1 - i);
        var bitIndex = 4 * hexIndex;
        var byteFactor = (uint)(1 << bitIndex);
        total += buffer[(int) (offset + i)] * byteFactor;
      }
      return total;
    }


    public static void WriteInt16(IList<byte> buffer, ref int offset, ushort data)
      => IoUtil.WriteInt(buffer, data, ref offset, 2);

    public static void WriteInt24(IList<byte> buffer, uint data, ref int offset)
      => IoUtil.WriteInt(buffer, data, ref offset, 3);

    public static void WriteInt32(IList<byte> buffer, uint data, ref int offset)
      => IoUtil.WriteInt(buffer, data, ref offset, 4);

    private static void WriteInt(IList<byte> buffer, uint data, ref int offset, int byteNum) {
      // TODO: Add this auto-extending logic back in?
      /*if (offset >= buffer.Length - 1) {
        Array.Resize(ref buffer, (int) (offset + byteNum + 1));
      }*/

      for (var i = 0; i < byteNum; ++i) {
        var shift = (byte) ((byteNum - 1 - i) * 8);
        buffer[offset + i] = (byte)(data >> shift & 0xFFL);
      }

      offset += byteNum;
    }
  }
}
