using System.IO;

using fin.io;

namespace zar.format.cmb {
  public class Combiner : IDeserializable {
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
    public TexCombinerColorOp operandAlpha0;
    public TexCombinerColorOp operandAlpha1;
    public TexCombinerColorOp operandAlpha2;
    public int constColorIndex;

    public void Read(EndianBinaryReader r) {
      this.combinerModeColor = (TexCombineMode) (r.ReadUInt16());
      this.combinerModeAlpha = (TexCombineMode) (r.ReadUInt16());
      this.scaleColor = (TexCombineScale)(r.ReadUInt16());
      this.scaleAlpha = (TexCombineScale)(r.ReadUInt16());
      this.bufferColor = (TexCombinerSource)(r.ReadUInt16());
      this.bufferAlpha = (TexCombinerSource)(r.ReadUInt16());
      this.sourceColor0 = (TexCombinerSource)(r.ReadUInt16());
      this.sourceColor1 = (TexCombinerSource)(r.ReadUInt16());
      this.sourceColor2 = (TexCombinerSource)(r.ReadUInt16());
      this.operandColor0 = (TexCombinerColorOp)(r.ReadUInt16());
      this.operandColor1 = (TexCombinerColorOp)(r.ReadUInt16());
      this.operandColor2 = (TexCombinerColorOp)(r.ReadUInt16());
      this.sourceAlpha0 = (TexCombinerSource)(r.ReadUInt16());
      this.sourceAlpha1 = (TexCombinerSource)(r.ReadUInt16());
      this.sourceAlpha2 = (TexCombinerSource)(r.ReadUInt16());
      this.operandAlpha0 = (TexCombinerColorOp)(r.ReadUInt16());
      this.operandAlpha1 = (TexCombinerColorOp)(r.ReadUInt16());
      this.operandAlpha2 = (TexCombinerColorOp)(r.ReadUInt16());
      this.constColorIndex = r.ReadInt32();
    }
  }
}