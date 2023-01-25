using fin.image;
using fin.model;


namespace gx {
  public enum GX_WRAP_TAG : byte {
    GX_CLAMP,
    GX_REPEAT,
    GX_MIRROR,
    GX_MAXTEXWRAPMODE,
  }

  public enum GX_MAG_TEXTURE_FILTER : byte {
    GX_NEAR,
    GX_LINEAR,
  }

  public enum GX_MIN_TEXTURE_FILTER : byte {
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
    GX_MAG_TEXTURE_FILTER MagTextureFilter { get; }
    GX_MIN_TEXTURE_FILTER MinTextureFilter { get; }
    ColorType ColorType { get; }
  }

  public class GxTexture2d : IGxTexture {
    public string Name { get; set; }
    public IImage Image { get; set; }
    public GX_WRAP_TAG WrapModeS { get; set; }
    public GX_WRAP_TAG WrapModeT { get; set; }

    public GX_MIN_TEXTURE_FILTER MinTextureFilter { get; set; } =
      GX_MIN_TEXTURE_FILTER.GX_LIN_MIP_LIN;

    public GX_MAG_TEXTURE_FILTER MagTextureFilter { get; set; } =
      GX_MAG_TEXTURE_FILTER.GX_LINEAR;
    public ColorType ColorType { get; set; }
  }
}
