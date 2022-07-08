using schema;


namespace bmd.schema.bmd.shp1 {
  [Schema]
  public partial class BatchAttribute : IDeserializable {
    public uint Attribute;
    public uint DataType;
  }
}
