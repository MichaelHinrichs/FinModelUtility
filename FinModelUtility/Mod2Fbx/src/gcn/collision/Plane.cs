using System.IO;

using schema;

namespace mod.gcn.collision {
  [Schema]
  public partial class Plane : IGcnSerializable {
    public readonly Vector3f position = new();
    public float diameter;

    public void Write(EndianBinaryWriter writer) {
      this.position.Write(writer);
      writer.Write(this.diameter);
    }
  }
}
