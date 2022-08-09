using schema;

namespace mod.schema.collision {
  [Schema]
  public partial class Plane : IBiSerializable {
    public readonly Vector3f position = new();
    public float diameter;
  }
}
