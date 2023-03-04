using fin.schema.matrix;

using schema.binary;

namespace geo.schema.rcb {
  public partial class Rcb : IBinaryDeserializable {
    public const float SCALE = 100;

    public IReadOnlyList<Skeleton> Skeletons { get; private set; }

    public void Read(IEndianBinaryReader er) {
      var fileLength = er.ReadUInt32();
      er.Position += 20;

      var skeletonCount = er.ReadUInt32();

      var dataOffset = er.ReadUInt32();
      var unk4 = er.ReadUInt32();
      var dataOffset2 = er.ReadUInt32();
      er.Position += 16;

      var unk5 = er.ReadUInt32();
      var unk6 = er.ReadUInt32();
      var dataOffset3 = er.ReadUInt32();

      er.Position = dataOffset;
      er.ReadNewArray<Skeleton>(out var skeletons, (int) skeletonCount);
      this.Skeletons = skeletons;
    }

    public class Skeleton : IBinaryDeserializable {
      public string SkeletonName { get; private set; }
      public IReadOnlyList<int> BoneParentIdMap { get; private set; }
      public IReadOnlyList<Bone> Bones { get; private set; }

      public void Read(IEndianBinaryReader er) {
        // Read skeleton header
        var unk7 = er.ReadUInt32();

        this.SkeletonName = er.ReadStringNTAtOffset(er.ReadUInt32());
        var boneCount = er.ReadUInt32();
        var boneIdTableOffset = er.ReadUInt32();
        var boneStart = er.ReadUInt32();
        var ukwTableOffset = er.ReadUInt32();

        // Get bone parent table
        er.Subread(
            boneIdTableOffset,
            ser => {
              ser.Position = boneIdTableOffset;

              var boneParentIdMap = new int[boneCount];
              for (var i = 0; i < boneCount; i++) {
                boneParentIdMap[i] = ser.ReadInt32();
                ser.Position += 12;
              }

              this.BoneParentIdMap = boneParentIdMap;
            });

        // Read bone matrices
        er.Subread(
            boneStart,
            ser => {
              er.ReadNewArray<Bone>(out var bones, (int) boneCount);
              this.Bones = bones;
            });
      }
    }

    [BinarySchema]
    public partial class Bone : IBinaryConvertible {
      public Matrix4x4f Matrix { get; } = new();
    }
  }
}