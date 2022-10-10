using schema;


namespace ast.schema {
  [BinarySchema]
  public partial class StrmSection : IBiSerializable {
    private readonly string magic_ = "STRM";
    private readonly byte[] bytes_ = new byte[0x40 - 4];
  }
}