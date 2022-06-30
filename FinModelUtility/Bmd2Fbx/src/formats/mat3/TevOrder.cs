using schema;


namespace bmd.formats.mat3 {
  public enum ColorChannel {
    GX_COLOR0,
    GX_COLOR1,
    GX_ALPHA0,
    GX_ALPHA1,
    GX_COLOR0A0,
    GX_COLOR1A1,
    GX_COLORZERO,
    GX_BUMP,
    GX_BUMPN,
    GX_COLORNULL,
  }

  [Schema]
  public partial class TevOrder : IDeserializable {
    public byte TexCoordId;
    public sbyte TexMap;
    [Format(SchemaNumberType.BYTE)]
    public ColorChannel ColorChannelId;
    private readonly byte padding_ = 0xff;
  }

}
