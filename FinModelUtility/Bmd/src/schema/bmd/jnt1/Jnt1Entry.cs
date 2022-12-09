using fin.schema.vector;
using schema;


namespace bmd.schema.bmd.jnt1 {
  [BinarySchema]
  public partial class Jnt1Entry : IBiSerializable {
    public ushort Unknown1;
    [IntegerFormat(SchemaIntegerType.BYTE)]
    public bool IgnoreParentScale { get; set; }
    private readonly byte padding1_ = byte.MaxValue;
    public Vector3f Scale { get; } = new();
    public Vector3s Rotation { get; } = new();
    private readonly ushort padding2_ = ushort.MaxValue;
    public Vector3f Translation { get; } = new();
    public float BoundingSphereDiameter { get; set; }
    public Vector3f BoundingBoxMin { get; } = new();
    public Vector3f BoundingBoxMax { get; } = new();
  }
}