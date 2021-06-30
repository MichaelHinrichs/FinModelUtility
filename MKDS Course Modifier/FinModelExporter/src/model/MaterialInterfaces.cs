using System.Collections.Generic;
using System.Drawing;

namespace fin.model {
  public interface IMaterialManager {
    IReadOnlyList<IMaterial> All { get; }
    ITextureMaterial AddTextureMaterial(ITexture texture);
    ILayerMaterial AddLayerMaterial();

    ITexture CreateTexture(Bitmap imageData);
  }

  public enum MaterialType {
    TEXTURED,
    PBR,
    LAYER,
  }


  public interface IMaterial {
    string Name { get; set; }

    IReadOnlyList<ITexture> Textures { get; }

    IShader Shader { get; }
  }


  public interface IShader {
    IReadOnlyList<IShaderParam> Params { get; }
  }

  public interface IShaderParam {
    string Name { get; }
  }

  public interface IShaderParam<out T> : IShaderParam {
    T Default { get; }
  }


  public interface ITextureMaterial : IMaterial {
    ITexture Texture { get; }

    bool Unlit { get; set; }
  }


  // TODO: Support empty white materials
  // TODO: Support basic diffuse materials
  // TODO: Support lit/unlit
  // TODO: Support merged diffuse/normal/etc. materials

  public interface ILayerMaterial : IMaterial {
    IReadOnlyList<ILayer> Layers { get; }

    ILayer AddColorLayer(byte r, byte g, byte b);

    // TODO: Force a default here?
    ILayer AddColorShaderParamLayer(string name);

    // TODO: Where to source data from?
    ILayer AddTextureLayer(ITexture texture);

    // TODO: Generate shader based on layers.
  }

  public enum BlendMode {
    NONE,
    ADD,
    SUBTRACT,
    REVERSE_SUBTRACT,
  }

  public enum BlendFactor {
    ZERO,
    ONE,
    SRC_COLOR,
    ONE_MINUS_SRC_COLOR,
    SRC_ALPHA,
    ONE_MINUS_SRC_ALPHA,
    DST_ALPHA,
    ONE_MINUS_DST_ALPHA,
  }

  public enum LogicOp {
    CLEAR,
    AND,
    AND_REVERSE,
    COPY,
    AND_INVERTED,
    NOOP,
    XOR,
    OR,
    NOR,
    EQUIV,
    INVERT,
    OR_REVERSE,
    COPY_INVERTED,
    OR_INVERTED,
    NAND,
    SET,
  }

  public interface ILayer {
    IColorSource ColorSource { get; }

    // TODO: Add depth settings

    BlendMode BlendMode { get; set; }
    BlendFactor SrcFactor { get; set; }
    BlendFactor DstFactor { get; set; }
    LogicOp LogicOp { get; set; }
  }

  public enum ColorSourceType {
    COLOR,
    SHADER_PARAM,
    TEXTURE,
  }

  public interface IColorSource {
    ColorSourceType Type { get; }
  }

  public interface IColor : IColorSource {
    // TODO: Specify as RGB

    float Rf { get; }
    float Gf { get; }
    float Bf { get; }

    byte Rb { get; }
    byte Gb { get; }
    byte Bb { get; }
  }

  public interface IColorShaderParam : IColorSource, IShaderParam<IColor> {}

  public enum UvType {
    NORMAL,
    SPHERICAL,
  }

  public enum WrapMode {
    CLAMP,
    REPEAT,
    MIRROR_REPEAT,
  }

  public interface ITexture : IColorSource {
    string Name { get; set; }

    int UvIndex { get; }
    UvType UvType { get; }

    Bitmap ImageData { get; }

    WrapMode WrapModeU { get; set; }
    WrapMode WrapModeV { get; set; }

    // TODO: UV Scaling
    // TODO: Support fixed # of repeats
    // TODO: Support animated textures
    // TODO: Support animated texture index param
  }
}