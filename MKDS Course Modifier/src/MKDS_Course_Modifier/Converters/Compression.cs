// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Converters.Compression
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using SevenZip;
using SevenZip.Compression.LZMA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using zlib;

namespace MKDS_Course_Modifier.Converters
{
  public class Compression
  {
    public static byte[] ZLibDecompress(byte[] CompData, int DecompLength)
    {
      return MKDS_Course_Modifier.Converters.Compression.ZLibDecompress(CompData, 0, DecompLength);
    }

    public static byte[] ZLibDecompress(byte[] CompData, int Offset, int DecompLength)
    {
      byte[] buffer = new byte[CompData.Length - Offset];
      Array.Copy((Array) CompData, Offset, (Array) buffer, 0, buffer.Length);
      ZInputStream zinputStream = new ZInputStream((Stream) new MemoryStream(buffer));
      byte[] b = new byte[DecompLength];
      zinputStream.read(b, 0, DecompLength);
      zinputStream.Close();
      return b;
    }

    public static byte[] LZMADecompress(byte[] CompData)
    {
      return MKDS_Course_Modifier.Converters.Compression.LZMADecompress(CompData, 0);
    }

    public static byte[] LZMADecompress(byte[] CompData, int Offset)
    {
      byte[] buffer = new byte[CompData.Length - Offset];
      Array.Copy((Array) CompData, Offset, (Array) buffer, 0, buffer.Length);
      MemoryStream memoryStream1 = new MemoryStream(buffer);
      Decoder decoder = new Decoder();
      byte[] numArray = new byte[5];
      memoryStream1.Read(numArray, 0, 5);
      decoder.SetDecoderProperties(numArray);
      long outSize = 0;
      for (int index = 0; index < 8; ++index)
      {
        int num = memoryStream1.ReadByte();
        outSize |= (long) (byte) num << 8 * index;
      }
      long inSize = memoryStream1.Length - memoryStream1.Position;
      MemoryStream memoryStream2 = new MemoryStream();
      decoder.Code((Stream) memoryStream1, (Stream) memoryStream2, inSize, outSize, (ICodeProgress) null);
      byte[] array = memoryStream2.ToArray();
      memoryStream2.Close();
      return array;
    }

    public static byte[] LZ77Decompress(byte[] source)
    {
      int length = (int) source[1] | (int) source[2] << 8 | (int) source[3] << 16;
      byte[] numArray = new byte[length];
      int index1 = 4;
      int num1 = 0;
      while (length > 0)
      {
        byte num2 = source[index1++];
        if (num2 != (byte) 0)
        {
          for (int index2 = 0; index2 < 8; ++index2)
          {
            if (((int) num2 & 128) != 0)
            {
              int num3 = (int) source[index1] << 8 | (int) source[index1 + 1];
              index1 += 2;
              int num4 = (num3 >> 12) + 3;
              int num5 = num3 & 4095;
              int num6 = num1 - num5 - 1;
              for (int index3 = 0; index3 < num4; ++index3)
              {
                numArray[num1++] = numArray[num6++];
                --length;
                if (length == 0)
                  return numArray;
              }
            }
            else
            {
              numArray[num1++] = source[index1++];
              --length;
              if (length == 0)
                return numArray;
            }
            num2 <<= 1;
          }
        }
        else
        {
          for (int index2 = 0; index2 < 8; ++index2)
          {
            numArray[num1++] = source[index1++];
            --length;
            if (length == 0)
              return numArray;
          }
        }
      }
      return numArray;
    }

