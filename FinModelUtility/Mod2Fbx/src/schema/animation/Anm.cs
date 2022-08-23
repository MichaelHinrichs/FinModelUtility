using schema;


namespace mod.schema.animation {
  [BinarySchema]
  public partial class Anm : IBiSerializable {
    [ArrayLengthSource(SchemaIntType.UINT32)]
    public DcxWrapper[] Wrappers { get; set; }
  }
}