using schema.binary;

namespace cmb.schema.cmb.luts {

  [BinarySchema]
  public partial class LutKeyframe : IBinaryConvertible {
    public float InSlope;
    public float OutSlope;
    public int Frame;
    public float Value;
  }
}