    public static byte[] LZ77DecompressHeader(byte[] source)
    {
      int length = (int) source[5] | (int) source[6] << 8 | (int) source[7] << 16;
      byte[] numArray = new byte[length];
      int index1 = 8;
      int num1 = 0;
      while (length > 0)
      {
        byte num2 = source[index1++];
        if (num2 != (byte) 0)
        {
          for (int index2 = 0; index2 < 8; ++index2)
          {
            if (((int) num2 & 128) != 0)
            {
              int num3 = (int) source[index1] << 8 | (int) source[index1 + 1];
              index1 += 2;
              int num4 = (num3 >> 12) + 3;
              int num5 = num3 & 4095;
              int num6 = num1 - num5 - 1;
              for (int index3 = 0; index3 < num4; ++index3)
              {
                numArray[num1++] = numArray[num6++];
                --length;
                if (length == 0)
                  return numArray;
              }
            }
            else
            {
              numArray[num1++] = source[index1++];
              --length;
              if (length == 0)
                return numArray;
            }
            num2 <<= 1;
          }
        }
        else
        {
          for (int index2 = 0; index2 < 8; ++index2)
          {
            numArray[num1++] = source[index1++];
            --length;
            if (length == 0)
              return numArray;
          }
        }
      }
      return numArray;
    }

    public static byte[] OverlayDecompress(byte[] sourcedata)
    {
      uint num1 = (uint) ((int) sourcedata[sourcedata.Length - 8] | (int) sourcedata[sourcedata.Length - 7] << 8 | (int) sourcedata[sourcedata.Length - 6] << 16 | (int) sourcedata[sourcedata.Length - 5] << 24);
      uint num2 = (uint) ((int) sourcedata[sourcedata.Length - 4] | (int) sourcedata[sourcedata.Length - 3] << 8 | (int) sourcedata[sourcedata.Length - 2] << 16 | (int) sourcedata[sourcedata.Length - 1] << 24);
      byte[] numArray = new byte[(long) sourcedata.Length + (long) num2];
      sourcedata.CopyTo((Array) numArray, 0);
      uint length = (uint) sourcedata.Length;
      if (length == 0U)
        return (byte[]) null;
      uint num3 = num1;
      uint num4 = num2;
      uint num5 = length + num4;
      uint num6 = length - (num3 >> 24);
      uint num7 = num3 & 16777215U;
      uint num8 = length - num7;
label_3:
      while (num6 > num8)
      {
        --num6;
        uint num9 = (uint) numArray[num6];
        uint dest1 = 8;
        do
        {
          bool N;
          bool V;
          MKDS_Course_Modifier.Converters.Compression.SubS(out dest1, dest1, 1U, out N, out V);
          if (N == V)
          {
            if (((int) num9 & 128) == 0)
            {
              --num6;
              uint num10 = (uint) numArray[num6];
              --num5;
              numArray[num5] = (byte) num10;
            }
            else
            {
              uint num10 = num6 - 1U;
              uint dest2 = (uint) numArray[num10];
              num6 = num10 - 1U;
              uint num11 = (((uint) numArray[num6] | dest2 << 8) & 4095U) + 2U;
              dest2 += 32U;
              do
              {
                uint num12 = (uint) numArray[(num5 + num11)];
                --num5;
                numArray[num5] = (byte) num12;
                MKDS_Course_Modifier.Converters.Compression.SubS(out dest2, dest2, 16U, out N, out V);
              }
              while (N == V);
            }
            num9 <<= 1;
          }
          else
            goto label_3;
        }
        while (num6 > num8);
        break;
      }
      return numArray;
    }

    private static void SubS(out uint dest, uint v1, uint v2, out bool N, out bool V)
    {
      dest = v1 - v2;
      N = ((int) dest & int.MinValue) != 0;
      V = ((int) v1 & int.MinValue) != 0 && ((int) v2 & int.MinValue) == 0 && ((int) dest & int.MinValue) == 0 || ((int) v1 & int.MinValue) == 0 && ((int) v2 & int.MinValue) != 0 && ((int) dest & int.MinValue) != 0;
    }

