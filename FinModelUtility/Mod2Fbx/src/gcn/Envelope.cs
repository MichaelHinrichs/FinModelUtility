using System.Collections.Generic;
using System.IO;

using schema;

namespace mod.gcn {
  [Schema]
  public partial class IndexAndWeight : IDeserializable {
    public ushort index;
    public float weight;
  }

  [Schema]
  public partial class Envelope : IDeserializable {
    [ArrayLengthSource(IntType.UINT16)]
    public IndexAndWeight[] indicesAndWeights;
  }
}