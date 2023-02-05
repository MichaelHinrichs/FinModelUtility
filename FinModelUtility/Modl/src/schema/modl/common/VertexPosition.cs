using schema.binary;


namespace modl.schema.modl.common {

  [BinarySchema]
  public partial class VertexPosition : IBiSerializable {
    public short X { get; set; }
    public short Y { get; set; }
    public short Z { get; set; }
  }
}
