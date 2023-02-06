using schema.text;

namespace xmod.schema {
  public enum PrimitiveType {
    TRIANGLE_STRIP,
    TRIANGLE_FAN,
  }

  public class Primitive : ITextDeserializable {
    public PrimitiveType Type { get; set; }
    public IReadOnlyList<int> VertexIndices { get; set; }

    public void Read(ITextReader tr) {
      tr.IgnoreManyIfPresent(TextReaderConstants.WHITESPACE_STRINGS);

      this.Type = tr.ReadString(3) switch {
        "str" => PrimitiveType.TRIANGLE_STRIP,
        "stp" => PrimitiveType.TRIANGLE_FAN,
      };

      this.VertexIndices = tr.ReadInt32s(TextReaderConstants.WHITESPACE_STRINGS,
                                         TextReaderConstants.NEWLINE_STRINGS);
      tr.IgnoreManyIfPresent(TextReaderConstants.WHITESPACE_STRINGS);
    }
  }
}
