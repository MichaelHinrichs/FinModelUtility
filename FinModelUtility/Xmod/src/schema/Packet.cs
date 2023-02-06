using schema.text;


namespace xmod.schema {
  public class Packet : ITextDeserializable {
    public IReadOnlyList<Adjunct> Adjuncts { get; set; }
    public IReadOnlyList<Primitive> Primitives { get; set; }

    public void Read(ITextReader tr) {
      tr.AssertString("packet");

      var numAdjuncts = tr.ReadInt32();
      var numPrimitives = tr.ReadInt32();
      var numMatrices = tr.ReadInt32();

      tr.ReadUpToAndPastTerminator("{");
      tr.IgnoreManyIfPresent(TextReaderConstants.WHITESPACE_STRINGS);

      this.Adjuncts = tr.ReadNewArray<Adjunct>(numAdjuncts);
      this.Primitives = tr.ReadNewArray<Primitive>(numPrimitives);

      tr.ReadUpToAndPastTerminator("}");
      tr.IgnoreManyIfPresent(TextReaderConstants.WHITESPACE_STRINGS);
    }
  }
}
