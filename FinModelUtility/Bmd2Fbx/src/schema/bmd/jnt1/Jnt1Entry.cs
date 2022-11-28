using fin.schema.vector;
using schema;


namespace bmd.schema.bmd.jnt1 {
  [BinarySchema]
  public partial class Jnt1Entry : IBiSerializable {
    public ushort Unknown1;
    public byte Unknown2;
    public byte Padding1;
    public Vector3f Scale { get; } = new();
    public Vector3s Rotation { get; } = new();
    public ushort Padding2;
    public Vector3f Translation { get; } = new();
    public float Unknown3;
    public readonly float[] BoundingBoxMin = new float[3];
    public readonly float[] BoundingBoxMax = new float[3];
  }
}
