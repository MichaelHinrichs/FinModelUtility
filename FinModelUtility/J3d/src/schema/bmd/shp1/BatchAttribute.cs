using gx;

using schema;


namespace j3d.schema.bmd.shp1 {
  [BinarySchema]
  public partial class BatchAttribute : IDeserializable {
    public GxAttribute Attribute { get; set; }
    public uint DataType { get; set; }
  }
}
