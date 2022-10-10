using schema;


namespace ast.schema {
  [BinarySchema]
  public partial class BlckSection : IBiSerializable {
    private readonly string magic_ = "BLCK";
  }
}