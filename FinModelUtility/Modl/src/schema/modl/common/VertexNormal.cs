using schema.binary;


namespace modl.schema.modl.common {
  [BinarySchema]
  public partial class VertexNormal : IBinaryConvertible {
    [NumberFormat(SchemaNumberType.SN8)]
    public float X { get; set; }

    [NumberFormat(SchemaNumberType.SN8)]
    public float Y { get; set; }
    
    [NumberFormat(SchemaNumberType.SN8)]
    public float Z { get; set; }
  }
}
