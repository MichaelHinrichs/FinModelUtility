using schema;

namespace mod.schema {
  [Schema]
  public partial class NBT : IBiSerializable {
    public readonly Vector3f normals = new();
    public readonly Vector3f binormals = new();
    public readonly Vector3f tangent = new();
  }
}