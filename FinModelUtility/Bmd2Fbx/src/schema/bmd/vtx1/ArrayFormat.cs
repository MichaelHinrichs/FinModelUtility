using schema;


namespace bmd.schema.bmd.vtx1 {
  [Schema]
  public partial class ArrayFormat : IBiSerializable {
    public uint ArrayType;
    public uint ComponentCount;
    public uint DataType;
    public byte DecimalPoint;
    public byte Unknown1;
    public ushort Unknown2;
  }
}
