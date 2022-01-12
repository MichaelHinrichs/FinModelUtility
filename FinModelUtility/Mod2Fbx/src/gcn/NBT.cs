using System.IO;

using schema;

namespace mod.gcn {
  [Schema]
  public partial class NBT : IGcnSerializable {
    public readonly Vector3f normals = new();
    public readonly Vector3f binormals = new();
    public readonly Vector3f tangent = new();

    public void Write(EndianBinaryWriter writer) {
      this.normals.Write(writer);
      this.binormals.Write(writer);
      this.tangent.Write(writer);
    }
  }
}