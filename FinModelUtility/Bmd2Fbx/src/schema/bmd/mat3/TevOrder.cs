using gx;

using schema;


namespace bmd.schema.bmd.mat3 {
  [Schema]
  public partial class TevOrder : IDeserializable {
    public byte TexCoordId;
    public sbyte TexMap;
    public ColorChannel ColorChannelId;
    private readonly byte padding_ = 0xff;
  }

}
