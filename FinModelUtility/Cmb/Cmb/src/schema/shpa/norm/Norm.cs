using schema.binary.attributes.sequence;
using schema.binary;

namespace cmb.schema.shpa.norm {

  [BinarySchema]
  public partial class Norm : IBinaryConvertible {
    [RSequenceUntilEndOfStream]
    public ShpaNormal[] Values { get; private set; }
  }
}
