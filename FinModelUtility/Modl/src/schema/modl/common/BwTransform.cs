using fin.schema.vector;
using schema;


namespace modl.schema.modl.common {

  [BinarySchema]
  public partial class BwTransform : IBiSerializable {
    public Vector3f Position { get; } = new();
    public Vector4f Rotation { get; } = new();
  }
}
