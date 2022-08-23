using schema;


namespace modl.schema.modl.bw1.node.display_list {

  [BinarySchema]
  public partial class VertexPosition : IBiSerializable {
    public short X { get; set; }
    public short Y { get; set; }
    public short Z { get; set; }
  }
}
