namespace schema {
  [BinarySchema]
  public partial class ClassWith1Bool : IBiSerializable {
    [NumberFormat(SchemaNumberType.INT16)]
    public bool Bool { get; set; }
  }
}
