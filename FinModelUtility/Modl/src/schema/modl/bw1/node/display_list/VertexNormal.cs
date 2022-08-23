using schema;


namespace modl.schema.modl.bw1.node.display_list {
  [BinarySchema]
  public partial class VertexNormal : IBiSerializable {
    [NumberFormat(SchemaNumberType.SN8)]
    public float X { get; set; }

    [NumberFormat(SchemaNumberType.SN8)]
    public float Y { get; set; }
    
    [NumberFormat(SchemaNumberType.SN8)]
    public float Z { get; set; }
  }
}
