using schema;


namespace bmd.schema.bmd.shp1 {
  [Schema]
  public partial class PacketLocation : IDeserializable {
    public uint Size;
    public uint Offset;
  }
}
