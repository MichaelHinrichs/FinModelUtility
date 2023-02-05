using schema.binary;


namespace modl.schema.modl.bw2.node {
  [BinarySchema]
  public partial class Bw2Material : IBwMaterial, IBiSerializable {
    [StringLengthSource(0x20)]
    public string Texture1 { get; set; } = "";

    [StringLengthSource(0x20)]
    public string Texture2 { get; set; } = "";

    [StringLengthSource(0x20)]
    public string Texture3 { get; set; } = "";

    [StringLengthSource(0x20)]
    public string Texture4 { get; set; } = "";

    public byte[] Data { get; } = new byte[0x24];
  }
}