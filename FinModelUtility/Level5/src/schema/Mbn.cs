using fin.schema.vector;
using schema;

namespace level5.schema {
  [BinarySchema]
  public partial class Mbn : IDeserializable {
    public int Id { get; private set; }

    public int ParentId { get; private set; }

    public int Unknown0 { get; private set; }

    public Vector3f Position { get; } = new();

    public float[] RotationMatrix3 { get; } = new float[9];

    public Vector3f Scale { get; } = new();
  }
}
