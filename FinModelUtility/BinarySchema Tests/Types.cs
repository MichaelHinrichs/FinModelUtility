namespace schema.binary {
  [BinarySchema]
  public partial class ClassWith1Bool : IBiSerializable {
    [IntegerFormat(SchemaIntegerType.INT16)]
    public bool Bool { get; private set; }
  }
}
