using schema;


namespace bmd.formats.shp1 {
  [Schema]
  public partial class PacketLocation : IDeserializable {
    public uint Size;
    public uint Offset;
  }
}
