// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.G3D_Binary_File_Format.NSBCA
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using OpenTK;
using System;
using System.IO;
using System.Windows.Forms;
using Tao.OpenGl;

namespace MKDS_Course_Modifier.G3D_Binary_File_Format
{
  public class NSBCA
  {
    public const string Signature = "BCA0";
    public FileHeader Header;
    public NSBCA.JointAnmSet jntAnmSet;

    public NSBCA(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      bool OK;
      this.Header = new FileHeader(er, "BCA0", out OK);
      if (!OK)
      {
        int num1 = (int) MessageBox.Show("Error 0");
      }
      else
      {
        this.jntAnmSet = new NSBCA.JointAnmSet(er, out OK);
        if (!OK)
        {
          int num2 = (int) MessageBox.Show("Error 1");
        }
      }
      er.ClearMarkers();
      er.Close();
    }

    public class JointAnmSet
    {
      public const string Signature = "JNT0";
      public DataBlockHeader header;
      public Dictionary<NSBCA.JointAnmSet.JointAnmSetData> dict;
      public NSBCA.JointAnmSet.JointAnm[] jntAnm;

      public JointAnmSet(EndianBinaryReader er, out bool OK)
      {
        er.SetMarkerOnCurrentOffset(nameof (JointAnmSet));
        bool OK1;
        this.header = new DataBlockHeader(er, "JNT0", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.dict = new Dictionary<NSBCA.JointAnmSet.JointAnmSetData>(er);
          this.jntAnm = new NSBCA.JointAnmSet.JointAnm[(int) this.dict.numEntry];
          long position = er.BaseStream.Position;
          for (int index = 0; index < (int) this.dict.numEntry; ++index)
          {
            er.BaseStream.Position = er.GetMarker(nameof (JointAnmSet)) + (long) this.dict[index].Value.Offset;
            this.jntAnm[index] = new NSBCA.JointAnmSet.JointAnm(er, out OK1);
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

      public class JointAnmSetData : DictionaryData
      {
        public uint Offset;

        public override void Read(EndianBinaryReader er)
        {
          this.Offset = er.ReadUInt32();
        }
      }

      public class JointAnm
      {
        public AnmHeader anmHeader;
        public ushort numFrame;
        public ushort numNode;
        public uint annFlag;
        public uint ofsRot3;
        public uint ofsRot5;
        public ushort[] ofsTag;
        public NSBCA.JointAnmSet.JointAnm.TagData[] tagData;

        public JointAnm(EndianBinaryReader er, out bool OK)
        {
          er.SetMarkerOnCurrentOffset(nameof (JointAnm));
          bool OK1;
          this.anmHeader = new AnmHeader(er, AnmHeader.Category0.J, AnmHeader.Category1.AC, out OK1);
          if (!OK1)
          {
            OK = false;
          }
          else
          {
            this.numFrame = er.ReadUInt16();
            this.numNode = er.ReadUInt16();
            this.annFlag = er.ReadUInt32();
            this.ofsRot3 = er.ReadUInt32();
            er.SetMarker(nameof (ofsRot3), (long) this.ofsRot3 + er.GetMarker(nameof (JointAnm)));
            this.ofsRot5 = er.ReadUInt32();
            er.SetMarker(nameof (ofsRot5), (long) this.ofsRot5 + er.GetMarker(nameof (JointAnm)));
            this.ofsTag = er.ReadUInt16s((int) this.numNode);
            er.ReadBytes(4);
            this.tagData = new NSBCA.JointAnmSet.JointAnm.TagData[(int) this.numNode];
            long position = er.BaseStream.Position;
            for (int index = 0; index < (int) this.numNode; ++index)
            {
              er.BaseStream.Position = (long) this.ofsTag[index] + er.GetMarker(nameof (JointAnm));
              this.tagData[index] = new NSBCA.JointAnmSet.JointAnm.TagData(er, (int) this.numFrame);
            }
            er.BaseStream.Position = position;
            er.RemoveMarker(nameof (JointAnm));
            er.RemoveMarker(nameof (ofsRot3));
            er.RemoveMarker(nameof (ofsRot5));
            OK = true;
          }
        }

        public class TagData
        {
          private uint NNS_G3D_JNTANM_SRTINFO_IDENTITY = 1;
          private uint NNS_G3D_JNTANM_SRTINFO_IDENTITY_T = 2;
          private uint NNS_G3D_JNTANM_SRTINFO_BASE_T = 4;
          private uint NNS_G3D_JNTANM_SRTINFO_CONST_TX = 8;
          private uint NNS_G3D_JNTANM_SRTINFO_CONST_TY = 16;
          private uint NNS_G3D_JNTANM_SRTINFO_CONST_TZ = 32;
          private uint NNS_G3D_JNTANM_SRTINFO_IDENTITY_R = 64;
          private uint NNS_G3D_JNTANM_SRTINFO_BASE_R = 128;
          private uint NNS_G3D_JNTANM_SRTINFO_CONST_R = 256;
          private uint NNS_G3D_JNTANM_SRTINFO_IDENTITY_S = 512;
          private uint NNS_G3D_JNTANM_SRTINFO_BASE_S = 1024;
          private uint NNS_G3D_JNTANM_SRTINFO_CONST_SX = 2048;
          private uint NNS_G3D_JNTANM_SRTINFO_CONST_SY = 4096;
          private uint NNS_G3D_JNTANM_SRTINFO_CONST_SZ = 8192;
          private uint NNS_G3D_JNTANM_SRTINFO_NODE_MASK = 4278190080;
          public uint flag;
          public NSBCA.JointAnmSet.JointAnm.JointAnmTrans tx;
          public NSBCA.JointAnmSet.JointAnm.JointAnmTrans ty;
          public NSBCA.JointAnmSet.JointAnm.JointAnmTrans tz;
          public NSBCA.JointAnmSet.JointAnm.JointAnmRot r;
          public NSBCA.JointAnmSet.JointAnm.JointAnmScale sx;
          public NSBCA.JointAnmSet.JointAnm.JointAnmScale sy;
          public NSBCA.JointAnmSet.JointAnm.JointAnmScale sz;

          public TagData(EndianBinaryReader er, int NrFrames)
          {
            this.flag = er.ReadUInt32();
            if (((int) this.flag & (int) this.NNS_G3D_JNTANM_SRTINFO_IDENTITY) != 0)
              return;
            if (((int) this.flag & (int) this.NNS_G3D_JNTANM_SRTINFO_IDENTITY_T) == 0 && ((int) this.flag & (int) this.NNS_G3D_JNTANM_SRTINFO_BASE_T) == 0)
            {
              this.tx = new NSBCA.JointAnmSet.JointAnm.JointAnmTrans(er, ((int) this.flag & (int) this.NNS_G3D_JNTANM_SRTINFO_CONST_TX) != 0, NrFrames);
              this.ty = new NSBCA.JointAnmSet.JointAnm.JointAnmTrans(er, ((int) this.flag & (int) this.NNS_G3D_JNTANM_SRTINFO_CONST_TY) != 0, NrFrames);
              this.tz = new NSBCA.JointAnmSet.JointAnm.JointAnmTrans(er, ((int) this.flag & (int) this.NNS_G3D_JNTANM_SRTINFO_CONST_TZ) != 0, NrFrames);
            }
            if (((int) this.flag & (int) this.NNS_G3D_JNTANM_SRTINFO_IDENTITY_R) == 0 && ((int) this.flag & (int) this.NNS_G3D_JNTANM_SRTINFO_BASE_R) == 0)
              this.r = new NSBCA.JointAnmSet.JointAnm.JointAnmRot(er, ((int) this.flag & (int) this.NNS_G3D_JNTANM_SRTINFO_CONST_R) != 0, NrFrames);
            if (((int) this.flag & (int) this.NNS_G3D_JNTANM_SRTINFO_IDENTITY_S) == 0 && ((int) this.flag & (int) this.NNS_G3D_JNTANM_SRTINFO_BASE_S) == 0)
            {
              this.sx = new NSBCA.JointAnmSet.JointAnm.JointAnmScale(er, ((int) this.flag & (int) this.NNS_G3D_JNTANM_SRTINFO_CONST_SX) != 0, NrFrames);
              this.sy = new NSBCA.JointAnmSet.JointAnm.JointAnmScale(er, ((int) this.flag & (int) this.NNS_G3D_JNTANM_SRTINFO_CONST_SY) != 0, NrFrames);
              this.sz = new NSBCA.JointAnmSet.JointAnm.JointAnmScale(er, ((int) this.flag & (int) this.NNS_G3D_JNTANM_SRTINFO_CONST_SZ) != 0, NrFrames);
            }
          }

          public float[] GetMatrix(
            int Frame,
            bool MayaScale,
            int PosScale,
            NSBMD.ModelSet.Model.NodeSet.NodeData mdl)
          {
            if (((int) this.flag & (int) this.NNS_G3D_JNTANM_SRTINFO_IDENTITY) != 0)
              return this.loadIdentity();
            float[] a1 = this.loadIdentity();
            this.loadIdentity();
            float[] a2 = this.loadIdentity();
            float[] a3 = this.loadIdentity();
            float num1 = this.tx == null ? (((int) this.flag & (int) this.NNS_G3D_JNTANM_SRTINFO_IDENTITY_T) == 0 ? mdl.Tx : 0.0f) : this.tx.GetValue(Frame);
            float num2 = this.ty == null ? (((int) this.flag & (int) this.NNS_G3D_JNTANM_SRTINFO_IDENTITY_T) == 0 ? mdl.Ty : 0.0f) : this.ty.GetValue(Frame);
            float num3 = this.tx == null ? (((int) this.flag & (int) this.NNS_G3D_JNTANM_SRTINFO_IDENTITY_T) == 0 ? mdl.Tz : 0.0f) : this.tz.GetValue(Frame);
            float[] b1 = this.translate(a3, num1 / (float) PosScale, num2 / (float) PosScale, num3 / (float) PosScale);
            bool flag = true;
            float[] b2 = this.r == null ? (((int) this.flag & (int) this.NNS_G3D_JNTANM_SRTINFO_IDENTITY_R) == 0 ? mdl.GetRotation() : this.loadIdentity()) : this.multMatrix(a2, (float[]) this.r.GetMatrix(Frame));
            float x = this.sx == null ? (((int) this.flag & (int) this.NNS_G3D_JNTANM_SRTINFO_IDENTITY_S) == 0 ? mdl.Sx : 1f) : this.sx.GetValue(Frame);
            float y = this.sy == null ? (((int) this.flag & (int) this.NNS_G3D_JNTANM_SRTINFO_IDENTITY_S) == 0 ? mdl.Sy : 1f) : this.sy.GetValue(Frame);
            float z = this.sz == null ? (((int) this.flag & (int) this.NNS_G3D_JNTANM_SRTINFO_IDENTITY_S) == 0 ? mdl.Sz : 1f) : this.sz.GetValue(Frame);
            float[] b3 = this.scale(a1, x, y, z);
            float[] a4 = this.multMatrix(this.loadIdentity(), b1);
            flag = true;
            return this.multMatrix(this.multMatrix(a4, b2), b3);
          }

          private float[] translate(float[] a, float x, float y, float z)
          {
            float[] b = this.loadIdentity();
            b[12] = x;
            b[13] = y;
            b[14] = z;
            return this.multMatrix(a, b);
          }

          private float[] loadIdentity()
          {
            float[] numArray = new float[16];
            numArray[0] = 1f;
            numArray[5] = 1f;
            numArray[10] = 1f;
            numArray[15] = 1f;
            return numArray;
          }

          private float[] multMatrix(float[] a, float[] b)
          {
            float[] numArray = new float[16];
            for (int index1 = 0; index1 < 4; ++index1)
            {
              for (int index2 = 0; index2 < 4; ++index2)
              {
                numArray[(index1 << 2) + index2] = 0.0f;
                for (int index3 = 0; index3 < 4; ++index3)
                  numArray[(index1 << 2) + index2] += a[(index3 << 2) + index2] * b[(index1 << 2) + index3];
              }
            }
            return numArray;
          }

          private float[] scale(float[] a, float x, float y, float z)
          {
            float[] b = this.loadIdentity();
            b[0] = x;
            b[5] = y;
            b[10] = z;
            return this.multMatrix(a, b);
          }
        }

        public class JointAnmTrans
        {
          private uint NNS_G3D_JNTANM_TINFO_STEP_MASK = 3221225472;
          private uint NNS_G3D_JNTANM_TINFO_STEP_2 = 1073741824;
          private uint NNS_G3D_JNTANM_TINFO_STEP_4 = 2147483648;
          private uint NNS_G3D_JNTANM_TINFO_FX16ARRAY = 536870912;
          private uint NNS_G3D_JNTANM_TINFO_LAST_INTERP_MASK = 536805376;
          public float const_trans;
          public uint info;
          public uint offset;
          public float[] Translation;

          public JointAnmTrans(EndianBinaryReader er, bool isConst, int NrFrames)
          {
            if (isConst)
            {
              this.const_trans = er.ReadSingleInt32Exp12();
            }
            else
            {
              this.info = er.ReadUInt32();
              this.offset = er.ReadUInt32();
              int num1 = (int) ((this.info & 3221225472U) >> 29);
              if (num1 == 0)
                ++num1;
              long position = er.BaseStream.Position;
              er.BaseStream.Position = (long) this.offset + er.GetMarker(nameof (JointAnm));
              int num2 = (int) ((this.info & this.NNS_G3D_JNTANM_TINFO_LAST_INTERP_MASK) >> 16);
              this.Translation = ((int) this.info & (int) this.NNS_G3D_JNTANM_TINFO_FX16ARRAY) == 0 ? er.ReadSingleInt32Exp12s(num2 / num1 + num1) : er.ReadSingleInt16Exp12s(num2 / num1 + num1);
              er.BaseStream.Position = position;
            }
          }

          public float GetValue(int Frame)
          {
            if (this.Translation == null)
              return this.const_trans;
            int num1 = (int) ((this.info & this.NNS_G3D_JNTANM_TINFO_LAST_INTERP_MASK) >> 16);
            if (((int) this.info & (int) this.NNS_G3D_JNTANM_TINFO_STEP_MASK) == 0)
              return this.Translation[Frame];
            if (((int) this.info & (int) this.NNS_G3D_JNTANM_TINFO_STEP_2) != 0)
            {
              if ((Frame & 1) == 0)
                return this.Translation[Frame >> 1];
              return Frame > num1 ? this.Translation[(num1 >> 1) + 1] : (float) ((double) this.Translation[Frame >> 1] / 2.0 + (double) this.Translation[(Frame >> 1) + 1] / 2.0);
            }
            if (((int) this.info & (int) this.NNS_G3D_JNTANM_TINFO_STEP_4) == 0)
              return 0.0f;
            if ((Frame & 3) == 0)
              return this.Translation[Frame >> 2];
            if (Frame > num1)
              return this.Translation[(num1 >> 2) + (Frame & 3)];
            if ((Frame & 1) == 0)
              return (float) ((double) this.Translation[Frame >> 2] / 2.0 + (double) this.Translation[(Frame >> 2) + 1] / 2.0);
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
            float num2 = this.Translation[index2];
            float num3 = this.Translation[index1];
            return (float) (((double) num2 + (double) num2 + (double) num2 + (double) num3) / 4.0);
          }
        }

        public class JointAnmRot
        {
          private uint NNS_G3D_JNTANM_RINFO_STEP_MASK = 3221225472;
          private uint NNS_G3D_JNTANM_RINFO_STEP_2 = 1073741824;
          private uint NNS_G3D_JNTANM_RINFO_STEP_4 = 2147483648;
          private uint NNS_G3D_JNTANM_RINFO_LAST_INTERP_MASK = 536805376;
          public uint const_offset;
          public NSBCA.JointAnmSet.JointAnmRot3 const_rot3;
          public NSBCA.JointAnmSet.JointAnmRot5 const_rot5;
          public uint info;
          public uint offset;
          public NSBCA.JointAnmSet.JointAnmRot3[] rot3;
          public NSBCA.JointAnmSet.JointAnmRot5[] rot5;

          public JointAnmRot(EndianBinaryReader er, bool isConst, int NrFrames)
          {
            if (isConst)
            {
              this.const_offset = er.ReadUInt32();
              long position = er.BaseStream.Position;
              if (this.const_offset >> 15 == 1U)
              {
                er.BaseStream.Position = er.GetMarker("ofsRot3") + (long) (uint) (6 * ((int) this.const_offset & (int) short.MaxValue));
                this.const_rot3 = new NSBCA.JointAnmSet.JointAnmRot3(er);
              }
              else
              {
                er.BaseStream.Position = er.GetMarker("ofsRot5") + (long) (uint) (10 * ((int) this.const_offset & (int) short.MaxValue));
                this.const_rot5 = new NSBCA.JointAnmSet.JointAnmRot5(er);
              }
              er.BaseStream.Position = position;
            }
            else
            {
              this.info = er.ReadUInt32();
              this.offset = er.ReadUInt32();
              int num1 = (int) ((this.info & 3221225472U) >> 29);
              if (num1 == 0)
                ++num1;
              int num2 = (int) ((this.info & this.NNS_G3D_JNTANM_RINFO_LAST_INTERP_MASK) >> 16);
              long position = er.BaseStream.Position;
              this.rot3 = new NSBCA.JointAnmSet.JointAnmRot3[num2 / num1 + num1];
              this.rot5 = new NSBCA.JointAnmSet.JointAnmRot5[num2 / num1 + num1];
              for (int index = 0; index < num2 / num1 + num1; ++index)
              {
                er.BaseStream.Position = (long) this.offset + er.GetMarker(nameof (JointAnm)) + (long) (index * 2);
                ushort num3 = er.ReadUInt16();
                if ((int) num3 >> 15 == 1)
                {
                  er.BaseStream.Position = er.GetMarker("ofsRot3") + (long) (6 * ((int) num3 & (int) short.MaxValue));
                  this.rot3[index] = new NSBCA.JointAnmSet.JointAnmRot3(er);
                }
                else
                {
                  er.BaseStream.Position = er.GetMarker("ofsRot5") + (long) (10 * ((int) num3 & (int) short.MaxValue));
                  this.rot5[index] = new NSBCA.JointAnmSet.JointAnmRot5(er);
                }
              }
              er.BaseStream.Position = position;
            }
          }

          public MTX44 GetMatrix(int Frame)
          {
            if (this.rot5 == null && this.rot3 == null)
              return this.const_rot3 == null ? this.const_rot5.GetMatrix() : this.const_rot3.GetMatrix();
            int num = (int) ((this.info & this.NNS_G3D_JNTANM_RINFO_LAST_INTERP_MASK) >> 16);
            if (((int) this.info & (int) this.NNS_G3D_JNTANM_RINFO_STEP_MASK) == 0)
              return this.rot3[Frame] == null ? this.rot5[Frame].GetMatrix() : this.rot3[Frame].GetMatrix();
            if (((int) this.info & (int) this.NNS_G3D_JNTANM_RINFO_STEP_2) != 0)
            {
              if ((Frame & 1) == 0)
                return this.rot3[Frame >> 1] == null ? this.rot5[Frame >> 1].GetMatrix() : this.rot3[Frame >> 1].GetMatrix();
              return Frame > num ? (this.rot3[(num >> 1) + 1] == null ? this.rot5[(num >> 1) + 1].GetMatrix() : this.rot3[(num >> 1) + 1].GetMatrix()) : this.InterpolateMTX44_50_50(this.rot3[Frame >> 1] == null ? this.rot5[Frame >> 1].GetMatrix() : this.rot3[Frame >> 1].GetMatrix(), this.rot3[(Frame >> 1) + 1] == null ? this.rot5[(Frame >> 1) + 1].GetMatrix() : this.rot3[(Frame >> 1) + 1].GetMatrix());
            }
            if (((int) this.info & (int) this.NNS_G3D_JNTANM_RINFO_STEP_4) == 0)
              return new MTX44();
            if ((Frame & 3) == 0)
              return this.rot3[Frame >> 2] == null ? this.rot5[Frame >> 2].GetMatrix() : this.rot3[Frame >> 2].GetMatrix();
            if (Frame > num)
              return this.rot3[(num >> 2) + (Frame & 3)] == null ? this.rot5[(num >> 2) + (Frame & 3)].GetMatrix() : this.rot3[(num >> 2) + (Frame & 3)].GetMatrix();
            if ((Frame & 1) == 0)
              return this.InterpolateMTX44_50_50(this.rot3[Frame >> 2] == null ? this.rot5[Frame >> 2].GetMatrix() : this.rot3[Frame >> 2].GetMatrix(), this.rot3[(Frame >> 2) + 1] == null ? this.rot5[(Frame >> 2) + 1].GetMatrix() : this.rot3[(Frame >> 2) + 1].GetMatrix());
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
            MTX44 mtX44_1 = this.rot3[index2] == null ? this.rot5[index2].GetMatrix() : this.rot3[index2].GetMatrix();
            MTX44 mtX44_2 = this.rot3[index1] == null ? this.rot5[index1].GetMatrix() : this.rot3[index1].GetMatrix();
            mtX44_1[0, 0] = mtX44_1[0, 0] * 3f + mtX44_2[0, 0];
            mtX44_1[1, 0] = mtX44_1[1, 0] * 3f + mtX44_2[1, 0];
            mtX44_1[2, 0] = mtX44_1[2, 0] * 3f + mtX44_2[2, 0];
            mtX44_1[0, 1] = mtX44_1[0, 1] * 3f + mtX44_2[0, 1];
            mtX44_1[1, 1] = mtX44_1[1, 1] * 3f + mtX44_2[1, 1];
            mtX44_1[2, 1] = mtX44_1[2, 1] * 3f + mtX44_2[2, 1];
            Vector3 vector3_1 = Vector3.Normalize(new Vector3(mtX44_1[0, 0], mtX44_1[1, 0], mtX44_1[2, 0]));
            mtX44_1[0, 0] = vector3_1.X;
            mtX44_1[1, 0] = vector3_1.Y;
            mtX44_1[2, 0] = vector3_1.Z;
            Vector3 vector3_2 = Vector3.Normalize(new Vector3(mtX44_1[0, 1], mtX44_1[1, 1], mtX44_1[2, 1]));
            mtX44_1[0, 1] = vector3_2.X;
            mtX44_1[1, 1] = vector3_2.Y;
            mtX44_1[2, 1] = vector3_2.Z;
            mtX44_1[0, 2] = mtX44_1[0, 2] * 3f + mtX44_2[0, 2];
            mtX44_1[1, 2] = mtX44_1[1, 2] * 3f + mtX44_2[1, 2];
            mtX44_1[2, 2] = mtX44_1[2, 2] * 3f + mtX44_2[2, 2];
            Vector3 vector3_3 = Vector3.Normalize(new Vector3(mtX44_1[0, 2], mtX44_1[1, 2], mtX44_1[2, 2]));
            mtX44_1[0, 2] = vector3_3.X;
            mtX44_1[1, 2] = vector3_3.Y;
            mtX44_1[2, 2] = vector3_3.Z;
            return mtX44_1;
          }

          private MTX44 InterpolateMTX44_50_50(MTX44 a, MTX44 b)
          {
            a[0, 0] += b[0, 0];
            a[1, 0] += b[1, 0];
            a[2, 0] += b[2, 0];
            a[0, 1] += b[0, 1];
            a[1, 1] += b[1, 1];
            a[2, 1] += b[2, 1];
            Vector3 vector3_1 = Vector3.Normalize(new Vector3(a[0, 0], a[1, 0], a[2, 0]));
            a[0, 0] = vector3_1.X;
            a[1, 0] = vector3_1.Y;
            a[2, 0] = vector3_1.Z;
            Vector3 vector3_2 = Vector3.Normalize(new Vector3(a[0, 1], a[1, 1], a[2, 1]));
            a[0, 1] = vector3_2.X;
            a[1, 1] = vector3_2.Y;
            a[2, 1] = vector3_2.Z;
            a[0, 2] += b[0, 2];
            a[1, 2] += b[1, 2];
            a[2, 2] += b[2, 2];
            Vector3 vector3_3 = Vector3.Normalize(new Vector3(a[0, 2], a[1, 2], a[2, 2]));
            a[0, 2] = vector3_3.X;
            a[1, 2] = vector3_3.Y;
            a[2, 2] = vector3_3.Z;
            return a;
          }
        }

        public class JointAnmScale
        {
          private uint NNS_G3D_JNTANM_SINFO_STEP_MASK = 3221225472;
          private uint NNS_G3D_JNTANM_SINFO_STEP_2 = 1073741824;
          private uint NNS_G3D_JNTANM_SINFO_STEP_4 = 2147483648;
          private uint NNS_G3D_JNTANM_SINFO_FX16ARRAY = 536870912;
          private uint NNS_G3D_JNTANM_SINFO_LAST_INTERP_MASK = 536805376;
          public float const_scale;
          public float const_invScale;
          public uint info;
          public uint offset;
          public NSBCA.JointAnmSet.JointAnmScaleFx16[] Scale16;
          public NSBCA.JointAnmSet.JointAnmScaleFx32[] Scale32;

          public JointAnmScale(EndianBinaryReader er, bool isConst, int NrFrames)
          {
            if (isConst)
            {
              this.const_scale = er.ReadSingleInt32Exp12();
              this.const_invScale = er.ReadSingleInt32Exp12();
            }
            else
            {
              this.info = er.ReadUInt32();
              this.offset = er.ReadUInt32();
              int num1 = (int) ((this.info & 3221225472U) >> 29);
              if (num1 == 0)
                ++num1;
              long position = er.BaseStream.Position;
              er.BaseStream.Position = (long) this.offset + er.GetMarker(nameof (JointAnm));
              int num2 = (int) ((this.info & this.NNS_G3D_JNTANM_SINFO_LAST_INTERP_MASK) >> 16);
              if (((int) this.info & (int) this.NNS_G3D_JNTANM_SINFO_FX16ARRAY) != 0)
              {
                this.Scale16 = new NSBCA.JointAnmSet.JointAnmScaleFx16[num2 / num1 + num1];
                for (int index = 0; index < num2 / num1 + num1; ++index)
                  this.Scale16[index] = new NSBCA.JointAnmSet.JointAnmScaleFx16(er);
              }
              else
              {
                this.Scale32 = new NSBCA.JointAnmSet.JointAnmScaleFx32[num2 / num1 + num1];
                for (int index = 0; index < num2 / num1 + num1; ++index)
                  this.Scale32[index] = new NSBCA.JointAnmSet.JointAnmScaleFx32(er);
              }
              er.BaseStream.Position = position;
            }
          }

          public float GetValue(int Frame)
          {
            if (this.Scale16 == null && this.Scale32 == null)
              return this.const_scale;
            int num = (int) ((this.info & this.NNS_G3D_JNTANM_SINFO_LAST_INTERP_MASK) >> 16);
            if (((int) this.info & (int) this.NNS_G3D_JNTANM_SINFO_STEP_MASK) == 0)
              return this.Scale16 == null ? this.Scale32[Frame].scale : this.Scale16[Frame].scale;
            if (((int) this.info & (int) this.NNS_G3D_JNTANM_SINFO_STEP_2) != 0)
              return (Frame & 1) != 0 ? (Frame > num ? (this.Scale16 == null ? this.Scale32[(num >> 1) + 1].scale : this.Scale16[(num >> 1) + 1].scale) : (this.Scale16 == null ? (float) (((double) this.Scale32[Frame >> 1].scale + (double) this.Scale32[(Frame >> 1) + 1].scale) / 2.0) : (float) (((double) this.Scale16[Frame >> 1].scale + (double) this.Scale16[(Frame >> 1) + 1].scale) / 2.0))) : (this.Scale16 == null ? this.Scale32[Frame >> 1].scale : this.Scale16[Frame >> 1].scale);
            if (((int) this.info & (int) this.NNS_G3D_JNTANM_SINFO_STEP_4) == 0)
              return 1f;
            if ((Frame & 3) != 0)
            {
              if (Frame > num)
                return this.Scale16 == null ? this.Scale32[(num >> 2) + (Frame & 3)].scale : this.Scale16[(num >> 2) + (Frame & 3)].scale;
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
                if (this.Scale16 != null)
                {
                  float scale1 = this.Scale16[index2].scale;
                  float scale2 = this.Scale16[index1].scale;
                  return (float) (((double) scale1 + (double) scale1 + (double) scale1 + (double) scale2) / 4.0);
                }
                float scale3 = this.Scale32[index2].scale;
                float scale4 = this.Scale32[index1].scale;
                return (float) (((double) scale3 + (double) scale3 + (double) scale3 + (double) scale4) / 2.0);
              }
              return this.Scale16 == null ? (float) ((double) this.Scale32[Frame >> 2].scale / 2.0 + (double) this.Scale32[(Frame >> 2) + 1].scale / 2.0) : (float) (((double) this.Scale16[Frame >> 2].scale + (double) this.Scale16[(Frame >> 2) + 1].scale) / 2.0);
            }
            return this.Scale16 == null ? this.Scale32[Frame >> 2].scale : this.Scale16[Frame >> 2].scale;
          }

          public float GetInvValue(int Frame)
          {
            if (this.Scale16 == null && this.Scale32 == null)
              return this.const_invScale;
            int num = (int) ((this.info & this.NNS_G3D_JNTANM_SINFO_LAST_INTERP_MASK) >> 16);
            if (((int) this.info & (int) this.NNS_G3D_JNTANM_SINFO_STEP_MASK) == 0)
              return this.Scale16 == null ? this.Scale32[Frame].invScale : this.Scale16[Frame].invScale;
            if (((int) this.info & (int) this.NNS_G3D_JNTANM_SINFO_STEP_2) != 0)
              return (Frame & 1) != 0 ? (Frame > num ? (this.Scale16 == null ? this.Scale32[num >> 2].invScale : this.Scale16[num >> 2].invScale) : (this.Scale16 == null ? (float) ((double) this.Scale32[Frame >> 1].invScale / 2.0 + (double) this.Scale32[(Frame >> 1) + 1].invScale / 2.0) : (float) (((double) this.Scale16[Frame >> 1].invScale + (double) this.Scale16[(Frame >> 1) + 1].invScale) / 2.0))) : (this.Scale16 == null ? this.Scale32[Frame >> 1].invScale : this.Scale16[Frame >> 1].invScale);
            if (((int) this.info & (int) this.NNS_G3D_JNTANM_SINFO_STEP_4) == 0)
              return 1f;
            if ((Frame & 3) != 0)
            {
              if (Frame > num)
                return this.Scale16 == null ? this.Scale32[(num >> 2) + (Frame & 3)].invScale : this.Scale16[(num >> 2) + (Frame & 3)].invScale;
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
                if (this.Scale16 != null)
                {
                  float invScale1 = this.Scale16[index2].invScale;
                  float invScale2 = this.Scale16[index1].invScale;
                  return (float) (((double) invScale1 + (double) invScale1 + (double) invScale1 + (double) invScale2) / 4.0);
                }
                float invScale3 = this.Scale32[index2].invScale;
                float invScale4 = this.Scale32[index1].invScale;
                return (float) (((double) invScale3 + (double) invScale3 + (double) invScale3 + (double) invScale4) / 2.0);
              }
              return this.Scale16 == null ? (float) ((double) this.Scale32[Frame >> 2].invScale / 2.0 + (double) this.Scale32[(Frame >> 2) + 1].invScale / 2.0) : (float) (((double) this.Scale16[Frame >> 2].invScale + (double) this.Scale16[(Frame >> 2) + 1].invScale) / 2.0);
            }
            return this.Scale16 == null ? this.Scale32[Frame >> 2].invScale : this.Scale16[Frame >> 2].invScale;
          }
        }
      }

      public class JointAnmRot3
      {
        public ushort info;
        public float A;
        public float B;

        public JointAnmRot3(EndianBinaryReader er)
        {
          this.info = er.ReadUInt16();
          this.A = er.ReadSingleInt16Exp12();
          this.B = er.ReadSingleInt16Exp12();
        }

        public MTX44 GetMatrix()
        {
          MTX44 mtX44 = new MTX44();
          return (MTX44) GlNitro.glNitroPivot(new float[2]
          {
            this.A,
            this.B
          }, (int) this.info & 15, (int) this.info >> 4 & 15);
        }
      }

      public class JointAnmRot5
      {
        public short[] data;

        public JointAnmRot5(EndianBinaryReader er)
        {
          this.data = er.ReadInt16s(5);
        }

        public MTX44 GetMatrix()
        {
          short num1 = (short) ((int) this.data[4] & 7);
          short num2 = (short) ((int) this.data[4] >> 3);
          short num3 = (short) ((int) num1 << 3 | (int) this.data[0] & 7);
          short num4 = (short) ((int) this.data[0] >> 3);
          short num5 = (short) ((int) num3 << 3 | (int) this.data[1] & 7);
          short num6 = (short) ((int) this.data[1] >> 3);
          short num7 = (short) ((int) num5 << 3 | (int) this.data[2] & 7);
          short num8 = (short) ((int) this.data[2] >> 3);
          short num9 = (short) ((int) num7 << 3 | (int) this.data[3] & 7);
          short num10 = (short) ((int) this.data[3] >> 3);
          short num11 = (short) ((int) (short) ((int) num9 << 3) >> 3);
          MTX44 mtX44 = new MTX44();
          mtX44[0, 0] = (float) num4;
          mtX44[1, 0] = (float) num6;
          mtX44[2, 0] = (float) num8;
          mtX44[0, 1] = (float) num10;
          mtX44[1, 1] = (float) num2;
          mtX44[2, 1] = (float) num11;
          Vector3 vector3 = this.vecCross_(new Vector3(mtX44[0, 0], mtX44[1, 0], mtX44[2, 0]), new Vector3(mtX44[0, 1], mtX44[1, 1], mtX44[2, 1]));
          mtX44[0, 2] = vector3.X;
          mtX44[1, 2] = vector3.Y;
          mtX44[2, 2] = vector3.Z;
          mtX44[0, 0] /= 4096f;
          mtX44[1, 0] /= 4096f;
          mtX44[2, 0] /= 4096f;
          mtX44[0, 1] /= 4096f;
          mtX44[1, 1] /= 4096f;
          mtX44[2, 1] /= 4096f;
          mtX44[0, 2] /= 4096f;
          mtX44[1, 2] /= 4096f;
          mtX44[2, 2] /= 4096f;
          return mtX44;
        }

        private Vector3 vecCross_(Vector3 a, Vector3 b)
        {
          return new Vector3((float) (((double) a.Y * (double) b.Z - (double) a.Z * (double) b.Y) / 4096.0), (float) (((double) a.Z * (double) b.X - (double) a.X * (double) b.Z) / 4096.0), (float) (((double) a.X * (double) b.Y - (double) a.Y * (double) b.X) / 4096.0));
        }
      }

      public class JointAnmScaleFx16
      {
        public float scale;
        public float invScale;

        public JointAnmScaleFx16(EndianBinaryReader er)
        {
          this.scale = er.ReadSingleInt16Exp12();
          this.invScale = er.ReadSingleInt16Exp12();
        }
      }

      public class JointAnmScaleFx32
      {
        public float scale;
        public float invScale;

        public JointAnmScaleFx32(EndianBinaryReader er)
        {
          this.scale = er.ReadSingleInt32Exp12();
          this.invScale = er.ReadSingleInt32Exp12();
        }
      }
    }
  }
}
