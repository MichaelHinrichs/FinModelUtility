using gx;
using schema;


namespace bmd.schema.bmd.mat3 {
  [BinarySchema]
  public partial class TevSwapMode : ITevSwapMode, IBiSerializable {
    public SwapTableId RasSel { get; set; }
    public SwapTableId TexSel { get; set; }
  }
}
