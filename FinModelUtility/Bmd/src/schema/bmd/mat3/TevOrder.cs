using gx;

using schema;


namespace bmd.schema.bmd.mat3 {
  [BinarySchema]
  public partial class TevOrder : ITevOrder, IDeserializable {
    public byte TexCoordId { get; set; }
    public sbyte TexMap { get; set; }
    public GxColorChannel ColorChannelId { get; set; }
    private readonly byte padding_ = 0xff;
  }
}
