using schema.binary;
using schema.binary.attributes;

namespace cmb.schema.cmb {
  [BinarySchema]
  public partial class Mshs : IBinaryConvertible {
    public readonly string magic = "mshs";
    public uint chunkSize;

    private uint meshCount;
    // The remainder are translucent meshes and always packed at the end
    public ushort opaqueMeshCount; 
    public ushort idCount;
    
    // Note: Mesh order = draw order
    [RSequenceLengthSource(nameof(meshCount))]
    public Mesh[] meshes;
  }
}
