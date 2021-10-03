using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using fin.util.asserts;

using mod.gcn;

namespace mod.gcn.animation {
  public class Anm : IGcnSerializable {
    public List<Dca> Dcas { get; } = new();

    public void Read(EndianBinaryReader reader) {
      var dcaCount = reader.ReadUInt32();

      for (var i = 0; i < dcaCount; ++i) {
        var dca = new Dca();
        dca.Read(reader);
        this.Dcas.Add(dca);
      }
    }

    public void Write(EndianBinaryWriter writer) {
      throw new NotImplementedException();
    }
  }

  public enum AnimationFormat {
    DCA = 2,
    DCK = 3,
  }

  public class Dca : IGcnSerializable {
    public bool Valid { get; private set; }
    public string Name { get; private set; }

    public uint FrameCount { get; private set; }
    public Dictionary<uint, JointKeyframes> jointKeyframesMap { get; } = new();

    public void Read(EndianBinaryReader reader) {
      this.Valid = false;

      // TODO: Pull this out as a common "animation header"
      uint animationLength;
      long startPosition;
      {
        var animationFormat = (AnimationFormat) reader.ReadUInt32();

        if (animationFormat == AnimationFormat.DCA) {
          ; // All good
        } else if (animationFormat == AnimationFormat.DCK) {
          ; // Not supported yet
        } else {
          ; // Uh... what is this?
        }

        animationLength = reader.ReadUInt32();

        var nameLength = reader.ReadInt32();
        this.Name = reader.ReadString(Encoding.ASCII, nameLength);

        startPosition = reader.Position;
        if (animationFormat != AnimationFormat.DCA) {
          reader.Position += animationLength;
          return;
        }
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

      this.jointKeyframesMap.Clear();
      for (var i = 0; i < jointCount; ++i) {
        var jointIndex = reader.ReadUInt32();
        var parentIndex = reader.ReadUInt32();

        var jointKeyframes = new JointKeyframes {JointIndex = jointIndex};

        for (var j = 0; j < 3; ++j) {
          var scaleFrameCount = reader.ReadInt32();
          var scaleFrameOffset = reader.ReadInt32();

          for (var s = 0; s < scaleFrameCount; ++s) {
            jointKeyframes.scale[j]
                          .Add(new Keyframe {
                              Index = s,
                              Value = scaleValues[scaleFrameOffset + s]
                          });
          }
        }
        for (var k = 0; k < 3; ++k) {
          var rotationFrameCount = reader.ReadInt32();
          var rotationFrameOffset = reader.ReadInt32();

          for (var r = 0; r < rotationFrameCount; ++r) {
            jointKeyframes.rotation[k]
                          .Add(new Keyframe {
                              Index = r,
                              Value = rotationValues[rotationFrameOffset + r]
                          });
          }
        }
        for (var l = 0; l < 3; ++l) {
          var positionFrameCount = reader.ReadInt32();
          var positionFrameOffset = reader.ReadInt32();

          for (var p = 0; p < positionFrameCount; ++p) {
            jointKeyframes.position[l]
                          .Add(new Keyframe {
                              Index = p,
                              Value = positionValues[positionFrameOffset + p]
                          });
          }
        }

        this.jointKeyframesMap[jointIndex] = jointKeyframes;
      }


      var endPosition = reader.Position;
      var readLength = endPosition - startPosition;
      Asserts.Equal(animationLength,
                    readLength,
                    "Read unexpected number of bytes in animation!");

      this.Valid = true;
    }

    public void Write(EndianBinaryWriter writer) {
      throw new NotImplementedException();
    }
  }

  public class JointKeyframes {
    public uint JointIndex { get; set; }

    public readonly List<Keyframe>[] scale = new List<Keyframe>[3];
    public readonly List<Keyframe>[] rotation = new List<Keyframe>[3];
    public readonly List<Keyframe>[] position = new List<Keyframe>[3];

    public JointKeyframes() {
      for (var i = 0; i < 3; ++i) {
        this.scale[i] = new List<Keyframe>();
        this.rotation[i] = new List<Keyframe>();
        this.position[i] = new List<Keyframe>();
      }
    }
  }

  public class Keyframe {
    public int Index { get; set; }
    public float Value { get; set; }
  }
}