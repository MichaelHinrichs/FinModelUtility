using gx;
using schema;


namespace bmd.schema.bmd.mat3 {
  [BinarySchema]
  public partial class TexCoordGen : ITexCoordGen, IBiSerializable {
    public GxTexGenType TexGenType { get; set; }
    public GxTexGenSrc TexGenSrc { get; set; }
    public GxTexMatrix TexMatrix { get; set; }
    private readonly byte padding_ = 0xff;
  }
}
