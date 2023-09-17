using schema.binary;

namespace cmb.schema.cmb.sklm {
  [BinarySchema]
  public partial class Sklm : IBinaryConvertible {
    public uint mshOffset;
    public uint shpOffset;

    public readonly Mshs mshs = new();
    public readonly Shp shapes = new();
  }
}
