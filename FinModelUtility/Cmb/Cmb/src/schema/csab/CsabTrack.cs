using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

using schema.binary;

namespace cmb.schema.csab {
  public enum TrackType {
    POSITION,
    SCALE,
    ROTATION
  }

  public class CsabTrack : IBinaryDeserializable {
    private readonly Csab parent_;
    private Func<IEndianBinaryReader, int> readRawLinearFloat_;
    private Func<IEndianBinaryReader, int> readRawLinearShort_;

    public CsabTrack(Csab parent,
                     TrackType valueType) {
      this.parent_ = parent;
      this.ValueType = valueType;

      // TODO: Is this right????
      this.readRawLinearFloat_ = this.ValueType switch {
          // Weird that this is different, but this really does seem to be right.
          TrackType.POSITION => er => er.ReadUInt16() / 2,
          TrackType.SCALE    => er => er.ReadInt16(),
          TrackType.ROTATION => er => er.ReadInt16(),
      };
      // TODO: Is this right????
      this.readRawLinearShort_ = this.ValueType switch {
          // Weird that this is different, but this really does seem to be right.
          TrackType.POSITION => er => er.ReadUInt16(),
          TrackType.SCALE    => er => er.ReadInt16(),
          TrackType.ROTATION => er => er.ReadInt16(),
      };
    }

    public TrackType ValueType { get; }

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
                Time = (uint) i, Value = ApplyScaleAndBias_(value, scale, bias),
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
        var raw = readRawLinearFloat_(er);
        var value = ApplyScaleAndBias_(raw, trackScale, trackBias);
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
        var raw = this.readRawLinearShort_(er);
        var value = ApplyScaleAndBias_(raw, trackScale, trackBias);
        return new CsabKeyframe {
            Time = (uint) (startFrame + index), Value = value,
        };
      }

      return new CsabKeyframe {
          Time = (uint) (startFrame + er.ReadUInt16()),
          Value = er.ReadSn16() * MathF.PI,
      };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private float ApplyScaleAndBias_(float value, float scale, float bias)
      => value * scale - bias;

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
          Value = er.ReadSn16() * MathF.PI,
          IncomingTangent = er.ReadSn16(),
          OutgoingTangent = er.ReadSn16(),
      };
  }
}