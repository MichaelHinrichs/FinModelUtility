using fin.image;
using fin.model;

namespace gx {
  public enum GX_MAG_TEXTURE_FILTER : byte {
    GX_NEAR,
    GX_LINEAR,
  }

  public static class GxTextureFilterExtensions {
    public static TextureMagFilter ToFinMagFilter(
        this GX_MAG_TEXTURE_FILTER gxMagFilter)
      => gxMagFilter switch {
          GX_MAG_TEXTURE_FILTER.GX_NEAR   => TextureMagFilter.NEAR,
          GX_MAG_TEXTURE_FILTER.GX_LINEAR => TextureMagFilter.LINEAR,
      };
  };

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
    GxWrapMode WrapModeS { get; }
    GxWrapMode WrapModeT { get; }
    GX_MAG_TEXTURE_FILTER MagTextureFilter { get; }
    GX_MIN_TEXTURE_FILTER MinTextureFilter { get; }
    ColorType ColorType { get; }
  }

  public class GxTexture2d : IGxTexture {
    public string Name { get; set; }
    public IImage Image { get; set; }
    public GxWrapMode WrapModeS { get; set; }
    public GxWrapMode WrapModeT { get; set; }

    public GX_MIN_TEXTURE_FILTER MinTextureFilter { get; set; } =
      GX_MIN_TEXTURE_FILTER.GX_LIN_MIP_LIN;

    public GX_MAG_TEXTURE_FILTER MagTextureFilter { get; set; } =
      GX_MAG_TEXTURE_FILTER.GX_LINEAR;

    public ColorType ColorType { get; set; }
  }
}