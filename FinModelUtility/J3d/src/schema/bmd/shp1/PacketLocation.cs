using schema;


namespace j3d.schema.bmd.shp1 {
  [BinarySchema]
  public partial class PacketLocation : IDeserializable {
    public uint Size;
    public uint Offset;
  }
}