    public static byte[] LZ11Decompress(byte[] Infile)
    {
      int length1 = Infile.Length;
      MemoryStream memoryStream1 = new MemoryStream(Infile);
      MemoryStream memoryStream2 = new MemoryStream();
      long num1 = 0;
      byte num2 = (byte) memoryStream1.ReadByte();
      if (num2 != (byte) 17)
        throw new InvalidDataException("The provided stream is not a valid LZ-0x11 compressed stream (invalid type 0x" + num2.ToString("X") + ")");
      byte[] numArray1 = new byte[3];
      memoryStream1.Read(numArray1, 0, 3);
      int num3 = Bytes.Read3BytesAsInt24(numArray1, 0);
      long num4 = num1 + 4L;
      if (num3 == 0)
      {
        byte[] numArray2 = new byte[4];
        memoryStream1.Read(numArray2, 0, 4);
        num3 = Bytes.Read4BytesAsInt32(numArray2, 0);
        num4 += 4L;
      }
      int length2 = 4096;
      byte[] numArray3 = new byte[length2];
      int index1 = 0;
      int num5 = 0;
      int num6 = 0;
      int num7 = 1;
      while (num5 < num3)
      {
        if (num7 == 1)
        {
          if (num4 >= (long) length1)
            throw new Exception();
          num6 = memoryStream1.ReadByte();
          ++num4;
          if (num6 < 0)
            throw new Exception();
          num7 = 128;
        }
        else
          num7 >>= 1;
        if ((num6 & num7) > 0)
        {
          if (num4 >= (long) length1)
            throw new Exception();
          int num8 = memoryStream1.ReadByte();
          long num9 = num4 + 1L;
          if (num8 < 0)
            throw new Exception();
          int num10;
          int num11;
          switch (num8 >> 4)
          {
            case 0:
              if (num9 + 1L >= (long) length1)
                throw new Exception();
              int num12 = memoryStream1.ReadByte();
              long num13 = num9 + 1L;
              int num14 = memoryStream1.ReadByte();
              num4 = num13 + 1L;
              if (num14 < 0)
                throw new Exception();
              num10 = ((num8 & 15) << 4 | num12 >> 4) + 17;
              num11 = ((num12 & 15) << 8 | num14) + 1;
              break;
            case 1:
              if (num9 + 2L >= (long) length1)
                throw new Exception();
              int num15 = memoryStream1.ReadByte();
              long num16 = num9 + 1L;
              int num17 = memoryStream1.ReadByte();
              long num18 = num16 + 1L;
              int num19 = memoryStream1.ReadByte();
              num4 = num18 + 1L;
              if (num19 < 0)
                throw new Exception();
              num10 = ((num8 & 15) << 12 | num15 << 4 | num17 >> 4) + 273;
              num11 = ((num17 & 15) << 8 | num19) + 1;
              break;
            default:
              if (num9 >= (long) length1)
                throw new Exception();
              int num20 = memoryStream1.ReadByte();
              num4 = num9 + 1L;
              if (num20 < 0)
                throw new Exception();
              num10 = ((num8 & 240) >> 4) + 1;
              num11 = ((num8 & 15) << 8 | num20) + 1;
              break;
          }
          if (num11 > num5)
            throw new InvalidDataException("Cannot go back more than already written. DISP = " + (object) num11 + ", #written bytes = 0x" + num5.ToString("X") + " before 0x" + memoryStream1.Position.ToString("X") + " with indicator 0x" + (num8 >> 4).ToString("X"));
          int num21 = index1 + length2 - num11;
          for (int index2 = 0; index2 < num10; ++index2)
          {
            byte num22 = numArray3[num21 % length2];
            ++num21;
            memoryStream2.WriteByte(num22);
            numArray3[index1] = num22;
            index1 = (index1 + 1) % length2;
          }
          num5 += num10;
        }
        else
        {
          if (num4 >= (long) length1)
            throw new Exception();
          int num8 = memoryStream1.ReadByte();
          ++num4;
          if (num8 < 0)
            throw new Exception();
          memoryStream2.WriteByte((byte) num8);
          ++num5;
          numArray3[index1] = (byte) num8;
          index1 = (index1 + 1) % length2;
        }
      }
      if (num4 < (long) length1 && (num4 ^ num4 & 3L) + 4L < (long) length1)
        throw new Exception();
      return memoryStream2.ToArray();
    }

