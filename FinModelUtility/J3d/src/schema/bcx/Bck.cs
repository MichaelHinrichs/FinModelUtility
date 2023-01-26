// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.GCN.BCK
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.IO;
using fin.util.asserts;
using j3d.G3D_Binary_File_Format;
using schema;
using schema.attributes.endianness;
using schema.attributes.size;
using System.Linq;


namespace j3d.schema.bcx {
  /// <summary>
  ///   BCK files define joint animations with sparse keyframes.
  ///
  ///   https://wiki.cloudmodding.com/tww/BCK
  /// </summary>
  [Endianness(Endianness.BigEndian)]
  public partial class Bck : IBcx {
    public BckHeader Header;
    public ANK1Section ANK1;

    public Bck(byte[] file) {
      using IEndianBinaryReader er =
          new EndianBinaryReader((Stream)new MemoryStream(file),
                                 Endianness.BigEndian);
      this.Header = er.ReadNew<BckHeader>();
      this.ANK1 = new ANK1Section(er, out _);
    }

    public IAnx1 Anx1 => this.ANK1;

    [BinarySchema]
    public partial class BckHeader : IBiSerializable {
      private readonly string magic_ = "J3D1bck1";

      [SizeOfStreamInBytes]
      private uint fileSize_;

      private readonly uint sectionCount_ = 1;

      [ArrayLengthSource(16)]
      private byte[] padding_;
    }

    public partial class ANK1Section : IAnx1 {
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

