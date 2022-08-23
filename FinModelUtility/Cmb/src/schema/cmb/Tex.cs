using fin.util.strings;

using schema;

namespace cmb.schema.cmb {
  [BinarySchema]
  public partial class Tex : IDeserializable {
    public readonly string magicText = "tex" + AsciiUtil.GetChar(0x20);
    public uint chunkSize { get; private set; }
    [ArrayLengthSource(SchemaIntegerType.UINT32)]
    public Texture[] textures { get; private set; }
  }
}
