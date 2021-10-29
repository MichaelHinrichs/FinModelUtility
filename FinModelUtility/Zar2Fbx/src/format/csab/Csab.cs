using System;
using System.Collections.Generic;
using System.IO;

using fin.io;
using fin.util.asserts;
using fin.util.optional;

namespace zar.format.csab {
  public class CsabKeyframe {
    public uint Time { get; set; }
    public float Value { get; set; }

    public Optional<float> IncomingTangent { get; set; } =
      Optional<float>.None();

    public Optional<float> OutgoingTangent { get; set; } =
      Optional<float>.None();
  }

  public enum TrackType {
    POSITION,
    SCALE,
    ROTATION
  }

  public class CsabTrack : IDeserializable {
    private readonly Csab parent_;

    public CsabTrack(Csab parent) {
      this.parent_ = parent;
    }

    public TrackType ValueType { get; set; }

    public AnimationTrackType Type { get; set; }

    public IList<CsabKeyframe> Keyframes { get; set; } =
      new List<CsabKeyframe>();

    public uint Duration { get; set; }

    public bool AreRotationsShort { get; set; }

    public void Read(EndianBinaryReader r) {
      bool isConstant = false;
      if (this.parent_.Version > 4) {
        isConstant = r.ReadByte() != 0;
        this.Type = (AnimationTrackType) r.ReadByte();
        this.Keyframes = new CsabKeyframe[r.ReadUInt16()];
      } else {
        this.Type = (AnimationTrackType) r.ReadUInt32();
        this.Keyframes = new CsabKeyframe[r.ReadUInt32()];
        var unk1 = r.ReadUInt32();
        this.Duration = r.ReadUInt32();
      }

      if (isConstant) {
        var scale = r.ReadSingle();
        var bias = r.ReadSingle();

        for (var i = 0; i < this.Keyframes.Count; ++i) {
          var value = r.ReadUInt32();

          ;

          this.Keyframes[i] = new CsabKeyframe {
              Time = (uint) i,
              Value = value * scale - bias,
          };
        }
      }

      float trackScale = -1;
      float trackBias = -1;
      if (this.parent_.Version > 4 && this.Type == AnimationTrackType.LINEAR) {
        trackScale = r.ReadSingle();
        trackBias = r.ReadSingle();

        ;
      }

      switch (this.Type) {
        case AnimationTrackType.LINEAR: {
          for (var i = 0; i < this.Keyframes.Count; ++i) {
            if (!this.AreRotationsShort) {
              if (this.parent_.Version > 4) {
                // TODO: Is this right????
                var raw = this.ValueType switch {
                    TrackType.POSITION => (int) r.ReadUInt16(),
                    TrackType.SCALE    => (int) r.ReadInt16(),
                    _                  => throw new NotSupportedException(),
                };
                var value = raw * trackScale - trackBias;
                this.Keyframes[i] = new CsabKeyframe {
                    Time = (uint) i,
                    Value = value,
                };
              } else {
                this.Keyframes[i] = new CsabKeyframe {
                    Time = r.ReadUInt32(),
                    Value = r.ReadSingle(),
                };
              }
            } else {
              if (this.parent_.Version > 4) {
                var value = r.ReadInt16() * trackScale + trackBias;
                this.Keyframes[i] = new CsabKeyframe {
                    Time = (uint) i,
                    Value = value,
                };
              } else {
                this.Keyframes[i] = new CsabKeyframe {
                    Time = r.ReadUInt16(),
                    Value = r.ReadSn16(),
                };
              }
            }
          }
          break;
        }
        case AnimationTrackType.HERMITE: {
          for (var i = 0; i < this.Keyframes.Count; ++i) {
            if (!this.AreRotationsShort) {
              this.Keyframes[i] = new CsabKeyframe {
                  Time = r.ReadUInt32(),
                  Value = r.ReadSingle(),
                  IncomingTangent = Optional<float>.Of(r.ReadSingle()),
                  OutgoingTangent = Optional<float>.Of(r.ReadSingle()),
              };
            } else {
              this.Keyframes[i] = new CsabKeyframe {
                  Time = r.ReadUInt16(),
                  Value = r.ReadSn16(),
                  IncomingTangent = Optional<float>.Of(r.ReadSn16()),
                  OutgoingTangent = Optional<float>.Of(r.ReadSn16()),
              };
            }
          }
          break;
        }
        default: throw new ArgumentOutOfRangeException();
      }
      r.Align(4);
    }
  }


  public class AnimationNode : IDeserializable {
    private readonly Csab parent_;

    public AnimationNode(Csab parent) {
      this.parent_ = parent;

      this.ScaleX = new(parent) {ValueType = TrackType.SCALE};
      this.ScaleY = new(parent) {ValueType = TrackType.SCALE};
      this.ScaleZ = new(parent) {ValueType = TrackType.SCALE};

      this.TranslationX = new(parent) {ValueType = TrackType.POSITION};
      this.TranslationY = new(parent) {ValueType = TrackType.POSITION};
      this.TranslationZ = new(parent) {ValueType = TrackType.POSITION};

      this.RotationX = new(parent) {ValueType = TrackType.ROTATION};
      this.RotationY = new(parent) {ValueType = TrackType.ROTATION};
      this.RotationZ = new(parent) {ValueType = TrackType.ROTATION};
    }

