using schema.binary;
using schema.binary.attributes;

namespace mod.schema {
  [BinarySchema]
  public partial class IndexAndWeight : IBinaryConvertible {
    public ushort index;
    public float weight;
  }

  [BinarySchema]
  public partial class Envelope : IBinaryConvertible {
    [ArrayLengthSource(SchemaIntegerType.UINT16)]
    public IndexAndWeight[] indicesAndWeights;
  }
}