      public ANK1Section(IEndianBinaryReader er, out bool OK) {
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
          er.Position = (long)(32U + this.ScaleOffset);
          this.Scale = er.ReadSingles((int)this.NrScale);
          er.Position = (long)(32U + this.RotOffset);
          this.Rotation = er.ReadInt16s((int)this.NrRot);
          er.Position = (long)(32U + this.TransOffset);
          this.Translation = er.ReadSingles((int)this.NrTrans);
          float RotScale =
              (float)(Math.Pow(2.0, (double)this.AngleMultiplier) *
                      Math.PI /
                      32768.0);
          er.Position = (long)(32U + this.JointOffset);
          this.Joints = new AnimatedJoint[(int)this.NrJoints];
          for (int index = 0; index < (int)this.NrJoints; ++index) {
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


      public partial class AnimatedJoint : IAnimatedJoint {
        public AnimComponent[] axes;

        public AnimatedJoint(IEndianBinaryReader er) {
          this.axes = new AnimComponent[3];
          for (var i = 0; i < axes.Length; ++i) {
            this.axes[i] = er.ReadNew<AnimComponent>();
          }
        }

        public IJointAnim Values { get; private set; }

        public void SetValues(
            float[] Scales,
            short[] Rotations,
            float[] Translations,
            float RotScale) {
          this.Values =
              new JointAnim(
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
          float num1 = (float)(2.0 * ((double)v1 - (double)v2)) + d1 + d2;
          float num2 =
              (float)(-3.0 * (double)v1 +
                      3.0 * (double)v2 -
                      2.0 * (double)d1) -
              d2;
          float num3 = d1;
          float num4 = v1;
          return ((num1 * t + num2) * t + num3) * t + num4;
        }

        public float GetAnimValue(JointAnim.Key[] keys, float t) {
          if (keys.Length == 0)
            return 0.0f;
          if (keys.Length == 1)
            return keys[0].Value;
          int index = 1;

          while ((double)keys[index].Frame < (double)t
                 // Don't shoot past the end of the keys list!
                 &&
                 index + 1 < keys.Length)
            ++index;

          if (index + 1 == keys.Length && keys[index].Frame < t) {
            return keys[0].Value;
          }

          float t1 = (float)(((double)t - (double)keys[index - 1].Frame) /
                             ((double)keys[index].Frame -
                              (double)keys[index - 1].Frame));


          return this.Interpolate(keys[index - 1].Value,
                                  keys[index - 1].OutgoingTangent,
                                  keys[index].Value,
                                  keys[index].IncomingTangent,
                                  t1);
        }

        [BinarySchema]
        public partial class AnimComponent : IBiSerializable {
          public AnimIndex S { get; } = new();
          public AnimIndex R { get; } = new();
          public AnimIndex T { get; } = new();
        }

        [BinarySchema]
        public partial class AnimIndex : IBiSerializable {
          public ushort Count;
          public ushort Index;
          public ushort TangentMode;
        }

        public class JointAnim : IJointAnim {
          public JointAnim(
              AnimatedJoint joint,
              float[] scales,
              short[] rotations,
              float[] translations,
              float rotScale) {
            this.Scales = joint.axes.Select(
                axisSrc => {
                  this.SetKeysSt_(out var axis, scales, axisSrc.S);
                  return axis;
                }).ToArray();
            this.Rotations = joint.axes.Select(
                axisSrc => {
                  this.SetKeysR_(out var axis, rotations, rotScale, axisSrc.R);
                  return axis;
                }).ToArray();
            this.Translations = joint.axes.Select(
                axisSrc => {
                  this.SetKeysSt_(out var axis, translations, axisSrc.T);
                  return axis;
                }).ToArray();
          }

          public IJointAnimKey[][] Scales { get; }
          public IJointAnimKey[][] Rotations { get; }
          public IJointAnimKey[][] Translations { get; }

          private void SetKeysSt_(
              out IJointAnimKey[] dst,
              float[] src,
              AnimIndex component) {
            dst = new IJointAnimKey[component.Count];
            if (component.Count <= 0)
              throw new Exception("Count <= 0");
            if (component.Count == 1) {
              dst[0] =
                  new Key(
                      0,
                      src[component.Index],
                      0,
                      0);
            } else {
              var tangentMode = component.TangentMode;
              var hasTwoTangents = tangentMode == 1;
              Asserts.True(tangentMode == 0 || tangentMode == 1);

              var stride = hasTwoTangents ? 4 : 3;
              for (var index = 0; index < component.Count; ++index) {
                var i = component.Index + stride * index;

                var time = (int) src[i + 0];
                var value = src[i + 1];

                float incomingTangent, outgoingTangent;
                if (hasTwoTangents) {
                  incomingTangent = src[i + 2];
                  outgoingTangent = src[i + 3];
                } else {
                  incomingTangent = outgoingTangent = src[i + 2];
                }

                dst[index] =
                    new Key(
                        time,
                        value,
                        incomingTangent,
                        outgoingTangent);
              }
            }
          }

          private void SetKeysR_(
              out IJointAnimKey[] dst,
              short[] src,
              float rotScale,
              AnimIndex component) {
            dst =
                new IJointAnimKey[component.Count];
            if (component.Count <= 0)
              throw new Exception("Count <= 0");
            if (component.Count == 1) {
              dst[0] = new JointAnim.Key(
                  0,
                  src[component.Index] * rotScale,
                  0,
                  0);
            } else {
              var tangentMode = component.TangentMode;
              var hasTwoTangents = tangentMode == 1;
              Asserts.True(tangentMode == 0 || tangentMode == 1);

              var stride = hasTwoTangents ? 4 : 3;
              for (var index = 0; index < component.Count; ++index) {
                var i = component.Index + stride * index;

                var time = src[i + 0];
                var value = src[i + 1] * rotScale;

                float incomingTangent, outgoingTangent;
                if (hasTwoTangents) {
                  incomingTangent = src[i + 2] * rotScale;
                  outgoingTangent = src[i + 3] * rotScale;
                } else {
                  incomingTangent = outgoingTangent = src[i + 2] * rotScale;
                }

                dst[index] =
                    new Key(
                        time,
                        value,
                        incomingTangent,
                        outgoingTangent);
              }
            }
          }

          public class Key : IJointAnimKey {
            public Key(
                int frame,
                float value,
                float incomingTangent,
                float outgoingTangent) {
              this.Frame = frame;
              this.Value = value;
              this.IncomingTangent = incomingTangent;
              this.OutgoingTangent = outgoingTangent;
            }

            public int Frame { get; }
            public float Value { get; }
            public float IncomingTangent { get; }
            public float OutgoingTangent { get; }
          }
        }
      }
    }
  }
}