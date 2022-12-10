using gx;
using schema;


namespace bmd.schema.bmd.mat3 {
  [BinarySchema]
  public partial class BlendFunction : IBlendFunction, IBiSerializable {
    public GxBlendMode BlendMode { get; set; }
    public GxBlendFactor SrcFactor { get; set; }
    public GxBlendFactor DstFactor { get; set; }
    public GxLogicOp LogicOp { get; set; }
  }
}