using gx;

using schema;


namespace j3d.schema.bmd.vtx1 {
  [BinarySchema]
  public partial class ArrayFormat : IBiSerializable {
    public GxAttribute ArrayType;
    public uint ComponentCount;
    public uint DataType;
    public byte DecimalPoint;
    public byte Unknown1;
    public ushort Unknown2;
  }
}
