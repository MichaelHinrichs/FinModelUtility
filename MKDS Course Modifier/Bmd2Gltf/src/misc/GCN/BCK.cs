// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.GCN.BCK
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier._3D_Formats;
using MKDS_Course_Modifier.G3D_Binary_File_Format;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using fin.log;

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;

using mkds.exporter;

namespace MKDS_Course_Modifier.GCN {
  /// <summary>
  /// https://wiki.cloudmodding.com/tww/BCK
  /// </summary>
  public class BCK : IBcx {
    public const string Signature = "J3D1bck1";
    public BCK.BCKHeader Header;
    public BCK.ANK1Section ANK1;

    public BCK(byte[] file) {
      EndianBinaryReader er =
          new EndianBinaryReader((Stream) new MemoryStream(file),
                                 Endianness.BigEndian);
      bool OK;
      this.Header = new BCK.BCKHeader(er, "J3D1bck1", out OK);
      if (!OK) {
        // TODO: Message box
        //int num1 = (int) MessageBox.Show("Error 1");
      } else {
        this.ANK1 = new BCK.ANK1Section(er, out OK);
        if (!OK) {
          // TODO: Message box
          //int num2 = (int) MessageBox.Show("Error 2");
        }
      }
      er.Close();
    }

    public IAnx1 Anx1 => this.ANK1;

    public class BCKHeader {
      public string Type;
      public uint FileSize;
      public uint NrSections;
      public byte[] Padding;

      public BCKHeader(EndianBinaryReader er, string Signature, out bool OK) {
        this.Type = er.ReadString(Encoding.ASCII, 8);
        if (this.Type != Signature) {
          OK = false;
        } else {
          this.FileSize = er.ReadUInt32();
          this.NrSections = er.ReadUInt32();
          this.Padding = er.ReadBytes(16);
          OK = true;
        }
      }
    }

    public class ANK1Section : IAnx1 {
      public const string Signature = "ANK1";
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

      public ANK1Section(EndianBinaryReader er, out bool OK) {
        bool OK1;
        this.Header = new DataBlockHeader(er, "ANK1", out OK1);
        if (!OK1) {
          OK = false;
        } else {
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
          float RotScale =
              (float) (Math.Pow(2.0, (double) this.AngleMultiplier) *
                       Math.PI /
                       32768.0);
          er.BaseStream.Position = (long) (32U + this.JointOffset);
          this.Joints = new AnimatedJoint[(int) this.NrJoints];
          for (int index = 0; index < (int) this.NrJoints; ++index) {
            var animatedJoint = new AnimatedJoint(er);
            animatedJoint.SetValues(this.Scale,
                                    this.Rotation,
                                    this.Translation,
                                    RotScale);
            this.Joints[index] = animatedJoint;
          }
          OK = true;
        }
      }

      public int FrameCount => this.AnimLength;
      public IAnimatedJoint[] Joints { get; }


      public byte[] ExportBonesAnimation(BMD b) {
        MA.Node[] joints = b.GetJoints();
        //ScaleDialog scaleDialog = new ScaleDialog();
        //int num = (int) scaleDialog.ShowDialog();
        float scale = 1; //scaleDialog.scale;
        List<MA.AnimatedNode> animatedNodeList = new List<MA.AnimatedNode>();
        foreach (MA.Node node in joints) {
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
          for (int index2 = 0; index2 < (int) this.AnimLength; ++index2) {
            floatList1.Add(this.Joints[index1]
                               .GetAnimValue(
                                   this.Joints[index1].Values.translationsX,
                                   (float) index2) *
                           scale);
            floatList2.Add(this.Joints[index1]
                               .GetAnimValue(
                                   this.Joints[index1].Values.translationsY,
                                   (float) index2) *
                           scale);
            floatList3.Add(this.Joints[index1]
                               .GetAnimValue(
                                   this.Joints[index1].Values.translationsZ,
                                   (float) index2) *
                           scale);
            floatList4.Add(this.Joints[index1]
                               .GetAnimValue(
                                   this.Joints[index1].Values.rotationsX,
                                   (float) index2));
            floatList5.Add(this.Joints[index1]
                               .GetAnimValue(
                                   this.Joints[index1].Values.rotationsY,
                                   (float) index2));
            floatList6.Add(this.Joints[index1]
                               .GetAnimValue(
                                   this.Joints[index1].Values.rotationsZ,
                                   (float) index2));
            floatList7.Add(this.Joints[index1]
                               .GetAnimValue(
                                   this.Joints[index1].Values.scalesX,
                                   (float) index2));
            floatList8.Add(this.Joints[index1]
                               .GetAnimValue(
                                   this.Joints[index1].Values.scalesY,
                                   (float) index2));
            floatList9.Add(this.Joints[index1]
                               .GetAnimValue(
                                   this.Joints[index1].Values.scalesZ,
                                   (float) index2));
          }
          animatedNodeList.Add(new MA.AnimatedNode(
                                   (int) this.AnimLength,
                                   floatList1.ToArray(),
                                   floatList2.ToArray(),
                                   floatList3.ToArray(),
                                   floatList4.ToArray(),
                                   floatList5.ToArray(),
                                   floatList6.ToArray(),
                                   floatList7.ToArray(),
                                   floatList8.ToArray(),
                                   floatList9.ToArray()));
        }
        return MA.WriteAnimation(joints, animatedNodeList.ToArray());
      }

