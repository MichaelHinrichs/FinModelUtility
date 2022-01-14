using System.IO;

using schema;

namespace zar.format.cmb {
  [Schema]
  public partial class Mshs : IDeserializable {
    public readonly string magic = "mshs";
    public uint chunkSize;
    
    public uint meshCount;
    // The remainder are translucent meshes and always packed at the end
    public ushort opaqueMeshCount; 
    public ushort idCount;
    
    // Note: Mesh order = draw order
    [ArrayLengthSource(nameof(Mshs.meshCount))]
    public Mesh[] meshes;
  }
}
