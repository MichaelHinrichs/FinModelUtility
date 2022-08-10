using gx;

using schema;


namespace bmd.schema.bmd.shp1 {
  [Schema]
  public partial class BatchAttribute : IDeserializable {
    public GxAttribute Attribute { get; set; }
    public uint DataType { get; set; }
  }
}
