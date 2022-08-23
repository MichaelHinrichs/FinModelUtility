using schema;


namespace mod.schema.animation {
  [BinarySchema]
  public partial class Anm : IBiSerializable {
    [ArrayLengthSource(SchemaIntegerType.UINT32)]
    public DcxWrapper[] Wrappers { get; set; }
  }
}