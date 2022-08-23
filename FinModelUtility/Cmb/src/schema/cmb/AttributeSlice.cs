using schema;

namespace cmb.schema.cmb {
  [BinarySchema]
  public partial class AttributeSlice : IDeserializable {
    public uint Size { get; private set; }
    public uint StartOffset { get; private set; }
  }
}