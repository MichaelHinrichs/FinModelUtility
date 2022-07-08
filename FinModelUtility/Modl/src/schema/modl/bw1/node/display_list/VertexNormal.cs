using schema;


namespace modl.schema.modl.bw1.node.display_list {
  [Schema]
  public partial class VertexNormal : IBiSerializable {
    [Format(SchemaNumberType.SN8)]
    public float X { get; set; }

    [Format(SchemaNumberType.SN8)]
    public float Y { get; set; }
    
    [Format(SchemaNumberType.SN8)]
    public float Z { get; set; }
  }
}