    public static unsafe byte[] LZ11Compress(byte[] infile)
    {
      long length = (long) infile.Length;
      MemoryStream memoryStream1 = new MemoryStream(infile);
      MemoryStream memoryStream2 = new MemoryStream();
      if (length > 16777215L)
        throw new Exception();
      byte[] buffer1 = new byte[length];
      if ((long) memoryStream1.Read(buffer1, 0, (int) length) != length)
        throw new Exception();
      memoryStream2.WriteByte((byte) 17);
      memoryStream2.WriteByte((byte) ((ulong) length & (ulong) byte.MaxValue));
      memoryStream2.WriteByte((byte) ((ulong) (length >> 8) & (ulong) byte.MaxValue));
      memoryStream2.WriteByte((byte) ((ulong) (length >> 16) & (ulong) byte.MaxValue));
      int num1 = 4;
      fixed (byte* numPtr = &buffer1[0])
      {
        byte[] buffer2 = new byte[33];
        buffer2[0] = (byte) 0;
        int count = 1;
        int num2 = 0;
        int val1 = 0;
        while ((long) val1 < length)
        {
          if (num2 == 8)
          {
            memoryStream2.Write(buffer2, 0, count);
            num1 += count;
            buffer2[0] = (byte) 0;
            count = 1;
            num2 = 0;
          }
          int oldLength = Math.Min(val1, 4096);
          int disp;
          int occurrenceLength = MKDS_Course_Modifier.Converters.Compression.GetOccurrenceLength(numPtr + val1, (int) Math.Min(length - (long) val1, 65808L), numPtr + val1 - oldLength, oldLength, out disp, 1);
          if (occurrenceLength < 3)
          {
            buffer2[count++] = numPtr[val1++];
          }
          else
          {
            val1 += occurrenceLength;
            buffer2[0] |= (byte) (1 << 7 - num2);
            if (occurrenceLength > 272)
            {
              buffer2[count] = (byte) 16;
              buffer2[count] |= (byte) (occurrenceLength - 273 >> 12 & 15);
              int index = count + 1;
              buffer2[index] = (byte) (occurrenceLength - 273 >> 4 & (int) byte.MaxValue);
              count = index + 1;
              buffer2[count] = (byte) (occurrenceLength - 273 << 4 & 240);
            }
            else if (occurrenceLength > 16)
            {
              buffer2[count] = (byte) 0;
              buffer2[count] |= (byte) (occurrenceLength - 273 >> 4 & 15);
              ++count;
              buffer2[count] = (byte) (occurrenceLength - 273 << 4 & 240);
            }
            else
              buffer2[count] = (byte) (occurrenceLength - 1 << 4 & 240);
            buffer2[count] |= (byte) (disp - 1 >> 8 & 15);
            int index1 = count + 1;
            buffer2[index1] = (byte) (disp - 1 & (int) byte.MaxValue);
            count = index1 + 1;
          }
          ++num2;
        }
        if (num2 > 0)
        {
          memoryStream2.Write(buffer2, 0, count);
          int num3 = num1 + count;
        }
      }
      return memoryStream2.ToArray();
    }

    private static unsafe int GetOccurrenceLength(
      byte* newPtr,
      int newLength,
      byte* oldPtr,
      int oldLength,
      out int disp,
      int minDisp = 1)
    {
      disp = 0;
      if (newLength == 0)
        return 0;
      int num1 = 0;
      for (int index1 = 0; index1 < oldLength - minDisp; ++index1)
      {
        byte* numPtr = oldPtr + index1;
        int num2 = 0;
        for (int index2 = 0; index2 < newLength && (int) numPtr[index2] == (int) newPtr[index2]; ++index2)
          ++num2;
        if (num2 > num1)
        {
          num1 = num2;
          disp = oldLength - index1;
          if (num1 == newLength)
            break;
        }
      }
      return num1;
    }

    public class MI_Compress
    {
      public static unsafe byte[] Compress(byte[] b, bool Header)
      {
        byte* dstp = stackalloc byte[5 + b.Length + b.Length / 7];
        fixed (byte* srcp = &b[0])
        {
          uint num = MKDS_Course_Modifier.Converters.Compression.MI_Compress.MI_CompressLZImpl(srcp, (uint) b.Length, dstp, false);
          byte[] destination = new byte[num];
          Marshal.Copy((IntPtr) (void*) dstp, destination, 0, (int) num);
          if (!Header)
            return destination;
          List<byte> byteList = new List<byte>();
          byteList.Add((byte) 76);
          byteList.Add((byte) 90);
          byteList.Add((byte) 55);
          byteList.Add((byte) 55);
          byteList.AddRange((IEnumerable<byte>) destination);
          return byteList.ToArray();
        }
      }

