﻿using fin.util.asserts;

using schema.binary;

namespace modl.schema.anim.bw1 {
  public class Bw1Anim : IAnim, IBinaryDeserializable {
    public List<IBwAnimBone> AnimBones { get; private set; }
    public List<AnimBoneFrames> AnimBoneFrames { get; private set; }

    public void Read(IBinaryReader br) {
      string name0;
      {
        br.PushMemberEndianness(Endianness.LittleEndian);
        var name0Length = br.ReadUInt32();
        name0 = br.ReadString((int) name0Length);
        br.PopEndianness();
      }

      var animStart = br.Position;

      var single0 = br.ReadSingle();
      var uint0 = br.ReadUInt32();

      // The name is repeated once more, excepted null-terminated.
      var name1 = br.ReadStringNT();

      // Next is a series of many "0xcd" values. Why??
      var cdCount = 0;
      while (br.ReadByte() == 0xcd) {
        cdCount++;
      }

      --br.Position;

      var single1 = br.ReadSingle();

      Asserts.Equal(0x4c, br.Position - animStart);

      // Next is a series of bone definitions. Each one has a length of 64.
      var boneCount = br.ReadUInt32();

      br.ReadUInt32s(2);

      this.AnimBones = new List<IBwAnimBone>((int) boneCount);
      this.AnimBoneFrames = new List<AnimBoneFrames>((int) boneCount);
      for (var i = 0; i < boneCount; ++i) {
        var bone = new Bw1AnimBone();
        bone.Read(br);
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
        var currentBuffer = br.ReadBytes((int) estimatedLengths[i]);
        boneBytes.Add(currentBuffer);

        if (br.ReadUInt16() != 0xcdcd) {
          br.Position -= 2;
        }
      }

      Span<double> parseBuffer = stackalloc double[4];
      for (var i = 0; i < boneCount; ++i) {
        var bone = this.AnimBones[i];

        var buffer = boneBytes[i];
        using var ber =
            new SchemaBinaryReader(buffer, Endianness.BigEndian);

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
        SchemaBinaryReader br,
        Span<double> outValues) {
      var first_uint = br.ReadUInt32();
      br.Position -= 2;
      var second_ushort = br.ReadUInt16();

      outValues[0] =
          WeirdFloatMath.CreateWeirdDoubleFromUInt32(first_uint >> 0x15) *
          animBone.XPosDelta + animBone.XPosMin;
      outValues[1] =
          WeirdFloatMath.CreateWeirdDoubleFromUInt32(
              (first_uint >> 10) & 0x7ff) * animBone.YPosDelta +
          animBone.YPosMin;
      outValues[2] =
          WeirdFloatMath.CreateWeirdDoubleFromUInt32(
              (uint) (second_ushort & 0x3ff)) * animBone.ZPosDelta +
          animBone.ZPosMin;
    }

    public bool Parse4RotationValuesFrom3UShorts_(IBinaryReader br,
                                                  Span<double> outValues) {
      Span<ushort> shorts = stackalloc ushort[3];
      br.ReadUInt16s(shorts);
      
      var first_ushort = shorts[0];
      var second_ushort = shorts[1];
      var third_ushort = shorts[2];

      var const_for_out_value_2 = WeirdFloatMath.C_3_05175_EN5;

      var out_x =
          ((WeirdFloatMath.InterpretAsDouble(
                WeirdFloatMath.Concat44(
                    0x43300000,
                    (uint) (first_ushort & 0x7fff))) -
            WeirdFloatMath.C_4503599627370496) -
           WeirdFloatMath.C_16384) *
          WeirdFloatMath.C_6_10351_EN5;
      var out_y =
          ((WeirdFloatMath.InterpretAsDouble(
                WeirdFloatMath.Concat44(0x43300000,
                                        (uint) (second_ushort & 0x7fff))) -
            WeirdFloatMath.C_4503599627370496) -
           WeirdFloatMath.C_16384) *
          WeirdFloatMath.C_6_10351_EN5;
      var third_parsed_thing =
          WeirdFloatMath.CreateWeirdDoubleFromUInt32(third_ushort);

      outValues[0] = out_x;
      outValues[1] = out_y;

      var out_z =
          (third_parsed_thing - 32768f) * const_for_out_value_2;
      outValues[2] = out_z;

      var expected_normalized_w =
          ((1 - out_x * out_x) - out_y * out_y) - out_z * out_z;
      var out_w = 0d;
      if (out_w <= expected_normalized_w) {
        if (WeirdFloatMath.C_ZERO < expected_normalized_w) {
          var inverse_sqrt_of_expected_normalized_w =
              1.0 / Math.Sqrt(expected_normalized_w);
          out_w =
              (float) (-(inverse_sqrt_of_expected_normalized_w *
                         inverse_sqrt_of_expected_normalized_w *
                         expected_normalized_w -
                         WeirdFloatMath.C_3) *
                       inverse_sqrt_of_expected_normalized_w *
                       WeirdFloatMath.C_HALF);
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