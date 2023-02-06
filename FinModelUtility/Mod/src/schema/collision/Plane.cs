using fin.schema.vector;

using schema.binary;

namespace mod.schema.collision {
  [BinarySchema]
  public partial class Plane : IBinaryConvertible {
    public readonly Vector3f position = new();
    public float diameter;
  }
}
