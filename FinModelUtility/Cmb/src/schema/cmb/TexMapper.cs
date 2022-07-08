using schema;

namespace cmb.schema.cmb {
  [Schema]
  public partial class TexMapper : IBiSerializable {
    public short textureId;
    private readonly ushort padding_ = 0;
    public TextureMinFilter minFilter;
    public TextureMagFilter magFilter;
    public TextureWrapMode wrapS;
    public TextureWrapMode wrapT;
    public float minLodBias;
    public float lodBias;
    public readonly byte[] borderColor = new byte[4];
  }
}