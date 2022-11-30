using schema;


namespace bmd.schema.bmd.shp1 {
  [BinarySchema]
  public partial class PacketLocation : IDeserializable {
    public uint Size;
    public uint Offset;
  }
}
