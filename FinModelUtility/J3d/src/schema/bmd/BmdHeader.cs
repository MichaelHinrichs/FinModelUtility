using schema.binary;
using schema.binary.attributes.size;


namespace j3d.schema.bmd {
  [BinarySchema]
  public partial class BmdHeader : IBiSerializable {
    private readonly string magic_ = "J3D2bmd3";

    [SizeOfStreamInBytes]
    public uint FileSize;

    public uint NrSections;
    public readonly byte[] Padding = new byte[16];
  }
}
