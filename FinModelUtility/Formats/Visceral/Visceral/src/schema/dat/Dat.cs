using schema.binary;

namespace visceral.schema.dat {
  [BinarySchema]
  public class Dat : IBinaryConvertible {
    private readonly string magic_ = "BIGH";
  }
}
