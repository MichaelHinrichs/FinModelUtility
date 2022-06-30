using schema;


namespace bmd.formats.shp1 {
  [Schema]
  public partial class BatchAttribute : IDeserializable {
    public uint Attribute;
    public uint DataType;
  }
}
