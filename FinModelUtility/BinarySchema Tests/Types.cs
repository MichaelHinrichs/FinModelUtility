namespace schema {
  [BinarySchema]
  public partial class ClassWith1Bool : IBiSerializable {
    [Format(SchemaNumberType.INT16)]
    public bool Bool { get; set; }
  }
}
