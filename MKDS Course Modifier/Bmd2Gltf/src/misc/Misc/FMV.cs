// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Misc.FMV
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Converters;
using MKDS_Course_Modifier.Exceptions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace MKDS_Course_Modifier.Misc
{
  public class FMV
  {
    private static readonly byte[] QuantizationGenerationTable1 = new byte[128]
    {
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 17,
      (byte) 18,
      (byte) 21,
      (byte) 24,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 16,
      (byte) 17,
      (byte) 19,
      (byte) 22,
      (byte) 25,
      (byte) 16,
      (byte) 16,
      (byte) 17,
      (byte) 18,
      (byte) 20,
      (byte) 22,
      (byte) 25,
      (byte) 29,
      (byte) 16,
      (byte) 16,
      (byte) 18,
      (byte) 21,
      (byte) 24,
      (byte) 27,
      (byte) 31,
      (byte) 36,
      (byte) 17,
      (byte) 17,
      (byte) 20,
      (byte) 24,
      (byte) 30,
      (byte) 35,
      (byte) 41,
      (byte) 47,
      (byte) 18,
      (byte) 19,
      (byte) 22,
      (byte) 27,
      (byte) 35,
      (byte) 44,
      (byte) 54,
      (byte) 65,
      (byte) 21,
      (byte) 22,
      (byte) 25,
      (byte) 31,
      (byte) 41,
      (byte) 54,
      (byte) 70,
      (byte) 88,
      (byte) 24,
      (byte) 25,
      (byte) 29,
      (byte) 36,
      (byte) 47,
      (byte) 65,
      (byte) 88,
      (byte) 115,
      (byte) 17,
      (byte) 18,
      (byte) 24,
      (byte) 47,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 18,
      (byte) 21,
      (byte) 26,
      (byte) 66,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 24,
      (byte) 26,
      (byte) 56,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 47,
      (byte) 66,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99
    };
    private static readonly byte[] QuantizationGenerationTable2 = new byte[128]
    {
      (byte) 16,
      (byte) 11,
      (byte) 10,
      (byte) 16,
      (byte) 24,
      (byte) 40,
      (byte) 51,
      (byte) 61,
      (byte) 12,
      (byte) 12,
      (byte) 14,
      (byte) 19,
      (byte) 26,
      (byte) 58,
      (byte) 60,
      (byte) 55,
      (byte) 14,
      (byte) 13,
      (byte) 16,
      (byte) 24,
      (byte) 40,
      (byte) 57,
      (byte) 69,
      (byte) 56,
      (byte) 14,
      (byte) 17,
      (byte) 22,
      (byte) 29,
      (byte) 51,
      (byte) 87,
      (byte) 80,
      (byte) 62,
      (byte) 18,
      (byte) 22,
      (byte) 37,
      (byte) 56,
      (byte) 68,
      (byte) 109,
      (byte) 103,
      (byte) 77,
      (byte) 24,
      (byte) 35,
      (byte) 55,
      (byte) 64,
      (byte) 81,
      (byte) 104,
      (byte) 113,
      (byte) 92,
      (byte) 49,
      (byte) 64,
      (byte) 78,
      (byte) 87,
      (byte) 103,
      (byte) 121,
      (byte) 120,
      (byte) 101,
      (byte) 72,
      (byte) 92,
      (byte) 95,
      (byte) 98,
      (byte) 112,
      (byte) 100,
      (byte) 103,
      (byte) 99,
      (byte) 17,
      (byte) 18,
      (byte) 24,
      (byte) 47,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 18,
      (byte) 21,
      (byte) 26,
      (byte) 66,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 24,
      (byte) 26,
      (byte) 56,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 47,
      (byte) 66,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99,
      (byte) 99
    };
    private static readonly uint[] AnotherQuantizationGenerationTable = new uint[8]
    {
      65536U,
      90901U,
      85627U,
      77062U,
      65536U,
      51491U,
      35468U,
      18081U
    };
    private static readonly byte[] ZigZagTable = new byte[64]
    {
      (byte) 0,
      (byte) 8,
      (byte) 1,
      (byte) 2,
      (byte) 9,
      (byte) 16,
      (byte) 24,
      (byte) 17,
      (byte) 10,
      (byte) 3,
      (byte) 4,
      (byte) 11,
      (byte) 18,
      (byte) 25,
      (byte) 32,
      (byte) 40,
      (byte) 33,
      (byte) 26,
      (byte) 19,
      (byte) 12,
      (byte) 5,
      (byte) 6,
      (byte) 13,
      (byte) 20,
      (byte) 27,
      (byte) 34,
      (byte) 41,
      (byte) 48,
      (byte) 56,
      (byte) 49,
      (byte) 42,
      (byte) 35,
      (byte) 28,
      (byte) 21,
      (byte) 14,
      (byte) 7,
      (byte) 15,
      (byte) 22,
      (byte) 29,
      (byte) 36,
      (byte) 43,
      (byte) 50,
      (byte) 57,
      (byte) 58,
      (byte) 51,
      (byte) 44,
      (byte) 37,
      (byte) 30,
      (byte) 23,
      (byte) 31,
      (byte) 38,
      (byte) 45,
      (byte) 52,
      (byte) 59,
      (byte) 60,
      (byte) 53,
      (byte) 46,
      (byte) 39,
      (byte) 47,
      (byte) 54,
      (byte) 61,
      (byte) 62,
      (byte) 55,
      (byte) 63
    };
    private FMV.FMVInfo Info;
    public FMV.FMVHeader Header;

    public FMV(byte[] Data)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(Data), Endianness.LittleEndian);
      try
      {
        this.Header = new FMV.FMVHeader(er);
        this.Info = new FMV.FMVInfo(this, er);
      }
      catch
      {
        er.Close();
      }
    }

    public Bitmap GetNextFrame(out byte[] Audio)
    {
      if (this.Info.er != null && this.Info.er.BaseStream.Position >= this.Info.er.BaseStream.Length)
      {
        this.Info.er.Close();
        this.Info.er = (EndianBinaryReader) null;
        Audio = (byte[]) null;
        return (Bitmap) null;
      }
      Audio = (byte[]) null;
      string str = this.Info.er.ReadString(Encoding.ASCII, 4);
      this.Info.er.BaseStream.Position -= 4L;
      switch (str)
      {
        case "FMVk":
          FMV.FMVk fmVk = new FMV.FMVk(this.Info, this.Info.FirstKeyFrame);
          this.Info.FirstKeyFrame = false;
          FMV.GenerateQuantizationTables(this.Info, (uint) fmVk.Quality);
          this.Info.LastFrame = FMV.ToBitmap(this.Info, fmVk.FrameBlockData, fmVk.FrameData);
          return this.Info.LastFrame;
        case "FMVd":
          FMV.FMVd fmVd = new FMV.FMVd(this.Info);
          FMV.GenerateQuantizationTables(this.Info, (uint) fmVd.Marker);
          this.Info.LastFrame = FMV.ToBitmap(this.Info, fmVd.FrameBlockData, fmVd.FrameData);
          return this.Info.LastFrame;
        case "FMVn":
          this.Info.er.BaseStream.Position += 8L;
          return this.Info.LastFrame;
        case "FMA\0":
          this.Info.er.BaseStream.Position += 4L;
          int count = (int) this.Info.er.ReadUInt32();
          Audio = this.Info.er.ReadBytes(count);
          return (Bitmap) null;
        default:
          return (Bitmap) null;
      }
    }

    public void Close()
    {
      if (this.Info.er == null)
        return;
      this.Info.er.Close();
      this.Info.er = (EndianBinaryReader) null;
    }

    private static void GenerateQuantizationTables(FMV.FMVInfo Info, uint Quality)
    {
      uint num1 = Quality;
      byte[] generationTable1 = FMV.QuantizationGenerationTable1;
      Info.QuantizationTables[0] = new uint[64];
      Info.QuantizationTables[1] = new uint[64];
      if (num1 <= 0U)
        num1 = 1U;
      if (num1 > 100U)
        num1 = 100U;
      uint num2 = num1 < 50U ? 5000U / num1 : 200U - (num1 << 1);
      uint num3 = 0;
      do
      {
        bool flag = true;
        uint num4 = 0;
        uint num5 = 1374389535;
        uint num6 = 1;
        do
        {
          flag = true;
          uint num7 = (uint) generationTable1[(64U * num3 + num4)];
          uint num8 = num4 >> 3;
          uint num9 = num4 & 7U;
          uint num10 = num2 * num7 + 50U;
          ulong num11 = (ulong) (int) num5 * (ulong) (int) num10;
          uint num12 = (uint) (num11 & (ulong) uint.MaxValue);
          uint num13 = (uint) (num11 >> 32 & (ulong) uint.MaxValue);
          uint num14 = num10 >> 31;
          uint num15 = FMV.AnotherQuantizationGenerationTable[num9];
          uint num16 = FMV.AnotherQuantizationGenerationTable[num8];
          uint num17 = num14 + (num13 >> 5);
          ulong num18 = (ulong) (int) num16 * (ulong) (int) num15;
          uint num19 = (uint) (num18 & (ulong) uint.MaxValue);
          uint num20 = (uint) (num18 >> 32 & (ulong) uint.MaxValue);
          if (num17 <= 0U)
            num17 = num6;
          uint num21 = num19 >> 16;
          if (num17 > (uint) byte.MaxValue)
            num17 = (uint) byte.MaxValue;
          uint num22 = num21 | num20 << 16;
          ulong num23 = (ulong) (int) num17 * (ulong) (int) num22;
          uint num24 = (uint) (num23 & (ulong) uint.MaxValue);
          uint num25 = (uint) (num23 >> 32 & (ulong) uint.MaxValue);
          uint num26 = num4 << 29;
          ++num4;
          uint num27 = num8 + (num26 >> 26);
          Info.QuantizationTables[num3][num27] = num24;
        }
        while (num4 < 64U);
        ++num3;
      }
      while (num3 < 2U);
    }

    private static Bitmap ToBitmap(FMV.FMVInfo Info, byte[] FrameBlockData, byte[] FrameData)
    {
      Info.BitReg = 0U;
      Info.BitRegNrBits = 0;
      Info.PhaseInfos[0].Field10 = 0;
      Info.PhaseInfos[1].Field10 = 0;
      Info.PhaseInfos[2].Field10 = 0;
      int offset = 0;
      for (int index1 = 0; index1 < Info.NrTilesY; ++index1)
      {
        for (int index2 = 0; index2 < Info.NrTilesX; ++index2)
        {
          switch (FrameBlockData[index1 * Info.NrTilesX + index2])
          {
            case 1:
              if (Info.BitRegNrBits < 5)
                FMV.FillBits(FrameData, ref Info.BitReg, ref Info.BitRegNrBits, ref offset);
              Info.BitRegNrBits -= 5;
              int num1 = (int) (Info.BitReg >> Info.BitRegNrBits) & 31;
              if (num1 > 15)
                num1 -= 32;
              if (Info.BitRegNrBits < 5)
                FMV.FillBits(FrameData, ref Info.BitReg, ref Info.BitRegNrBits, ref offset);
              Info.BitRegNrBits -= 5;
              int num2 = (int) (Info.BitReg >> Info.BitRegNrBits) & 31;
              if (num2 > 15)
                num2 -= 32;
              for (int index3 = 0; index3 < 16; ++index3)
              {
                for (int index4 = 0; index4 < 16; ++index4)
                  Info.PhaseInfos[0].PhaseData[(index1 * 16 + index3) * Info.PhaseInfos[0].PhaseDataWidth + (index2 * 16 + index4)] = Info.PhaseInfos[0].PhaseDataOld[(index1 * 16 + index3 - num2) * Info.PhaseInfos[0].PhaseDataWidth + (index2 * 16 + index4) - num1];
              }
              for (int index3 = 0; index3 < 8; ++index3)
              {
                for (int index4 = 0; index4 < 8; ++index4)
                  Info.PhaseInfos[1].PhaseData[(index1 * 8 + index3) * Info.PhaseInfos[1].PhaseDataWidth + (index2 * 8 + index4)] = Info.PhaseInfos[1].PhaseDataOld[(index1 * 8 + index3 - num2 / 2) * Info.PhaseInfos[1].PhaseDataWidth + (index2 * 8 + index4) - num1 / 2];
              }
              for (int index3 = 0; index3 < 8; ++index3)
              {
                for (int index4 = 0; index4 < 8; ++index4)
                  Info.PhaseInfos[2].PhaseData[(index1 * 8 + index3) * Info.PhaseInfos[2].PhaseDataWidth + (index2 * 8 + index4)] = Info.PhaseInfos[2].PhaseDataOld[(index1 * 8 + index3 - num2 / 2) * Info.PhaseInfos[2].PhaseDataWidth + (index2 * 8 + index4) - num1 / 2];
              }
              break;
            case 2:
              if (Info.BitRegNrBits < 8)
                FMV.FillBits(FrameData, ref Info.BitReg, ref Info.BitRegNrBits, ref offset);
              Info.BitRegNrBits -= 8;
              int num3 = (int) (Info.BitReg >> Info.BitRegNrBits) & (int) byte.MaxValue;
              if (Info.BitRegNrBits < 8)
                FMV.FillBits(FrameData, ref Info.BitReg, ref Info.BitRegNrBits, ref offset);
              Info.BitRegNrBits -= 8;
              int num4 = (int) (Info.BitReg >> Info.BitRegNrBits) & (int) byte.MaxValue;
              if (Info.BitRegNrBits < 8)
                FMV.FillBits(FrameData, ref Info.BitReg, ref Info.BitRegNrBits, ref offset);
              Info.BitRegNrBits -= 8;
              int num5 = (int) (Info.BitReg >> Info.BitRegNrBits) & (int) byte.MaxValue;
              for (int index3 = 0; index3 < 16; ++index3)
              {
                for (int index4 = 0; index4 < 16; ++index4)
                  Info.PhaseInfos[0].PhaseData[(index1 * 16 + index3) * Info.PhaseInfos[0].PhaseDataWidth + (index2 * 16 + index4)] = (byte) num3;
              }
              for (int index3 = 0; index3 < 8; ++index3)
              {
                for (int index4 = 0; index4 < 8; ++index4)
                  Info.PhaseInfos[1].PhaseData[(index1 * 8 + index3) * Info.PhaseInfos[1].PhaseDataWidth + (index2 * 8 + index4)] = (byte) num4;
              }
              for (int index3 = 0; index3 < 8; ++index3)
              {
                for (int index4 = 0; index4 < 8; ++index4)
                  Info.PhaseInfos[2].PhaseData[(index1 * 8 + index3) * Info.PhaseInfos[2].PhaseDataWidth + (index2 * 8 + index4)] = (byte) num5;
              }
              break;
            case byte.MaxValue:
              for (int index3 = 0; index3 < Info.NrPhases; ++index3)
              {
                for (int index4 = 0; index4 < Info.PhaseInfos[index3].NrBlocksX * Info.PhaseInfos[index3].NrBlocksY; ++index4)
                {
                  ushort[] r0 = new ushort[64];
                  if (Info.BitRegNrBits < 16)
                    FMV.FillBits(FrameData, ref Info.BitReg, ref Info.BitRegNrBits, ref offset);
                  int index5 = (int) (Info.BitReg >> Info.BitRegNrBits - 10) & 1023;
                  int index6 = (int) Info.field_0[Info.PhaseInfos[index3].Table1Idx][index5];
                  bool flag;
                  int num6;
                  if (index6 >= (int) byte.MaxValue)
                  {
                    uint num7 = (Info.BitRegNrBits < 16 ? Info.BitReg << 16 - Info.BitRegNrBits : Info.BitReg >> Info.BitRegNrBits - 16) & (uint) ushort.MaxValue;
                    int index7 = 11;
                    while (true)
                    {
                      flag = true;
                      if (num7 >= Info.field_804[Info.PhaseInfos[index3].Table1Idx][index7])
                        ++index7;
                      else
                        break;
                    }
                    int index8 = (int) ((long) ((1 << index7) - 1) & (long) (Info.BitReg >> Info.BitRegNrBits - index7)) + (int) Info.field_84C[Info.PhaseInfos[index3].Table1Idx][index7];
                    Info.BitRegNrBits -= index7;
                    num6 = (int) Info.field_600[Info.PhaseInfos[index3].Table1Idx][index8];
                  }
                  else
                  {
                    int num7 = (int) Info.field_700[Info.PhaseInfos[index3].Table1Idx][index6];
                    Info.BitRegNrBits -= num7;
                    num6 = (int) Info.field_600[Info.PhaseInfos[index3].Table1Idx][index6];
                  }
                  uint num8 = 0;
                  FMV.sub_213474C(ref r0, 0U, 128U);
                  if (num6 != 0)
                  {
                    if (Info.BitRegNrBits < num6)
                      FMV.FillBits(FrameData, ref Info.BitReg, ref Info.BitRegNrBits, ref offset);
                    Info.BitRegNrBits -= num6;
                    int num7 = (1 << num6) - 1;
                    num8 = Info.BitReg >> Info.BitRegNrBits & (uint) num7;
                    if ((long) num8 < (long) (1 << num6 - 1))
                    {
                      uint maxValue = uint.MaxValue;
                      num8 = num8 + (maxValue << num6) + 1U;
                    }
                  }
                  Info.PhaseInfos[index3].Field10 += (int) num8;
                  r0[0] = (ushort) Info.PhaseInfos[index3].Field10;
                  int num9 = 1;
                  do
                  {
                    flag = true;
                    if (Info.BitRegNrBits < 16)
                      FMV.FillBits(FrameData, ref Info.BitReg, ref Info.BitRegNrBits, ref offset);
                    int index7 = (int) (Info.BitReg >> Info.BitRegNrBits - 10) & 1023;
                    int index8 = (int) Info.field_1200[Info.PhaseInfos[index3].Table2Idx][index7];
                    int num7;
                    if (index8 >= (int) byte.MaxValue)
                    {
                      uint num10;
                      if (Info.BitRegNrBits < 16)
                      {
                        uint num11 = (uint) (16 - Info.BitRegNrBits);
                        num10 = Info.BitReg << (int) num11;
                      }
                      else
                      {
                        uint num11 = (uint) (Info.BitRegNrBits - 16);
                        num10 = Info.BitReg >> (int) num11;
                      }
                      uint num12 = num10 << 16 >> 16;
                      int index9 = 11;
                      while (true)
                      {
                        flag = true;
                        if (num12 >= Info.field_1A04[Info.PhaseInfos[index3].Table2Idx][index9])
                          ++index9;
                        else
                          break;
                      }
                      uint num13 = (uint) (1 << index9) - 1U;
                      uint num14 = (uint) (Info.BitRegNrBits - index9);
                      uint num15 = num13 & Info.BitReg >> (int) num14;
                      int num16 = (int) Info.field_1A4C[Info.PhaseInfos[index3].Table2Idx][index9];
                      Info.BitRegNrBits -= index9;
                      num7 = (int) Info.field_1800[Info.PhaseInfos[index3].Table2Idx][(long) num15 + (long) num16];
                    }
                    else
                    {
                      int num10 = (int) Info.field_1900[Info.PhaseInfos[index3].Table2Idx][index8];
                      Info.BitRegNrBits -= num10;
                      num7 = (int) Info.field_1800[Info.PhaseInfos[index3].Table2Idx][index8];
                    }
                    int num17 = num7 & 15;
                    if (num17 != 0)
                    {
                      int index9 = num9 + (num7 >> 4);
                      if (Info.BitRegNrBits < num17)
                        FMV.FillBits(FrameData, ref Info.BitReg, ref Info.BitRegNrBits, ref offset);
                      Info.BitRegNrBits -= num17;
                      int num10 = (1 << num17) - 1;
                      uint num11 = (uint) ((ulong) (Info.BitReg >> Info.BitRegNrBits) & (ulong) num10);
                      if ((long) num11 < (long) (1 << num17 - 1))
                      {
                        uint maxValue = uint.MaxValue;
                        num11 = num11 + (maxValue << num17) + 1U;
                      }
                      uint num12 = (uint) FMV.ZigZagTable[index9];
                      num9 = index9 + 1;
                      r0[num12] = (ushort) num11;
                    }
                    else if (num7 == 240)
                      num9 += 16;
                    else
                      break;
                  }
                  while (num9 < 64);
                  byte[] numArray = FMV.sub_20A66B0(r0, Info.QuantizationTables[Info.PhaseInfos[index3].QTIdx]);
                  switch (index3)
                  {
                    case 0:
                      for (int index7 = 0; index7 < 64; ++index7)
                      {
                        int num7 = index7 % 8 + index2 * 16 + index4 % 2 * 8;
                        int num10 = index7 / 8 + index1 * 16 + index4 / 2 * 8;
                        Info.PhaseInfos[index3].PhaseData[num10 * Info.PhaseInfos[index3].PhaseDataWidth + num7] = numArray[index7];
                      }
                      break;
                    case 1:
                      for (int index7 = 0; index7 < 64; ++index7)
                      {
                        int num7 = index7 % 8 + index2 * 8;
                        int num10 = index7 / 8 + index1 * 8;
                        Info.PhaseInfos[index3].PhaseData[num10 * Info.PhaseInfos[index3].PhaseDataWidth + num7] = numArray[index7];
                      }
                      break;
                    default:
                      for (int index7 = 0; index7 < 64; ++index7)
                      {
                        int num7 = index7 % 8 + index2 * 8;
                        int num10 = index7 / 8 + index1 * 8;
                        Info.PhaseInfos[index3].PhaseData[num10 * Info.PhaseInfos[index3].PhaseDataWidth + num7] = numArray[index7];
                      }
                      break;
                  }
                }
              }
              break;
          }
        }
      }
      Bitmap bitmap = new Bitmap(Info.Width, Info.Height);
      for (int y = 0; y < Info.Height; ++y)
      {
        for (int x = 0; x < Info.Width; ++x)
        {
          float num1 = (float) Info.PhaseInfos[0].PhaseData[y * Info.PhaseInfos[0].PhaseDataWidth + x] / 256f;
          float num2 = (float) ((double) Info.PhaseInfos[1].PhaseData[y / 2 * Info.PhaseInfos[1].PhaseDataWidth + x / 2] / 256.0 - 0.5);
          float num3 = (float) ((double) Info.PhaseInfos[2].PhaseData[y / 2 * Info.PhaseInfos[2].PhaseDataWidth + x / 2] / 256.0 - 0.5);
          float num4 = num1 + num3 * 1.13983f;
          float num5 = (float) ((double) num1 - (double) num2 * 0.394650012254715 - (double) num3 * 0.580600023269653);
          float num6 = num1 + num2 * 2.03211f;
          if ((double) num4 < 0.0)
            num4 = 0.0f;
          if ((double) num4 > 1.0)
            num4 = 1f;
          if ((double) num5 < 0.0)
            num5 = 0.0f;
          if ((double) num5 > 1.0)
            num5 = 1f;
          if ((double) num6 < 0.0)
            num6 = 0.0f;
          if ((double) num6 > 1.0)
            num6 = 1f;
          bitmap.SetPixel(x, y, Color.FromArgb((int) ((double) num4 * (double) byte.MaxValue), (int) ((double) num5 * (double) byte.MaxValue), (int) ((double) num6 * (double) byte.MaxValue)));
        }
      }
      Array.Copy((Array) Info.PhaseInfos[0].PhaseData, (Array) Info.PhaseInfos[0].PhaseDataOld, Info.PhaseInfos[0].PhaseData.Length);
      Array.Copy((Array) Info.PhaseInfos[1].PhaseData, (Array) Info.PhaseInfos[1].PhaseDataOld, Info.PhaseInfos[1].PhaseData.Length);
      Array.Copy((Array) Info.PhaseInfos[2].PhaseData, (Array) Info.PhaseInfos[2].PhaseDataOld, Info.PhaseInfos[2].PhaseData.Length);
      return bitmap;
    }

    private static void FillBits(
      byte[] FrameData,
      ref uint bitreg,
      ref int nrbits,
      ref int offset)
    {
      while (nrbits <= 24)
      {
        byte num = offset + 1 >= FrameData.Length ? (byte) 0 : FrameData[offset++];
        bitreg = bitreg << 8 | (uint) num;
        nrbits += 8;
      }
    }

    private static void sub_213474C(ref ushort[] r0, uint r1, uint r2)
    {
      byte[] numArray1 = new byte[r0.Length * 2];
      for (int index = 0; index < r0.Length; ++index)
      {
        numArray1[index * 2] = (byte) ((uint) r0[index] & (uint) byte.MaxValue);
        numArray1[index * 2 + 1] = (byte) ((int) r0[index] >> 8 & (int) byte.MaxValue);
      }
      int num1 = 0;
      uint num2 = r1 & (uint) byte.MaxValue;
      if (r2 >= 32U)
      {
        if (num2 != 0U)
        {
          r1 = num2 << 16;
          r1 |= num2 << 24;
          r1 |= num2 << 8;
          num2 |= r1;
        }
        for (r1 = r2 >> 5; r1 != 0U; --r1)
        {
          byte[] numArray2 = numArray1;
          int index1 = num1;
          int num3 = index1 + 1;
          int num4 = (int) (byte) (num2 & (uint) byte.MaxValue);
          numArray2[index1] = (byte) num4;
          byte[] numArray3 = numArray1;
          int index2 = num3;
          int num5 = index2 + 1;
          int num6 = (int) (byte) (num2 >> 8 & (uint) byte.MaxValue);
          numArray3[index2] = (byte) num6;
          byte[] numArray4 = numArray1;
          int index3 = num5;
          int num7 = index3 + 1;
          int num8 = (int) (byte) (num2 >> 16 & (uint) byte.MaxValue);
          numArray4[index3] = (byte) num8;
          byte[] numArray5 = numArray1;
          int index4 = num7;
          int num9 = index4 + 1;
          int num10 = (int) (byte) (num2 >> 24 & (uint) byte.MaxValue);
          numArray5[index4] = (byte) num10;
          byte[] numArray6 = numArray1;
          int index5 = num9;
          int num11 = index5 + 1;
          int num12 = (int) (byte) (num2 & (uint) byte.MaxValue);
          numArray6[index5] = (byte) num12;
          byte[] numArray7 = numArray1;
          int index6 = num11;
          int num13 = index6 + 1;
          int num14 = (int) (byte) (num2 >> 8 & (uint) byte.MaxValue);
          numArray7[index6] = (byte) num14;
          byte[] numArray8 = numArray1;
          int index7 = num13;
          int num15 = index7 + 1;
          int num16 = (int) (byte) (num2 >> 16 & (uint) byte.MaxValue);
          numArray8[index7] = (byte) num16;
          byte[] numArray9 = numArray1;
          int index8 = num15;
          int num17 = index8 + 1;
          int num18 = (int) (byte) (num2 >> 24 & (uint) byte.MaxValue);
          numArray9[index8] = (byte) num18;
          byte[] numArray10 = numArray1;
          int index9 = num17;
          int num19 = index9 + 1;
          int num20 = (int) (byte) (num2 & (uint) byte.MaxValue);
          numArray10[index9] = (byte) num20;
          byte[] numArray11 = numArray1;
          int index10 = num19;
          int num21 = index10 + 1;
          int num22 = (int) (byte) (num2 >> 8 & (uint) byte.MaxValue);
          numArray11[index10] = (byte) num22;
          byte[] numArray12 = numArray1;
          int index11 = num21;
          int num23 = index11 + 1;
          int num24 = (int) (byte) (num2 >> 16 & (uint) byte.MaxValue);
          numArray12[index11] = (byte) num24;
          byte[] numArray13 = numArray1;
          int index12 = num23;
          int num25 = index12 + 1;
          int num26 = (int) (byte) (num2 >> 24 & (uint) byte.MaxValue);
          numArray13[index12] = (byte) num26;
          byte[] numArray14 = numArray1;
          int index13 = num25;
          int num27 = index13 + 1;
          int num28 = (int) (byte) (num2 & (uint) byte.MaxValue);
          numArray14[index13] = (byte) num28;
          byte[] numArray15 = numArray1;
          int index14 = num27;
          int num29 = index14 + 1;
          int num30 = (int) (byte) (num2 >> 8 & (uint) byte.MaxValue);
          numArray15[index14] = (byte) num30;
          byte[] numArray16 = numArray1;
          int index15 = num29;
          int num31 = index15 + 1;
          int num32 = (int) (byte) (num2 >> 16 & (uint) byte.MaxValue);
          numArray16[index15] = (byte) num32;
          byte[] numArray17 = numArray1;
          int index16 = num31;
          int num33 = index16 + 1;
          int num34 = (int) (byte) (num2 >> 24 & (uint) byte.MaxValue);
          numArray17[index16] = (byte) num34;
          byte[] numArray18 = numArray1;
          int index17 = num33;
          int num35 = index17 + 1;
          int num36 = (int) (byte) (num2 & (uint) byte.MaxValue);
          numArray18[index17] = (byte) num36;
          byte[] numArray19 = numArray1;
          int index18 = num35;
          int num37 = index18 + 1;
          int num38 = (int) (byte) (num2 >> 8 & (uint) byte.MaxValue);
          numArray19[index18] = (byte) num38;
          byte[] numArray20 = numArray1;
          int index19 = num37;
          int num39 = index19 + 1;
          int num40 = (int) (byte) (num2 >> 16 & (uint) byte.MaxValue);
          numArray20[index19] = (byte) num40;
          byte[] numArray21 = numArray1;
          int index20 = num39;
          int num41 = index20 + 1;
          int num42 = (int) (byte) (num2 >> 24 & (uint) byte.MaxValue);
          numArray21[index20] = (byte) num42;
          byte[] numArray22 = numArray1;
          int index21 = num41;
          int num43 = index21 + 1;
          int num44 = (int) (byte) (num2 & (uint) byte.MaxValue);
          numArray22[index21] = (byte) num44;
          byte[] numArray23 = numArray1;
          int index22 = num43;
          int num45 = index22 + 1;
          int num46 = (int) (byte) (num2 >> 8 & (uint) byte.MaxValue);
          numArray23[index22] = (byte) num46;
          byte[] numArray24 = numArray1;
          int index23 = num45;
          int num47 = index23 + 1;
          int num48 = (int) (byte) (num2 >> 16 & (uint) byte.MaxValue);
          numArray24[index23] = (byte) num48;
          byte[] numArray25 = numArray1;
          int index24 = num47;
          int num49 = index24 + 1;
          int num50 = (int) (byte) (num2 >> 24 & (uint) byte.MaxValue);
          numArray25[index24] = (byte) num50;
          byte[] numArray26 = numArray1;
          int index25 = num49;
          int num51 = index25 + 1;
          int num52 = (int) (byte) (num2 & (uint) byte.MaxValue);
          numArray26[index25] = (byte) num52;
          byte[] numArray27 = numArray1;
          int index26 = num51;
          int num53 = index26 + 1;
          int num54 = (int) (byte) (num2 >> 8 & (uint) byte.MaxValue);
          numArray27[index26] = (byte) num54;
          byte[] numArray28 = numArray1;
          int index27 = num53;
          int num55 = index27 + 1;
          int num56 = (int) (byte) (num2 >> 16 & (uint) byte.MaxValue);
          numArray28[index27] = (byte) num56;
          byte[] numArray29 = numArray1;
          int index28 = num55;
          int num57 = index28 + 1;
          int num58 = (int) (byte) (num2 >> 24 & (uint) byte.MaxValue);
          numArray29[index28] = (byte) num58;
          byte[] numArray30 = numArray1;
          int index29 = num57;
          int num59 = index29 + 1;
          int num60 = (int) (byte) (num2 & (uint) byte.MaxValue);
          numArray30[index29] = (byte) num60;
          byte[] numArray31 = numArray1;
          int index30 = num59;
          int num61 = index30 + 1;
          int num62 = (int) (byte) (num2 >> 8 & (uint) byte.MaxValue);
          numArray31[index30] = (byte) num62;
          byte[] numArray32 = numArray1;
          int index31 = num61;
          int num63 = index31 + 1;
          int num64 = (int) (byte) (num2 >> 16 & (uint) byte.MaxValue);
          numArray32[index31] = (byte) num64;
          byte[] numArray33 = numArray1;
          int index32 = num63;
          num1 = index32 + 1;
          int num65 = (int) (byte) (num2 >> 24 & (uint) byte.MaxValue);
          numArray33[index32] = (byte) num65;
        }
        r1 = r2 & 31U;
        for (r1 >>= 2; r1 != 0U; --r1)
        {
          byte[] numArray2 = numArray1;
          int index1 = num1;
          int num3 = index1 + 1;
          int num4 = (int) (byte) (num2 & (uint) byte.MaxValue);
          numArray2[index1] = (byte) num4;
          byte[] numArray3 = numArray1;
          int index2 = num3;
          int num5 = index2 + 1;
          int num6 = (int) (byte) (num2 >> 8 & (uint) byte.MaxValue);
          numArray3[index2] = (byte) num6;
          byte[] numArray4 = numArray1;
          int index3 = num5;
          int num7 = index3 + 1;
          int num8 = (int) (byte) (num2 >> 16 & (uint) byte.MaxValue);
          numArray4[index3] = (byte) num8;
          byte[] numArray5 = numArray1;
          int index4 = num7;
          num1 = index4 + 1;
          int num9 = (int) (byte) (num2 >> 24 & (uint) byte.MaxValue);
          numArray5[index4] = (byte) num9;
        }
        r2 &= 3U;
      }
      if (r2 != 0U)
      {
        r1 = num2 & (uint) byte.MaxValue;
        do
        {
          numArray1[num1++] = (byte) r1;
          --r2;
        }
        while (r2 != 0U);
      }
      for (int index = 0; index < r0.Length; ++index)
        r0[index] = (ushort) ((uint) numArray1[index * 2] | (uint) numArray1[index * 2 + 1] << 8);
    }

    private static byte[] sub_20A66B0(ushort[] indata, uint[] QuantizationTable)
    {
      byte[] bytes = new byte[indata.Length * 2];
      for (int index = 0; index < indata.Length; ++index)
      {
        bytes[index * 2] = (byte) ((uint) indata[index] & (uint) byte.MaxValue);
        bytes[index * 2 + 1] = (byte) ((int) indata[index] >> 8 & (int) byte.MaxValue);
      }
      int offset = 0;
      int index1 = 0;
      int index2 = 0;
      int index3 = 0;
      int num1 = 0;
      byte[] numArray1 = new byte[64];
      uint[] numArray2 = new uint[64];
      bool flag1;
      do
      {
        flag1 = true;
        uint num2 = (uint) Bytes.Read2BytesAsInt16(bytes, offset + 2);
        bool flag2;
        if ((flag2 = num2 == 0U) && ((flag2 = Bytes.Read4BytesAsUInt32(bytes, offset + 4) == 0U) && (flag2 = Bytes.Read4BytesAsUInt32(bytes, offset + 8) == 0U)))
          flag2 = Bytes.Read4BytesAsUInt32(bytes, offset + 12) == 0U;
        if (flag2)
        {
          uint num3 = (uint) Bytes.Read2BytesAsInt16(bytes, offset) * QuantizationTable[index1];
          numArray2[index3] = num3;
          numArray2[index3 + 8] = num3;
          numArray2[index3 + 16] = num3;
          numArray2[index3 + 24] = num3;
          numArray2[index3 + 32] = num3;
          numArray2[index3 + 40] = num3;
          numArray2[index3 + 48] = num3;
          numArray2[index3 + 56] = num3;
        }
        else
        {
          uint num3 = QuantizationTable[index1 + 1];
          uint num4 = (uint) Bytes.Read2BytesAsInt16(bytes, offset + 6);
          uint num5 = num2 * num3;
          uint num6 = QuantizationTable[index1 + 3];
          uint num7 = (uint) Bytes.Read2BytesAsInt16(bytes, offset + 10);
          uint num8 = num4 * num6;
          uint num9 = QuantizationTable[index1 + 5];
          uint num10 = (uint) Bytes.Read2BytesAsInt16(bytes, offset);
          uint num11 = QuantizationTable[index1];
          uint num12 = num7 * num9;
          uint num13 = num10 * num11;
          uint num14 = (uint) Bytes.Read2BytesAsInt16(bytes, offset + 4);
          uint num15 = QuantizationTable[index1 + 2];
          uint num16 = (uint) Bytes.Read2BytesAsInt16(bytes, offset + 8);
          uint num17 = num14 * num15;
          uint num18 = QuantizationTable[index1 + 4];
          uint num19 = (uint) Bytes.Read2BytesAsInt16(bytes, offset + 12);
          uint num20 = QuantizationTable[index1 + 6];
          uint num21 = num16 * num18;
          uint num22 = num19 * num20;
          uint num23 = num13 + num21;
          uint num24 = num13 - num21;
          uint num25 = num17 + num22;
          ulong num26 = (ulong) (int) (num17 - num22) * 92682UL;
          uint num27 = ((uint) (num26 & (ulong) uint.MaxValue) >> 16 | (uint) (num26 >> 32 & (ulong) uint.MaxValue) << 16) - num25;
          uint num28 = num23 + num25;
          uint num29 = num23 - num25;
          uint num30 = num24 + num27;
          uint num31 = num24 - num27;
          uint num32 = num30;
          uint num33 = num12 + num8;
          uint num34 = (uint) Bytes.Read2BytesAsInt16(bytes, offset + 14);
          uint num35 = QuantizationTable[index1 + 7];
          uint num36 = num12 - num8;
          uint num37 = num34 * num35;
          uint num38 = num5 + num37;
          uint num39 = num28;
          uint num40 = num5 - num37;
          uint num41 = num38 + num33;
          ulong num42 = (ulong) (int) (num38 - num33) * 92682UL;
          uint num43 = (uint) (num42 & (ulong) uint.MaxValue) >> 16 | (uint) (num42 >> 32 & (ulong) uint.MaxValue) << 16;
          uint num44 = 121095;
          ulong num45 = (ulong) (int) (num36 + num40) * (ulong) (int) num44;
          uint num46 = (uint) (num45 & (ulong) uint.MaxValue) >> 16 | (uint) (num45 >> 32 & (ulong) uint.MaxValue) << 16;
          ulong num47 = 70936UL * (ulong) (int) num40;
          uint num48 = (uint) (num47 & (ulong) uint.MaxValue) >> 16 | (uint) (num47 >> 32 & (ulong) uint.MaxValue) << 16;
          uint num49 = 4294796043;
          uint num50 = num48 - num46;
          ulong num51 = (ulong) (int) num49 * (ulong) (int) num36;
          uint num52 = ((uint) (num51 & (ulong) uint.MaxValue) >> 16 | (uint) (num51 >> 32 & (ulong) uint.MaxValue) << 16) + num46 - num41;
          uint num53 = num43 - num52;
          uint num54 = num50 + num53;
          uint num55 = num39;
          uint num56 = num55 + num41;
          uint num57 = num55 - num41;
          numArray2[index3] = num56;
          numArray2[index3 + 56] = num57;
          uint num58 = num32;
          uint num59 = num58 + num52;
          uint num60 = num58 - num52;
          numArray2[index3 + 8] = num59;
          numArray2[index3 + 48] = num60;
          uint num61 = num31;
          uint num62 = num61 + num53;
          uint num63 = num61 - num53;
          numArray2[index3 + 16] = num62;
          numArray2[index3 + 40] = num63;
          uint num64 = num29 + num54;
          uint num65 = num29 - num54;
          numArray2[index3 + 32] = num64;
          numArray2[index3 + 24] = num65;
        }
        offset += 16;
        ++num1;
        index1 += 8;
        ++index3;
      }
      while (num1 < 8);
      int num66 = 0;
      int index4 = 0;
      uint maxValue = (uint) byte.MaxValue;
      do
      {
        flag1 = true;
        uint num2 = numArray2[index4 + 4];
        uint num3 = numArray2[index4];
        uint num4 = numArray2[index4 + 6];
        uint num5 = numArray2[index4 + 2];
        uint num6 = num3 + num2;
        uint num7 = num3 - num2;
        uint num8 = num5 + num4;
        uint num9 = num5 - num4;
        uint num10 = 92682;
        uint num11 = num6 + num8;
        ulong num12 = (ulong) (int) num9 * (ulong) (int) num10;
        uint num13 = ((uint) (num12 & (ulong) uint.MaxValue) >> 16 | (uint) (num12 >> 32 & (ulong) uint.MaxValue) << 16) - num8;
        uint num14 = num6 - num8;
        uint num15 = numArray2[index4 + 3];
        uint num16 = numArray2[index4 + 5];
        uint num17 = num7 + num13;
        uint num18 = num7 - num13;
        uint num19 = num16 + num15;
        uint num20 = num16 - num15;
        uint num21 = 4294796043;
        uint num22 = num19;
        ulong num23 = (ulong) (int) num21 * (ulong) (int) num20;
        uint num24 = (uint) (num23 & (ulong) uint.MaxValue);
        uint num25 = (uint) (num23 >> 32 & (ulong) uint.MaxValue);
        uint num26 = num24 >> 16;
        uint num27 = numArray2[index4 + 7];
        uint num28 = numArray2[index4 + 1];
        uint num29 = num26 | num25 << 16;
        uint num30 = num28 + num27;
        uint num31 = num28 - num27;
        uint num32 = 121095;
        ulong num33 = (ulong) (int) (num20 + num31) * (ulong) (int) num32;
        uint num34 = (uint) (num33 & (ulong) uint.MaxValue);
        uint num35 = (uint) (num33 >> 32 & (ulong) uint.MaxValue);
        uint num36 = num34 >> 16;
        uint num37 = num35;
        index4 += 8;
        uint num38 = num36 | num37 << 16;
        uint num39 = 70936;
        uint num40 = num29 + num38;
        ulong num41 = (ulong) (int) num39 * (ulong) (int) num31;
        uint num42 = ((uint) (num41 & (ulong) uint.MaxValue) >> 16 | (uint) (num41 >> 32 & (ulong) uint.MaxValue) << 16) - num38;
        uint num43 = num22;
        uint num44 = num30 + num43;
        uint num45 = num30 - num43;
        uint num46 = 92682;
        uint num47 = num40 - num44;
        ulong num48 = (ulong) (int) num45 * (ulong) (int) num46;
        uint num49 = ((uint) (num48 & (ulong) uint.MaxValue) >> 16 | (uint) (num48 >> 32 & (ulong) uint.MaxValue) << 16) - num47;
        uint num50 = num42 + num49;
        uint num51 = (num11 + num44 >> 19) + 128U;
        uint num52 = num11 - num44;
        if ((int) num51 < 0)
          num51 = 0U;
        if (num51 > (uint) byte.MaxValue)
          num51 = maxValue;
        uint num53 = (num52 >> 19) + 128U;
        if ((int) num53 < 0)
          num53 = 0U;
        numArray1[index2] = (byte) num51;
        if (num53 > (uint) byte.MaxValue)
          num53 = maxValue;
        numArray1[index2 + 7] = (byte) num53;
        uint num54 = (num17 + num47 >> 19) + 128U;
        uint num55 = num17 - num47;
        if ((int) num54 < 0)
          num54 = 0U;
        if (num54 > (uint) byte.MaxValue)
          num54 = maxValue;
        uint num56 = (num55 >> 19) + 128U;
        if ((int) num56 < 0)
          num56 = 0U;
        numArray1[index2 + 1] = (byte) num54;
        if (num56 > (uint) byte.MaxValue)
          num56 = maxValue;
        numArray1[index2 + 6] = (byte) num56;
        uint num57 = (num18 + num49 >> 19) + 128U;
        uint num58 = num18 - num49;
        if ((int) num57 < 0)
          num57 = 0U;
        if (num57 > (uint) byte.MaxValue)
          num57 = maxValue;
        uint num59 = (num58 >> 19) + 128U;
        if ((int) num59 < 0)
          num59 = 0U;
        numArray1[index2 + 2] = (byte) num57;
        if (num59 > (uint) byte.MaxValue)
          num59 = maxValue;
        numArray1[index2 + 5] = (byte) num59;
        uint num60 = (num14 + num50 >> 19) + 128U;
        uint num61 = num14 - num50;
        if ((int) num60 < 0)
          num60 = 0U;
        if (num60 > (uint) byte.MaxValue)
          num60 = maxValue;
        uint num62 = (num61 >> 19) + 128U;
        if ((int) num62 < 0)
          num62 = 0U;
        numArray1[index2 + 4] = (byte) num60;
        if (num62 > (uint) byte.MaxValue)
          num62 = maxValue;
        numArray1[index2 + 3] = (byte) num62;
        index2 += 8;
        ++num66;
      }
      while (num66 < 8);
      return numArray1;
    }

    private static byte[] DecompressRLE(byte[] Data, int DataOffset, int Length)
    {
      List<byte> byteList = new List<byte>();
      int num1 = DataOffset;
      byte[] numArray = Data;
      int index = num1;
      int num2 = index + 1;
      int num3 = (int) numArray[index];
      do
      {
        bool flag = true;
        int num4 = (int) Data[num2++];
        if (num4 == num3)
        {
          int num5 = (int) Data[num2++];
          if (num5 <= 2)
          {
            int num6 = 0;
            do
            {
              flag = true;
              ++num6;
              byteList.Add((byte) num3);
            }
            while (num6 <= num5);
          }
          else
          {
            if ((num5 & 128) != 0)
              num5 = (int) Data[num2++] + (num5 << 25 >> 17);
            int num6 = (int) Data[num2++];
            int num7 = 0;
            do
            {
              flag = true;
              ++num7;
              byteList.Add((byte) num6);
            }
            while (num7 <= num5);
          }
        }
        else
          byteList.Add((byte) num4);
      }
      while (num2 < Length + DataOffset);
      return byteList.ToArray();
    }

    public class FMVHeader
    {
      public string Signature;
      public ushort Version;
      public ushort HeaderLength;
      public ushort Width;
      public ushort Height;
      public uint NrBlocks;
      public uint Unknown1;
      public ushort FrameRate;
      public uint Flags;
      public uint Unknown2;
      public ushort Unknown3;
      public uint AudioRate;
      public byte Unknown4;
      public byte Unknown5;
      public byte Unknown6;
      public byte Unknown7;

      public FMVHeader(EndianBinaryReader er)
      {
        this.Signature = er.ReadString(Encoding.ASCII, 4);
        if (this.Signature != "FMV!")
          throw new SignatureNotCorrectException(this.Signature, "FMV!", er.BaseStream.Position - 4L);
        this.Version = er.ReadUInt16();
        this.HeaderLength = er.ReadUInt16();
        this.Width = er.ReadUInt16();
        this.Height = er.ReadUInt16();
        this.NrBlocks = er.ReadUInt32();
        this.Unknown1 = er.ReadUInt32();
        this.FrameRate = er.ReadUInt16();
        this.Flags = er.ReadUInt32();
        this.Unknown2 = er.ReadUInt32();
        if (((int) this.Flags & 4) != 4)
          return;
        this.Unknown3 = er.ReadUInt16();
        this.AudioRate = er.ReadUInt32();
        this.Unknown4 = er.ReadByte();
        this.Unknown5 = er.ReadByte();
        this.Unknown6 = er.ReadByte();
        this.Unknown7 = er.ReadByte();
      }
    }

    private class FMVk
    {
      public string Signature;
      public uint SectionSize;
      public ushort FrameBlockDataSize;
      public byte[] FrameBlockData;
      public byte Quality;
      public byte[] TableInfo1a;
      public byte[] Table1a;
      public byte[] TableInfo2a;
      public byte[] Table2a;
      public byte[] TableInfo1b;
      public byte[] Table1b;
      public byte[] TableInfo2b;
      public byte[] Table2b;
      public byte[] FrameData;

      public FMVk(FMV.FMVInfo Info, bool Tables)
      {
        this.Signature = Info.er.ReadString(Encoding.ASCII, 4);
        if (this.Signature != nameof (FMVk))
          throw new SignatureNotCorrectException(this.Signature, nameof (FMVk), Info.er.BaseStream.Position - 4L);
        this.SectionSize = Info.er.ReadUInt32();
        this.FrameBlockDataSize = Info.er.ReadUInt16();
        this.FrameBlockData = FMV.DecompressRLE(Info.er.ReadBytes((int) this.FrameBlockDataSize), 0, (int) this.FrameBlockDataSize);
        this.Quality = Info.er.ReadByte();
        int num1 = 0;
        if (Tables)
        {
          this.TableInfo1a = Info.er.ReadBytes(16);
          int count1 = 0;
          foreach (byte num2 in this.TableInfo1a)
            count1 += (int) num2;
          this.Table1a = Info.er.ReadBytes(count1);
          int num3 = num1 + (count1 + 16);
          this.TableInfo2a = Info.er.ReadBytes(16);
          int count2 = 0;
          foreach (byte num2 in this.TableInfo2a)
            count2 += (int) num2;
          this.Table2a = Info.er.ReadBytes(count2);
          int num4 = num3 + (count2 + 16);
          this.TableInfo1b = Info.er.ReadBytes(16);
          int count3 = 0;
          foreach (byte num2 in this.TableInfo1b)
            count3 += (int) num2;
          this.Table1b = Info.er.ReadBytes(count3);
          int num5 = num4 + (count3 + 16);
          this.TableInfo2b = Info.er.ReadBytes(16);
          int count4 = 0;
          foreach (byte num2 in this.TableInfo2b)
            count4 += (int) num2;
          this.Table2b = Info.er.ReadBytes(count4);
          num1 = num5 + (count4 + 16);
          this.ReadTable(Info, this.TableInfo1a, this.Table1a, 0, 0);
          this.ReadTable(Info, this.TableInfo2a, this.Table2a, 1, 0);
          this.ReadTable(Info, this.TableInfo1b, this.Table1b, 0, 1);
          this.ReadTable(Info, this.TableInfo2b, this.Table2b, 1, 1);
        }
        this.FrameData = Info.er.ReadBytes((int) this.SectionSize - 3 - (int) this.FrameBlockDataSize - num1);
      }

      private void ReadTable(FMV.FMVInfo Info, byte[] TableInfo, byte[] Table, int r1, int r2)
      {
        if (r1 == 1)
        {
          this.CreateTables(ref Info.field_1200[r2], ref Info.field_1600[r2], ref Info.field_1900[r2], ref Info.field_1A04[r2], ref Info.field_1A4C[r2], TableInfo);
          Info.field_1800[r2] = new byte[Table.Length];
          Array.Copy((Array) Table, (Array) Info.field_1800[r2], Table.Length);
        }
        else
        {
          this.CreateTables(ref Info.field_0[r2], ref Info.field_400[r2], ref Info.field_700[r2], ref Info.field_804[r2], ref Info.field_84C[r2], TableInfo);
          Info.field_600[r2] = new byte[Table.Length];
          Array.Copy((Array) Table, (Array) Info.field_600[r2], Table.Length);
        }
      }

      private void CreateTables(
        ref byte[] Table0x0,
        ref ushort[] Table0x400,
        ref byte[] Table0x700,
        ref uint[] Table0x804,
        ref uint[] Table0x84C,
        byte[] TableInfo)
      {
        List<byte> byteList = new List<byte>();
        for (int index1 = 0; index1 < 16; ++index1)
        {
          for (int index2 = 0; index2 < (int) TableInfo[index1]; ++index2)
            byteList.Add((byte) (index1 + 1 & (int) byte.MaxValue));
        }
        byteList.Add((byte) 0);
        Table0x700 = byteList.ToArray();
        Table0x804 = new uint[18];
        Table0x84C = new uint[18];
        List<ushort> ushortList = new List<ushort>();
        uint num1 = 0;
        int index3 = 0;
        int index4 = 1;
        do
        {
          Table0x84C[index4] = (uint) ((ulong) index3 - (ulong) num1);
          for (; index4 == (int) Table0x700[index3]; ++index3)
            ushortList.Add((ushort) num1++);
          uint num2 = num1 << 16 - index4;
          Table0x804[index4++] = num2;
          num1 *= 2U;
        }
        while (index4 <= 16);
        Table0x804[index4] = uint.MaxValue;
        Table0x400 = ushortList.ToArray();
        Table0x0 = new byte[1024];
        for (int index1 = 0; index1 < 1024; ++index1)
          Table0x0[index1] = byte.MaxValue;
        for (int index1 = 0; index1 < index3; ++index1)
        {
          int num2 = (int) Table0x700[index1];
          if (num2 <= 10)
          {
            int num3 = 10 - num2;
            int num4 = 1 << num3;
            int num5 = 0;
            int num6 = (int) Table0x400[index1];
            if (1 << num3 > 0)
            {
              do
              {
                Table0x0[(num6 << num3) + num5++] = (byte) index1;
              }
              while (num5 < num4);
            }
          }
        }
      }
    }

    private class FMVd
    {
      public string Signature;
      public uint SectionSize;
      public ushort FrameBlockDataSize;
      public byte[] FrameBlockData;
      public byte Marker;
      public byte[] FrameData;

      public FMVd(FMV.FMVInfo Info)
      {
        this.Signature = Info.er.ReadString(Encoding.ASCII, 4);
        if (this.Signature != nameof (FMVd))
          throw new SignatureNotCorrectException(this.Signature, nameof (FMVd), Info.er.BaseStream.Position - 4L);
        this.SectionSize = Info.er.ReadUInt32();
        this.FrameBlockDataSize = Info.er.ReadUInt16();
        this.FrameBlockData = FMV.DecompressRLE(Info.er.ReadBytes((int) this.FrameBlockDataSize), 0, (int) this.FrameBlockDataSize);
        this.Marker = Info.er.ReadByte();
        this.FrameData = Info.er.ReadBytes((int) this.SectionSize - 3 - (int) this.FrameBlockDataSize);
      }
    }

    private class FMVInfo
    {
      public byte[][] field_0 = new byte[2][];
      public ushort[][] field_400 = new ushort[2][];
      public byte[][] field_600 = new byte[2][];
      public byte[][] field_700 = new byte[2][];
      public uint[][] field_804 = new uint[2][];
      public uint[][] field_84C = new uint[2][];
      public byte[][] field_1200 = new byte[2][];
      public ushort[][] field_1600 = new ushort[2][];
      public byte[][] field_1800 = new byte[2][];
      public byte[][] field_1900 = new byte[2][];
      public uint[][] field_1A04 = new uint[2][];
      public uint[][] field_1A4C = new uint[2][];
      public bool FirstKeyFrame = true;
      public int NrPhases = 3;
      public int Quality = 0;
      public uint[][] QuantizationTables = new uint[2][];
      public uint BitReg = 0;
      public int BitRegNrBits = 0;
      public FMV.FMVInfo.PhaseInfo[] PhaseInfos = new FMV.FMVInfo.PhaseInfo[3];
      public EndianBinaryReader er;
      public int Width;
      public int Height;
      public int NrTilesX;
      public int NrTilesY;
      public Bitmap LastFrame;

      public FMVInfo(FMV f, EndianBinaryReader er)
      {
        this.er = er;
        this.Width = (int) f.Header.Width;
        this.Height = (int) f.Header.Height;
        this.NrTilesX = this.Width / 16;
        this.NrTilesY = this.Height / 16;
        this.PhaseInfos[0] = new FMV.FMVInfo.PhaseInfo(this, 2, 2, 0, 0, 0);
        this.PhaseInfos[1] = new FMV.FMVInfo.PhaseInfo(this, 1, 1, 1, 1, 1);
        this.PhaseInfos[2] = new FMV.FMVInfo.PhaseInfo(this, 1, 1, 1, 1, 1);
      }

      public class PhaseInfo
      {
        public int NrBlocksX;
        public int NrBlocksY;
        public int QTIdx;
        public int Table1Idx;
        public int Table2Idx;
        public int Field10;
        public int PhaseDataWidth;
        public int PhaseDataHeight;
        public byte[] PhaseData;
        public byte[] PhaseDataOld;

        public PhaseInfo(
          FMV.FMVInfo Info,
          int NrBlocksX,
          int NrBlocksY,
          int QTIdx,
          int Table1Idx,
          int Table2Idx)
        {
          this.NrBlocksX = NrBlocksX;
          this.NrBlocksY = NrBlocksY;
          this.QTIdx = QTIdx;
          this.Table1Idx = Table1Idx;
          this.Table2Idx = Table2Idx;
          this.Field10 = 0;
          this.PhaseDataWidth = 8 * Info.NrTilesX * NrBlocksX;
          this.PhaseDataHeight = 8 * Info.NrTilesY * NrBlocksY;
          this.PhaseData = new byte[this.PhaseDataWidth * this.PhaseDataHeight];
          this.PhaseDataOld = new byte[this.PhaseDataWidth * this.PhaseDataHeight];
        }
      }
    }
  }
}
