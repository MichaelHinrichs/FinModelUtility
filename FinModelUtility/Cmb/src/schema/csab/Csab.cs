using System.Collections.Generic;
using System.IO;

using fin.util.asserts;

using schema.binary;
using schema.binary.attributes.endianness;


namespace cmb.schema.csab {
  public class CsabKeyframe {
    public uint Time { get; set; }
    public float Value { get; set; }

    public float? IncomingTangent { get; set; }
    public float? OutgoingTangent { get; set; }
  }

  public enum TrackType {
    POSITION,
    SCALE,
    ROTATION
  }

  public class CsabTrack : IBinaryDeserializable {
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

    public bool IsPastVersion4 => this.parent_.IsPastVersion4;

    public void Read(IEndianBinaryReader r) {
      var startFrame = 0;
      if (IsPastVersion4) {
        var isConstant = r.ReadByte() != 0;
        this.Type = (AnimationTrackType) r.ReadByte();
        this.Keyframes = new CsabKeyframe[r.ReadUInt16()];

        if (isConstant) {
          for (var i = 0; i < this.Keyframes.Count; ++i) {
            var scale = r.ReadSingle();
            var bias = r.ReadSingle();
            var value = r.ReadUInt32();

            this.Keyframes[i] = new CsabKeyframe {
                Time = (uint) i, Value = value * scale - bias,
            };
          }

          return;
        }
      } else {
        this.Type = (AnimationTrackType) r.ReadUInt32();
        this.Keyframes = new CsabKeyframe[r.ReadUInt32()];
        startFrame = r.ReadInt32();
        this.Duration = r.ReadUInt32();
      }

      if (startFrame < 0) {
        ;
      }

      float trackScale = -1;
      float trackBias = -1;
      if (IsPastVersion4 && this.Type == AnimationTrackType.LINEAR) {
        trackScale = r.ReadSingle();
        trackBias = r.ReadSingle();
      }

      for (var i = 0; i < this.Keyframes.Count; ++i) {
        this.Keyframes[i] = this.Type switch {
            AnimationTrackType.LINEAR when !this.AreRotationsShort
                => this.ReadKeyframeLinearFloat_(
                    r,
                    trackScale,
                    trackBias,
                    startFrame,
                    i),
            AnimationTrackType.LINEAR when this.AreRotationsShort
                => this.ReadKeyframeLinearShort_(
                    r,
                    trackScale,
                    trackBias,
                    startFrame,
                    i),
            AnimationTrackType.HERMITE when !this.AreRotationsShort
                => this.ReadKeyframeHermiteFloat_(r, startFrame),
            AnimationTrackType.HERMITE when this.AreRotationsShort
                => this.ReadKeyframeHermiteShort_(r, startFrame),
        };
      }

      r.Align(4);
    }

    private CsabKeyframe ReadKeyframeLinearFloat_(
        IEndianBinaryReader er,
        float trackScale,
        float trackBias,
        int startFrame,
        int index) {
      if (IsPastVersion4) {
        // TODO: Is this right????
        var raw = this.ValueType switch {
            TrackType.POSITION => (int) er.ReadUInt16(),
            TrackType.SCALE    => (int) er.ReadInt16(),
            TrackType.ROTATION => (int) er.ReadInt16(),
        };

        var value = raw * trackScale - trackBias;
        return new CsabKeyframe {
            Time = (uint) (startFrame + index), Value = value,
        };
      }

      return new CsabKeyframe {
          Time = (uint) (startFrame + er.ReadUInt32()), Value = er.ReadSingle(),
      };
    }

    private CsabKeyframe ReadKeyframeLinearShort_(
        IEndianBinaryReader er,
        float trackScale,
        float trackBias,
        int startFrame,
        int index) {
      if (IsPastVersion4) {
        // TODO: Is this right????
        var raw = this.ValueType switch {
            TrackType.POSITION => (int) er.ReadUInt16(),
            TrackType.SCALE    => (int) er.ReadInt16(),
            TrackType.ROTATION => (int) er.ReadInt16(),
        };

        var value = raw * trackScale - trackBias;
        return new CsabKeyframe {
            Time = (uint) (startFrame + index), Value = value,
        };
      }

      return new CsabKeyframe {
          Time = (uint) (startFrame + er.ReadUInt16()), Value = er.ReadSn16(),
      };
    }

    private CsabKeyframe ReadKeyframeHermiteFloat_(IEndianBinaryReader er,
                                                   int startFrame)
      => new CsabKeyframe {
          Time = (uint) (startFrame + er.ReadUInt32()),
          Value = er.ReadSingle(),
          IncomingTangent = er.ReadSingle(),
          OutgoingTangent = er.ReadSingle(),
      };

    private CsabKeyframe ReadKeyframeHermiteShort_(IEndianBinaryReader er,
                                                   int startFrame)
      => new CsabKeyframe {
          Time = (uint) (startFrame + er.ReadUInt16()),
          Value = er.ReadSn16(),
          IncomingTangent = er.ReadSn16(),
          OutgoingTangent = er.ReadSn16(),
      };
  }


  public class AnimationNode : IBinaryDeserializable {
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

    public bool IsPastVersion4 => this.parent_.IsPastVersion4;

    public void Read(IEndianBinaryReader r) {
      var basePosition = r.Position;

      r.AssertMagicText("anod");

      this.BoneIndex = r.ReadUInt16();

      bool isRotationShort;
      if (IsPastVersion4) {
        isRotationShort = r.ReadByte() != 0;
        var unk = r.ReadByte();
      } else {
        isRotationShort = r.ReadUInt16() != 0;
      }

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

  [Endianness(Endianness.LittleEndian)]
  public class Csab : IBinaryDeserializable {
    public uint Version { get; set; }
    public bool IsPastVersion4 => this.Version > 4;

    public uint Duration { get; set; }

    public Dictionary<int, AnimationNode>
        BoneIndexToAnimationNode { get; set; } = new();

    public void Read(IEndianBinaryReader r) {
      var basePosition = r.Position;

      r.AssertMagicText("csab");
      var size = r.ReadUInt32();

      // Subversion?
      this.Version = r.ReadUInt32();

      r.AssertUInt32(0x00);

      if (IsPastVersion4) {
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