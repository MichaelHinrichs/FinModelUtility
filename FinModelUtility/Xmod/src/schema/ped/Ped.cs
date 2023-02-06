using schema.text;


namespace xmod.schema.ped {
  public class Ped : ITextDeserializable {
    public string SkelName { get; set; }
    public string XmodName { get; set; }
    public IDictionary<string, string> AnimMap { get; set; }

    public void Read(ITextReader tr) {
      SkelName = TextReaderUtils.ReadKeyValue(tr, "skel").Trim();
      tr.IgnoreManyIfPresent(TextReaderConstants.WHITESPACE_STRINGS);

      tr.AssertString("lod 0 {");
      XmodName = tr.ReadUpToAndPastTerminator("}").Trim();
      tr.IgnoreManyIfPresent(TextReaderConstants.WHITESPACE_STRINGS);

      this.AnimMap = new Dictionary<string, string>();
      tr.AssertString("anim {");
      while(true) {
        tr.IgnoreManyIfPresent(TextReaderConstants.WHITESPACE_STRINGS);
        if (tr.Matches(out _, "}")) {
          break;
        }

        var key = tr.ReadUpToAndPastTerminator(":");
        var value = tr.ReadLine().Trim();
        this.AnimMap[key] = value;
      }
    }
  }
}
