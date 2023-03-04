using fin.schema.matrix;

using schema.binary;

namespace geo.schema.rcb {
  public partial class Rcb : IBinaryDeserializable {
    public const float SCALE = 100;

    public string SkeletonName { get; private set; }
    public IReadOnlyList<int> BoneParentIdMap { get; private set; }
    public IReadOnlyList<Bone> Bones { get; private set; }

    public void Read(IEndianBinaryReader er) {
      var fileLength = er.ReadUInt32();
      er.Position += 20;

      er.AssertUInt32(1); // Skeleton count

      var dataOffset = er.ReadUInt32();
      var unk4 = er.ReadUInt32();
      var dataOffset2 = er.ReadUInt32();
      er.Position += 16;

      var unk5 = er.ReadUInt32();
      var unk6 = er.ReadUInt32();
      var dataOffset3 = er.ReadUInt32();

      er.Position = dataOffset;

      // Read skeleton header
      var unk7 = er.ReadUInt32();

      this.SkeletonName = er.ReadStringNTAtOffset(er.ReadUInt32());
      var boneCount = er.ReadUInt32();
      var boneIdTableOffset = er.ReadUInt32();
      var boneStart = er.ReadUInt32();
      var ukwTableOffset = er.ReadUInt32();

      // Get bone parent table
      er.Position = boneIdTableOffset;

      var boneParentIdMap = new int[boneCount];
      for (var i = 0; i < boneCount; i++) {
        boneParentIdMap[i] = er.ReadInt32();
        er.Position += 12;
      }
      this.BoneParentIdMap = boneParentIdMap;

      // Read bone matrices
      er.Position = boneStart;
      er.ReadNewArray<Bone>(out var bones, (int) boneCount);
      this.Bones = bones;
    }

    [BinarySchema]
    public partial class Bone : IBinaryConvertible {
      public Matrix4x4f Matrix { get; } = new();
    }
  }
}