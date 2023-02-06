using schema.text;


namespace xmod.schema.xmod {
  public class Packet : ITextDeserializable {
    public IReadOnlyList<Adjunct> Adjuncts { get; set; }
    public IReadOnlyList<Primitive> Primitives { get; set; }
    public IReadOnlyList<int> MatrixTable { get; set; }

    public void Read(ITextReader tr) {
      tr.AssertString("packet");

      var numAdjuncts = tr.ReadInt32();
      var numPrimitives = tr.ReadInt32();
      var numMatrices = tr.ReadInt32();

      tr.ReadUpToAndPastTerminator("{");
      tr.IgnoreManyIfPresent(TextReaderConstants.WHITESPACE_STRINGS);

      this.Adjuncts = tr.ReadNewArray<Adjunct>(numAdjuncts);
      this.Primitives = tr.ReadNewArray<Primitive>(numPrimitives);

      tr.AssertString("mtx");
      this.MatrixTable = tr.ReadInt32s(TextReaderConstants.WHITESPACE_STRINGS,
                                       TextReaderConstants.NEWLINE_STRINGS);

      tr.ReadUpToAndPastTerminator("}");
      tr.IgnoreManyIfPresent(TextReaderConstants.WHITESPACE_STRINGS);
    }
  }
}