using fin.schema;

using gx;

using schema.binary;

namespace j3d.schema.bmd.vtx1 {
  [BinarySchema]
  public partial class ArrayFormat : IBinaryConvertible {
    public GxAttribute ArrayType;
    public uint ComponentCount;
    public uint DataType;
    public byte DecimalPoint;
    [Unknown]
    public byte Unknown1;
    [Unknown]
    public ushort Unknown2;
  }
}
