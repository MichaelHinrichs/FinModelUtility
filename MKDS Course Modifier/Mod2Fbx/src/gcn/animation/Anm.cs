using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using fin.model;
using fin.model.impl;
using fin.util.asserts;
using fin.util.optional;

namespace mod.gcn.animation {
  public class Anm : IGcnSerializable {
    public List<IDcx> Dcxes { get; } = new();

    public void Read(EndianBinaryReader reader) {
      var dcaCount = reader.ReadUInt32();

      for (var i = 0; i < dcaCount; ++i) {
        var animationFormat = (AnimationFormat) reader.ReadUInt32();

        if (animationFormat == AnimationFormat.DCA) {
          var dca = new Dca();
          dca.Read(reader);
          this.Dcxes.Add(dca);
        } else if (animationFormat == AnimationFormat.DCK) {
          var dck = new Dck();
          dck.Read(reader);
          this.Dcxes.Add(dck);
        } else {
          Asserts.Fail($"Unexpected animation format: {animationFormat}");
        }
      }

      ;
    }

    public void Write(EndianBinaryWriter writer) {
      throw new NotImplementedException();
    }
  }

  public enum AnimationFormat {
    DCA = 2,
    DCK = 3,
  }

  public interface IDcx {
    string Name { get; }
    uint FrameCount { get; }
    Dictionary<uint, IBoneTracks> JointKeyframesMap { get; }
  }

  public static class DcxHelpers {
    public static Keyframe<float>[] ReadDenseFrames(
        float[] values,
        int offset,
        int count
    ) {
      var keyframes = new Keyframe<float>[count];
      for (var i = 0; i < count; ++i) {
        keyframes[i] =
            new Keyframe<float>(i, values[offset + i], Optional.None<float>());
      }
      return keyframes;
    }

    public static Keyframe<float>[] ReadSparseFrames(
        float[] values,
        int offset,
        int count
    ) {
      var keyframes = new Keyframe<float>[count];
      for (var i = 0; i < count; ++i) {
        var index = (int) values[offset + 3 * i];
        var value = values[offset + 3 * i + 1];
        var tangent = values[offset + 3 * i + 2];

        keyframes[i] = new Keyframe<float>(index, value, Optional.Of(tangent));
      }
      return keyframes;
    }

    // TODO: Do this sparsely
    public static void MergeKeyframesToPositionTrack(
        Keyframe<float>[][] positionKeyframes,
        IPositionTrack positionTrack) {
      for (var i = 0; i < 3; ++i) {
        foreach (var keyframe in positionKeyframes[i]) {
          positionTrack.Set(keyframe.Frame,
                            i,
                            keyframe.Value,
                            keyframe.Tangent);
        }
      }
    }

    public static void MergeKeyframesToRotationTrack(
        Keyframe<float>[][] rotationKeyframes,
        IRadiansRotationTrack rotationTrack) {
      for (var i = 0; i < 3; ++i) {
        foreach (var keyframe in rotationKeyframes[i]) {
          rotationTrack.Set(keyframe.Frame,
                            i,
                            keyframe.Value,
                            keyframe.Tangent);
        }
      }
    }

    public static void MergeKeyframesToScaleTrack(
        Keyframe<float>[][] scaleKeyframes,
        IScaleTrack scaleTrack) {
      for (var i = 0; i < 3; ++i) {
        foreach (var keyframe in scaleKeyframes[i]) {
          scaleTrack.Set(keyframe.Frame,
                         i,
                         keyframe.Value,
                         keyframe.Tangent);
        }
      }
    }
  }

  public class Dca : IDcx, IGcnSerializable {
    public string Name { get; private set; }

    public uint FrameCount { get; private set; }
    public Dictionary<uint, IBoneTracks> JointKeyframesMap { get; } = new();

