using schema.binary;

namespace jsystem.schema.bmd.shp1 {
  [BinarySchema]
  public partial class PacketLocation : IBinaryDeserializable {
    public uint Size;
    public uint Offset;
  }
}
