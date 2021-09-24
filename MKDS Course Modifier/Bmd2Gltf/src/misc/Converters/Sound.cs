// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Converters.Sound
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.Collections.Generic;

namespace MKDS_Course_Modifier.Converters
{
  public class Sound
  {
    private static int[] indexTable = new int[16]
    {
      -1,
      -1,
      -1,
      -1,
      2,
      4,
      6,
      8,
      -1,
      -1,
      -1,
      -1,
      2,
      4,
      6,
      8
    };
    private static int[] stepsizeTable = new int[89]
    {
      7,
      8,
      9,
      10,
      11,
      12,
      13,
      14,
      16,
      17,
      19,
      21,
      23,
      25,
      28,
      31,
      34,
      37,
      41,
      45,
      50,
      55,
      60,
      66,
      73,
      80,
      88,
      97,
      107,
      118,
      130,
      143,
      157,
      173,
      190,
      209,
      230,
      253,
      279,
      307,
      337,
      371,
      408,
      449,
      494,
      544,
      598,
      658,
      724,
      796,
      876,
      963,
      1060,
      1166,
      1282,
      1411,
      1552,
      1707,
      1878,
      2066,
      2272,
      2499,
      2749,
      3024,
      3327,
      3660,
      4026,
      4428,
      4871,
      5358,
      5894,
      6484,
      7132,
      7845,
      8630,
      9493,
      10442,
      11487,
      12635,
      13899,
      15289,
      16818,
      18500,
      20350,
      22385,
      24623,
      27086,
      29794,
      (int) short.MaxValue
    };

    private static byte[] Bit8ToBit4(byte[] data)
    {
      List<byte> byteList = new List<byte>();
      for (int index = 0; index < data.Length; ++index)
      {
        byteList.Add((byte) ((uint) data[index] & 15U));
        byteList.Add((byte) (((int) data[index] & 240) >> 4));
      }
      return byteList.ToArray();
    }

    public static unsafe void ConvertImaAdpcm(byte* buf, int length, byte* outbuffer)
    {
      int decompSample = (int) *(short*) buf;
      int stepIndex = (int) *(short*) (buf + 2) & (int) sbyte.MaxValue;
      *(short*) outbuffer = (short) decompSample;
      Sound.ConvertImaAdpcm(buf + 4, length - 4, outbuffer + 2, ref decompSample, ref stepIndex);
    }

    public static unsafe void ConvertImaAdpcm(
      byte* buf,
      int length,
      byte* outbuffer,
      ref int decompSample,
      ref int stepIndex)
    {
      uint num1 = 0;
      uint num2 = 0;
      while ((long) num2 < (long) length)
      {
        byte data4bit = buf[(int) num2++];
        Sound.process_nibble(data4bit, ref stepIndex, ref decompSample);
        byte* numPtr1 = outbuffer;
        int num3 = (int) num1;
        uint num4 = (uint) (num3 + 1);
        var num5 = (uint) num3 * 2;
        *(short*) (numPtr1 + num5) = (short) decompSample;
        Sound.process_nibble((byte) (((int) data4bit & 240) >> 4), ref stepIndex, ref decompSample);
        byte* numPtr2 = outbuffer;
        int num6 = (int) num4;
        num1 = (uint) (num6 + 1);
        var num7 = (uint) num6 * 2;
        *(short*) (numPtr2 + num7) = (short) decompSample;
      }
    }

    private static int IMAMax(int samp)
    {
      return samp > (int) short.MaxValue ? (int) short.MaxValue : samp;
    }

    private static int IMAMin(int samp)
    {
      return samp < -32767 ? -32767 : samp;
    }

    private static int IMAIndexMinMax(int index, int min, int max)
    {
      return index > max ? max : (index < min ? min : index);
    }

    private static void process_nibble(byte data4bit, ref int Index, ref int Pcm16bit)
    {
      int num = Sound.stepsizeTable[Index] / 8;
      if (((int) data4bit & 1) != 0)
        num += Sound.stepsizeTable[Index] / 4;
      if (((int) data4bit & 2) != 0)
        num += Sound.stepsizeTable[Index] / 2;
      if (((int) data4bit & 4) != 0)
        num += Sound.stepsizeTable[Index] / 1;
      if (((int) data4bit & 8) == 0)
        Pcm16bit = Sound.IMAMax(Pcm16bit + num);
      if (((int) data4bit & 8) == 8)
        Pcm16bit = Sound.IMAMin(Pcm16bit - num);
      Index = Sound.IMAIndexMinMax(Index + Sound.indexTable[(int) data4bit & 7], 0, 88);
    }

    private static void clamp_step_index(ref int stepIndex)
    {
      if (stepIndex < 0)
        stepIndex = 0;
      if (stepIndex <= 88)
        return;
      stepIndex = 88;
    }

    private static void clamp_sample(ref int decompSample)
    {
      if (decompSample < (int) short.MinValue)
        decompSample = (int) short.MinValue;
      if (decompSample <= (int) short.MaxValue)
        return;
      decompSample = (int) short.MaxValue;
    }

    public static byte[] SignedPCM8ToUnsigned(byte[] data)
    {
      byte[] numArray = new byte[data.Length];
      for (int index = 0; index < data.Length; ++index)
        numArray[index] = (byte) ((uint) data[index] ^ 128U);
      return numArray;
    }
  }
}
