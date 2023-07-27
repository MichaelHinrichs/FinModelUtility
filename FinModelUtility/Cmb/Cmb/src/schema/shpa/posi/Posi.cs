using fin.schema.vector;

using schema.binary.attributes.sequence;
using schema.binary;

namespace cmb.schema.shpa.posi {

  [BinarySchema]
  public partial class Posi : IBinaryConvertible {
    [RSequenceUntilEndOfStream]
    public Vector3f[] Values { get; private set; }
  }
}
