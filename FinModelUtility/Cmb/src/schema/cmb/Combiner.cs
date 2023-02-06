using schema.binary;

namespace cmb.schema.cmb {
  [BinarySchema]
  public partial class Combiner : IBinaryConvertible {
    public TexCombineMode combinerModeColor;
    public TexCombineMode combinerModeAlpha;
    public TexCombineScale scaleColor;
    public TexCombineScale scaleAlpha;
    public TexCombinerSource bufferColor;
    public TexCombinerSource bufferAlpha;
    public TexCombinerSource sourceColor0;
    public TexCombinerSource sourceColor1;
    public TexCombinerSource sourceColor2;
    public TexCombinerColorOp operandColor0;
    public TexCombinerColorOp operandColor1;
    public TexCombinerColorOp operandColor2;
    public TexCombinerSource sourceAlpha0;
    public TexCombinerSource sourceAlpha1;
    public TexCombinerSource sourceAlpha2;
    public TexCombinerAlphaOp operandAlpha0;
    public TexCombinerAlphaOp operandAlpha1;
    public TexCombinerAlphaOp operandAlpha2;
    public int constColorIndex;
  }
}