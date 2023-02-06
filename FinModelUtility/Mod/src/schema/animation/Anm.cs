using schema.binary;


namespace mod.schema.animation {
  [BinarySchema]
  public partial class Anm : IBinaryConvertible {
    [ArrayLengthSource(SchemaIntegerType.UINT32)]
    public DcxWrapper[] Wrappers { get; set; }
  }
}