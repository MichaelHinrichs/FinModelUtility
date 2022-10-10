using schema;


namespace ast.schema {
  [BinarySchema]
  public partial class Ast : IBiSerializable {
    public StrmSection Strm { get; set; }
    public BlckSection Blck { get; set; }
  }
}