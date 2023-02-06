using schema.text;

namespace xmod.schema {
  public class Material : ITextDeserializable {
    public string Name { get; set; }

    public void Read(ITextReader tr) {
      tr.AssertString("mtl ");

      this.Name = tr.ReadUpToAndPastTerminator(" {");

      TextReaderUtils.ReadKeyValueNumber<int>(tr, "packets");
      TextReaderUtils.ReadKeyValueNumber<int>(tr, "primitives");
      TextReaderUtils.ReadKeyValueNumber<int>(tr, "textures");
      TextReaderUtils.ReadKeyValue(tr, "illum");
      TextReaderUtils.ReadKeyValueInstance<Vector3>(tr, "ambient");
      TextReaderUtils.ReadKeyValueInstance<Vector3>(tr, "diffuse");
      TextReaderUtils.ReadKeyValueInstance<Vector3>(tr, "specular");
      TextReaderUtils.ReadKeyValueNumber<int>(tr, "attributes");

      tr.ReadUpToAndPastTerminator("}");
      tr.IgnoreManyIfPresent(TextReaderConstants.WHITESPACE_STRINGS);
    }
  }
}