    public ushort BoneIndex { get; set; }

    public CsabTrack ScaleX { get; }
    public CsabTrack ScaleY { get; }
    public CsabTrack ScaleZ { get; }

    public CsabTrack TranslationX { get; }
    public CsabTrack TranslationY { get; }
    public CsabTrack TranslationZ { get; }

    public CsabTrack RotationX { get; }
    public CsabTrack RotationY { get; }
    public CsabTrack RotationZ { get; }

    public void Read(EndianBinaryReader r) {
      var basePosition = r.Position;

      r.AssertMagicText("anod");

      this.BoneIndex = r.ReadUInt16();

      var isRotationShort = r.ReadUInt16() != 0;
      this.RotationX.AreRotationsShort = isRotationShort;
      this.RotationY.AreRotationsShort = isRotationShort;
      this.RotationZ.AreRotationsShort = isRotationShort;


      var translationXOffset = r.ReadUInt16();
      var translationYOffset = r.ReadUInt16();
      var translationZOffset = r.ReadUInt16();

      var rotationXOffset = r.ReadUInt16();
      var rotationYOffset = r.ReadUInt16();
      var rotationZOffset = r.ReadUInt16();

      var scaleXOffset = r.ReadUInt16();
      var scaleYOffset = r.ReadUInt16();
      var scaleZOffset = r.ReadUInt16();

      r.AssertUInt16(0x00);

      if (translationXOffset != 0) {
        r.Subread(basePosition + translationXOffset,
                  sr => this.TranslationX.Read(sr));
      }
      if (translationYOffset != 0) {
        r.Subread(basePosition + translationYOffset,
                  sr => this.TranslationY.Read(sr));
      }
      if (translationZOffset != 0) {
        r.Subread(basePosition + translationZOffset,
                  sr => this.TranslationZ.Read(sr));
      }

      if (rotationXOffset != 0) {
        r.Subread(basePosition + rotationXOffset,
                  sr => this.RotationX.Read(sr));
      }
      if (rotationYOffset != 0) {
        r.Subread(basePosition + rotationYOffset,
                  sr => this.RotationY.Read(sr));
      }
      if (rotationZOffset != 0) {
        r.Subread(basePosition + rotationZOffset,
                  sr => this.RotationZ.Read(sr));
      }

      if (scaleXOffset != 0) {
        r.Subread(basePosition + scaleXOffset,
                  sr => this.ScaleX.Read(sr));
      }
      if (scaleYOffset != 0) {
        r.Subread(basePosition + scaleYOffset,
                  sr => this.ScaleY.Read(sr));
      }
      if (scaleZOffset != 0) {
        r.Subread(basePosition + scaleZOffset,
                  sr => this.ScaleZ.Read(sr));
      }
    }
  }

  public class Csab : IDeserializable {
    public uint Version { get; set; }
    public uint Duration { get; set; }

    public Dictionary<int, AnimationNode>
        BoneIndexToAnimationNode { get; set; } = new();

    public void Read(EndianBinaryReader r) {
      var basePosition = r.Position;

      r.AssertMagicText("csab");
      var size = r.ReadUInt32();

      // Subversion?
      this.Version = r.ReadUInt32();

      r.AssertUInt32(0x00);

      if (this.Version > 4) {
        // M-1: Min or max?
        r.ReadSingle();
        r.ReadSingle();
        r.ReadSingle();
      }

      // Num animations?
      r.AssertUInt32(0x01);
      // Location?
      var animationOffset = r.ReadUInt32();

      r.AssertUInt32(0x00);
      r.AssertUInt32(0x00);
      r.AssertUInt32(0x00);
      r.AssertUInt32(0x00);

      this.Duration = r.ReadUInt32();

      // Jasper and M-1 believe this is loop mode, where 0 is a non-looping and
      // 1 is looping. But this doesn't seem to actually correlate with the
      // animations you'd expect to be looping vs. non-looping?
      var loopMode = r.ReadUInt32();

      var anodCount = r.ReadUInt32();
      var boneCount = r.ReadUInt32();
      Asserts.True(anodCount <= boneCount);

      // Jasper: This appears to be an inverse of the bone index in each array,
      // probably for fast binding?
      var boneToAnimationTable = new short[boneCount];
      //var boneTableIdx = basePosition + animationOffset + 0x20;
      //r.Position = boneTableIdx;
      for (var i = 0; i < boneCount; ++i) {
        boneToAnimationTable[i] = r.ReadInt16();
      }

      // TODO(jstpierre): This doesn't seem like a Grezzo thing to do.
      var anodTableIdx = this.Align_(r.Position, 0x04);
      r.Position = anodTableIdx;

      var animationNodes = new AnimationNode[anodCount];
      for (var i = 0; i < anodCount; ++i) {
        var anod = new AnimationNode(this);

        var offset = r.ReadUInt32();
        r.Subread(basePosition + animationOffset + offset, sr => anod.Read(sr));

        animationNodes[i] = anod;
      }

      for (var b = 0; b < boneCount; ++b) {
        var anodIndex = boneToAnimationTable[b];
        if (anodIndex != -1) {
          var anod = animationNodes[anodIndex];
          this.BoneIndexToAnimationNode[b] = anod;
        }
      }
    }

    private long Align_(long n, int multiple) {
      var mask = multiple - 1;
      return (n + mask) & ~mask;
    }
  }
}