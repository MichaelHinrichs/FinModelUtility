using fin.data;
using fin.math;
using fin.util.asserts;
using System.Numerics;


namespace level5.schema {
  public class Mtn2 {
    public GenericAnimation Anim { get; } = new GenericAnimation();

    public ListDictionary<uint, (short, float)> Somethings { get; } = new();

    public class AnimTrack {
      public int Type { get; set; }
      public int DataType { get; set; }
      public int Unk { get; set; }
      public int DataCount { get; set; }
      public int Start { get; set; }
      public int End { get; set; }
    }

    public void Open(byte[] bytes) {
      var endianness = Endianness.LittleEndian;
      using (var r =
             new EndianBinaryReader(new MemoryStream(bytes), endianness)) {
        r.Position = 0x08;
        var decomSize = r.ReadInt32();
        var nameOffset = r.ReadUInt32();
        var compDataOffset = r.ReadUInt32();
        var positionCount = r.ReadInt32();
        var rotationCount = r.ReadInt32();
        var scaleCount = r.ReadInt32();

        // This corresponds to PRMs that may be toggled on/off in the animation.
        var toggledPrmCount = r.ReadInt32();
        // This corresponds to bones that will be moved in the animation.
        var boneCount = r.ReadInt32();

        r.Position = 0x54;
        this.Anim.FrameCount = r.ReadInt32();

        r.Position = nameOffset;
        this.Anim.NameHash = r.ReadUInt32();
        this.Anim.Name = r.ReadStringNT();

        var data = Decompress.Level5Decom(
            r.ReadBytesAtOffset(compDataOffset,
                                (int)(r.Length - compDataOffset)));

        using (var d =
               new EndianBinaryReader(new MemoryStream(data), endianness)) {
          // Header
          var hashTableOffset = d.ReadUInt32();
          var trackInfoOffset = d.ReadUInt32();
          var dataOffset = d.ReadUInt32();

          // Bone Hashes
          List<uint> hashes = new List<uint>();
          d.Position = (hashTableOffset);
          while (d.Position < trackInfoOffset) {
            hashes.Add(d.ReadUInt32());
          }
          Asserts.Equal(toggledPrmCount + boneCount, hashes.Count);

          // Track Information
          List<AnimTrack> tracks = new List<AnimTrack>();
          for (int i = 0; i < 4; i++) {
            d.Position = ((uint)(trackInfoOffset + 2 * i));
            d.Position = (d.ReadUInt16());

            tracks.Add(new AnimTrack() {
                Type = d.ReadByte(),
                DataType = d.ReadByte(),
                Unk = d.ReadByte(),
                DataCount = d.ReadByte(),
                Start = d.ReadUInt16(),
                End = d.ReadUInt16()
            });
          }

          foreach (var v in tracks)
            Console.WriteLine(v.Type + " "
                                     + v.DataType + " "
                                     + v.DataCount
                                     + " " + v.Start.ToString("X")
                                     + " " + v.End.ToString("X"));

          // Data

          foreach (var v in hashes) {
            var node = new GenericAnimationTransform();
            node.Hash = v;
            node.HashType = AnimNodeHashType.CRC32C;
            this.Anim.TransformNodes.Add(node);
          }

          var offset = 0;
          this.ReadFrameData_(d, offset, positionCount, dataOffset, boneCount,
                              tracks[0]);
          offset += positionCount;
          this.ReadFrameData_(d, offset, rotationCount, dataOffset, boneCount,
                              tracks[1]);
          offset += rotationCount;
          this.ReadFrameData_(d, offset, scaleCount, dataOffset, boneCount,
                              tracks[2]);
          offset += scaleCount;
          this.ReadFrameData_(d, offset, toggledPrmCount, dataOffset, boneCount,
                              tracks[3]);
          ;
        }
      }
    }

    private void ReadFrameData_(EndianBinaryReader d,
                                int offset,
                                int count,
                                uint dataOffset,
                                int boneCount,
                                AnimTrack track) {
      for (int i = offset; i < offset + count; i++) {
        d.Position = ((uint)(dataOffset + 4 * 4 * i));
        var flagOffset = d.ReadUInt32();
        var keyFrameOffset = d.ReadUInt32();
        var keyDataOffset = d.ReadUInt32();
        d.AssertUInt32(0);

        d.Position = (flagOffset);
        var boneIndex = d.ReadInt16();
        var keyFrameCount = d.ReadByte();
        var flag = d.ReadByte();

        var nodeIndex = boneIndex + (flag == 0 ? boneCount : 0);

        var node = this.Anim.TransformNodes[nodeIndex];

        d.Position = (keyDataOffset);
        for (int k = 0; k < keyFrameCount; k++) {
          var temp = d.Position;
          d.Position = ((uint)(keyFrameOffset + k * 2));
          var frame = d.ReadInt16();
          d.Position = (temp);

          if (d.Eof) {
            break;
          }

          float[] animdata = new float[track.DataCount];
          for (int j = 0; j < track.DataCount; j++)
            switch (track.DataType) {
              case 1:
                animdata[j] = d.ReadInt16() / (float)short.MaxValue;
                break;
              case 2:
                animdata[j] = d.ReadSingle();
                break;
              case 4:
                animdata[j] = d.ReadInt16();
                break;
              default:
                throw new NotImplementedException(
                    "Data Type " + track.DataType + " not implemented");
            }

          switch (track.Type) {
            case 1:
              node.AddKey(frame, animdata[0],
                          AnimationTrackFormat.TranslateX);
              node.AddKey(frame, animdata[1],
                          AnimationTrackFormat.TranslateY);
              node.AddKey(frame, animdata[2],
                          AnimationTrackFormat.TranslateZ);
              break;
            case 2:
              // TODO: Invert?
              var e = QuaternionUtil.ToEulerRadians(
                  new Quaternion(animdata[0], animdata[1], animdata[2],
                                 animdata[3]));
              node.AddKey(frame, e.X, AnimationTrackFormat.RotateX);
              node.AddKey(frame, e.Y, AnimationTrackFormat.RotateY);
              node.AddKey(frame, e.Z, AnimationTrackFormat.RotateZ);
              break;
            case 3:
              node.AddKey(frame, animdata[0], AnimationTrackFormat.ScaleX);
              node.AddKey(frame, animdata[1], AnimationTrackFormat.ScaleY);
              node.AddKey(frame, animdata[2], AnimationTrackFormat.ScaleZ);
              break;
            case 9: {
              Asserts.Equal(1, animdata.Length);
              this.Somethings.Add(node.Hash, (frame, animdata[0]));
              break;
            }
          }
        }
      }
    }
  }
}