      public static unsafe byte[] FastCompress(byte[] b, bool Header)
      {
        byte* dstp = (byte*) (void*) Marshal.UnsafeAddrOfPinnedArrayElement((Array) new byte[5 + b.Length + b.Length / 7], 0);
        fixed (byte* srcp = &b[0])
        {
          byte* work = (byte*) (void*) Marshal.UnsafeAddrOfPinnedArrayElement((Array) new byte[9216], 0);
          uint num = MKDS_Course_Modifier.Converters.Compression.MI_Compress.MI_CompressLZFastImpl(srcp, (uint) b.Length, dstp, work, false);
          byte[] destination = new byte[num];
          Marshal.Copy((IntPtr) (void*) dstp, destination, 0, (int) num);
          if (!Header)
            return destination;
          List<byte> byteList = new List<byte>();
          byteList.Add((byte) 76);
          byteList.Add((byte) 90);
          byteList.Add((byte) 55);
          byteList.Add((byte) 55);
          byteList.AddRange((IEnumerable<byte>) destination);
          return byteList.ToArray();
        }
      }

      private static unsafe uint MI_CompressLZImpl(
        byte* srcp,
        uint size,
        byte* dstp,
        bool exFormat)
      {
        uint maxLength = exFormat ? 65808U : 18U;
        *(int*) dstp = (int) (uint) ((long) (uint) ((int) size << 8 | 16) | (exFormat ? 1L : 0L));
        dstp += 4;
        uint num1 = 4;
        byte* startp = srcp;
        uint num2 = size;
        while (size > 0U)
        {
          byte num3 = 0;
          byte* numPtr = dstp++;
          ++num1;
          for (byte index = 0; index < (byte) 8; ++index)
          {
            num3 <<= 1;
            if (size > 0U)
            {
              ushort num4;
              uint num5;
              if ((num5 = MKDS_Course_Modifier.Converters.Compression.MI_Compress.SearchLZ(startp, srcp, size, &num4, maxLength)) != 0U)
              {
                num3 |= (byte) 1;
                if (num1 + 2U >= num2)
                  return 0;
                uint num6;
                if (exFormat)
                {
                  if (num5 >= 273U)
                  {
                    num6 = (uint) ((int) num5 - (int) byte.MaxValue - 15 - 3);
                    *dstp++ = (byte) (16U | num6 >> 12);
                    *dstp++ = (byte) (num6 >> 4);
                    num1 += 2U;
                  }
                  else if (num5 >= 17U)
                  {
                    num6 = (uint) ((int) num5 - 15 - 2);
                    *dstp++ = (byte) (num6 >> 4);
                    ++num1;
                  }
                  else
                    num6 = num5 - 1U;
                }
                else
                  num6 = num5 - 3U;
                *dstp++ = (byte) ((ulong) (num6 << 4) | (ulong) ((int) num4 - 1 >> 8));
                *dstp++ = (byte) ((int) num4 - 1 & (int) byte.MaxValue);
                num1 += 2U;
                srcp += (int) num5;
                size -= num5;
              }
              else
              {
                if (num1 + 1U < num2)
                  ;
                *dstp++ = *srcp++;
                --size;
                ++num1;
              }
            }
          }
          *numPtr = num3;
        }
        for (byte index = 0; ((int) num1 + (int) index & 3) != 0; ++index)
          *dstp++ = (byte) 0;
        return num1;
      }

