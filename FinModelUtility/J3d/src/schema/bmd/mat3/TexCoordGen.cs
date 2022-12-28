using gx;
using schema;


namespace j3d.schema.bmd.mat3 {
  [BinarySchema]
  public partial class TexCoordGen : ITexCoordGen, IBiSerializable {
    public GxTexGenType TexGenType { get; set; }
    public GxTexGenSrc TexGenSrc { get; set; }
    public GxTexMatrix TexMatrix { get; set; }
    private readonly byte padding_ = 0xff;
  }
}
