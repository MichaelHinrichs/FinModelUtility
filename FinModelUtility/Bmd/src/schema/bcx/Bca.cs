// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.GCN.BCA
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using bmd.G3D_Binary_File_Format;
using schema;
using System;
using System.IO;
using System.Text;
using schema.attributes.endianness;
using schema.attributes.size;


namespace bmd.schema.bcx {
  /// <summary>
  ///   BCA files define joint animations where each frame is defined.
  /// </summary>
  [Endianness(Endianness.BigEndian)]
  public partial class Bca : IBcx {
    public BcaHeader Header;
    public ANF1Section ANF1;

    public Bca(byte[] file) {
      using EndianBinaryReader er =
          new EndianBinaryReader((Stream)new MemoryStream(file),
                                 Endianness.BigEndian);
      this.Header = er.ReadNew<BcaHeader>();
      this.ANF1 = new Bca.ANF1Section(er, out _);
    }

    public IAnx1 Anx1 => this.ANF1;

    [BinarySchema]
    public partial class BcaHeader : IBiSerializable {
      private readonly string magic_ = "J3D1bca1";

      [SizeOfStreamInBytes]
      private uint fileSize_;

      private readonly uint sectionCount_ = 1;

      [ArrayLengthSource(16)]
      private byte[] padding_;
    }

    public partial class ANF1Section : IAnx1 {
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

      public ANF1Section(EndianBinaryReader er, out bool OK) {
        bool OK1;

        this.Header = new DataBlockHeader(er, "ANF1", out OK1);
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
          float RotScale = (float)(1 * Math.PI / 32768f);
          er.Position = (long)(32U + this.JointOffset);
          this.Joints = new Bca.ANF1Section.AnimatedJoint[(int)this.NrJoints];
          for (int index = 0; index < (int)this.NrJoints; ++index) {
            var animatedJoint = new Bca.ANF1Section.AnimatedJoint(er);
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
        public AnimComponent X;
        public AnimComponent Y;
        public AnimComponent Z;

        public AnimatedJoint(EndianBinaryReader er) {
          this.X = er.ReadNew<AnimComponent>();
          this.Y = er.ReadNew<AnimComponent>();
          this.Z = er.ReadNew<AnimComponent>();
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

        public float GetAnimValue(IJointAnimKey[] keys, float t) {
          if (keys.Length == 0)
            return 0.0f;
          return keys.Length == 1 ? keys[0].Value : keys[(int)t].Value;
        }

        [BinarySchema]
        public partial class AnimComponent : IBiSerializable {
          public AnimIndex S { get; } = new();
          public AnimIndex R { get; } = new();
          public AnimIndex T { get; } = new();

          [BinarySchema]
          public partial class AnimIndex : IBiSerializable {
            public ushort Count;
            public ushort Index;
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
              Bca.ANF1Section.AnimatedJoint Joint,
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
              Bca.ANF1Section.AnimatedJoint.AnimComponent.AnimIndex Component) {
            Destination = new IJointAnimKey[(int)Component.Count];
            if (Component.Count <= (ushort)0)
              throw new Exception("Count <= 0");
            if (Component.Count == (ushort)1) {
              Destination[0] = new Key(0, Source[(int)Component.Index]);
            } else {
              for (int index = 0; index < (int)Component.Count; ++index)
                Destination[index] =
                    new Key(index, Source[(int)Component.Index + index]);
            }
          }

          private void SetKeysR(
              out IJointAnimKey[] Destination,
              short[] Source,
              float RotScale,
              Bca.ANF1Section.AnimatedJoint.AnimComponent.AnimIndex Component) {
            Destination = new IJointAnimKey[(int)Component.Count];
            if (Component.Count <= (ushort)0)
              throw new Exception("Count <= 0");
            if (Component.Count == (ushort)1) {
              Destination[0] =
                  new Key(0, (float)Source[(int)Component.Index] * RotScale);
            } else {
              for (int index = 0; index < (int)Component.Count; ++index)
                Destination[index] =
                    new Key(index, (float)Source[(int)Component.Index + index] *
                            RotScale);
            }
          }

          private sealed class Key : IJointAnimKey {
            public Key(float time, float value) {
              this.Time = time;
              this.Value = value;
            }

            public float Time { get; }
            public float Value { get; }

            // TODO: What should this actually be?
            public float IncomingTangent => 1;
            public float OutgoingTangent => 1;
          }
        }
      }
    }
  }
}