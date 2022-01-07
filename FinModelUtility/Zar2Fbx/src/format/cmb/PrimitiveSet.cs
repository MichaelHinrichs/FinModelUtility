using System.IO;

using fin.util.asserts;

using schema;

namespace zar.format.cmb {
  public class PrimitiveSet : IDeserializable {
    public uint chunkSize;
    public uint primitiveCount;
    public SkinningMode skinningMode;
    public ushort boneTableCount;
    public uint boneTableOffset;
    public uint primitiveOffset;
    public short[] boneTable;
    public readonly Primitive primitive = new();

    public void Read(EndianBinaryReader r) {
      r.AssertMagicText("prms");

      this.chunkSize = r.ReadUInt32();

      // Actually an array but more than one is never used
      this.primitiveCount = r.ReadUInt32();
      Asserts.Equal((uint) 1, this.primitiveCount);

      this.skinningMode = (SkinningMode) r.ReadUInt16();
      this.boneTableCount = r.ReadUInt16();
      this.boneTableOffset = r.ReadUInt32();
      this.primitiveOffset = r.ReadUInt32();

      this.boneTable = r.ReadInt16s(this.boneTableCount);

      r.Align(4);
      this.primitive.Read(r);
    }
  }
}