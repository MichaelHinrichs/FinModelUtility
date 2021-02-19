// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.GCN.BCA
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier._3D_Formats;
using MKDS_Course_Modifier.G3D_Binary_File_Format;
//using MKDS_Course_Modifier.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.GCN
{
  public class BCA
  {
    public const string Signature = "J3D1bca1";
    public BCA.BCAHeader Header;
    public BCA.ANF1Section ANF1;

    public BCA(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.BigEndian);
      bool OK;
      this.Header = new BCA.BCAHeader(er, "J3D1bca1", out OK);
      if (!OK)
      {
        int num1 = (int) MessageBox.Show("Error 1");
      }
      else
      {
        this.ANF1 = new BCA.ANF1Section(er, out OK);
        if (!OK)
        {
          int num2 = (int) MessageBox.Show("Error 2");
        }
      }
      er.Close();
    }

    public class BCAHeader
    {
      public string Type;
      public uint FileSize;
      public uint NrSections;
      public byte[] Padding;

      public BCAHeader(EndianBinaryReader er, string Signature, out bool OK)
      {
        this.Type = er.ReadString(Encoding.ASCII, 8);
        if (this.Type != Signature)
        {
          OK = false;
        }
        else
        {
          this.FileSize = er.ReadUInt32();
          this.NrSections = er.ReadUInt32();
          this.Padding = er.ReadBytes(16);
          OK = true;
        }
      }
    }

    public class ANF1Section
    {
      public const string Signature = "ANF1";
      public DataBlockHeader Header;
      public byte LoopFlags;
      public byte AngleMultiplier;
      public ushort AnimLength;
      public ushort NrJoints;
      public ushort NrScale;
      public ushort NrRot;
      public ushort NrTrans;
      public uint JointOffset;
      public uint ScaleOffset;
      public uint RotOffset;
      public uint TransOffset;
      public float[] Scale;
      public short[] Rotation;
      public float[] Translation;
      public BCA.ANF1Section.AnimatedJoint[] Joints;

      public ANF1Section(EndianBinaryReader er, out bool OK)
      {
        bool OK1;

        this.Header = new DataBlockHeader(er, "ANF1", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.LoopFlags = er.ReadByte();
          this.AngleMultiplier = er.ReadByte();
          this.AnimLength = er.ReadUInt16();
          this.NrJoints = er.ReadUInt16();
          this.NrScale = er.ReadUInt16();
          this.NrRot = er.ReadUInt16();
          this.NrTrans = er.ReadUInt16();
          this.JointOffset = er.ReadUInt32();
          this.ScaleOffset = er.ReadUInt32();
          this.RotOffset = er.ReadUInt32();
          this.TransOffset = er.ReadUInt32();

          er.BaseStream.Position = (long) (32U + this.ScaleOffset);
          this.Scale = er.ReadSingles((int) this.NrScale);
          er.BaseStream.Position = (long) (32U + this.RotOffset);
          this.Rotation = er.ReadInt16s((int) this.NrRot);
          er.BaseStream.Position = (long) (32U + this.TransOffset);
          this.Translation = er.ReadSingles((int) this.NrTrans);
          float RotScale = (float) (1 * Math.PI / 32768f);
          er.BaseStream.Position = (long) (32U + this.JointOffset);
          this.Joints = new BCA.ANF1Section.AnimatedJoint[(int) this.NrJoints];
          for (int index = 0; index < (int) this.NrJoints; ++index)
          {
            this.Joints[index] = new BCA.ANF1Section.AnimatedJoint(er);
            this.Joints[index].SetValues(this.Scale, this.Rotation, this.Translation, RotScale);
          }
          OK = true;
        }
      }

      public byte[] ExportAsMa(BMD b)
      {
        MA.Node[] joints = b.GetJoints();
        //ScaleDialog scaleDialog = new ScaleDialog();
        //int num = (int) scaleDialog.ShowDialog();
        float scale = 1; //scaleDialog.scale;
        List<MA.AnimatedNode> animatedNodeList = new List<MA.AnimatedNode>();
        foreach (MA.Node node in joints)
        {
          int index1 = b.JNT1.StringTable[node.Name];
          node.Trans.X *= scale;
          node.Trans.Y *= scale;
          node.Trans.Z *= scale;
          List<float> floatList1 = new List<float>();
          List<float> floatList2 = new List<float>();
          List<float> floatList3 = new List<float>();
          List<float> floatList4 = new List<float>();
          List<float> floatList5 = new List<float>();
          List<float> floatList6 = new List<float>();
          List<float> floatList7 = new List<float>();
          List<float> floatList8 = new List<float>();
          List<float> floatList9 = new List<float>();
          for (int index2 = 0; index2 < (int) this.AnimLength; ++index2)
          {
            floatList1.Add(this.Joints[index1].GetAnimValue(this.Joints[index1].Values.translationsX, (float) index2) * scale);
            floatList2.Add(this.Joints[index1].GetAnimValue(this.Joints[index1].Values.translationsY, (float) index2) * scale);
            floatList3.Add(this.Joints[index1].GetAnimValue(this.Joints[index1].Values.translationsZ, (float) index2) * scale);
            floatList4.Add(this.Joints[index1].GetAnimValue(this.Joints[index1].Values.rotationsX, (float) index2));
            floatList5.Add(this.Joints[index1].GetAnimValue(this.Joints[index1].Values.rotationsY, (float) index2));
            floatList6.Add(this.Joints[index1].GetAnimValue(this.Joints[index1].Values.rotationsZ, (float) index2));
            floatList7.Add(this.Joints[index1].GetAnimValue(this.Joints[index1].Values.scalesX, (float) index2));
            floatList8.Add(this.Joints[index1].GetAnimValue(this.Joints[index1].Values.scalesY, (float) index2));
            floatList9.Add(this.Joints[index1].GetAnimValue(this.Joints[index1].Values.scalesZ, (float) index2));
          }
          animatedNodeList.Add(new MA.AnimatedNode((int) this.AnimLength, floatList1.ToArray(), floatList2.ToArray(), floatList3.ToArray(), floatList4.ToArray(), floatList5.ToArray(), floatList6.ToArray(), floatList7.ToArray(), floatList8.ToArray(), floatList9.ToArray()));
        }
        return MA.WriteAnimation(joints, animatedNodeList.ToArray());
      }

      public class AnimatedJoint
      {
        public BCA.ANF1Section.AnimatedJoint.AnimComponent X;
        public BCA.ANF1Section.AnimatedJoint.AnimComponent Y;
        public BCA.ANF1Section.AnimatedJoint.AnimComponent Z;
        public BCA.ANF1Section.AnimatedJoint.JointAnim Values;

        public AnimatedJoint(EndianBinaryReader er)
        {
          this.X = new BCA.ANF1Section.AnimatedJoint.AnimComponent(er);
          this.Y = new BCA.ANF1Section.AnimatedJoint.AnimComponent(er);
          this.Z = new BCA.ANF1Section.AnimatedJoint.AnimComponent(er);
        }

        public void SetValues(
          float[] Scales,
          short[] Rotations,
          float[] Translations,
          float RotScale)
        {
          this.Values = new BCA.ANF1Section.AnimatedJoint.JointAnim(this, Scales, Rotations, Translations, RotScale);
        }

        private float Interpolate(float v1, float d1, float v2, float d2, float t)
        {
          float num1 = (float) (2.0 * ((double) v1 - (double) v2)) + d1 + d2;
          float num2 = (float) (-3.0 * (double) v1 + 3.0 * (double) v2 - 2.0 * (double) d1) - d2;
          float num3 = d1;
          float num4 = v1;
          return ((num1 * t + num2) * t + num3) * t + num4;
        }

        public float GetAnimValue(float[] keys, float t)
        {
          if (keys.Length == 0)
            return 0.0f;
          return keys.Length == 1 ? keys[0] : keys[(int) t];
        }

        public class AnimComponent
        {
          public BCA.ANF1Section.AnimatedJoint.AnimComponent.AnimIndex S;
          public BCA.ANF1Section.AnimatedJoint.AnimComponent.AnimIndex R;
          public BCA.ANF1Section.AnimatedJoint.AnimComponent.AnimIndex T;

          public AnimComponent(EndianBinaryReader er)
          {
            this.S = new BCA.ANF1Section.AnimatedJoint.AnimComponent.AnimIndex(er);
            this.R = new BCA.ANF1Section.AnimatedJoint.AnimComponent.AnimIndex(er);
            this.T = new BCA.ANF1Section.AnimatedJoint.AnimComponent.AnimIndex(er);
          }

          public class AnimIndex
          {
            public ushort Count;
            public ushort Index;

            public AnimIndex(EndianBinaryReader er)
            {
              this.Count = er.ReadUInt16();
              this.Index = er.ReadUInt16();
            }
          }
        }

        public class JointAnim
        {
          public float[] scalesX;
          public float[] scalesY;
          public float[] scalesZ;
          public float[] rotationsX;
          public float[] rotationsY;
          public float[] rotationsZ;
          public float[] translationsX;
          public float[] translationsY;
          public float[] translationsZ;

          public JointAnim(
            BCA.ANF1Section.AnimatedJoint Joint,
            float[] Scales,
            short[] Rotations,
            float[] Translations,
            float RotScale)
          {
            this.SetKeysST(out this.scalesX, Scales, Joint.X.S);
            this.SetKeysST(out this.scalesY, Scales, Joint.Y.S);
            this.SetKeysST(out this.scalesZ, Scales, Joint.Z.S);
            this.SetKeysR(out this.rotationsX, Rotations, RotScale, Joint.X.R);
            this.SetKeysR(out this.rotationsY, Rotations, RotScale, Joint.Y.R);
            this.SetKeysR(out this.rotationsZ, Rotations, RotScale, Joint.Z.R);
            this.SetKeysST(out this.translationsX, Translations, Joint.X.T);
            this.SetKeysST(out this.translationsY, Translations, Joint.Y.T);
            this.SetKeysST(out this.translationsZ, Translations, Joint.Z.T);
          }

          private void SetKeysST(
            out float[] Destination,
            float[] Source,
            BCA.ANF1Section.AnimatedJoint.AnimComponent.AnimIndex Component)
          {
            Destination = new float[(int) Component.Count];
            if (Component.Count <= (ushort) 0)
              throw new Exception("Count <= 0");
            if (Component.Count == (ushort) 1)
            {
              Destination[0] = Source[(int) Component.Index];
            }
            else
            {
              for (int index = 0; index < (int) Component.Count; ++index)
                Destination[index] = Source[(int) Component.Index + index];
            }
          }

          private void SetKeysR(
            out float[] Destination,
            short[] Source,
            float RotScale,
            BCA.ANF1Section.AnimatedJoint.AnimComponent.AnimIndex Component)
          {
            Destination = new float[(int) Component.Count];
            if (Component.Count <= (ushort) 0)
              throw new Exception("Count <= 0");
            if (Component.Count == (ushort) 1)
            {
              Destination[0] = (float) Source[(int) Component.Index] * RotScale;
            }
            else
            {
              for (int index = 0; index < (int) Component.Count; ++index)
                Destination[index] = (float) Source[(int) Component.Index + index] * RotScale;
            }
          }
        }
      }
    }
  }
}