      private static unsafe uint SearchLZ(
        byte* startp,
        byte* nextp,
        uint remainSize,
        ushort* offset,
        uint maxLength)
      {
        ushort num1 = 0;
        uint num2 = 2;
        if (remainSize < 3U)
          return 0;
        byte* numPtr1 = nextp - 4096;
        if (numPtr1 < startp)
          numPtr1 = startp;
        for (; nextp - numPtr1 >= 2L; ++numPtr1)
        {
          byte* numPtr2 = nextp;
          while ((int) *numPtr2 != (int) *numPtr1 || (int) numPtr2[1] != (int) numPtr1[1] || (int) numPtr2[2] != (int) numPtr1[2])
          {
            ++numPtr1;
            if (nextp - numPtr1 < 2L)
              goto label_14;
          }
          byte* numPtr3 = numPtr1 + 3;
          byte* numPtr4 = numPtr2 + 3;
          uint num3 = 3;
          while ((uint) (numPtr4 - nextp) < remainSize && (int) *numPtr4 == (int) *numPtr3)
          {
            ++numPtr4;
            ++numPtr3;
            ++num3;
            if ((int) num3 == (int) maxLength)
              break;
          }
          if (num3 > num2)
          {
            num2 = num3;
            num1 = (ushort) (nextp - numPtr1);
            if ((int) num2 == (int) maxLength || (int) num2 == (int) remainSize)
              break;
          }
        }
label_14:
        if (num2 < 3U)
          return 0;
        *offset = num1;
        return num2;
      }

      private static unsafe void LZInitTable(
        MKDS_Course_Modifier.Converters.Compression.MI_Compress.LZCompressInfo* info,
        byte* work)
      {
        info->LZOffsetTable = (short*) work;
        info->LZByteTable = (short*) (work + 8192);
        info->LZEndTable = (short*) (work + 8192 + 512);
        for (ushort index = 0; index < (ushort) 256; ++index)
        {
          info->LZByteTable[index] = (short) -1;
          info->LZEndTable[index] = (short) -1;
        }
        info->windowPos = (ushort) 0;
        info->windowLen = (ushort) 0;
      }

      private static unsafe void SlideByte(MKDS_Course_Modifier.Converters.Compression.MI_Compress.LZCompressInfo* info, byte* srcp)
      {
        byte index1 = *srcp;
        short* lzByteTable = info->LZByteTable;
        short* lzOffsetTable = info->LZOffsetTable;
        short* lzEndTable = info->LZEndTable;
        ushort windowPos = info->windowPos;
        ushort windowLen = info->windowLen;
        ushort index2;
        if (windowLen == (ushort) 4096)
        {
          byte index3 = *(srcp - 4096);
          if ((lzByteTable[index3] = lzOffsetTable[lzByteTable[index3]]) == (short) -1)
            lzEndTable[index3] = (short) -1;
          index2 = windowPos;
        }
        else
          index2 = windowLen;
        short index4 = lzEndTable[index1];
        if (index4 == (short) -1)
          lzByteTable[index1] = (short) index2;
        else
          lzOffsetTable[index4] = (short) index2;
        lzEndTable[index1] = (short) index2;
        lzOffsetTable[index2] = (short) -1;
        if (windowLen == (ushort) 4096)
          info->windowPos = (ushort) (((int) windowPos + 1) % 4096);
        else
          ++info->windowLen;
      }

      private static unsafe void LZSlide(
        MKDS_Course_Modifier.Converters.Compression.MI_Compress.LZCompressInfo* info,
        byte* srcp,
        uint n)
      {
        for (uint index = 0; index < n; ++index)
          MKDS_Course_Modifier.Converters.Compression.MI_Compress.SlideByte(info, srcp++);
      }

