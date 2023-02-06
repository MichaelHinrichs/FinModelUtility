using gx;
using schema.binary;


namespace j3d.schema.bmd.mat3 {
  [BinarySchema]
  public partial class TevSwapMode : ITevSwapMode, IBinaryConvertible {
    public SwapTableId RasSel { get; set; }
    public SwapTableId TexSel { get; set; }
  }
}
