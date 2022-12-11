using fin.image;
using fin.model;


namespace gx {
  public enum GX_WRAP_TAG : byte {
    GX_CLAMP,
    GX_REPEAT,
    GX_MIRROR,
    GX_MAXTEXWRAPMODE,
  }

  public enum GX_TEXTURE_FILTER : byte {
    GX_NEAR,
    GX_LINEAR,
    GX_NEAR_MIP_NEAR,
    GX_LIN_MIP_NEAR,
    GX_NEAR_MIP_LIN,
    GX_LIN_MIP_LIN,
    GX_NEAR2,
    GX_NEAR3,
  }

  public interface IGxTexture {
    string Name { get; }
    IImage Image { get; }
    GX_WRAP_TAG WrapModeS { get; }
    GX_WRAP_TAG WrapModeT { get; }
    ColorType ColorType { get; }
  }
}
