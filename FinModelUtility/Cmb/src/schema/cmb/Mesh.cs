using schema.binary;
using schema.binary.attributes.ignore;
using schema.binary.attributes.sequence;


namespace cmb.schema.cmb {
  [BinarySchema]
  public partial class Mesh : IBinaryConvertible {
    public ushort shapeIndex;
    public byte materialIndex;
    public byte id;

    [Ignore]
    private int unknownLength_ => CmbHeader.Version switch {
        Version.OCARINA_OF_TIME_3D => 0,
        Version.MAJORAS_MASK_3D    => 0x8,
        Version.EVER_OASIS         => 0xC,
        Version.LUIGIS_MANSION_3D  => 0x54,
    };

    [RSequenceLengthSource(nameof(unknownLength_))]
    private byte[] unknown_;
  }
}