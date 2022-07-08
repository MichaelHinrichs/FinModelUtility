using System.IO;

using schema;

namespace cmb.schema.cmb {
  [Schema]
  public partial class Sklm : IDeserializable {
    public readonly string magic = "sklm";
    
    public uint chunkSize;
    public uint mshOffset;
    public uint shpOffset;

    public readonly Mshs meshes = new();
    public readonly Shp shapes = new();
  }
}
