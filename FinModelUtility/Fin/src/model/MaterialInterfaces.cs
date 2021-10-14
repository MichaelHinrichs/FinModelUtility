using System.Collections.Generic;
using System.Drawing;

using fin.language.equations.fixedFunction;

namespace fin.model {
  public interface IMaterialManager {
    IReadOnlyList<IMaterial> All { get; }
    ITextureMaterial AddTextureMaterial(ITexture texture);
    ILayerMaterial AddLayerMaterial();
    IFixedFunctionMaterial AddFixedFunctionMaterial();

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
    MULTIPLY,
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

    byte TexCoordIndex { get; set; }
    BlendMode BlendMode { get; set; }
  }


  public enum FixedFunctionSource {
    TEXTURE,

    COLOR_0,
    COLOR_1,
    
    ALPHA_0,
    ALPHA_1,

    VERTEX_COLOR,
    VERTEX_ALPHA,

    OUTPUT_COLOR,
    OUTPUT_ALPHA,
  }

  public interface IFixedFunctionMaterial : IMaterial {
    IFixedFunctionEquations<FixedFunctionSource> Equations { get; }

    IReadOnlyList<ITexture?> TextureSources { get; }
    IReadOnlyList<IColor?> ColorSources { get; }
    IReadOnlyList<float?> AlphaSources { get; }

    IFixedFunctionMaterial SetTextureSource(int textureIndex, ITexture texture);
    // TODO: This should be rgb specifically
    IFixedFunctionMaterial SetColorSource(int colorIndex, IColor color);
    IFixedFunctionMaterial SetAlphaSource(int alphaIndex, float alpha);
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
    float Af { get; }

    byte Rb { get; }
    byte Gb { get; }
    byte Bb { get; }
    byte Ab { get; }
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

    int UvIndex { get; set; }
    UvType UvType { get; }

    Bitmap ImageData { get; }
    bool IsTransparent { get; }

    WrapMode WrapModeU { get; set; }
    WrapMode WrapModeV { get; set; }

    // TODO: UV Scaling
    // TODO: Support fixed # of repeats
    // TODO: Support animated textures
    // TODO: Support animated texture index param
  }
}