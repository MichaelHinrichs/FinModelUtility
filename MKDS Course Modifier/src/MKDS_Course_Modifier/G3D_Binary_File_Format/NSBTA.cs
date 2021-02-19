// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTA
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.IO;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.G3D_Binary_File_Format
{
  public class NSBTA
  {
    public const string Signature = "BTA0";
    public FileHeader Header;
    public NSBTA.TexSRTAnmSet texSRTAnmSet;

    public NSBTA(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      bool OK;
      this.Header = new FileHeader(er, "BTA0", out OK);
      if (!OK)
      {
        int num1 = (int) MessageBox.Show("Error 0");
      }
      else
      {
        this.texSRTAnmSet = new NSBTA.TexSRTAnmSet(er, out OK);
        if (!OK)
        {
          int num2 = (int) MessageBox.Show("Error 1");
        }
      }
      er.ClearMarkers();
      er.Close();
    }

    public class TexSRTAnmSet
    {
      public const string Signature = "SRT0";
      public DataBlockHeader header;
      public Dictionary<NSBTA.TexSRTAnmSet.TexSRTAnmSetData> dict;
      public NSBTA.TexSRTAnmSet.TexSRTAnm[] texSRTAnm;

      public TexSRTAnmSet(EndianBinaryReader er, out bool OK)
      {
        er.SetMarkerOnCurrentOffset(nameof (TexSRTAnmSet));
        bool OK1;
        this.header = new DataBlockHeader(er, "SRT0", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.dict = new Dictionary<NSBTA.TexSRTAnmSet.TexSRTAnmSetData>(er);
          this.texSRTAnm = new NSBTA.TexSRTAnmSet.TexSRTAnm[(int) this.dict.numEntry];
          long position = er.BaseStream.Position;
          for (int index = 0; index < (int) this.dict.numEntry; ++index)
          {
            er.BaseStream.Position = er.GetMarker(nameof (TexSRTAnmSet)) + (long) this.dict[index].Value.Offset;
            this.texSRTAnm[index] = new NSBTA.TexSRTAnmSet.TexSRTAnm(er, out OK1);
            if (!OK1)
            {
              OK = false;
              return;
            }
          }
          er.BaseStream.Position = position;
          OK = true;
        }
      }

      public class TexSRTAnmSetData : DictionaryData
      {
        public uint Offset;

        public override void Read(EndianBinaryReader er)
        {
          this.Offset = er.ReadUInt32();
        }
      }

      public class TexSRTAnm
      {
        private ushort NNS_G3D_TEXSRTANM_OPTION_INTERPOLATION = 1;
        private ushort NNS_G3D_TEXSRTANM_OPTION_END_TO_START_INTERPOLATION = 2;
        public AnmHeader anmHeader;
        public ushort numFrame;
        public byte flag;
        public byte texMtxMode;
        public Dictionary<NSBTA.TexSRTAnmSet.TexSRTAnm.TexSRTAnmData> dict;

        public TexSRTAnm(EndianBinaryReader er, out bool OK)
        {
          er.SetMarkerOnCurrentOffset(nameof (TexSRTAnm));
          bool OK1;
          this.anmHeader = new AnmHeader(er, AnmHeader.Category0.M, AnmHeader.Category1.AT, out OK1);
          if (!OK1)
          {
            OK = false;
          }
          else
          {
            this.numFrame = er.ReadUInt16();
            er.SetMarker(nameof (numFrame), (long) this.numFrame);
            this.flag = er.ReadByte();
            this.texMtxMode = er.ReadByte();
            this.dict = new Dictionary<NSBTA.TexSRTAnmSet.TexSRTAnm.TexSRTAnmData>(er);
            er.RemoveMarker(nameof (TexSRTAnm));
            er.RemoveMarker(nameof (numFrame));
            OK = true;
          }
        }

        public int Contains(string Name)
        {
          for (int index = 0; index < (int) this.dict.numEntry; ++index)
          {
            if (this.dict[index].Key == Name)
              return index;
          }
          return -1;
        }

        public class TexSRTAnmData : DictionaryData
        {
          private uint NNS_G3D_TEXSRTANM_ELEM_STEP_MASK = 3221225472;
          private uint NNS_G3D_TEXSRTANM_ELEM_STEP_2 = 1073741824;
          private uint NNS_G3D_TEXSRTANM_ELEM_STEP_4 = 2147483648;
          private uint NNS_G3D_TEXSRTANM_ELEM_CONST = 536870912;
          private uint NNS_G3D_TEXSRTANM_ELEM_FX16 = 268435456;
          private uint NNS_G3D_TEXSRTANM_ELEM_LAST_INTERP_MASK = (uint) ushort.MaxValue;
          public uint scaleS;
          public int scaleSEx;
          public uint scaleT;
          public int scaleTEx;
          public uint rot;
          public uint rotEx;
          public uint transS;
          public int transSEx;
          public uint transT;
          public int transTEx;
          public float[] Ss;
          public float[] St;
          public uint[] r;
          public float[] Ts;
          public float[] Tt;

          public override void Read(EndianBinaryReader er)
          {
            this.scaleS = er.ReadUInt32();
            this.scaleSEx = er.ReadInt32();
            this.scaleT = er.ReadUInt32();
            this.scaleTEx = er.ReadInt32();
            this.rot = er.ReadUInt32();
            this.rotEx = er.ReadUInt32();
            this.transS = er.ReadUInt32();
            this.transSEx = er.ReadInt32();
            this.transT = er.ReadUInt32();
            this.transTEx = er.ReadInt32();
            int marker = (int) er.GetMarker("numFrame");
            long position = er.BaseStream.Position;
            if (((int) this.scaleS & (int) this.NNS_G3D_TEXSRTANM_ELEM_CONST) == 0)
            {
              er.BaseStream.Position = er.GetMarker(nameof (TexSRTAnm)) + (long) this.scaleSEx;
              int num = (int) ((this.scaleS & 3221225472U) >> 29);
              if (num == 0)
                ++num;
              this.Ss = ((int) this.scaleS & (int) this.NNS_G3D_TEXSRTANM_ELEM_FX16) == 0 ? er.ReadSingleInt32Exp12s(((int) this.scaleS & (int) this.NNS_G3D_TEXSRTANM_ELEM_LAST_INTERP_MASK) / num + num) : er.ReadSingleInt16Exp12s(((int) this.scaleS & (int) this.NNS_G3D_TEXSRTANM_ELEM_LAST_INTERP_MASK) / num + num);
            }
            if (((int) this.scaleT & (int) this.NNS_G3D_TEXSRTANM_ELEM_CONST) == 0)
            {
              er.BaseStream.Position = er.GetMarker(nameof (TexSRTAnm)) + (long) this.scaleTEx;
              int num = (int) ((this.scaleT & 3221225472U) >> 29);
              if (num == 0)
                ++num;
              this.St = ((int) this.scaleT & (int) this.NNS_G3D_TEXSRTANM_ELEM_FX16) == 0 ? er.ReadSingleInt32Exp12s(((int) this.scaleT & (int) this.NNS_G3D_TEXSRTANM_ELEM_LAST_INTERP_MASK) / num + num) : er.ReadSingleInt16Exp12s(((int) this.scaleT & (int) this.NNS_G3D_TEXSRTANM_ELEM_LAST_INTERP_MASK) / num + num);
            }
            if (((int) this.rot & (int) this.NNS_G3D_TEXSRTANM_ELEM_CONST) == 0)
            {
              er.BaseStream.Position = er.GetMarker(nameof (TexSRTAnm)) + (long) this.rotEx;
              int num = (int) ((this.rot & 3221225472U) >> 29);
              if (num == 0)
                ++num;
              this.r = er.ReadUInt32s(((int) this.rot & (int) this.NNS_G3D_TEXSRTANM_ELEM_LAST_INTERP_MASK) / num + num);
            }
            if (((int) this.transS & (int) this.NNS_G3D_TEXSRTANM_ELEM_CONST) == 0)
            {
              er.BaseStream.Position = er.GetMarker(nameof (TexSRTAnm)) + (long) this.transSEx;
              int num = (int) ((this.transS & 3221225472U) >> 29);
              if (num == 0)
                ++num;
              this.Ts = ((int) this.transS & (int) this.NNS_G3D_TEXSRTANM_ELEM_FX16) == 0 ? er.ReadSingleInt32Exp12s(((int) this.transS & (int) this.NNS_G3D_TEXSRTANM_ELEM_LAST_INTERP_MASK) / num + num) : er.ReadSingleInt16Exp12s(((int) this.transS & (int) this.NNS_G3D_TEXSRTANM_ELEM_LAST_INTERP_MASK) / num + num);
            }
            if (((int) this.transT & (int) this.NNS_G3D_TEXSRTANM_ELEM_CONST) == 0)
            {
              er.BaseStream.Position = er.GetMarker(nameof (TexSRTAnm)) + (long) this.transTEx;
              int num = (int) ((this.transT & 3221225472U) >> 29);
              if (num == 0)
                ++num;
              this.Tt = ((int) this.transT & (int) this.NNS_G3D_TEXSRTANM_ELEM_FX16) == 0 ? er.ReadSingleInt32Exp12s(((int) this.transT & (int) this.NNS_G3D_TEXSRTANM_ELEM_LAST_INTERP_MASK) / num + num) : er.ReadSingleInt16Exp12s(((int) this.transT & (int) this.NNS_G3D_TEXSRTANM_ELEM_LAST_INTERP_MASK) / num + num);
            }
            er.BaseStream.Position = position;
          }

          public int GetInterpolation(string element)
          {
            switch (element)
            {
              case "Scale S":
                if (((int) this.scaleS & (int) this.NNS_G3D_TEXSRTANM_ELEM_CONST) != 0)
                  return 0;
                int num1 = (int) ((this.scaleS & 3221225472U) >> 29);
                if (num1 == 0)
                  ++num1;
                return num1;
              case "Scale T":
                if (((int) this.scaleT & (int) this.NNS_G3D_TEXSRTANM_ELEM_CONST) != 0)
                  return 0;
                int num2 = (int) ((this.scaleT & 3221225472U) >> 29);
                if (num2 == 0)
                  ++num2;
                return num2;
              case "Rotation":
                if (((int) this.rot & (int) this.NNS_G3D_TEXSRTANM_ELEM_CONST) != 0)
                  return 0;
                int num3 = (int) ((this.rot & 3221225472U) >> 29);
                if (num3 == 0)
                  ++num3;
                return num3;
              case "Translation S":
                if (((int) this.transS & (int) this.NNS_G3D_TEXSRTANM_ELEM_CONST) != 0)
                  return 0;
                int num4 = (int) ((this.transS & 3221225472U) >> 29);
                if (num4 == 0)
                  ++num4;
                return num4;
              case "Translation T":
                if (((int) this.transT & (int) this.NNS_G3D_TEXSRTANM_ELEM_CONST) != 0)
                  return 0;
                int num5 = (int) ((this.transT & 3221225472U) >> 29);
                if (num5 == 0)
                  ++num5;
                return num5;
              default:
                return 1;
            }
          }

          public float[] GetMatrix(int Frame, int origWidth, int origHeight)
          {
            float num1 = this.GetValue(this.scaleS, this.scaleSEx, this.Ss, Frame);
            float num2 = this.GetValue(this.scaleT, this.scaleTEx, this.St, Frame);
            uint sinCosValue = this.GetSinCosValue(this.rot, this.rotEx, this.r, Frame);
            float num3 = (float) (sinCosValue & (uint) ushort.MaxValue) / 4096f;
            float num4 = (float) (sinCosValue >> 16) / 4096f;
            float num5 = this.GetValue(this.transS, this.transSEx, this.Ts, Frame);
            float num6 = this.GetValue(this.transT, this.transTEx, this.Tt, Frame);
            MTX44 mtX44 = new MTX44();
            mtX44.Zero();
            bool flag1 = this.Ss != null || this.St != null || (double) num1 != 1.0 || (double) num2 != 1.0;
            bool flag2 = this.r != null || (double) num3 != 0.0 || (double) num4 != 1.0;
            bool flag3 = this.Ts != null || this.Tt != null || (double) num5 != 0.0 || (double) num6 != 0.0;
            if (!flag1 && !flag2 && !flag3)
            {
              mtX44[0, 0] = 1f;
              mtX44[1, 0] = 0.0f;
              mtX44[0, 1] = 0.0f;
              mtX44[1, 1] = 1f;
              mtX44[0, 3] = 0.0f;
              mtX44[1, 3] = 0.0f;
            }
            else if (!flag1 && !flag2 && flag3)
            {
              mtX44[0, 0] = 1f;
              mtX44[1, 1] = 1f;
              mtX44[1, 0] = 0.0f;
              mtX44[0, 3] = (float) (-((double) num5 * (double) origWidth) * 16.0);
              mtX44[1, 3] = (float) ((double) num6 * (double) origHeight * 16.0);
              mtX44[0, 1] = 0.0f;
            }
            else if (!flag1 && flag2 && !flag3)
            {
              float num7 = (float) origWidth * 4096f;
              float num8 = (float) origHeight * 4096f;
              float num9 = num8 / num7;
              mtX44[0, 0] = num4;
              mtX44[1, 1] = num4;
              mtX44[1, 0] = (float) (-(double) num3 * (double) num9 / 4096.0);
              float num10 = num7 / num8;
              mtX44[0, 3] = (float) ((-(double) num3 - (double) num4 + 1.0) * (double) origWidth * 8.0);
              mtX44[1, 3] = (float) (((double) num3 - (double) num4 + 1.0) * (double) origHeight * 8.0);
              mtX44[0, 1] = (float) ((double) num3 * (double) num10 / 4096.0);
            }
            else if (!flag1 && flag2 && flag3)
            {
              float num7 = (float) origWidth * 4096f;
              float num8 = (float) origHeight * 4096f;
              float num9 = num8 / num7;
              mtX44[0, 0] = num4;
              mtX44[1, 1] = num4;
              mtX44[1, 0] = (float) (-(double) num3 * (double) num9 / 4096.0);
              float num10 = num7 / num8;
              mtX44[0, 3] = (float) ((-(double) num3 - (double) num4 + 1.0) * (double) origWidth * 8.0 - (double) num5 * (double) origWidth * 16.0);
              mtX44[1, 3] = (float) (((double) num3 - (double) num4 + 1.0) * (double) origHeight * 8.0 + (double) num6 * (double) origHeight * 16.0);
              mtX44[0, 1] = (float) ((double) num3 * (double) num10 / 4096.0);
            }
            else if (flag1 && !flag2 && !flag3)
            {
              mtX44[0, 0] = num1;
              mtX44[1, 1] = num2;
              mtX44[1, 0] = 0.0f;
              mtX44[0, 3] = 0.0f;
              mtX44[1, 3] = (float) ((-2.0 * (double) num2 + 2.0) * (double) origHeight * 8.0);
              mtX44[0, 1] = 0.0f;
            }
            else if (flag1 && !flag2 && flag3)
            {
              mtX44[0, 0] = num1;
              mtX44[1, 1] = num2;
              mtX44[1, 0] = 0.0f;
              mtX44[0, 3] = (float) -((double) num1 * (double) num5 / (double) byte.MaxValue) * (float) origWidth;
              mtX44[1, 3] = (float) ((-(double) num2 - (double) num2 + 2.0) * (double) origHeight * 8.0 + (double) num2 * (double) num6 / (double) byte.MaxValue * (double) origHeight);
              mtX44[0, 1] = 0.0f;
            }
            else if (flag1 && flag2 && !flag3)
            {
              float num7 = (float) origWidth * 4096f;
              float num8 = (float) origHeight * 4096f;
              float num9 = num8 / num7;
              float num10 = (float) ((double) num1 * (double) num3 / 4096.0);
              float num11 = (float) ((double) num1 * (double) num4 / 4096.0);
              float num12 = (float) ((double) num2 * (double) num3 / 4096.0);
              float num13 = (float) ((double) num2 * (double) num4 / 4096.0);
              mtX44[0, 0] = num11;
              mtX44[1, 1] = num13;
              mtX44[0, 1] = (float) (-(double) num12 * (double) num9 / 4096.0);
              float num14 = num7 / num8;
              mtX44[0, 3] = (float) ((-(double) num10 - (double) num11 + (double) num1) * (double) origWidth * 8.0);
              mtX44[1, 3] = (float) (((double) num12 - (double) num13 - (double) num2 + 2.0) * (double) origHeight * 8.0);
              mtX44[0, 1] = (float) ((double) num10 * (double) num14 / 4096.0);
            }
            else if (flag1 && flag2 && flag3)
            {
              float num7 = (float) origWidth * 4096f;
              float num8 = (float) origHeight * 4096f;
              float num9 = num8 / num7;
              float num10 = (float) ((double) num1 * (double) num3 / 4096.0);
              float num11 = (float) ((double) num1 * (double) num4 / 4096.0);
              float num12 = (float) ((double) num2 * (double) num3 / 4096.0);
              float num13 = (float) ((double) num2 * (double) num4 / 4096.0);
              mtX44[0, 0] = num11;
              mtX44[1, 1] = num13;
              mtX44[1, 0] = (float) (-(double) num12 * (double) num9 / 4096.0);
              float num14 = num7 / num8;
              mtX44[0, 3] = (float) ((-(double) num10 - (double) num11 + (double) num1) * (double) origWidth * 8.0 - (double) num1 * (double) num5 / (double) byte.MaxValue * (double) origWidth);
              mtX44[1, 3] = (float) (((double) num12 - (double) num13 - (double) num2 + 2.0) * (double) origHeight * 8.0 + (double) num2 * (double) num6 / (double) byte.MaxValue * (double) origHeight);
              mtX44[0, 1] = (float) ((double) num10 * (double) num14 / 4096.0);
            }
            mtX44[2, 2] = 1f;
            mtX44[3, 3] = 1f;
            return (float[]) mtX44;
          }

          private float GetValue(uint Info, int data, float[] arraydata, int Frame)
          {
            if (((int) Info & (int) this.NNS_G3D_TEXSRTANM_ELEM_CONST) != 0)
              return (float) data / 4096f;
            if (((int) Info & (int) this.NNS_G3D_TEXSRTANM_ELEM_STEP_MASK) == 0)
              return arraydata[Frame];
            if (((int) Info & (int) this.NNS_G3D_TEXSRTANM_ELEM_STEP_2) != 0)
            {
              if ((Frame & 1) == 0)
                return arraydata[Frame >> 1];
              return (long) Frame > (long) (Info & this.NNS_G3D_TEXSRTANM_ELEM_LAST_INTERP_MASK) ? arraydata[(((Info & this.NNS_G3D_TEXSRTANM_ELEM_LAST_INTERP_MASK) >> 1) + 1U)] : (float) (((double) arraydata[Frame >> 1] + (double) arraydata[(Frame >> 1) + 1]) / 2.0);
            }
            if (((int) Info & (int) this.NNS_G3D_TEXSRTANM_ELEM_STEP_4) == 0)
              return -1f;
            if ((Frame & 3) == 0)
              return arraydata[Frame >> 2];
            if ((long) Frame > (long) (Info & this.NNS_G3D_TEXSRTANM_ELEM_LAST_INTERP_MASK))
              return arraydata[(long) ((Info & this.NNS_G3D_TEXSRTANM_ELEM_LAST_INTERP_MASK) >> 2) + (long) (Frame & 3)];
            if ((Frame & 1) == 0)
              return (float) (((double) arraydata[Frame >> 2] + (double) arraydata[(Frame >> 2) + 1]) / 2.0);
            int index1;
            int index2;
            if ((Frame & 2) != 0)
            {
              index1 = Frame >> 2;
              index2 = index1 + 1;
            }
            else
            {
              index2 = Frame >> 2;
              index1 = index2 + 1;
            }
            float num1 = arraydata[index2];
            float num2 = arraydata[index1];
            return (float) (((double) num1 + (double) num1 + (double) num1 + (double) num2) / 4.0);
          }

          private uint GetSinCosValue(uint Info, uint data, uint[] arraydata, int Frame)
          {
            if (((int) Info & (int) this.NNS_G3D_TEXSRTANM_ELEM_CONST) != 0)
              return data;
            if (((int) Info & (int) this.NNS_G3D_TEXSRTANM_ELEM_STEP_MASK) == 0)
              return arraydata[Frame];
            if (((int) Info & (int) this.NNS_G3D_TEXSRTANM_ELEM_STEP_2) != 0)
            {
              if ((Frame & 1) == 0)
                return arraydata[Frame >> 1];
              if ((long) Frame > (long) (Info & this.NNS_G3D_TEXSRTANM_ELEM_LAST_INTERP_MASK))
                return arraydata[(uint) (((int) Info & (int) (this.NNS_G3D_TEXSRTANM_ELEM_LAST_INTERP_MASK >> 1)) + 1)];
              float num1 = (float) (arraydata[Frame >> 1] & (uint) ushort.MaxValue) / 4096f;
              float num2 = (float) (arraydata[Frame >> 1] >> 16) / 4096f;
              float num3 = (float) (arraydata[(Frame >> 1) + 1] & (uint) ushort.MaxValue) / 4096f;
              float num4 = (float) (arraydata[(Frame >> 1) + 1] >> 16) / 4096f;
              return (uint) ((int) (ushort) (((double) num1 + (double) num3) / 2.0 * 4096.0) & (int) ushort.MaxValue | (int) (ushort) (((double) num2 + (double) num4) / 2.0 * 4096.0) << 16);
            }
            if (((int) Info & (int) this.NNS_G3D_TEXSRTANM_ELEM_STEP_4) == 0)
              return 0;
            if ((Frame & 3) == 0)
              return arraydata[Frame >> 2];
            if ((long) Frame > (long) (Info & this.NNS_G3D_TEXSRTANM_ELEM_LAST_INTERP_MASK))
              return arraydata[(long) (Info & this.NNS_G3D_TEXSRTANM_ELEM_LAST_INTERP_MASK >> 2) + (long) (Frame & 3)];
            if ((Frame & 1) != 0)
            {
              int index1;
              int index2;
              if ((Frame & 2) != 0)
              {
                index1 = Frame >> 2;
                index2 = index1 + 1;
              }
              else
              {
                index2 = Frame >> 2;
                index1 = index2 + 1;
              }
              float num1 = (float) (arraydata[index2] & (uint) ushort.MaxValue) / 4096f;
              float num2 = (float) (arraydata[index2] >> 16) / 4096f;
              float num3 = (float) (arraydata[index1] & (uint) ushort.MaxValue) / 4096f;
              float num4 = (float) (arraydata[index1] >> 16) / 4096f;
              return (uint) ((int) (ushort) (((double) num1 + (double) num1 + (double) num1 + (double) num3) / 4.0 * 4096.0) & (int) ushort.MaxValue | (int) (ushort) (((double) num2 + (double) num2 + (double) num2 + (double) num4) / 4.0 * 4096.0) << 16);
            }
            float num5 = (float) (arraydata[Frame >> 2] & (uint) ushort.MaxValue) / 4096f;
            float num6 = (float) (arraydata[Frame >> 2] >> 16) / 4096f;
            float num7 = (float) (arraydata[(Frame >> 2) + 1] & (uint) ushort.MaxValue) / 4096f;
            float num8 = (float) (arraydata[(Frame >> 2) + 1] >> 16) / 4096f;
            return (uint) ((int) (ushort) (((double) num5 + (double) num7) / 2.0 * 4096.0) & (int) ushort.MaxValue | (int) (ushort) (((double) num6 + (double) num8) / 2.0 * 4096.0) << 16);
          }
        }
      }
    }
  }
}
