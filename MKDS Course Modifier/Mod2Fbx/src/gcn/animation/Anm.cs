using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

using fin.model;
using fin.model.impl;
using fin.util.asserts;

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
    Dictionary<uint, JointKeyframes> JointKeyframesMap { get; }
  }

  public static class DcxHelpers {
    public static Keyframe[] ReadDenseFrames(
        float[] values,
        int offset,
        int count
    ) {
      var keyframes = new Keyframe[count];
      for (var i = 0; i < count; ++i) {
        keyframes[i] = new Keyframe {
            Index = i,
            Value = values[offset + i]
        };
      }
      return keyframes;
    }

    public static Keyframe[] ReadSparseFrames(
        float[] values,
        int offset,
        int count
    ) {
      var keyframes = new Keyframe[count];
      for (var i = 0; i < count; ++i) {
        var index = (int) values[offset + 3 * i];
        var value = values[offset + 3 * i + 1];
        var tangent = values[offset + 3 * i + 2];

        keyframes[i] = new Keyframe {
            Index = index,
            Value = value
        };
      }
      return keyframes;
    }

    // TODO: Do this sparsely
    public static void MergeKeyframesToPositionTrack(
        Keyframe[][] positionKeyframes,
        ITrack<IPosition> positionTrack,
        uint frameCount) {
      var indices = new int[3];

      /*for (var f = 0; f < frameCount; ++f) {
        // Check if we need to progress the keyframes
        for (var i = 0; i < 3; i++) {
          if 

          var index = indices[i];
          var keyframe = positionKeyframes[index];
        }

        var position = new ModelImpl.PositionImpl {
            X = positionKeyframes[]
        };

        positionTrack.Set(i, );

      }*/
    }

    public static void MergeKeyframesToRotationTrack(
        Keyframe[][] positionKeyframes,
        ITrack<IRotation, Quaternion> positionTrack,
        uint frameCount) {
      for (var i = 0; i < frameCount; ++i) {}
    }

    public static void MergeKeyframesToScaleTrack(
        Keyframe[][] positionKeyframes,
        ITrack<IScale> positionTrack,
        uint frameCount) {
      for (var i = 0; i < frameCount; ++i) {}
    }
  }

  public class Dca : IDcx, IGcnSerializable {
    public string Name { get; private set; }

    public uint FrameCount { get; private set; }
    public Dictionary<uint, JointKeyframes> JointKeyframesMap { get; } = new();

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

        var jointKeyframes = new JointKeyframes {JointIndex = jointIndex};

        Keyframe[][] frames;

        frames = this.ReadKeyframes_(reader, scaleValues);
        DcxHelpers.MergeKeyframesToScaleTrack(
            frames,
            jointKeyframes.Scales,
            this.FrameCount);

        frames = this.ReadKeyframes_(reader, rotationValues);
        DcxHelpers.MergeKeyframesToRotationTrack(
            frames,
            jointKeyframes.Rotations,
            this.FrameCount);

        frames = this.ReadKeyframes_(reader, positionValues);
        DcxHelpers.MergeKeyframesToPositionTrack(
            frames,
            jointKeyframes.Positions,
            this.FrameCount);

        this.JointKeyframesMap[jointIndex] = jointKeyframes;
      }

      var endPosition = reader.Position;
      var readLength = endPosition - startPosition;
      Asserts.Equal(animationLength,
                    readLength,
                    "Read unexpected number of bytes in animation!");
    }

    private Keyframe[][] ReadKeyframes_(
        EndianBinaryReader reader,
        float[] values) {
      var frames = new Keyframe[3][];
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
    public Dictionary<uint, JointKeyframes> JointKeyframesMap { get; } = new();

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

        var jointKeyframes = new JointKeyframes {JointIndex = jointIndex};

        for (var j = 0; j < 3; ++j) {
          var scaleFrameCount = reader.ReadInt32();
          var scaleFrameOffset = reader.ReadInt32();
          var scaleFrameUnk = reader.ReadInt32();

          var sparse = (scaleFrameCount != 1 &&
                        scaleFrameCount != this.FrameCount);
          if (scaleFrameUnk != 0) {
            ;
          }

          var frames = !sparse
                           ? DcxHelpers.ReadDenseFrames(
                               scaleValues,
                               scaleFrameOffset,
                               scaleFrameCount)
                           : DcxHelpers.ReadSparseFrames(
                               scaleValues,
                               scaleFrameOffset,
                               scaleFrameCount);
          /*DcxHelpers.MergeKeyframesToScaleTrack(
              null,
              jointKeyframes.Scales,
              this.FrameCount);*/
        }
        for (var k = 0; k < 3; ++k) {
          var rotationFrameCount = reader.ReadInt32();
          var rotationFrameOffset = reader.ReadInt32();
          var rotationFrameUnk = reader.ReadInt32();

          var sparse = (rotationFrameCount != 1 &&
                        rotationFrameCount != this.FrameCount);
          if (rotationFrameUnk != 0) {
            ;
          }

          var frames = !sparse
                           ? DcxHelpers.ReadDenseFrames(
                               rotationValues,
                               rotationFrameOffset,
                               rotationFrameCount)
                           : DcxHelpers.ReadSparseFrames(
                               rotationValues,
                               rotationFrameOffset,
                               rotationFrameCount);
          /*DcxHelpers.MergeKeyframesToRotationTrack(
              null,
              jointKeyframes.Rotations,
              this.FrameCount);*/
        }
        for (var l = 0; l < 3; ++l) {
          var positionFrameCount = reader.ReadInt32();
          var positionFrameOffset = reader.ReadInt32();
          var positionFrameUnk = reader.ReadInt32();

          var sparse = (positionFrameCount != 1 &&
                        positionFrameCount != this.FrameCount);
          if (positionFrameUnk != 0) {
            ;
          }

          var frames = !sparse
                           ? DcxHelpers.ReadDenseFrames(
                               positionValues,
                               positionFrameOffset,
                               positionFrameCount)
                           : DcxHelpers.ReadSparseFrames(
                               positionValues,
                               positionFrameOffset,
                               positionFrameCount);
          /*DcxHelpers.MergeKeyframesToPositionTrack(
              null,
              jointKeyframes.Positions,
              this.FrameCount);*/
        }

        this.JointKeyframesMap[jointIndex] = jointKeyframes;
      }


      var endPosition = reader.Position;
      var readLength = endPosition - startPosition;
      Asserts.Equal(animationLength,
                    readLength,
                    "Read unexpected number of bytes in animation!");
    }

    public void Write(EndianBinaryWriter writer) {
      throw new NotImplementedException();
    }
  }

  public class JointKeyframes {
    public uint JointIndex { get; set; }

    public ITrack<IPosition> Positions { get; } =
      new ModelImpl.TrackImpl<IPosition>(
          ModelImpl.TrackInterpolators.PositionInterpolator);

    public ITrack<IRotation, Quaternion> Rotations { get; } =
      new ModelImpl.TrackImpl<IRotation, Quaternion>(
          ModelImpl.TrackInterpolators.RotationInterpolator);

    public ITrack<IScale> Scales { get; } =
      new ModelImpl.TrackImpl<IScale>(ModelImpl.TrackInterpolators
                                               .ScaleInterpolator);
  }

  public class Keyframe {
    public int Index { get; set; }
    public float Value { get; set; }
  }
}