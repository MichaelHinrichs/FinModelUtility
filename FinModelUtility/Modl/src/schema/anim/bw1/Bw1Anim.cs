using System.Runtime.CompilerServices;

using fin.util.asserts;

using schema.binary;


namespace modl.schema.anim.bw1 {
  public class Bw1Anim : IAnim, IBinaryDeserializable {
    public List<IBwAnimBone> AnimBones { get; private set; }
    public List<AnimBoneFrames> AnimBoneFrames { get; private set; }

    public void Read(IEndianBinaryReader er) {
      string name0;
      {
        er.PushMemberEndianness(Endianness.LittleEndian);
        var name0Length = er.ReadUInt32();
        name0 = er.ReadString((int) name0Length);
        er.PopEndianness();
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

      er.ReadUInt32s(2);

      this.AnimBones = new List<IBwAnimBone>((int) boneCount);
      this.AnimBoneFrames = new List<AnimBoneFrames>((int) boneCount);
      for (var i = 0; i < boneCount; ++i) {
        var bone = new Bw1AnimBone();
        bone.Read(er);
        this.AnimBones.Add(bone);
      }

      // Next is blocks of data separated by "0xcd" bytes. The lengths of these can be predicted with the ints in the bones, so these seem to be keyframes.
      var estimatedLengths = this.AnimBones
                                 .Select(
                                     bone =>
                                         bone.PositionKeyframeCount * 4 +
                                         bone.RotationKeyframeCount * 6)
                                 .ToArray();

      // TODO: Remove this list allocation
      var boneBytes = new List<byte[]>();
      for (var i = 0; i < this.AnimBones.Count; ++i) {
        var currentBuffer = er.ReadBytes((int) estimatedLengths[i]);
        boneBytes.Add(currentBuffer);

        if (er.ReadUInt16() != 0xcdcd) {
          er.Position -= 2;
        }
      }

      Span<double> parseBuffer = stackalloc double[4];
      for (var i = 0; i < boneCount; ++i) {
        var bone = this.AnimBones[i];

        var buffer = boneBytes[i];
        using var ber =
            new EndianBinaryReader(buffer, Endianness.BigEndian);

        var animBoneFrames = new AnimBoneFrames(
            (int) bone.PositionKeyframeCount,
            (int) bone.RotationKeyframeCount);
        this.AnimBoneFrames.Add(animBoneFrames);

        for (var p = 0; p < bone.PositionKeyframeCount; ++p) {
          Parse3PositionValuesFrom2UShorts_(bone, ber, parseBuffer);
          animBoneFrames.PositionFrames.Add(((float) parseBuffer[0],
                                             (float) parseBuffer[1],
                                             (float) parseBuffer[2]));
        }

        for (var p = 0; p < bone.RotationKeyframeCount; ++p) {
          var flipSigns =
              Parse4RotationValuesFrom3UShorts_(ber, parseBuffer);
          if (flipSigns) {
            for (var f = 0; f < parseBuffer.Length; f++) {
              parseBuffer[f] *= -1;
            }
          }

          animBoneFrames.RotationFrames.Add(((float) -parseBuffer[0],
                                             (float) -parseBuffer[1],
                                             (float) -parseBuffer[2],
                                             (float) parseBuffer[3]));
        }
      }
    }

    public void Parse3PositionValuesFrom2UShorts_(
        IBwAnimBone animBone,
        IEndianBinaryReader er,
        Span<double> outValues) {
      var first_uint = er.ReadUInt32();
      er.Position -= 2;
      var second_ushort = er.ReadUInt16();

      outValues[0] =
          (WeirdFloatMath.INTERPRET_AS_DOUBLE_(
               WeirdFloatMath.CONCAT44_(0x43300000,
                                        (uint) (first_uint >> 0x15))) -
           WeirdFloatMath.INTERPRET_AS_DOUBLE_(0x4330000000000000)) *
          animBone.XPosDelta + animBone.XPosMin;
      outValues[1] =
          (WeirdFloatMath.INTERPRET_AS_DOUBLE_(
               WeirdFloatMath.CONCAT44_(0x43300000,
                                        (uint) ((first_uint >> 10) & 0x7ff))) -
           WeirdFloatMath.INTERPRET_AS_DOUBLE_(0x4330000000000000)) *
          animBone.YPosDelta + animBone.YPosMin;
      outValues[2] =
          (WeirdFloatMath.INTERPRET_AS_DOUBLE_(
               WeirdFloatMath.CONCAT44_(0x43300000,
                                        (uint) (second_ushort & 0x3ff))) -
           WeirdFloatMath.INTERPRET_AS_DOUBLE_(0x4330000000000000)) *
          animBone.ZPosDelta + animBone.ZPosMin;
    }

    public bool Parse4RotationValuesFrom3UShorts_(IEndianBinaryReader er,
                                                  Span<double> outValues) {
      var first_ushort = er.ReadUInt16();
      var second_ushort = er.ReadUInt16();
      var third_ushort = er.ReadUInt16();

      var const_for_out_value_2 = WeirdFloatMath.INTERPRET_AS_SINGLE_(0x38000000);
      var fVar1 = WeirdFloatMath.INTERPRET_AS_SINGLE_(0x47000000);

      var out_x =
          ((WeirdFloatMath.INTERPRET_AS_DOUBLE_(WeirdFloatMath.CONCAT44_(
                                                    0x43300000,
                                                    (uint) (first_ushort &
                                                        0x7fff))) -
            WeirdFloatMath.INTERPRET_AS_DOUBLE_(0x4330000000000000)) -
           WeirdFloatMath.INTERPRET_AS_SINGLE_(0x46800000)) *
          WeirdFloatMath.INTERPRET_AS_SINGLE_(0x38800000);
      var out_y =
          ((WeirdFloatMath.INTERPRET_AS_DOUBLE_(
                WeirdFloatMath.CONCAT44_(0x43300000,
                                         (uint) (second_ushort & 0x7fff))) -
            WeirdFloatMath.INTERPRET_AS_DOUBLE_(0x4330000000000000)) -
           WeirdFloatMath.INTERPRET_AS_SINGLE_(0x46800000)) *
          WeirdFloatMath.INTERPRET_AS_SINGLE_(0x38800000);
      var third_parsed_thing =
          WeirdFloatMath.INTERPRET_AS_DOUBLE_(
              WeirdFloatMath.CONCAT44_(0x43300000, third_ushort)) -
          WeirdFloatMath.INTERPRET_AS_DOUBLE_(0x4330000000000000);

      outValues[0] = out_x;
      outValues[1] = out_y;

      var out_z =
          (third_parsed_thing - fVar1) * const_for_out_value_2;
      outValues[2] = out_z;

      var expected_normalized_w =
          ((1 - out_x * out_x) - out_y * out_y) - out_z * out_z;
      var out_w = 0d;
      if (out_w <= expected_normalized_w) {
        if (WeirdFloatMath.INTERPRET_AS_SINGLE_(0x0229C4AB) <
            expected_normalized_w) {
          var inverse_sqrt_of_expected_normalized_w =
              1.0 / Math.Sqrt(expected_normalized_w);
          out_w =
              (float) (-(inverse_sqrt_of_expected_normalized_w *
                         inverse_sqrt_of_expected_normalized_w *
                         expected_normalized_w -
                         WeirdFloatMath.INTERPRET_AS_SINGLE_(0x40400000)) *
                       inverse_sqrt_of_expected_normalized_w *
                       WeirdFloatMath.INTERPRET_AS_SINGLE_(0x3F000000));
          if (out_w <= 0.0) {
            out_w = expected_normalized_w;
          }

          out_w = expected_normalized_w * out_w;
        }

        var sign = (first_ushort >> 0xf) switch {
            0 => 1,
            1 => -1,
            _ => throw new InvalidDataException(),
        };

        outValues[3] = out_w * sign;
      } else {
        outValues[3] = out_w;
      }

      return (short) second_ushort < 0;
    }
  }
}