      private static unsafe uint MI_CompressLZFastImpl(
        byte* srcp,
        uint size,
        byte* dstp,
        byte* work,
        bool exFormat)
      {
        uint maxLength = exFormat ? 65808U : 18U;
        *(int*) dstp = (int) (uint) ((long) (uint) ((int) size << 8 | 16) | (exFormat ? 1L : 0L));
        dstp += 4;
        uint num1 = 4;
        uint num2 = size;
        MKDS_Course_Modifier.Converters.Compression.MI_Compress.LZCompressInfo lzCompressInfo;
        MKDS_Course_Modifier.Converters.Compression.MI_Compress.LZInitTable(&lzCompressInfo, work);
        while (size > 0U)
        {
          byte num3 = 0;
          byte* numPtr = dstp++;
          ++num1;
          for (byte index = 0; index < (byte) 8; ++index)
          {
            num3 <<= 1;
            if (size > 0U)
            {
              ushort num4;
              uint n;
              if ((n = MKDS_Course_Modifier.Converters.Compression.MI_Compress.SearchLZFast(&lzCompressInfo, srcp, size, &num4, maxLength)) != 0U)
              {
                num3 |= (byte) 1;
                if (num1 + 2U < num2)
                  ;
                uint num5;
                if (exFormat)
                {
                  if (n >= 273U)
                  {
                    num5 = (uint) ((int) n - (int) byte.MaxValue - 15 - 3);
                    *dstp++ = (byte) (16U | num5 >> 12);
                    *dstp++ = (byte) (num5 >> 4);
                    num1 += 2U;
                  }
                  else if (n >= 17U)
                  {
                    num5 = (uint) ((int) n - 15 - 2);
                    *dstp++ = (byte) (num5 >> 4);
                    ++num1;
                  }
                  else
                    num5 = n - 1U;
                }
                else
                  num5 = n - 3U;
                *dstp++ = (byte) ((ulong) (num5 << 4) | (ulong) ((int) num4 - 1 >> 8));
                *dstp++ = (byte) ((int) num4 - 1 & (int) byte.MaxValue);
                num1 += 2U;
                MKDS_Course_Modifier.Converters.Compression.MI_Compress.LZSlide(&lzCompressInfo, srcp, n);
                srcp += (int) n;
                size -= n;
              }
              else
              {
                if (num1 + 1U < num2)
                  ;
                MKDS_Course_Modifier.Converters.Compression.MI_Compress.LZSlide(&lzCompressInfo, srcp, 1U);
                *dstp++ = *srcp++;
                --size;
                ++num1;
              }
            }
          }
          *numPtr = num3;
        }
        for (byte index = 0; ((int) num1 + (int) index & 3) != 0; ++index)
          *dstp++ = (byte) 0;
        return num1;
      }

      private static unsafe uint SearchLZFast(
        MKDS_Course_Modifier.Converters.Compression.MI_Compress.LZCompressInfo* info,
        byte* nextp,
        uint remainSize,
        ushort* offset,
        uint maxLength)
      {
        ushort num1 = 0;
        uint num2 = 2;
        short* lzOffsetTable = info->LZOffsetTable;
        ushort windowPos = info->windowPos;
        ushort windowLen = info->windowLen;
        if (remainSize < 3U)
          return 0;
        int index = (int) info->LZByteTable[*nextp];
        while (index != -1)
        {
          byte* numPtr1 = index >= (int) windowPos ? nextp - (int) windowLen - (int) windowPos + index : nextp - (int) windowPos + index;
          if ((int) numPtr1[1] != (int) nextp[1] || (int) numPtr1[2] != (int) nextp[2])
            index = (int) lzOffsetTable[index];
          else if (nextp - numPtr1 >= 2L)
          {
            uint num3 = 3;
            byte* numPtr2 = numPtr1 + 3;
            byte* numPtr3 = nextp + 3;
            while ((uint) (numPtr3 - nextp) < remainSize && (int) *numPtr3 == (int) *numPtr2)
            {
              ++numPtr3;
              ++numPtr2;
              ++num3;
              if ((int) num3 == (int) maxLength)
                break;
            }
            if (num3 > num2)
            {
              num2 = num3;
              num1 = (ushort) (nextp - numPtr1);
              if ((int) num2 == (int) maxLength || (int) num2 == (int) remainSize)
                break;
            }
            index = (int) lzOffsetTable[index];
          }
          else
            break;
        }
        if (num2 < 3U)
          return 0;
        *offset = num1;
        return num2;
      }

      private enum MICompressionType : byte
      {
        MI_COMPRESSION_LZ = 16, // 0x10
        MI_COMPRESSION_HUFFMAN = 32, // 0x20
        MI_COMPRESSION_RL = 48, // 0x30
        MI_COMPRESSION_DIFF = 128, // 0x80
        MI_COMPRESSION_TYPE_MASK = 240, // 0xF0
        MI_COMPRESSION_TYPE_EX_MASK = 255, // 0xFF
      }

      private struct LZCompressInfo
      {
        public ushort windowPos;
        public ushort windowLen;
        public unsafe short* LZOffsetTable;
        public unsafe short* LZByteTable;
        public unsafe short* LZEndTable;
      }
    }
  }
}