      public class AnimatedJoint : IAnimatedJoint {
        public BCK.ANK1Section.AnimatedJoint.AnimComponent X;
        public BCK.ANK1Section.AnimatedJoint.AnimComponent Y;
        public BCK.ANK1Section.AnimatedJoint.AnimComponent Z;

        public AnimatedJoint(EndianBinaryReader er) {
          this.X = new BCK.ANK1Section.AnimatedJoint.AnimComponent(er);
          this.Y = new BCK.ANK1Section.AnimatedJoint.AnimComponent(er);
          this.Z = new BCK.ANK1Section.AnimatedJoint.AnimComponent(er);
        }

        public IJointAnim Values { get; private set; }

        public void SetValues(
            float[] Scales,
            short[] Rotations,
            float[] Translations,
            float RotScale) {
          this.Values =
              new BCK.ANK1Section.AnimatedJoint.JointAnim(
                  this,
                  Scales,
                  Rotations,
                  Translations,
                  RotScale);
        }

        private float Interpolate(
            float v1,
            float d1,
            float v2,
            float d2,
            float t) {
          float num1 = (float) (2.0 * ((double) v1 - (double) v2)) + d1 + d2;
          float num2 =
              (float) (-3.0 * (double) v1 +
                       3.0 * (double) v2 -
                       2.0 * (double) d1) -
              d2;
          float num3 = d1;
          float num4 = v1;
          return ((num1 * t + num2) * t + num3) * t + num4;
        }

        public float GetAnimValue(IJointAnimKey[] keys, float t) {
          if (keys.Length == 0)
            return 0.0f;
          if (keys.Length == 1)
            return keys[0].Value;
          int index = 1;
          while ((double) keys[index].Time < (double) t)
            ++index;
          float t1 = (float) (((double) t - (double) keys[index - 1].Time) /
                              ((double) keys[index].Time -
                               (double) keys[index - 1].Time));
          return this.Interpolate(keys[index - 1].Value,
                                  keys[index - 1].OutgoingTangent,
                                  keys[index].Value,
                                  keys[index].IncomingTangent,
                                  t1);
        }

        public class AnimComponent {
          public BCK.ANK1Section.AnimatedJoint.AnimComponent.AnimIndex S;
          public BCK.ANK1Section.AnimatedJoint.AnimComponent.AnimIndex R;
          public BCK.ANK1Section.AnimatedJoint.AnimComponent.AnimIndex T;

          public AnimComponent(EndianBinaryReader er) {
            this.S =
                new BCK.ANK1Section.AnimatedJoint.AnimComponent.AnimIndex(er);
            this.R =
                new BCK.ANK1Section.AnimatedJoint.AnimComponent.AnimIndex(er);
            this.T =
                new BCK.ANK1Section.AnimatedJoint.AnimComponent.AnimIndex(er);
          }

          public class AnimIndex {
            public ushort Count;
            public ushort Index;
            public ushort TangentMode;

            public AnimIndex(EndianBinaryReader er) {
              this.Count = er.ReadUInt16();
              this.Index = er.ReadUInt16();
              this.TangentMode = er.ReadUInt16();
            }
          }
        }

