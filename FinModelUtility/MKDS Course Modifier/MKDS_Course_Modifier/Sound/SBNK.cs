// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Sound.SBNK
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.G3D_Binary_File_Format;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.Sound
{
  public class SBNK
  {
    private static byte[] AttackTimeTable = new byte[19]
    {
      (byte) 0,
      (byte) 1,
      (byte) 5,
      (byte) 14,
      (byte) 26,
      (byte) 38,
      (byte) 51,
      (byte) 63,
      (byte) 73,
      (byte) 84,
      (byte) 92,
      (byte) 100,
      (byte) 109,
      (byte) 116,
      (byte) 123,
      (byte) 127,
      (byte) 132,
      (byte) 137,
      (byte) 143
    };
    private static ushort[] sustainLevTable = new ushort[128]
    {
      (ushort) 64813,
      (ushort) 64814,
      (ushort) 64815,
      (ushort) 64885,
      (ushort) 64935,
      (ushort) 64974,
      (ushort) 65006,
      (ushort) 65033,
      (ushort) 65056,
      (ushort) 65076,
      (ushort) 65094,
      (ushort) 65111,
      (ushort) 65126,
      (ushort) 65140,
      (ushort) 65153,
      (ushort) 65165,
      (ushort) 65176,
      (ushort) 65187,
      (ushort) 65197,
      (ushort) 65206,
      (ushort) 65215,
      (ushort) 65223,
      (ushort) 65231,
      (ushort) 65239,
      (ushort) 65247,
      (ushort) 65254,
      (ushort) 65260,
      (ushort) 65267,
      (ushort) 65273,
      (ushort) 65279,
      (ushort) 65285,
      (ushort) 65291,
      (ushort) 65297,
      (ushort) 65302,
      (ushort) 65307,
      (ushort) 65312,
      (ushort) 65317,
      (ushort) 65322,
      (ushort) 65326,
      (ushort) 65331,
      (ushort) 65335,
      (ushort) 65340,
      (ushort) 65344,
      (ushort) 65348,
      (ushort) 65352,
      (ushort) 65356,
      (ushort) 65360,
      (ushort) 65363,
      (ushort) 65367,
      (ushort) 65371,
      (ushort) 65374,
      (ushort) 65378,
      (ushort) 65381,
      (ushort) 65384,
      (ushort) 65387,
      (ushort) 65391,
      (ushort) 65394,
      (ushort) 65397,
      (ushort) 65400,
      (ushort) 65403,
      (ushort) 65406,
      (ushort) 65409,
      (ushort) 65411,
      (ushort) 65414,
      (ushort) 65417,
      (ushort) 65420,
      (ushort) 65422,
      (ushort) 65425,
      (ushort) 65427,
      (ushort) 65430,
      (ushort) 65433,
      (ushort) 65435,
      (ushort) 65437,
      (ushort) 65440,
      (ushort) 65442,
      (ushort) 65445,
      (ushort) 65447,
      (ushort) 65449,
      (ushort) 65451,
      (ushort) 65454,
      (ushort) 65456,
      (ushort) 65458,
      (ushort) 65460,
      (ushort) 65462,
      (ushort) 65464,
      (ushort) 65466,
      (ushort) 65468,
      (ushort) 65470,
      (ushort) 65472,
      (ushort) 65474,
      (ushort) 65476,
      (ushort) 65478,
      (ushort) 65480,
      (ushort) 65482,
      (ushort) 65484,
      (ushort) 65486,
      (ushort) 65487,
      (ushort) 65489,
      (ushort) 65491,
      (ushort) 65493,
      (ushort) 65494,
      (ushort) 65496,
      (ushort) 65498,
      (ushort) 65500,
      (ushort) 65501,
      (ushort) 65503,
      (ushort) 65505,
      (ushort) 65506,
      (ushort) 65508,
      (ushort) 65509,
      (ushort) 65511,
      (ushort) 65513,
      (ushort) 65514,
      (ushort) 65516,
      (ushort) 65517,
      (ushort) 65519,
      (ushort) 65520,
      (ushort) 65522,
      (ushort) 65523,
      (ushort) 65525,
      (ushort) 65526,
      (ushort) 65528,
      (ushort) 65529,
      (ushort) 65530,
      (ushort) 65532,
      (ushort) 65533,
      ushort.MaxValue,
      (ushort) 0
    };
    private static double INTR_FREQUENCY = 1.0 / 192.0;
    public const string Signature = "SBNK";
    public FileHeader.HeaderInfo Header;
    public SBNK.DataSection Data;

    public SBNK(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      bool OK;
      this.Header = new FileHeader.HeaderInfo(er, nameof (SBNK), out OK);
      if (!OK)
      {
        int num1 = (int) MessageBox.Show("Error 1");
      }
      else
      {
        this.Data = new SBNK.DataSection(er, out OK);
        if (!OK)
        {
          int num2 = (int) MessageBox.Show("Error 2");
        }
      }
      er.Close();
    }

    public static SBNK InitDLS(SBNK k, SWAR[] waves)
    {
      SBNK sbnk = k;
      for (int index1 = 0; (long) index1 < (long) sbnk.Data.nCount; ++index1)
      {
        if (sbnk.Data.Ins[index1].fRecord == (byte) 0)
        {
          SBNK.DataSection.SbnkInstrument.iRecord0 iInfo1 = (SBNK.DataSection.SbnkInstrument.iRecord0) sbnk.Data.Ins[index1].iInfo;
        }
        else if (sbnk.Data.Ins[index1].fRecord < (byte) 16)
        {
          SBNK.DataSection.SbnkInstrument.iRecord_16 iInfo2 = (SBNK.DataSection.SbnkInstrument.iRecord_16) sbnk.Data.Ins[index1].iInfo;
          if (sbnk.Data.Ins[index1].fRecord != (byte) 2 && sbnk.Data.Ins[index1].fRecord != (byte) 3 && sbnk.Data.Ins[index1].fRecord != (byte) 5)
          {
            iInfo2.WaveData = waves[(int) iInfo2.nrSwar][(int) iInfo2.nrSwav];
          }
          else
          {
            List<byte> byteList = new List<byte>();
            if (sbnk.Data.Ins[index1].fRecord == (byte) 2)
            {
              for (int index2 = 0; index2 < (int) iInfo2.nrSwav + 1; ++index2)
              {
                for (int index3 = 0; index3 < 21; ++index3)
                  byteList.Add((byte) 127);
              }
              for (int index2 = 0; index2 < 8 - ((int) iInfo2.nrSwav + 1); ++index2)
              {
                for (int index3 = 0; index3 < 21; ++index3)
                  byteList.Add((byte) 129);
              }
            }
            else
            {
              ushort num = (ushort) short.MaxValue;
              for (int index2 = 0; index2 < 352800; ++index2)
              {
                if (((int) num & 1) != 0)
                {
                  num = (ushort) ((int) num >> 1 ^ 24576);
                  byteList.Add((byte) 129);
                }
                else
                {
                  num >>= 1;
                  byteList.Add((byte) 127);
                }
              }
            }
            iInfo2.WaveData = new SWAV(new SWAV.SWAVInfo()
            {
              bLoop = (byte) 1,
              nNonLoopLen = (uint) byteList.Count / 4U,
              nLoopOffset = (ushort) 0,
              nTime = (ushort) (44100 / byteList.Count),
              nWaveType = (byte) 0,
              nSampleRate = (ushort) 44100
            }, byteList.ToArray());
          }
          byte num1 = iInfo2.AttackRate < (byte) 109 ? (byte) ((uint) byte.MaxValue - (uint) iInfo2.AttackRate) : SBNK.AttackTimeTable[(int) sbyte.MaxValue - (int) iInfo2.AttackRate];
          short fallingRate1 = (short) SBNK.GetFallingRate(iInfo2.DecayRate);
          short fallingRate2 = (short) SBNK.GetFallingRate(iInfo2.ReleaseRate);
          int num2 = 0;
          for (long index2 = 92544; index2 != 0L; index2 = index2 * (long) num1 >> 8)
            ++num2;
          iInfo2.RealAttackRate = (double) num2 * SBNK.INTR_FREQUENCY;
          long num3 = iInfo2.SustainLevel != (byte) 127 ? (long) (65536 - (int) SBNK.sustainLevTable[(int) iInfo2.SustainLevel] << 7) : 0L;
          if (iInfo2.DecayRate == (byte) 127)
          {
            iInfo2.RealDecayRate = 0.001;
          }
          else
          {
            int num4 = 92544 / (int) fallingRate1;
            iInfo2.RealDecayRate = (double) num4 * SBNK.INTR_FREQUENCY;
          }
          iInfo2.RealSustainLevel = num3 != 0L ? (double) (92544L - num3) / 92544.0 : 1.0;
          int num5 = 92544 / (int) fallingRate2;
          iInfo2.RealReleaseRate = (double) num5 * SBNK.INTR_FREQUENCY;
          iInfo2.RealPan = iInfo2.Pan != (byte) 0 ? (iInfo2.Pan != (byte) 127 ? (iInfo2.Pan != (byte) 64 ? ((double) iInfo2.Pan - 63.5) / 64.0 : 0.0) : 1.0) : -1.0;
          if (iInfo2.RealReleaseRate < 0.001)
            iInfo2.RealReleaseRate = 0.001;
          if (iInfo2.RealDecayRate < 0.001)
            iInfo2.RealDecayRate = 0.001;
          if (iInfo2.RealAttackRate < 0.001)
            iInfo2.RealAttackRate = 0.001;
          long num6 = (long) Math.Ceiling(Math.Log(iInfo2.RealAttackRate) / Math.Log(2.0) * 1200.0 * 65536.0);
          long num7 = (long) Math.Ceiling(Math.Log(iInfo2.RealDecayRate) / Math.Log(2.0) * 1200.0 * 65536.0);
          long num8 = (long) (65536000.0 * iInfo2.RealSustainLevel);
          long num9 = (long) Math.Ceiling(Math.Log(iInfo2.RealReleaseRate) / Math.Log(2.0) * 1200.0 * 65536.0);
          long num10 = 0;
          if (iInfo2.RealPan != 0.0)
            num10 = (long) (iInfo2.RealPan * 32768000.0);
          iInfo2.RealPan = (double) num10;
          iInfo2.RealAttackRate = (double) num6;
          iInfo2.RealDecayRate = (double) num7;
          iInfo2.RealReleaseRate = (double) num9;
          iInfo2.RealSustainLevel = (double) num8;
          sbnk.Data.Ins[index1].iInfo = (object) iInfo2;
        }
        else if (sbnk.Data.Ins[index1].fRecord == (byte) 16)
        {
          SBNK.DataSection.SbnkInstrument.iRecord16 iInfo2 = (SBNK.DataSection.SbnkInstrument.iRecord16) sbnk.Data.Ins[index1].iInfo;
          int num1 = (int) iInfo2.uNote - (int) iInfo2.lNote + 1;
          for (int index2 = 0; index2 < num1; ++index2)
          {
            iInfo2.Info[index2].WaveData = waves[(int) iInfo2.Info[index2].nrSwar][(int) iInfo2.Info[index2].nrSwav];
            byte num2 = iInfo2.Info[index2].AttackRate < (byte) 109 ? (byte) ((uint) byte.MaxValue - (uint) iInfo2.Info[index2].AttackRate) : SBNK.AttackTimeTable[(int) sbyte.MaxValue - (int) iInfo2.Info[index2].AttackRate];
            short fallingRate1 = (short) SBNK.GetFallingRate(iInfo2.Info[index2].DecayRate);
            short fallingRate2 = (short) SBNK.GetFallingRate(iInfo2.Info[index2].ReleaseRate);
            int num3 = 0;
            for (long index3 = 92544; index3 != 0L; index3 = index3 * (long) num2 >> 8)
              ++num3;
            iInfo2.Info[index2].RealAttackRate = (double) num3 * SBNK.INTR_FREQUENCY;
            long num4 = iInfo2.Info[index2].SustainLevel != (byte) 127 ? (long) (65536 - (int) SBNK.sustainLevTable[(int) iInfo2.Info[index2].SustainLevel] << 7) : 0L;
            if (iInfo2.Info[index2].DecayRate == (byte) 127)
            {
              iInfo2.Info[index2].RealDecayRate = 0.001;
            }
            else
            {
              int num5 = 92544 / (int) fallingRate1;
              iInfo2.Info[index2].RealDecayRate = (double) num5 * SBNK.INTR_FREQUENCY;
            }
            iInfo2.Info[index2].RealSustainLevel = num4 != 0L ? (double) (92544L - num4) / 92544.0 : 1.0;
            int num6 = 92544 / (int) fallingRate2;
            iInfo2.Info[index2].RealReleaseRate = (double) num6 * SBNK.INTR_FREQUENCY;
            iInfo2.Info[index2].RealPan = iInfo2.Info[index2].Pan != (byte) 0 ? (iInfo2.Info[index2].Pan != (byte) 127 ? (iInfo2.Info[index2].Pan != (byte) 64 ? ((double) iInfo2.Info[index2].Pan - 63.5) / 64.0 : 0.0) : 1.0) : -1.0;
            if (iInfo2.Info[index2].RealReleaseRate < 0.001)
              iInfo2.Info[index2].RealReleaseRate = 0.001;
            if (iInfo2.Info[index2].RealDecayRate < 0.001)
              iInfo2.Info[index2].RealDecayRate = 0.001;
            if (iInfo2.Info[index2].RealAttackRate < 0.001)
              iInfo2.Info[index2].RealAttackRate = 0.001;
            long num7 = (long) Math.Ceiling(Math.Log(iInfo2.Info[index2].RealAttackRate) / Math.Log(2.0) * 1200.0 * 65536.0);
            long num8 = (long) Math.Ceiling(Math.Log(iInfo2.Info[index2].RealDecayRate) / Math.Log(2.0) * 1200.0 * 65536.0);
            long num9 = (long) (65536000.0 * iInfo2.Info[index2].RealSustainLevel);
            long num10 = (long) Math.Ceiling(Math.Log(iInfo2.Info[index2].RealReleaseRate) / Math.Log(2.0) * 1200.0 * 65536.0);
            long num11 = 0;
            if (iInfo2.Info[index2].RealPan != 0.0)
              num11 = (long) (iInfo2.Info[index2].RealPan * 32768000.0);
            iInfo2.Info[index2].RealPan = (double) num11;
            iInfo2.Info[index2].RealAttackRate = (double) num7;
            iInfo2.Info[index2].RealDecayRate = (double) num8;
            iInfo2.Info[index2].RealReleaseRate = (double) num10;
            iInfo2.Info[index2].RealSustainLevel = (double) num9;
          }
          sbnk.Data.Ins[index1].iInfo = (object) iInfo2;
        }
        else if (sbnk.Data.Ins[index1].fRecord == (byte) 17)
        {
          SBNK.DataSection.SbnkInstrument.iRecord17 iInfo2 = (SBNK.DataSection.SbnkInstrument.iRecord17) sbnk.Data.Ins[index1].iInfo;
          for (int index2 = 0; index2 < iInfo2.nr; ++index2)
          {
            iInfo2.Info[index2].WaveData = waves[(int) iInfo2.Info[index2].nrSwar][(int) iInfo2.Info[index2].nrSwav];
            byte num1 = iInfo2.Info[index2].AttackRate < (byte) 109 ? (byte) ((uint) byte.MaxValue - (uint) iInfo2.Info[index2].AttackRate) : SBNK.AttackTimeTable[(int) sbyte.MaxValue - (int) iInfo2.Info[index2].AttackRate];
            short fallingRate1 = (short) SBNK.GetFallingRate(iInfo2.Info[index2].DecayRate);
            short fallingRate2 = (short) SBNK.GetFallingRate(iInfo2.Info[index2].ReleaseRate);
            int num2 = 0;
            for (long index3 = 92544; index3 != 0L; index3 = index3 * (long) num1 >> 8)
              ++num2;
            iInfo2.Info[index2].RealAttackRate = (double) num2 * SBNK.INTR_FREQUENCY;
            long num3 = iInfo2.Info[index2].SustainLevel != (byte) 127 ? (long) (65536 - (int) SBNK.sustainLevTable[(int) iInfo2.Info[index2].SustainLevel] << 7) : 0L;
            if (iInfo2.Info[index2].DecayRate == (byte) 127)
            {
              iInfo2.Info[index2].RealDecayRate = 0.001;
            }
            else
            {
              int num4 = 92544 / (int) fallingRate1;
              iInfo2.Info[index2].RealDecayRate = (double) num4 * SBNK.INTR_FREQUENCY;
            }
            iInfo2.Info[index2].RealSustainLevel = num3 != 0L ? (double) (92544L - num3) / 92544.0 : 1.0;
            int num5 = 92544 / (int) fallingRate2;
            iInfo2.Info[index2].RealReleaseRate = (double) num5 * SBNK.INTR_FREQUENCY;
            iInfo2.Info[index2].RealPan = iInfo2.Info[index2].Pan != (byte) 0 ? (iInfo2.Info[index2].Pan != (byte) 127 ? (iInfo2.Info[index2].Pan != (byte) 64 ? ((double) iInfo2.Info[index2].Pan - 63.5) / 64.0 : 0.0) : 1.0) : -1.0;
            if (iInfo2.Info[index2].RealReleaseRate < 0.001)
              iInfo2.Info[index2].RealReleaseRate = 0.001;
            if (iInfo2.Info[index2].RealDecayRate < 0.001)
              iInfo2.Info[index2].RealDecayRate = 0.001;
            if (iInfo2.Info[index2].RealAttackRate < 0.001)
              iInfo2.Info[index2].RealAttackRate = 0.001;
            long num6 = (long) Math.Ceiling(Math.Log(iInfo2.Info[index2].RealAttackRate) / Math.Log(2.0) * 1200.0 * 65536.0);
            long num7 = (long) Math.Ceiling(Math.Log(iInfo2.Info[index2].RealDecayRate) / Math.Log(2.0) * 1200.0 * 65536.0);
            long num8 = (long) (65536000.0 * iInfo2.Info[index2].RealSustainLevel);
            long num9 = (long) Math.Ceiling(Math.Log(iInfo2.Info[index2].RealReleaseRate) / Math.Log(2.0) * 1200.0 * 65536.0);
            long num10 = 0;
            if (iInfo2.Info[index2].RealPan != 0.0)
              num10 = (long) (iInfo2.Info[index2].RealPan * 32768000.0);
            iInfo2.Info[index2].RealPan = (double) num10;
            iInfo2.Info[index2].RealAttackRate = (double) num6;
            iInfo2.Info[index2].RealDecayRate = (double) num7;
            iInfo2.Info[index2].RealReleaseRate = (double) num9;
            iInfo2.Info[index2].RealSustainLevel = (double) num8;
          }
          sbnk.Data.Ins[index1].iInfo = (object) iInfo2;
        }
      }
      return sbnk;
    }

    private static ushort GetFallingRate(byte DecayTime)
    {
      ulong num1;
      switch (DecayTime)
      {
        case 126:
          num1 = 15360UL;
          break;
        case 127:
          num1 = (ulong) ushort.MaxValue;
          break;
        default:
          if (DecayTime < (byte) 50)
          {
            num1 = (ulong) DecayTime * 2UL + 1UL & (ulong) ushort.MaxValue;
            break;
          }
          ulong num2 = 7680;
          DecayTime = (byte) (126U - (uint) DecayTime);
          num1 = num2 / (ulong) DecayTime & (ulong) ushort.MaxValue;
          break;
      }
      return (ushort) num1;
    }

    public static byte[] ToDLS(SBNK s)
    {
      List<KeyValuePair<int, int>> keyValuePairList = new List<KeyValuePair<int, int>>();
      List<WAV> wavList = new List<WAV>();
      int num1 = 0;
      for (int index1 = 0; index1 < s.Data.Ins.Length; ++index1)
      {
        if (s.Data.Ins[index1].fRecord == (byte) 0)
        {
          SBNK.DataSection.SbnkInstrument.iRecord0 iInfo1 = (SBNK.DataSection.SbnkInstrument.iRecord0) s.Data.Ins[index1].iInfo;
        }
        else if (s.Data.Ins[index1].fRecord < (byte) 16)
        {
          ++num1;
          SBNK.DataSection.SbnkInstrument.iRecord_16 iInfo2 = (SBNK.DataSection.SbnkInstrument.iRecord_16) s.Data.Ins[index1].iInfo;
          if (s.Data.Ins[index1].fRecord == (byte) 1 && !keyValuePairList.Contains(new KeyValuePair<int, int>((int) iInfo2.nrSwar, (int) iInfo2.nrSwav)))
          {
            keyValuePairList.Add(new KeyValuePair<int, int>((int) iInfo2.nrSwar, (int) iInfo2.nrSwav));
            wavList.Add(iInfo2.WaveData.ToWave());
          }
          else if (s.Data.Ins[index1].fRecord > (byte) 1 && !keyValuePairList.Contains(new KeyValuePair<int, int>((int) -s.Data.Ins[index1].fRecord, (int) iInfo2.nrSwav)))
          {
            keyValuePairList.Add(new KeyValuePair<int, int>((int) -s.Data.Ins[index1].fRecord, (int) iInfo2.nrSwav));
            wavList.Add(iInfo2.WaveData.ToWave());
          }
        }
        else if (s.Data.Ins[index1].fRecord == (byte) 16)
        {
          ++num1;
          SBNK.DataSection.SbnkInstrument.iRecord16 iInfo2 = (SBNK.DataSection.SbnkInstrument.iRecord16) s.Data.Ins[index1].iInfo;
          int num2 = (int) iInfo2.uNote - (int) iInfo2.lNote + 1;
          for (int index2 = 0; index2 < num2; ++index2)
          {
            if (!keyValuePairList.Contains(new KeyValuePair<int, int>((int) iInfo2.Info[index2].nrSwar, (int) iInfo2.Info[index2].nrSwav)))
            {
              keyValuePairList.Add(new KeyValuePair<int, int>((int) iInfo2.Info[index2].nrSwar, (int) iInfo2.Info[index2].nrSwav));
              wavList.Add(iInfo2.Info[index2].WaveData.ToWave());
            }
          }
        }
        else if (s.Data.Ins[index1].fRecord == (byte) 17)
        {
          ++num1;
          SBNK.DataSection.SbnkInstrument.iRecord17 iInfo2 = (SBNK.DataSection.SbnkInstrument.iRecord17) s.Data.Ins[index1].iInfo;
          for (int index2 = 0; index2 < iInfo2.nr; ++index2)
          {
            if (!keyValuePairList.Contains(new KeyValuePair<int, int>((int) iInfo2.Info[index2].nrSwar, (int) iInfo2.Info[index2].nrSwav)))
            {
              keyValuePairList.Add(new KeyValuePair<int, int>((int) iInfo2.Info[index2].nrSwar, (int) iInfo2.Info[index2].nrSwav));
              wavList.Add(iInfo2.Info[index2].WaveData.ToWave());
            }
          }
        }
      }
      MemoryStream memoryStream = new MemoryStream();
      EndianBinaryWriter endianBinaryWriter = new EndianBinaryWriter((Stream) memoryStream, Endianness.LittleEndian);
      List<long> source = new List<long>();
      endianBinaryWriter.Write("RIFF", Encoding.ASCII, false);
      source.Add(endianBinaryWriter.BaseStream.Position);
      endianBinaryWriter.Write(0);
      endianBinaryWriter.Write("DLS colh", Encoding.ASCII, false);
      endianBinaryWriter.Write(4);
      endianBinaryWriter.Write(num1);
      endianBinaryWriter.Write("LIST", Encoding.ASCII, false);
      source.Add(endianBinaryWriter.BaseStream.Position);
      endianBinaryWriter.Write(0);
      endianBinaryWriter.Write("lins", Encoding.ASCII, false);
      for (int index1 = 0; (long) index1 < (long) s.Data.nCount; ++index1)
      {
        if (s.Data.Ins[index1].fRecord != (byte) 0)
        {
          endianBinaryWriter.Write("LIST", Encoding.ASCII, false);
          source.Add(endianBinaryWriter.BaseStream.Position);
          endianBinaryWriter.Write(0);
          endianBinaryWriter.Write("ins ", Encoding.ASCII, false);
          endianBinaryWriter.Write("insh", Encoding.ASCII, false);
          endianBinaryWriter.Write(12);
          int bLoop;
          int num2;
          if (s.Data.Ins[index1].fRecord < (byte) 16)
          {
            SBNK.DataSection.SbnkInstrument.iRecord_16 iInfo = (SBNK.DataSection.SbnkInstrument.iRecord_16) s.Data.Ins[index1].iInfo;
            endianBinaryWriter.Write(1);
            endianBinaryWriter.Write(0);
            endianBinaryWriter.Write(index1);
            endianBinaryWriter.Write("LIST", Encoding.ASCII, false);
            source.Add(endianBinaryWriter.BaseStream.Position);
            endianBinaryWriter.Write(0);
            endianBinaryWriter.Write("lrgn", Encoding.ASCII, false);
            endianBinaryWriter.Write("LIST", Encoding.ASCII, false);
            source.Add(endianBinaryWriter.BaseStream.Position);
            endianBinaryWriter.Write(0);
            endianBinaryWriter.Write("rgn2", Encoding.ASCII, false);
            endianBinaryWriter.Write("rgnh", Encoding.ASCII, false);
            endianBinaryWriter.Write(14);
            endianBinaryWriter.Write((short) 0);
            endianBinaryWriter.Write((short) sbyte.MaxValue);
            endianBinaryWriter.Write((short) 0);
            endianBinaryWriter.Write((short) sbyte.MaxValue);
            endianBinaryWriter.Write((short) 1);
            endianBinaryWriter.Write((short) 0);
            endianBinaryWriter.Write((short) 1);
            endianBinaryWriter.Write("wsmp", Encoding.ASCII, false);
            source.Add(endianBinaryWriter.BaseStream.Position);
            endianBinaryWriter.Write(0);
            endianBinaryWriter.Write(20);
            endianBinaryWriter.Write((short) iInfo.Note);
            endianBinaryWriter.Write((short) 0);
            endianBinaryWriter.Write(0);
            endianBinaryWriter.Write(1);
            endianBinaryWriter.Write((int) iInfo.WaveData.Data.Info.bLoop);
            if (iInfo.WaveData.Data.Info.bLoop == (byte) 1)
            {
              endianBinaryWriter.Write(16);
              int num3 = (int) iInfo.WaveData.Data.Info.nNonLoopLen * 4;
              int num4 = (int) iInfo.WaveData.Data.Info.nLoopOffset * 4;
              int num5 = iInfo.WaveData.Data.Info.nWaveType != (byte) 2 ? num4 + num3 : num4 + num3 - 4;
              int num6 = 0;
              if (iInfo.WaveData.Data.Info.nWaveType == (byte) 2)
              {
                num6 = 1;
                num4 = num4 * 2 - 8 + 1;
                num3 = num5 * 2 + 1 - num4;
              }
              int num7 = (iInfo.WaveData.Data.Info.nWaveType == (byte) 0 ? 8 : 16) / 8;
              double num8 = 1.0;
              bLoop = (int) iInfo.WaveData.Data.Info.bLoop;
              num2 = 0;
              int num9 = num6 == 0 ? num4 * (int) num8 / num7 : num4;
              int num10 = num6 == 0 ? num3 * (int) num8 / num7 : num3;
              endianBinaryWriter.Write(0);
              endianBinaryWriter.Write(num9 - 25);
              endianBinaryWriter.Write(num10);
            }
            long position1 = endianBinaryWriter.BaseStream.Position;
            endianBinaryWriter.BaseStream.Position = source.Last<long>();
            endianBinaryWriter.Write((int) (position1 - source.Last<long>() - 4L));
            endianBinaryWriter.BaseStream.Position = position1;
            source.Remove(source.Last<long>());
            endianBinaryWriter.Write("wlnk", Encoding.ASCII, false);
            endianBinaryWriter.Write(12);
            endianBinaryWriter.Write((short) 0);
            endianBinaryWriter.Write((short) 0);
            endianBinaryWriter.Write(1);
            if (s.Data.Ins[index1].fRecord == (byte) 1)
              endianBinaryWriter.Write(keyValuePairList.IndexOf(new KeyValuePair<int, int>((int) iInfo.nrSwar, (int) iInfo.nrSwav)));
            else
              endianBinaryWriter.Write(keyValuePairList.IndexOf(new KeyValuePair<int, int>((int) -s.Data.Ins[index1].fRecord, (int) iInfo.nrSwav)));
            endianBinaryWriter.Write("LIST", Encoding.ASCII, false);
            source.Add(endianBinaryWriter.BaseStream.Position);
            endianBinaryWriter.Write(0);
            endianBinaryWriter.Write("lar2", Encoding.ASCII, false);
            endianBinaryWriter.Write("art2", Encoding.ASCII, false);
            source.Add(endianBinaryWriter.BaseStream.Position);
            endianBinaryWriter.Write(0);
            endianBinaryWriter.Write(8);
            endianBinaryWriter.Write(5);
            endianBinaryWriter.Write((short) 0);
            endianBinaryWriter.Write((short) 0);
            endianBinaryWriter.Write((short) 4);
            endianBinaryWriter.Write((short) 0);
            endianBinaryWriter.Write((uint) iInfo.RealPan);
            endianBinaryWriter.Write((short) 0);
            endianBinaryWriter.Write((short) 0);
            endianBinaryWriter.Write((short) 518);
            endianBinaryWriter.Write((short) 0);
            endianBinaryWriter.Write((uint) iInfo.RealAttackRate);
            endianBinaryWriter.Write((short) 0);
            endianBinaryWriter.Write((short) 0);
            endianBinaryWriter.Write((short) 519);
            endianBinaryWriter.Write((short) 0);
            endianBinaryWriter.Write((uint) iInfo.RealDecayRate);
            endianBinaryWriter.Write((short) 0);
            endianBinaryWriter.Write((short) 0);
            endianBinaryWriter.Write((short) 522);
            endianBinaryWriter.Write((short) 0);
            endianBinaryWriter.Write((uint) iInfo.RealSustainLevel);
            endianBinaryWriter.Write((short) 0);
            endianBinaryWriter.Write((short) 0);
            endianBinaryWriter.Write((short) 521);
            endianBinaryWriter.Write((short) 0);
            endianBinaryWriter.Write((uint) iInfo.RealReleaseRate);
            long position2 = endianBinaryWriter.BaseStream.Position;
            endianBinaryWriter.BaseStream.Position = source.Last<long>();
            endianBinaryWriter.Write((int) (position2 - source.Last<long>() - 4L));
            endianBinaryWriter.BaseStream.Position = position2;
            source.Remove(source.Last<long>());
            long position3 = endianBinaryWriter.BaseStream.Position;
            endianBinaryWriter.BaseStream.Position = source.Last<long>();
            endianBinaryWriter.Write((int) (position3 - source.Last<long>() - 4L));
            endianBinaryWriter.BaseStream.Position = position3;
            source.Remove(source.Last<long>());
            long position4 = endianBinaryWriter.BaseStream.Position;
            endianBinaryWriter.BaseStream.Position = source.Last<long>();
            endianBinaryWriter.Write((int) (position4 - source.Last<long>() - 4L));
            endianBinaryWriter.BaseStream.Position = position4;
            source.Remove(source.Last<long>());
            long position5 = endianBinaryWriter.BaseStream.Position;
            endianBinaryWriter.BaseStream.Position = source.Last<long>();
            endianBinaryWriter.Write((int) (position5 - source.Last<long>() - 4L));
            endianBinaryWriter.BaseStream.Position = position5;
            source.Remove(source.Last<long>());
          }
          else if (s.Data.Ins[index1].fRecord == (byte) 16)
          {
            SBNK.DataSection.SbnkInstrument.iRecord16 iInfo = (SBNK.DataSection.SbnkInstrument.iRecord16) s.Data.Ins[index1].iInfo;
            int num3 = (int) iInfo.uNote - (int) iInfo.lNote + 1;
            endianBinaryWriter.Write(num3);
            endianBinaryWriter.Write(index1 == (int) sbyte.MaxValue ? int.MinValue : 0);
            endianBinaryWriter.Write(index1);
            endianBinaryWriter.Write("LIST", Encoding.ASCII, false);
            source.Add(endianBinaryWriter.BaseStream.Position);
            endianBinaryWriter.Write(0);
            endianBinaryWriter.Write("lrgn", Encoding.ASCII, false);
            for (int index2 = 0; index2 < num3; ++index2)
            {
              endianBinaryWriter.Write("LIST", Encoding.ASCII, false);
              source.Add(endianBinaryWriter.BaseStream.Position);
              endianBinaryWriter.Write(0);
              endianBinaryWriter.Write("rgn2", Encoding.ASCII, false);
              endianBinaryWriter.Write("rgnh", Encoding.ASCII, false);
              endianBinaryWriter.Write(14);
              endianBinaryWriter.Write((short) ((int) iInfo.lNote + index2));
              endianBinaryWriter.Write((short) ((int) iInfo.lNote + index2));
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write((short) sbyte.MaxValue);
              endianBinaryWriter.Write((short) 1);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write((short) 1);
              endianBinaryWriter.Write("wsmp", Encoding.ASCII, false);
              source.Add(endianBinaryWriter.BaseStream.Position);
              endianBinaryWriter.Write(0);
              endianBinaryWriter.Write(20);
              endianBinaryWriter.Write((short) iInfo.Info[index2].Note);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write(0);
              endianBinaryWriter.Write(1);
              endianBinaryWriter.Write((int) iInfo.Info[index2].WaveData.Data.Info.bLoop);
              if (iInfo.Info[index2].WaveData.Data.Info.bLoop == (byte) 1)
              {
                endianBinaryWriter.Write(16);
                int num4 = (int) iInfo.Info[index2].WaveData.Data.Info.nNonLoopLen * 4;
                int num5 = (int) iInfo.Info[index2].WaveData.Data.Info.nLoopOffset * 4;
                int num6 = iInfo.Info[index2].WaveData.Data.Info.nWaveType != (byte) 2 ? num5 + num4 : num5 + num4 - 4;
                int num7 = 0;
                if (iInfo.Info[index2].WaveData.Data.Info.nWaveType == (byte) 2)
                {
                  num7 = 1;
                  num5 = num5 * 2 - 8 + 1;
                  num4 = num6 * 2 + 1 - num5;
                }
                int num8 = (iInfo.Info[index2].WaveData.Data.Info.nWaveType == (byte) 0 ? 8 : 16) / 8;
                double num9 = 1.0;
                bLoop = (int) iInfo.Info[index2].WaveData.Data.Info.bLoop;
                num2 = 0;
                int num10 = num7 == 0 ? num5 * (int) num9 / num8 : num5;
                int num11 = num7 == 0 ? num4 * (int) num9 / num8 : num4;
                endianBinaryWriter.Write(0);
                endianBinaryWriter.Write(num10);
                endianBinaryWriter.Write(num11);
              }
              long position1 = endianBinaryWriter.BaseStream.Position;
              endianBinaryWriter.BaseStream.Position = source.Last<long>();
              endianBinaryWriter.Write((int) (position1 - source.Last<long>() - 4L));
              endianBinaryWriter.BaseStream.Position = position1;
              source.Remove(source.Last<long>());
              endianBinaryWriter.Write("wlnk", Encoding.ASCII, false);
              endianBinaryWriter.Write(12);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write(1);
              endianBinaryWriter.Write(keyValuePairList.IndexOf(new KeyValuePair<int, int>((int) iInfo.Info[index2].nrSwar, (int) iInfo.Info[index2].nrSwav)));
              endianBinaryWriter.Write("LIST", Encoding.ASCII, false);
              source.Add(endianBinaryWriter.BaseStream.Position);
              endianBinaryWriter.Write(0);
              endianBinaryWriter.Write("lar2", Encoding.ASCII, false);
              endianBinaryWriter.Write("art2", Encoding.ASCII, false);
              source.Add(endianBinaryWriter.BaseStream.Position);
              endianBinaryWriter.Write(0);
              endianBinaryWriter.Write(8);
              endianBinaryWriter.Write(5);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write((short) 4);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write((uint) iInfo.Info[index2].RealPan);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write((short) 518);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write((uint) iInfo.Info[index2].RealAttackRate);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write((short) 519);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write((uint) iInfo.Info[index2].RealDecayRate);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write((short) 522);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write((uint) iInfo.Info[index2].RealSustainLevel);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write((short) 521);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write((uint) iInfo.Info[index2].RealReleaseRate);
              long position2 = endianBinaryWriter.BaseStream.Position;
              endianBinaryWriter.BaseStream.Position = source.Last<long>();
              endianBinaryWriter.Write((int) (position2 - source.Last<long>() - 4L));
              endianBinaryWriter.BaseStream.Position = position2;
              source.Remove(source.Last<long>());
              long position3 = endianBinaryWriter.BaseStream.Position;
              endianBinaryWriter.BaseStream.Position = source.Last<long>();
              endianBinaryWriter.Write((int) (position3 - source.Last<long>() - 4L));
              endianBinaryWriter.BaseStream.Position = position3;
              source.Remove(source.Last<long>());
              long position4 = endianBinaryWriter.BaseStream.Position;
              endianBinaryWriter.BaseStream.Position = source.Last<long>();
              endianBinaryWriter.Write((int) (position4 - source.Last<long>() - 4L));
              endianBinaryWriter.BaseStream.Position = position4;
              source.Remove(source.Last<long>());
            }
            long position = endianBinaryWriter.BaseStream.Position;
            endianBinaryWriter.BaseStream.Position = source.Last<long>();
            endianBinaryWriter.Write((int) (position - source.Last<long>() - 4L));
            endianBinaryWriter.BaseStream.Position = position;
            source.Remove(source.Last<long>());
          }
          else if (s.Data.Ins[index1].fRecord == (byte) 17)
          {
            SBNK.DataSection.SbnkInstrument.iRecord17 iInfo = (SBNK.DataSection.SbnkInstrument.iRecord17) s.Data.Ins[index1].iInfo;
            endianBinaryWriter.Write(iInfo.nr);
            endianBinaryWriter.Write(0);
            endianBinaryWriter.Write(index1);
            endianBinaryWriter.Write("LIST", Encoding.ASCII, false);
            source.Add(endianBinaryWriter.BaseStream.Position);
            endianBinaryWriter.Write(0);
            endianBinaryWriter.Write("lrgn", Encoding.ASCII, false);
            for (int index2 = 0; index2 < iInfo.nr; ++index2)
            {
              endianBinaryWriter.Write("LIST", Encoding.ASCII, false);
              source.Add(endianBinaryWriter.BaseStream.Position);
              endianBinaryWriter.Write(0);
              endianBinaryWriter.Write("rgn2", Encoding.ASCII, false);
              endianBinaryWriter.Write("rgnh", Encoding.ASCII, false);
              endianBinaryWriter.Write(14);
              endianBinaryWriter.Write(index2 == 0 ? (short) 0 : (short) ((int) iInfo.Regions[index2 - 1] + 1));
              endianBinaryWriter.Write((short) iInfo.Regions[index2]);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write((short) sbyte.MaxValue);
              endianBinaryWriter.Write((short) 1);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write((short) 1);
              endianBinaryWriter.Write("wsmp", Encoding.ASCII, false);
              source.Add(endianBinaryWriter.BaseStream.Position);
              endianBinaryWriter.Write(0);
              endianBinaryWriter.Write(20);
              endianBinaryWriter.Write((short) iInfo.Info[index2].Note);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write(0);
              endianBinaryWriter.Write(1);
              endianBinaryWriter.Write((int) iInfo.Info[index2].WaveData.Data.Info.bLoop);
              if (iInfo.Info[index2].WaveData.Data.Info.bLoop == (byte) 1)
              {
                endianBinaryWriter.Write(16);
                int num3 = (int) iInfo.Info[index2].WaveData.Data.Info.nNonLoopLen * 4;
                int num4 = (int) iInfo.Info[index2].WaveData.Data.Info.nLoopOffset * 4;
                int num5 = iInfo.Info[index2].WaveData.Data.Info.nWaveType != (byte) 2 ? num4 + num3 : num4 + num3 - 4;
                int num6 = 0;
                if (iInfo.Info[index2].WaveData.Data.Info.nWaveType == (byte) 2)
                {
                  num6 = 1;
                  num4 = num4 * 2 - 8 + 1;
                  num3 = num5 * 2 + 1 - num4;
                }
                int num7 = (iInfo.Info[index2].WaveData.Data.Info.nWaveType == (byte) 0 ? 8 : 16) / 8;
                double num8 = 1.0;
                bLoop = (int) iInfo.Info[index2].WaveData.Data.Info.bLoop;
                num2 = 0;
                int num9 = num6 == 0 ? num4 * (int) num8 / num7 : num4;
                int num10 = num6 == 0 ? num3 * (int) num8 / num7 : num3;
                endianBinaryWriter.Write(0);
                endianBinaryWriter.Write(num9);
                endianBinaryWriter.Write(num10);
              }
              long position1 = endianBinaryWriter.BaseStream.Position;
              endianBinaryWriter.BaseStream.Position = source.Last<long>();
              endianBinaryWriter.Write((int) (position1 - source.Last<long>() - 4L));
              endianBinaryWriter.BaseStream.Position = position1;
              source.Remove(source.Last<long>());
              endianBinaryWriter.Write("wlnk", Encoding.ASCII, false);
              endianBinaryWriter.Write(12);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write(1);
              endianBinaryWriter.Write(keyValuePairList.IndexOf(new KeyValuePair<int, int>((int) iInfo.Info[index2].nrSwar, (int) iInfo.Info[index2].nrSwav)));
              endianBinaryWriter.Write("LIST", Encoding.ASCII, false);
              source.Add(endianBinaryWriter.BaseStream.Position);
              endianBinaryWriter.Write(0);
              endianBinaryWriter.Write("lar2", Encoding.ASCII, false);
              endianBinaryWriter.Write("art2", Encoding.ASCII, false);
              source.Add(endianBinaryWriter.BaseStream.Position);
              endianBinaryWriter.Write(0);
              endianBinaryWriter.Write(8);
              endianBinaryWriter.Write(5);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write((short) 4);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write((uint) iInfo.Info[index2].RealPan);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write((short) 518);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write((uint) iInfo.Info[index2].RealAttackRate);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write((short) 519);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write((uint) iInfo.Info[index2].RealDecayRate);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write((short) 522);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write((uint) iInfo.Info[index2].RealSustainLevel);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write((short) 521);
              endianBinaryWriter.Write((short) 0);
              endianBinaryWriter.Write((uint) iInfo.Info[index2].RealReleaseRate);
              long position2 = endianBinaryWriter.BaseStream.Position;
              endianBinaryWriter.BaseStream.Position = source.Last<long>();
              endianBinaryWriter.Write((int) (position2 - source.Last<long>() - 4L));
              endianBinaryWriter.BaseStream.Position = position2;
              source.Remove(source.Last<long>());
              long position3 = endianBinaryWriter.BaseStream.Position;
              endianBinaryWriter.BaseStream.Position = source.Last<long>();
              endianBinaryWriter.Write((int) (position3 - source.Last<long>() - 4L));
              endianBinaryWriter.BaseStream.Position = position3;
              source.Remove(source.Last<long>());
              long position4 = endianBinaryWriter.BaseStream.Position;
              endianBinaryWriter.BaseStream.Position = source.Last<long>();
              endianBinaryWriter.Write((int) (position4 - source.Last<long>() - 4L));
              endianBinaryWriter.BaseStream.Position = position4;
              source.Remove(source.Last<long>());
            }
            long position = endianBinaryWriter.BaseStream.Position;
            endianBinaryWriter.BaseStream.Position = source.Last<long>();
            endianBinaryWriter.Write((int) (position - source.Last<long>() - 4L));
            endianBinaryWriter.BaseStream.Position = position;
            source.Remove(source.Last<long>());
          }
          endianBinaryWriter.Write("LIST", Encoding.ASCII, false);
          source.Add(endianBinaryWriter.BaseStream.Position);
          endianBinaryWriter.Write(0);
          endianBinaryWriter.Write("INFO", Encoding.ASCII, false);
          endianBinaryWriter.Write("INAM", Encoding.ASCII, false);
          source.Add(endianBinaryWriter.BaseStream.Position);
          endianBinaryWriter.Write(0);
          endianBinaryWriter.Write("Unnamed Instrument", Encoding.ASCII, false);
          endianBinaryWriter.Write((short) 0);
          long position6 = endianBinaryWriter.BaseStream.Position;
          endianBinaryWriter.BaseStream.Position = source.Last<long>();
          endianBinaryWriter.Write((int) (position6 - source.Last<long>() - 4L));
          endianBinaryWriter.BaseStream.Position = position6;
          source.Remove(source.Last<long>());
          long position7 = endianBinaryWriter.BaseStream.Position;
          endianBinaryWriter.BaseStream.Position = source.Last<long>();
          endianBinaryWriter.Write((int) (position7 - source.Last<long>() - 4L));
          endianBinaryWriter.BaseStream.Position = position7;
          source.Remove(source.Last<long>());
          long position8 = endianBinaryWriter.BaseStream.Position;
          endianBinaryWriter.BaseStream.Position = source.Last<long>();
          endianBinaryWriter.Write((int) (position8 - source.Last<long>() - 4L));
          endianBinaryWriter.BaseStream.Position = position8;
          source.Remove(source.Last<long>());
        }
      }
      long position9 = endianBinaryWriter.BaseStream.Position;
      endianBinaryWriter.BaseStream.Position = source.Last<long>();
      endianBinaryWriter.Write((int) (position9 - source.Last<long>() - 4L));
      endianBinaryWriter.BaseStream.Position = position9;
      source.Remove(source.Last<long>());
      endianBinaryWriter.Write("ptbl", Encoding.ASCII, false);
      source.Add(endianBinaryWriter.BaseStream.Position);
      endianBinaryWriter.Write(0);
      endianBinaryWriter.Write(8);
      endianBinaryWriter.Write(keyValuePairList.Count);
      int num12 = 0;
      for (int index = 0; index < keyValuePairList.Count; ++index)
      {
        endianBinaryWriter.Write(num12);
        num12 += wavList[index].Wave.DATA.Data.Length + 12 + 8 + 18 + 8 + 12 + 8 + 14;
      }
      long position10 = endianBinaryWriter.BaseStream.Position;
      endianBinaryWriter.BaseStream.Position = source.Last<long>();
      endianBinaryWriter.Write((int) (position10 - source.Last<long>() - 4L));
      endianBinaryWriter.BaseStream.Position = position10;
      source.Remove(source.Last<long>());
      endianBinaryWriter.Write("LIST", Encoding.ASCII, false);
      source.Add(endianBinaryWriter.BaseStream.Position);
      endianBinaryWriter.Write(0);
      endianBinaryWriter.Write("wvpl", Encoding.ASCII, false);
      for (int index = 0; index < wavList.Count; ++index)
      {
        endianBinaryWriter.Write("LIST", Encoding.ASCII, false);
        source.Add(endianBinaryWriter.BaseStream.Position);
        endianBinaryWriter.Write(0);
        endianBinaryWriter.Write("wave", Encoding.ASCII, false);
        endianBinaryWriter.Write("fmt ", Encoding.ASCII, false);
        endianBinaryWriter.Write(18);
        endianBinaryWriter.Write(Convert.ToUInt16((object) wavList[index].Wave.FMT.AudioFormat));
        endianBinaryWriter.Write(wavList[index].Wave.FMT.NrChannel);
        endianBinaryWriter.Write(wavList[index].Wave.FMT.SampleRate);
        endianBinaryWriter.Write(wavList[index].Wave.FMT.ByteRate);
        endianBinaryWriter.Write(wavList[index].Wave.FMT.BlockAlign);
        endianBinaryWriter.Write(wavList[index].Wave.FMT.BitsPerSample);
        endianBinaryWriter.Write((short) 0);
        endianBinaryWriter.Write("data", Encoding.ASCII, false);
        endianBinaryWriter.Write(wavList[index].Wave.DATA.Header.size);
        endianBinaryWriter.Write(wavList[index].Wave.DATA.Data, 0, wavList[index].Wave.DATA.Data.Length);
        endianBinaryWriter.Write("LIST", Encoding.ASCII, false);
        source.Add(endianBinaryWriter.BaseStream.Position);
        endianBinaryWriter.Write(0);
        endianBinaryWriter.Write("INFO", Encoding.ASCII, false);
        endianBinaryWriter.Write("INAM", Encoding.ASCII, false);
        source.Add(endianBinaryWriter.BaseStream.Position);
        endianBinaryWriter.Write(0);
        endianBinaryWriter.Write("Unnamed Wave", Encoding.ASCII, false);
        endianBinaryWriter.Write((short) 0);
        long position1 = endianBinaryWriter.BaseStream.Position;
        endianBinaryWriter.BaseStream.Position = source.Last<long>();
        endianBinaryWriter.Write((int) (position1 - source.Last<long>() - 4L));
        endianBinaryWriter.BaseStream.Position = position1;
        source.Remove(source.Last<long>());
        long position2 = endianBinaryWriter.BaseStream.Position;
        endianBinaryWriter.BaseStream.Position = source.Last<long>();
        endianBinaryWriter.Write((int) (position2 - source.Last<long>() - 4L));
        endianBinaryWriter.BaseStream.Position = position2;
        source.Remove(source.Last<long>());
        long position3 = endianBinaryWriter.BaseStream.Position;
        endianBinaryWriter.BaseStream.Position = source.Last<long>();
        endianBinaryWriter.Write((int) (position3 - source.Last<long>() - 4L));
        endianBinaryWriter.BaseStream.Position = position3;
        source.Remove(source.Last<long>());
      }
      long position11 = endianBinaryWriter.BaseStream.Position;
      endianBinaryWriter.BaseStream.Position = source.Last<long>();
      endianBinaryWriter.Write((int) (position11 - source.Last<long>() - 4L));
      endianBinaryWriter.BaseStream.Position = position11;
      source.Remove(source.Last<long>());
      endianBinaryWriter.Write("LIST", Encoding.ASCII, false);
      source.Add(endianBinaryWriter.BaseStream.Position);
      endianBinaryWriter.Write(0);
      endianBinaryWriter.Write("INFO", Encoding.ASCII, false);
      endianBinaryWriter.Write("INAM", Encoding.ASCII, false);
      source.Add(endianBinaryWriter.BaseStream.Position);
      endianBinaryWriter.Write(0);
      endianBinaryWriter.Write("Unnamed Instrumentset", Encoding.ASCII, false);
      endianBinaryWriter.Write((short) 0);
      long position12 = endianBinaryWriter.BaseStream.Position;
      endianBinaryWriter.BaseStream.Position = source.Last<long>();
      endianBinaryWriter.Write((int) (position12 - source.Last<long>() - 4L));
      endianBinaryWriter.BaseStream.Position = position12;
      source.Remove(source.Last<long>());
      long position13 = endianBinaryWriter.BaseStream.Position;
      endianBinaryWriter.BaseStream.Position = source.Last<long>();
      endianBinaryWriter.Write((int) (position13 - source.Last<long>() - 4L));
      endianBinaryWriter.BaseStream.Position = position13;
      source.Remove(source.Last<long>());
      long position14 = endianBinaryWriter.BaseStream.Position;
      endianBinaryWriter.BaseStream.Position = source.Last<long>();
      endianBinaryWriter.Write((int) (position14 - source.Last<long>() - 4L));
      endianBinaryWriter.BaseStream.Position = position14;
      source.Remove(source.Last<long>());
      byte[] array = memoryStream.ToArray();
      endianBinaryWriter.Close();
      return array;
    }

    public class DataSection
    {
      public const string Signature = "DATA";
      public DataBlockHeader Header;
      public uint nCount;
      public SBNK.DataSection.SbnkInstrument[] Ins;

      public DataSection(EndianBinaryReader er, out bool OK)
      {
        bool OK1;
        this.Header = new DataBlockHeader(er, "DATA", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          er.ReadBytes(32);
          this.nCount = er.ReadUInt32();
          this.Ins = new SBNK.DataSection.SbnkInstrument[(IntPtr) this.nCount];
          for (int index = 0; (long) index < (long) this.nCount; ++index)
            this.Ins[index] = new SBNK.DataSection.SbnkInstrument(er);
          OK = true;
        }
      }

      public class SbnkInstrument
      {
        public byte fRecord;
        public short nOffset;
        public byte reserved;
        public object iInfo;

        public SbnkInstrument(EndianBinaryReader er)
        {
          this.fRecord = er.ReadByte();
          this.nOffset = er.ReadInt16();
          this.reserved = er.ReadByte();
          long position = er.BaseStream.Position;
          er.BaseStream.Position = (long) this.nOffset;
          if (this.fRecord == (byte) 0)
            this.iInfo = (object) new SBNK.DataSection.SbnkInstrument.iRecord0();
          else if (this.fRecord < (byte) 16)
            this.iInfo = (object) new SBNK.DataSection.SbnkInstrument.iRecord_16(er);
          else if (this.fRecord == (byte) 16)
            this.iInfo = (object) new SBNK.DataSection.SbnkInstrument.iRecord16(er);
          else if (this.fRecord == (byte) 17)
            this.iInfo = (object) new SBNK.DataSection.SbnkInstrument.iRecord17(er);
          er.BaseStream.Position = position;
        }

        public class iRecord0
        {
        }

        public class iRecord_16
        {
          public short nrSwav;
          public short nrSwar;
          public byte Note;
          public byte AttackRate;
          public double RealAttackRate;
          public byte DecayRate;
          public double RealDecayRate;
          public byte SustainLevel;
          public double RealSustainLevel;
          public byte ReleaseRate;
          public double RealReleaseRate;
          public byte Pan;
          public double RealPan;
          public SWAV WaveData;

          public iRecord_16(EndianBinaryReader er)
          {
            this.nrSwav = er.ReadInt16();
            this.nrSwar = er.ReadInt16();
            this.Note = er.ReadByte();
            this.AttackRate = er.ReadByte();
            this.DecayRate = er.ReadByte();
            this.SustainLevel = er.ReadByte();
            this.ReleaseRate = er.ReadByte();
            this.Pan = er.ReadByte();
          }
        }

        public class iRecord16
        {
          public byte lNote;
          public byte uNote;
          public SBNK.DataSection.SbnkInstrument.iRecord16.info[] Info;

          public iRecord16(EndianBinaryReader er)
          {
            this.lNote = er.ReadByte();
            this.uNote = er.ReadByte();
            int length = (int) this.uNote - (int) this.lNote + 1;
            this.Info = new SBNK.DataSection.SbnkInstrument.iRecord16.info[length];
            for (int index = 0; index < length; ++index)
              this.Info[index] = new SBNK.DataSection.SbnkInstrument.iRecord16.info(er);
          }

          public class info
          {
            public byte[] Unknown;
            public short nrSwav;
            public short nrSwar;
            public byte Note;
            public byte AttackRate;
            public double RealAttackRate;
            public byte DecayRate;
            public double RealDecayRate;
            public byte SustainLevel;
            public double RealSustainLevel;
            public byte ReleaseRate;
            public double RealReleaseRate;
            public byte Pan;
            public double RealPan;
            public SWAV WaveData;

            public info(EndianBinaryReader er)
            {
              this.Unknown = er.ReadBytes(2);
              this.nrSwav = er.ReadInt16();
              this.nrSwar = er.ReadInt16();
              this.Note = er.ReadByte();
              this.AttackRate = er.ReadByte();
              this.DecayRate = er.ReadByte();
              this.SustainLevel = er.ReadByte();
              this.ReleaseRate = er.ReadByte();
              this.Pan = er.ReadByte();
            }
          }
        }

        public class iRecord17
        {
          public byte[] Regions;
          public SBNK.DataSection.SbnkInstrument.iRecord17.info[] Info;
          public int nr;

          public iRecord17(EndianBinaryReader er)
          {
            this.Regions = new byte[8];
            for (int index = 0; index < 8; ++index)
            {
              this.Regions[index] = er.ReadByte();
              if (this.nr == 0 && this.Regions[index] == (byte) 0)
                this.nr = index;
            }
            this.Info = new SBNK.DataSection.SbnkInstrument.iRecord17.info[this.nr];
            for (int index = 0; index < this.nr; ++index)
              this.Info[index] = new SBNK.DataSection.SbnkInstrument.iRecord17.info(er);
          }

          public class info
          {
            public byte[] Unknown;
            public short nrSwav;
            public short nrSwar;
            public byte Note;
            public byte AttackRate;
            public double RealAttackRate;
            public byte DecayRate;
            public double RealDecayRate;
            public byte SustainLevel;
            public double RealSustainLevel;
            public byte ReleaseRate;
            public double RealReleaseRate;
            public byte Pan;
            public double RealPan;
            public SWAV WaveData;

            public info(EndianBinaryReader er)
            {
              this.Unknown = er.ReadBytes(2);
              this.nrSwav = er.ReadInt16();
              this.nrSwar = er.ReadInt16();
              this.Note = er.ReadByte();
              this.AttackRate = er.ReadByte();
              this.DecayRate = er.ReadByte();
              this.SustainLevel = er.ReadByte();
              this.ReleaseRate = er.ReadByte();
              this.Pan = er.ReadByte();
            }
          }
        }
      }
    }
  }
}
