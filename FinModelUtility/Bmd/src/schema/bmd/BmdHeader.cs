using schema;
using schema.attributes.size;


namespace bmd.schema.bmd {
  [BinarySchema]
  public partial class BmdHeader : IBiSerializable {
    private readonly string magic_ = "J3D2bmd3";

    [SizeOfStreamInBytes]
    public uint FileSize;

    public uint NrSections;
    public readonly byte[] Padding = new byte[16];
  }
}
