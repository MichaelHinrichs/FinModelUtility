using System.IO;

using schema;

namespace mod.gcn.collision {
  [Schema]
  public partial class Plane : IBiSerializable {
    public readonly Vector3f position = new();
    public float diameter;
  }
}
