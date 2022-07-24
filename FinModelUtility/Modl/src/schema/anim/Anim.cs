﻿using System.Security.Cryptography.X509Certificates;

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

      er.ReadUInt32s(2);

      for (var i = 0; i < boneCount; ++i) {
        var bone = new AnimBone();

        bone.Data.Read(er);
        bone.BoneIndex = 31 & bone.Data.WeirdId;

        this.AnimBones.Add(bone);
      }
      //er.AssertUInt64(0);

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
          Parse3PositionValuesFrom2UShorts_(bone, ber, out var floats);
          animBoneFrames.PositionFrames.Add(((float) floats[0],
                                             (float) floats[1],
                                             (float) floats[2]));
        }

        for (var p = 0; p < bone.Data.RotationKeyframeCount; ++p) {
          var flipSigns =
              Parse4RotationValuesFrom3UShorts_(ber, out var floats);
          if (flipSigns) {
            for (var f = 0; f < floats.Length; f++) {
              floats[f] *= -1;
            }
          }

          animBoneFrames.RotationFrames.Add(((float) -floats[0],
                                             (float) -floats[1],
                                             (float) -floats[2],
                                             (float) floats[3]));
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

    public void Parse3PositionValuesFrom2UShorts_(
        AnimBone animBone,
        EndianBinaryReader er,
        out double[] outValues) {
      var animBoneData = animBone.Data;

      var first_ushort = er.ReadUInt16();
      var second_ushort = er.ReadUInt16();

      outValues = new double[3];
      outValues[0] =
          (INTERPRET_AS_DOUBLE_(
               CONCAT44_(0x43300000, (uint) (first_ushort >> 0x15))) -
           INTERPRET_AS_DOUBLE_(0x4330000000000000)) *
          animBoneData.XPosDelta + animBoneData.XPosMin;
      outValues[1] =
          (INTERPRET_AS_DOUBLE_(
               CONCAT44_(0x43300000, (uint) (first_ushort >> 10 & 0x7ff))) -
           INTERPRET_AS_DOUBLE_(0x4330000000000000)) *
          animBoneData.YPosDelta + animBoneData.YPosMin;
      outValues[2] =
          (INTERPRET_AS_DOUBLE_(
               CONCAT44_(0x43300000, (uint) (second_ushort & 0x3ff))) -
           INTERPRET_AS_DOUBLE_(0x4330000000000000)) *
          animBoneData.ZPosDelta + animBoneData.ZPosMin;
    }

    public bool Parse4RotationValuesFrom3UShorts_(EndianBinaryReader er,
                                                  out double[] outValues) {
      var first_ushort = er.ReadUInt16();
      var second_ushort = er.ReadUInt16();
      var third_ushort = er.ReadUInt16();

      var const_for_out_value_2 = INTERPRET_AS_SINGLE_(0x38000000);
      var fVar1 = INTERPRET_AS_SINGLE_(0x47000000);

      var out_x =
          ((INTERPRET_AS_DOUBLE_(CONCAT44_(0x43300000,
                                           (uint) (first_ushort & 0x7fff))) -
            INTERPRET_AS_DOUBLE_(0x4330000000000000)) -
           INTERPRET_AS_SINGLE_(0x46800000)) * INTERPRET_AS_SINGLE_(0x38800000);
      var out_y =
          ((INTERPRET_AS_DOUBLE_(
                CONCAT44_(0x43300000, (uint) (second_ushort & 0x7fff))) -
            INTERPRET_AS_DOUBLE_(0x4330000000000000)) -
           INTERPRET_AS_SINGLE_(0x46800000)) * INTERPRET_AS_SINGLE_(0x38800000);
      var third_parsed_thing =
          INTERPRET_AS_DOUBLE_(CONCAT44_(0x43300000, third_ushort)) -
          INTERPRET_AS_DOUBLE_(0x4330000000000000);

      outValues = new double[4];
      outValues[0] = out_x;
      outValues[1] = out_y;

      var out_z =
          (third_parsed_thing - fVar1) * const_for_out_value_2;
      outValues[2] = out_z;

      var expected_normalized_w =
          ((1 - out_x * out_x) - out_y * out_y) - out_z * out_z;
      var out_w = 0d;
      if (out_w <= expected_normalized_w) {
        if (INTERPRET_AS_SINGLE_(0x0229C4AB) < expected_normalized_w) {
          var inverse_sqrt_of_expected_normalized_w =
              1.0 / Math.Sqrt(expected_normalized_w);
          out_w =
              (float) (-(inverse_sqrt_of_expected_normalized_w *
                         inverse_sqrt_of_expected_normalized_w *
                         expected_normalized_w -
                         INTERPRET_AS_SINGLE_(0x40400000)) *
                       inverse_sqrt_of_expected_normalized_w *
                       INTERPRET_AS_SINGLE_(0x3F000000));
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
    [StringLengthSource(16)] public string Name { get; set; }

    // The number of what seem to be position keyframes? These have a length of 4 bytes each. 
    public uint PositionKeyframeCount { get; set; }

    // The number of what seem to be rotation keyframes? These have a length of 6 bytes each. 
    public uint RotationKeyframeCount { get; set; }

    private readonly ulong padding0_ = 0;
    public float XPosDelta { get; set; }
    public float YPosDelta { get; set; }
    public float ZPosDelta { get; set; }
    public float XPosMin { get; set; }
    public float YPosMin { get; set; }
    public float ZPosMin { get; set; }
    private readonly uint padding1_ = 0;

    public uint WeirdId { get; set; }
  }

  public class AnimBone {
    public AnimBoneData Data { get; } = new();
    public uint BoneIndex { get; set; }
  }

  public class AnimBoneFrames {
    public List<(float, float, float)> PositionFrames { get; } = new();
    public List<(float, float, float, float)> RotationFrames { get; } = new();
  }
}