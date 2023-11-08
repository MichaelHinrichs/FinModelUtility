using gx;

using schema.binary;

namespace jsystem.schema.bmd.shp1 {
  [BinarySchema]
  public partial class BatchAttribute : IBinaryDeserializable {
    public GxAttribute Attribute { get; set; }
    public uint DataType { get; set; }
  }
}
