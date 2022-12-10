namespace gx {
  public enum GxColorChannel : byte {
    GX_COLOR0,
    GX_COLOR1,
    GX_ALPHA0,
    GX_ALPHA1,
    GX_COLOR0A0,
    GX_COLOR1A1,
    GX_COLORZERO,
    GX_BUMP,
    GX_BUMPN,
    GX_COLORNULL,
  }

  public interface ITevOrder {
    byte TexCoordId { get; }
    sbyte TexMap { get; }
    GxColorChannel ColorChannelId { get; }
  }
}