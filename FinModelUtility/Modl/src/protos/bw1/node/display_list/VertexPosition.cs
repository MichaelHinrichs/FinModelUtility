using schema;


namespace modl.protos.bw1.node.display_list {

  [Schema]
  public partial class VertexPosition : IBiSerializable {
    public short X { get; set; }
    public short Y { get; set; }
    public short Z { get; set; }
  }
}
