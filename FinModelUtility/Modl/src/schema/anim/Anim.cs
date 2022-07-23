using System.Security.Cryptography.X509Certificates;

using fin.util.asserts;

using schema;


namespace modl.schema.anim {
  public class Anim : IDeserializable {
    public List<AnimBone> AnimBones { get; } = new();
    public List<AnimBoneFrames> AnimBoneFrames { get; } = new();

    public void Read(EndianBinaryReader er) {
      string name0;
      {
        var endianness = er.Endianness;
        er.Endianness = Endianness.LittleEndian;

        var name0Length = er.ReadUInt32();
        name0 = er.ReadString((int) name0Length);

        er.Endianness = endianness;
      }

      var animStart = er.Position;

      var single0 = er.ReadSingle();
      var uint0 = er.ReadUInt32();

      // The name is repeated once more, excepted null-terminated.
      var name1 = er.ReadStringNT();

      // Next is a series of many "0xcd" values. Why??
      var cdCount = 0;
      while (er.ReadByte() == 0xcd) {
        cdCount++;
      }
      --er.Position;

      var single1 = er.ReadSingle();

      Asserts.Equal(0x4c, er.Position - animStart);

      // Next is a series of bone definitions. Each one has a length of 64.
      var boneCount = er.ReadUInt32();
      for (var i = 0; i < boneCount; ++i) {
        var bone = new AnimBone();

        bone.Data.Read(er);
        bone.BoneIndex = 31 & bone.Data.FlagAndBoneIndex;

        this.AnimBones.Add(bone);
      }
      er.AssertUInt64(0);

      var remainingLength = er.Length - er.Position;

      // Next is blocks of data separated by "0xcd" bytes. The lengths of these can be predicted with the ints in the bones, so these seem to be keyframes.
      var estimatedLengths = this.AnimBones
                                 .Select(
                                     bone =>
                                         bone.Data.PositionKeyframeCount * 4 +
                                         bone.Data.RotationKeyframeCount * 6)
                                 .ToArray();

      var boneBytes = new List<List<byte>>();
      var currentBuffer = new List<byte>();

      var prevC = new byte[2];
      while (!er.Eof) {
        var c = prevC[0] = er.ReadByte();

        currentBuffer.Add(c);

        if (prevC.All(c => c == 0xcd)) {
          if (currentBuffer.Count <= 2) {
            currentBuffer.Clear();
          } else {
            currentBuffer.RemoveRange(currentBuffer.Count - 2, 2);
          }

          var i = boneBytes.Count;
          Asserts.Equal((int) estimatedLengths[i], currentBuffer.Count);

          boneBytes.Add(currentBuffer);

          currentBuffer = new List<byte>();
          prevC[0] = 0;
        }

        prevC[1] = prevC[0];
      }
      Asserts.Equal((uint) boneBytes.Count, boneCount);

      for (var i = 0; i < boneCount; ++i) {
        var bone = this.AnimBones[i];

        var buffer = boneBytes[i];
        using var ber =
            new EndianBinaryReader(new MemoryStream(buffer.ToArray()),
                                   Endianness.BigEndian);

        var animBoneFrames = new AnimBoneFrames();
        this.AnimBoneFrames.Add(animBoneFrames);

        for (var p = 0; p < bone.Data.PositionKeyframeCount; ++p) {
          animBoneFrames.PositionFrames.Add(ber.ReadUInt32());

          ber.Position -= 4;

          var b0 = ber.ReadByte();
          var b1 = ber.ReadByte();
          var b2 = ber.ReadByte();
          var b3 = ber.ReadByte();
          animBoneFrames.PositionBytes.Add((b0, b1, b2, b3));
        }

        for (var p = 0; p < bone.Data.RotationKeyframeCount; ++p) {
          var flipSigns = ParseValuesFromUShorts_(ber, out var floats);
          if (flipSigns) {
            for (var f = 0; f < floats.Length; f++) {
              floats[f] *= -1;
            }
          }

          animBoneFrames.RotationFrames.Add(((float) floats[0],
                                             (float) floats[1],
                                             (float) floats[2]));

          ber.Position -= 6;

          animBoneFrames.RotationBytes.Add(
              (ber.ReadSByte(),
               ber.ReadSByte(),
               ber.ReadSByte(),
               ber.ReadSByte(),
               ber.ReadSByte(),
               ber.ReadSByte()));
        }

        var deltas = new List<(float, float, float)>();
        for (var f = 1; f < animBoneFrames.RotationFrames.Count; ++f) {
          var prev = animBoneFrames.RotationFrames[f - 1];
          var current = animBoneFrames.RotationFrames[f];

          deltas.Add((current.Item1 - prev.Item1,
                      current.Item2 - prev.Item2,
                      current.Item3 - prev.Item3));
        }

        ;
      }

      // TODO: There can be a little bit of extra data at the end, what is this for??
    }

