using schema;


namespace bmd.schema.bmd.shp1 {
  [Schema]
  public partial class MatrixData : IBiSerializable {
    public ushort Unknown { get; set; }
    public ushort Count { get; set; }
    public uint FirstIndex { get; set; }
  }
}
