using System.IO;

using schema.binary;

namespace cmb.schema.cmb {
  [BinarySchema]
  public partial class Sklm : IBinaryDeserializable {
    public readonly string magic = "sklm";
    
    public uint chunkSize;
    public uint mshOffset;
    public uint shpOffset;

    public readonly Mshs meshes = new();
    public readonly Shp shapes = new();
  }
}
