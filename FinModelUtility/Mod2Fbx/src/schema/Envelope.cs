using System.Collections.Generic;
using System.IO;

using schema;

namespace mod.schema {
  [Schema]
  public partial class IndexAndWeight : IBiSerializable {
    public ushort index;
    public float weight;
  }

  [Schema]
  public partial class Envelope : IBiSerializable {
    [ArrayLengthSource(IntType.UINT16)]
    public IndexAndWeight[] indicesAndWeights;
  }
}