using schema;

namespace cmb.schema.cmb {
  [BinarySchema]
  public partial class VertexAttribute : IDeserializable {
    public uint Start { get; private set; }
    public float Scale { get; private set; } 

    [NumberFormat(SchemaNumberType.UINT16)]
    public DataType DataType { get; private set; }
    
    [NumberFormat(SchemaNumberType.UINT16)]
    public VertexAttributeMode Mode { get; private set; }

    public float[] Constants { get; } = new float[4];
  }
}