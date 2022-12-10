using gx;
using schema;


namespace bmd.schema.bmd.mat3 {
  [BinarySchema]
  public partial class BlendFunction : IBiSerializable {
    public GxBlendMode BlendMode;
    public GxBlendFactor SrcFactor;
    public GxBlendFactor DstFactor;
    public GxLogicOp LogicOp;
  }
}
