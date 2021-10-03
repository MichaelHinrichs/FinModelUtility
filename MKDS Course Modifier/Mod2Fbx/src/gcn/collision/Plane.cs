using System.IO;

namespace mod.gcn.collision {
  public class Plane : IGcnSerializable {
    public readonly Vector3f position = new();
    public float diameter = 0;

    public void Read(EndianBinaryReader reader) {
      this.position.Read(reader);
      this.diameter = reader.ReadSingle();
    }

    public void Write(EndianBinaryWriter writer) {
      this.position.Write(writer);
      writer.Write(this.diameter);
    }
  }
}
