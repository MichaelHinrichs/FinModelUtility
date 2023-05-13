using schema.binary;

namespace cmb.schema.cmb {
  [BinarySchema]
  public partial class AttributeSlice : IBinaryConvertible {
    public uint Size { get; private set; }
    public uint StartOffset { get; private set; }
  }
}