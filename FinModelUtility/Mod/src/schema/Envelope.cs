using schema;

namespace mod.schema {
  [BinarySchema]
  public partial class IndexAndWeight : IBiSerializable {
    public ushort index;
    public float weight;
  }

  [BinarySchema]
  public partial class Envelope : IBiSerializable {
    [ArrayLengthSource(SchemaIntegerType.UINT16)]
    public IndexAndWeight[] indicesAndWeights;
  }
}