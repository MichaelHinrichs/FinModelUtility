using System.Collections.Generic;
using System.IO;

using fin.schema.vector;

using schema;


namespace cmb.schema.cmb {
  public class Bone : IDeserializable {
    public ushort id;
    public bool hasSkinningMatrix;
    public short parentId;
    public Vector3f scale { get; } = new();
    public Vector3f rotation { get; } = new();
    public Vector3f translation { get; } = new();
    public uint unk0;

    public Bone? parent;
    public IList<Bone> children = new List<Bone>();

    public void Read(EndianBinaryReader r) {
      // Because only 12 bits are used, 4095 is the max bone count. (In
      // versions > OoT3D anyway)
      this.id = r.ReadUInt16();

      // M-1:
      // Other 4 bits are probably more flags, but they're not used in any of
      // the three games
      // Though I probably missed a few compressed files. It's most likely
      // these flags below:
      // IsSegmentScaleCompensate, IsCompressible, IsNeededRendering, HasSkinningMatrix
      this.hasSkinningMatrix = ((this.id >> 4) & 1) != 0;

      this.id &= 0xFFF; // Get boneID
      this.parentId = r.ReadInt16();

      this.scale.Read(r);
      this.rotation.Read(r);
      this.translation.Read(r);

      if (CmbHeader.Version > CmbVersion.OCARINA_OF_TIME_3D) {
        // M-1: I assume a crc32 of the bone name
        this.unk0 = r.ReadUInt32();
      }
    }
  }
}