    public void Read(EndianBinaryReader reader) {
      // TODO: Pull this out as a common "animation header"
      uint animationLength;
      long startPosition;
      {
        animationLength = reader.ReadUInt32();

        var nameLength = reader.ReadInt32();
        this.Name = reader.ReadString(Encoding.ASCII, nameLength);

        startPosition = reader.Position;
      }

      var jointCount = reader.ReadUInt32();
      this.FrameCount = reader.ReadUInt32();

      var scaleValueCount = reader.ReadInt32();
      ;
      var scaleValues = reader.ReadSingles(scaleValueCount);

      var rotationValueCount = reader.ReadInt32();
      var rotationValues = reader.ReadSingles(rotationValueCount);

      var positionValueCount = reader.ReadInt32();
      var positionValues = reader.ReadSingles(positionValueCount);

      this.JointKeyframesMap.Clear();
      for (var i = 0; i < jointCount; ++i) {
        var jointIndex = reader.ReadUInt32();
        var parentIndex = reader.ReadUInt32();

        var jointKeyframes = new ModelImpl.BoneTracksImpl();

        Keyframe<float>[][] frames;

        frames = this.ReadKeyframes_(reader, scaleValues);
        DcxHelpers.MergeKeyframesToScaleTrack(
            frames,
            jointKeyframes.Scales);

        frames = this.ReadKeyframes_(reader, rotationValues);
        DcxHelpers.MergeKeyframesToRotationTrack(
            frames,
            jointKeyframes.Rotations);

        frames = this.ReadKeyframes_(reader, positionValues);
        DcxHelpers.MergeKeyframesToPositionTrack(
            frames,
            jointKeyframes.Positions);

        this.JointKeyframesMap[jointIndex] = jointKeyframes;
      }

      var endPosition = reader.Position;
      var readLength = endPosition - startPosition;
      Asserts.Equal(animationLength,
                    readLength,
                    "Read unexpected number of bytes in animation!");
    }

    private Keyframe<float>[][] ReadKeyframes_(
        EndianBinaryReader reader,
        float[] values) {
      var frames = new Keyframe<float>[3][];
      for (var i = 0; i < 3; ++i) {
        var frameCount = reader.ReadInt32();
        var frameOffset = reader.ReadInt32();
        frames[i] = DcxHelpers.ReadDenseFrames(
            values,
            frameOffset,
            frameCount);
      }
      return frames;
    }


    public void Write(EndianBinaryWriter writer) {
      throw new NotImplementedException();
    }
  }

  public class Dck : IDcx, IGcnSerializable {
    public string Name { get; private set; }

    public uint FrameCount { get; private set; }
    public Dictionary<uint, IBoneTracks> JointKeyframesMap { get; } = new();

    public void Read(EndianBinaryReader reader) {
      // TODO: Pull this out as a common "animation header"
      uint animationLength;
      long startPosition;
      {
        animationLength = reader.ReadUInt32();

        var nameLength = reader.ReadInt32();
        this.Name = reader.ReadString(Encoding.ASCII, nameLength);

        startPosition = reader.Position;
      }

      var jointCount = reader.ReadUInt32();
      this.FrameCount = reader.ReadUInt32();

      var scaleValueCount = reader.ReadInt32();
      ;
      var scaleValues = reader.ReadSingles(scaleValueCount);

      var rotationValueCount = reader.ReadInt32();
      var rotationValues = reader.ReadSingles(rotationValueCount);

      var positionValueCount = reader.ReadInt32();
      var positionValues = reader.ReadSingles(positionValueCount);

      this.JointKeyframesMap.Clear();
      for (var i = 0; i < jointCount; ++i) {
        var jointIndex = reader.ReadUInt32();
        var parentIndex = reader.ReadUInt32();

        var jointKeyframes = new ModelImpl.BoneTracksImpl();

        Keyframe<float>[][] frames;
        frames = this.ReadKeyframes_(reader, scaleValues);
        DcxHelpers.MergeKeyframesToScaleTrack(
            frames,
            jointKeyframes.Scales);

        frames = this.ReadKeyframes_(reader, rotationValues);
        DcxHelpers.MergeKeyframesToRotationTrack(
            frames,
            jointKeyframes.Rotations);

        frames = this.ReadKeyframes_(reader, positionValues);
        DcxHelpers.MergeKeyframesToPositionTrack(
            frames,
            jointKeyframes.Positions);

        this.JointKeyframesMap[jointIndex] = jointKeyframes;
      }


      var endPosition = reader.Position;
      var readLength = endPosition - startPosition;
      Asserts.Equal(animationLength,
                    readLength,
                    "Read unexpected number of bytes in animation!");
    }

    private Keyframe<float>[][] ReadKeyframes_(
        EndianBinaryReader reader,
        float[] values) {
      var frames = new Keyframe<float>[3][];
      for (var i = 0; i < 3; ++i) {
        var frameCount = reader.ReadInt32();
        var frameOffset = reader.ReadInt32();
        var unk = reader.ReadInt32();

        var sparse = (frameCount != 1 && frameCount != this.FrameCount);

        frames[i] = !sparse
                        ? DcxHelpers.ReadDenseFrames(
                            values,
                            frameOffset,
                            frameCount)
                        : DcxHelpers.ReadSparseFrames(
                            values,
                            frameOffset,
                            frameCount);
      }
      return frames;
    }

    public void Write(EndianBinaryWriter writer) {
      throw new NotImplementedException();
    }
  }
}