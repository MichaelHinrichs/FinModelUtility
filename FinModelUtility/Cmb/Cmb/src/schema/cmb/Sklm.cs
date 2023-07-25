using schema.binary;

namespace cmb.schema.cmb {
  [BinarySchema]
  public partial class Sklm : IBinaryConvertible {
    public uint mshOffset;
    public uint shpOffset;

    public readonly Mshs meshes = new();
    public readonly Shp shapes = new();
  }
}
