using schema.binary;
using schema.binary.attributes.ignore;


namespace cmb.schema.cmb {
  [BinarySchema]
  public partial class Mesh : IBiSerializable {
    public ushort shapeIndex;
    public byte materialIndex;
    public byte id;

    [Ignore]
    private int unknownLength_ => CmbHeader.Version switch {
        CmbVersion.OCARINA_OF_TIME_3D => 0,
        CmbVersion.MAJORAS_MASK_3D    => 0x8,
        CmbVersion.EVER_OASIS         => 0xC,
        CmbVersion.LUIGIS_MANSION_3D  => 0x54,
    };

    [ArrayLengthSource(nameof(unknownLength_))]
    private byte[] unknown_;
  }
}