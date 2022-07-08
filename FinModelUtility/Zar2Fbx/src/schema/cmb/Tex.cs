using fin.util.strings;

using schema;

namespace zar.schema.cmb {
  [Schema]
  public partial class Tex : IDeserializable {
    public readonly string magicText = "tex" + AsciiUtil.GetChar(0x20);
    public uint chunkSize { get; private set; }
    [ArrayLengthSource(IntType.UINT32)]
    public Texture[] textures { get; private set; }
  }
}
