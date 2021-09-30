using System.IO;

namespace mod.gcn {
  public class NBT : IGcnSerializable {
    public readonly Vector3f normals = new();
    public readonly Vector3f binormals = new();
    public readonly Vector3f tangent = new();

    public void Read(EndianBinaryReader reader) {
      this.normals.Read(reader);
      this.binormals.Read(reader);
      this.tangent.Read(reader);
    }

    public void Write(EndianBinaryWriter writer) {
      this.normals.Write(writer);
      this.binormals.Write(writer);
      this.tangent.Write(writer);
    }
  }
}