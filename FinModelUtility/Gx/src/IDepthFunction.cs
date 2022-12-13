namespace gx {
  public interface IDepthFunction  {
    bool Enable { get; }
    GxCompareType Func { get; }
    bool WriteNewValueIntoDepthBuffer { get; }
  }
}