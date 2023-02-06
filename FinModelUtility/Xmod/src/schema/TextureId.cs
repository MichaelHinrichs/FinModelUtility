using schema.text;

namespace xmod.schema {
  public class TextureId : ITextDeserializable {
    public int Index { get; set; }
    public string Name { get; set; }

    public void Read(ITextReader tr) {
      tr.IgnoreManyIfPresent(TextReaderConstants.WHITESPACE_STRINGS);
      this.Index = tr.ReadInt32();
      tr.IgnoreManyIfPresent(TextReaderConstants.WHITESPACE_STRINGS);
      tr.AssertChar('"');
      this.Name = tr.ReadUpToAndPastTerminator("\"");
    }
  }
}