        public class JointAnim : IJointAnim {
          private IJointAnimKey[] scalesX_;
          private IJointAnimKey[] scalesY_;
          private IJointAnimKey[] scalesZ_;
          private IJointAnimKey[] rotationsX_;
          private IJointAnimKey[] rotationsY_;
          private IJointAnimKey[] rotationsZ_;
          private IJointAnimKey[] translationsX_;
          private IJointAnimKey[] translationsY_;
          private IJointAnimKey[] translationsZ_;

          public JointAnim(
              BCK.ANK1Section.AnimatedJoint Joint,
              float[] Scales,
              short[] Rotations,
              float[] Translations,
              float RotScale) {
            this.SetKeysST(out this.scalesX_, Scales, Joint.X.S);
            this.SetKeysST(out this.scalesY_, Scales, Joint.Y.S);
            this.SetKeysST(out this.scalesZ_, Scales, Joint.Z.S);
            this.SetKeysR(out this.rotationsX_, Rotations, RotScale, Joint.X.R);
            this.SetKeysR(out this.rotationsY_, Rotations, RotScale, Joint.Y.R);
            this.SetKeysR(out this.rotationsZ_, Rotations, RotScale, Joint.Z.R);
            this.SetKeysST(out this.translationsX_, Translations, Joint.X.T);
            this.SetKeysST(out this.translationsY_, Translations, Joint.Y.T);
            this.SetKeysST(out this.translationsZ_, Translations, Joint.Z.T);
          }

          public IJointAnimKey[] scalesX => this.scalesX_;
          public IJointAnimKey[] scalesY => this.scalesY_;
          public IJointAnimKey[] scalesZ => this.scalesZ_;

          public IJointAnimKey[] rotationsX => this.rotationsX_;
          public IJointAnimKey[] rotationsY => this.rotationsY_;
          public IJointAnimKey[] rotationsZ => this.rotationsZ_;

          public IJointAnimKey[] translationsX => this.translationsX_;
          public IJointAnimKey[] translationsY => this.translationsY_;
          public IJointAnimKey[] translationsZ => this.translationsZ_;

          private void SetKeysST(
              out IJointAnimKey[] Destination,
              float[] Source,
              AnimComponent.AnimIndex Component) {
            Destination = new IJointAnimKey[(int) Component.Count];
            if (Component.Count <= (ushort) 0)
              throw new Exception("Count <= 0");
            if (Component.Count == (ushort) 1) {
              Destination[0] =
                  new Key(
                      0.0f,
                      Source[(int) Component.Index],
                      0,
                      0);
            } else {
              for (int index = 0; index < (int) Component.Count; ++index) {
                // TODO: What to do about incoming/outgoing?
                var tangent = Source[(int) Component.Index + 3 * index + 2];
                Destination[index] =
                    new Key(
                        Source[(int) Component.Index + 3 * index],
                        Source[(int) Component.Index + 3 * index + 1],
                        tangent,
                        tangent);
              }
            }
          }

          private void SetKeysR(
              out IJointAnimKey[] Destination,
              short[] Source,
              float RotScale,
              BCK.ANK1Section.AnimatedJoint.AnimComponent.AnimIndex Component) {
            Destination =
                new IJointAnimKey[(int) Component
                    .Count];
            if (Component.Count <= (ushort) 0)
              throw new Exception("Count <= 0");
            if (Component.Count == (ushort) 1) {
              Destination[0] = new BCK.ANK1Section.AnimatedJoint.JointAnim.Key(
                  0.0f,
                  (float) Source[(int) Component.Index] * RotScale,
                  0,
                  0);
            } else {
              for (int index = 0; index < (int) Component.Count; ++index) {
                // TODO: What to do about incoming/outgoing?
                var tangent =
                    (float) Source[(int) Component.Index + 3 * index + 2] *
                    RotScale;

                Destination[index] =
                    new BCK.ANK1Section.AnimatedJoint.JointAnim.Key(
                        (float) Source[(int) Component.Index + 3 * index],
                        (float) Source[(int) Component.Index + 3 * index + 1] *
                        RotScale,
                        tangent,
                        tangent);
              }
            }
          }

          public class Key : IJointAnimKey {
            public Key(
                float Time,
                float Value,
                float incomingTangent,
                float outgoingTangent) {
              this.Time = Time;
              this.Value = Value;
              this.IncomingTangent = incomingTangent;
              this.OutgoingTangent = outgoingTangent;
            }

            public float Time { get; }
            public float Value { get; }
            public float IncomingTangent { get; } = 1;
            public float OutgoingTangent { get; } = 1;
          }
        }
      }
    }
  }
}