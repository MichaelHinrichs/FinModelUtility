using asserts;

using schema.binary;


namespace modl.schema.anim.bw2 {
  public class Bw2Anim : IAnim, IBinaryDeserializable {
    public List<IBwAnimBone> AnimBones { get; } = new();
    public List<AnimBoneFrames> AnimBoneFrames { get; } = new();

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

      for (var i = 0; i < boneCount; ++i) {
        var bone = new Bw2AnimBone();
        bone.Read(er);
        this.AnimBones.Add(bone);
      }

      // Next is blocks of data separated by "0xcd" bytes. The lengths of these can be predicted with the ints in the bones, so these seem to be keyframes.
      var estimatedLengths = this.AnimBones
                                 .Select(
                                     bone =>
                                         bone.PositionKeyframeCount * 2 * 2 +
                                         bone.RotationKeyframeCount * 2 * 4)
                                 .ToArray();

      var totalLength = (uint) 0;
      foreach (var estimatedLength in estimatedLengths) {
        totalLength += estimatedLength;
      }

      var boneBytes = new List<byte[]>();
      for (var i = 0; i < this.AnimBones.Count; ++i) {
        var currentBuffer = er.ReadBytes((int) estimatedLengths[i]);
        boneBytes.Add(currentBuffer);

        // TODO: May no longer be necessary
        if (i + 1 < this.AnimBones.Count) {
          if (er.ReadUInt16() != 0xcdcd) {
            er.Position -= 2;
          }
        }
      }

      for (var i = 0; i < boneCount; ++i) {
        var bone = this.AnimBones[i];

        var buffer = boneBytes[i];
        using var ber =
            new EndianBinaryReader(buffer, Endianness.BigEndian);

        var animBoneFrames = new AnimBoneFrames();
        this.AnimBoneFrames.Add(animBoneFrames);

        for (var p = 0; p < bone.PositionKeyframeCount; ++p) {
          Parse3PositionValuesFrom2UShorts_(bone, ber, out var floats);
          animBoneFrames.PositionFrames.Add(((float) floats[0],
                                             (float) floats[1],
                                             (float) floats[2]));
        }

        for (var p = 0; p < bone.RotationKeyframeCount; ++p) {
          var flipSigns =
              this.Parse4RotationValuesFrom4UShorts_(ber, out var floats);
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
      }
    }

    public void Parse3PositionValuesFrom2UShorts_(
        IBwAnimBone animBone,
        IEndianBinaryReader er,
        out double[] outValues) {
      var first_uint = er.ReadUInt32();
      er.Position -= 2;
      var second_ushort = er.ReadUInt16();

      outValues = new double[3];
      outValues[0] =
          (INTERPRET_AS_DOUBLE_(
               CONCAT44_(0x43300000, (uint) (first_uint >> 0x15))) -
           INTERPRET_AS_DOUBLE_(0x4330000000000000)) *
          animBone.XPosDelta + animBone.XPosMin;
      outValues[1] =
          (INTERPRET_AS_DOUBLE_(
               CONCAT44_(0x43300000, (uint) ((first_uint >> 10) & 0x7ff))) -
           INTERPRET_AS_DOUBLE_(0x4330000000000000)) *
          animBone.YPosDelta + animBone.YPosMin;
      outValues[2] =
          (INTERPRET_AS_DOUBLE_(
               CONCAT44_(0x43300000, (uint) (second_ushort & 0x3ff))) -
           INTERPRET_AS_DOUBLE_(0x4330000000000000)) *
          animBone.ZPosDelta + animBone.ZPosMin;
    }

    public bool Parse4RotationValuesFrom4UShorts_(IEndianBinaryReader er,
                                                  out double[] outValues) {
      var first_ushort = er.ReadUInt16();
      var second_ushort = er.ReadUInt16();
      var third_ushort = er.ReadUInt16();
      var fourth_ushort = er.ReadUInt16();

      const double DOUBLE_80600f40 = 4.503601774854144E15;
      const double FLOAT_80603708 = 3.0517578E-5;

      var outX = (float) (INTERPRET_AS_DOUBLE_(CONCAT44_(0x43300000,
                                                 ((first_ushort & 0x3fffU) <<
                                                  1) ^
                                                 0x80000000)) -
                          DOUBLE_80600f40) * FLOAT_80603708;
      var outY = (float) (INTERPRET_AS_DOUBLE_(CONCAT44_(0x43300000,
                                                 (uint) ((second_ushort &
                                                             0x3fff) <<
                                                       1) ^
                                                 0x80000000)) -
                          DOUBLE_80600f40)
                 * FLOAT_80603708;
      var outZ = (float) (INTERPRET_AS_DOUBLE_(CONCAT44_(0x43300000,
                                                 third_ushort & 0x7fffU ^
                                                 0x80000000)) -
                          DOUBLE_80600f40) * FLOAT_80603708;
      var outW = (float) (INTERPRET_AS_DOUBLE_(CONCAT44_(0x43300000,
                                                 fourth_ushort & 0x7fffU ^
                                                 0x80000000)) -
                          DOUBLE_80600f40) * FLOAT_80603708;
      if (((int) first_ushort & 0x4000U) != 0) {
        outX = -outX;
      }
      if (((int) (short) (second_ushort << 1) & 0x8000U) != 0) {
        outY = -outY;
      }
      if (((int) third_ushort & 0x8000U) != 0) {
        outZ = -outZ;
      }
      if (((int) fourth_ushort & 0x8000U) != 0) {
        outW = -outW;
      }

      outValues = new double[4];
      outValues[0] = outX;
      outValues[1] = outY;
      outValues[2] = outZ;
      outValues[3] = outW;

      return (-(second_ushort >> 0xf & 1) >> 0x1f) != 0;
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
}