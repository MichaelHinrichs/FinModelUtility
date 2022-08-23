using schema;


namespace bmd.schema.bmd.jnt1 {
  [BinarySchema]
  public partial class Jnt1Entry : IBiSerializable {
    public ushort Unknown1;
    public byte Unknown2;
    public byte Padding1;
    public float Sx;
    public float Sy;
    public float Sz;
    public short Rx;
    public short Ry;
    public short Rz;
    public ushort Padding2;
    public float Tx;
    public float Ty;
    public float Tz;
    public float Unknown3;
    public readonly float[] BoundingBoxMin = new float[3];
    public readonly float[] BoundingBoxMax = new float[3];
  }
}
