using fin.util.asserts;
using fin.util.strings;

using schema;


namespace modl.schema.anim {
  public class Anim : IDeserializable {
    public void Read(EndianBinaryReader er) {
      string name0;
      {
        var endianness = er.Endianness;
        er.Endianness = Endianness.LittleEndian;

        var name0Length = er.ReadUInt32();
        name0 = er.ReadString((int) name0Length);

        er.Endianness = endianness;
      }

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

      // Next is a series of bone definitions. Each one has a length of 64.
      var boneCount = er.ReadUInt32();
      var bones = new List<AnimBone>();
      for (var i = 0; i < boneCount; ++i) {
        var bone = new AnimBone();
        bone.Data.Read(er);
        bones.Add(bone);
      }
      er.AssertUInt64(0);

      var remainingLength = er.Length - er.Position;

      // Next is blocks of data separated by "0xcd" bytes. The lengths of these can be predicted with the ints in the bones, so these seem to be keyframes.
      var estimatedLengths = bones
                             .Select(
                                 bone => bone.Data.PositionKeyframeCount * 4 +
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

      // TODO: There can be a little bit of extra data at the end, what is this for??
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
}