    public bool ParseValuesFromUShorts_(EndianBinaryReader er,
                                        out double[] outValues) {
      var first_ushort = er.ReadUInt16();
      var second_ushort = er.ReadUInt16();
      var third_ushort = er.ReadUInt16();

      var fVar2 = INTERPRET_AS_SINGLE_(0x3F800000);
      var const_for_out_value_2 = INTERPRET_AS_SINGLE_(0x38000000);
      var fVar1 = INTERPRET_AS_SINGLE_(0x47000000);

      var first_parsed_thing =
          ((INTERPRET_AS_DOUBLE_(CONCAT44_(0x43300000,
                                           (uint) (first_ushort & 0x7fff))) -
            INTERPRET_AS_DOUBLE_(0x4330000000000000)) -
           INTERPRET_AS_SINGLE_(0x46800000)) * INTERPRET_AS_SINGLE_(0x38800000);


      var second_parsed_thing =
          ((INTERPRET_AS_DOUBLE_(
                CONCAT44_(0x43300000, (uint) (second_ushort & 0x7fff))) -
            INTERPRET_AS_DOUBLE_(0x4330000000000000)) -
           INTERPRET_AS_SINGLE_(0x46800000)) * INTERPRET_AS_SINGLE_(0x38800000);
      var third_parsed_thing =
          INTERPRET_AS_DOUBLE_(CONCAT44_(0x43300000, third_ushort)) -
          INTERPRET_AS_DOUBLE_(0x4330000000000000);

      outValues = new double[4];
      outValues[1] = second_parsed_thing;
      var out_value_3 = (double) INTERPRET_AS_SINGLE_(0);

      var out_value_2 =
          (third_parsed_thing - fVar1) * const_for_out_value_2;
      outValues[2] = out_value_2;
      outValues[0] = first_parsed_thing;
      first_parsed_thing =
          ((fVar2 - first_parsed_thing * first_parsed_thing) -
           second_parsed_thing * second_parsed_thing) -
          out_value_2 * out_value_2;
      if (out_value_3 <= first_parsed_thing) {
        if (INTERPRET_AS_SINGLE_(0x0229C4AB) < first_parsed_thing) {
          var inverse_sqrt_of_thing = 1.0 / Math.Sqrt(first_parsed_thing);
          out_value_3 =
              (float) (-(inverse_sqrt_of_thing * inverse_sqrt_of_thing *
                         first_parsed_thing -
                         INTERPRET_AS_SINGLE_(0x40400000)) *
                       inverse_sqrt_of_thing *
                       INTERPRET_AS_SINGLE_(0x3F000000));
          if (out_value_3 <= 0.0) {
            out_value_3 = first_parsed_thing;
          }
          out_value_3 = first_parsed_thing * out_value_3;
        }

        var CONST_0x3f800000 = INTERPRET_AS_SINGLE_(0x3f800000);
        var CONST_0xbf800000 = INTERPRET_AS_SINGLE_(0xbf800000);

        var multiplier = (first_ushort >> 0xf) switch {
            0 => CONST_0x3f800000,
            1 => CONST_0xbf800000,
            _ => throw new InvalidDataException(),
        };

        outValues[3] = out_value_3 * multiplier;
      } else {
        outValues[3] = out_value_3;
      }

      return (short) second_ushort < 0;
    }

    static ulong CONCAT44_(uint first, uint second) =>
        ((ulong) first << 32) | second;

    static double INTERPRET_AS_DOUBLE_(ulong value) {
      var bytes = BitConverter.GetBytes(value);
      return BitConverter.ToDouble(bytes);
    }

    static float INTERPRET_AS_SINGLE_(uint value) {
      var bytes = BitConverter.GetBytes(value);
      return BitConverter.ToSingle(bytes);
    }
  }

  [Schema]
  public partial class AnimBoneData : IBiSerializable {
    private readonly uint padding0_ = 0;

    // This seems to sometimes set the "32" bit to true, not sure why? 
    public uint FlagAndBoneIndex { get; set; }

    [StringLengthSource(16)] public string Name { get; set; }

    // The number of what seem to be position keyframes? These have a length of 4 bytes each. 
    public uint PositionKeyframeCount { get; set; }

    // The number of what seem to be rotation keyframes? These have a length of 6 bytes each. 
    public uint RotationKeyframeCount { get; set; }

    private readonly ulong padding1_ = 0;

    public float[] Floats0 { get; } = new float[6];
  }

  public class AnimBone {
    public AnimBoneData Data { get; } = new();
    public uint BoneIndex { get; set; }
  }

  public class AnimBoneFrames {
    public List<(byte, byte, byte, byte)> PositionBytes { get; } = new();

    public List<(float, float, float, float, float, float)> RotationBytes {
      get;
    } =
      new();

    public List<uint> PositionFrames { get; } = new();
    public List<(float, float, float)> RotationFrames { get; } = new();
  }
}