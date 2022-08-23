using System.Collections.Generic;
using System.Drawing;

using fin.image;
using fin.io;
using fin.language.equations.fixedFunction;
using fin.util.image;


namespace fin.model {
  public interface IMaterialManager {
    IReadOnlyList<IMaterial> All { get; }

    // TODO: Name is actually required, should be required in the creation scripts?
    ITextureMaterial AddTextureMaterial(ITexture texture);
    IStandardMaterial AddStandardMaterial();
    ILayerMaterial AddLayerMaterial();
    IFixedFunctionMaterial AddFixedFunctionMaterial();

    ITexture CreateTexture(IImage imageData);
  }

  public enum MaterialType {
    TEXTURED,
    PBR,
    LAYER,
  }

  public enum CullingMode {
    SHOW_FRONT_ONLY,
    SHOW_BACK_ONLY,
    SHOW_BOTH,
    SHOW_NEITHER,
  }


  public interface IMaterial {
    string? Name { get; set; }

    IEnumerable<ITexture> Textures { get; }

    IShader Shader { get; }
    CullingMode CullingMode { get; set; }
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

  public interface IStandardMaterial : IMaterial {
    ITexture? DiffuseTexture { get; set; }
    ITexture? AmbientOcclusionTexture { get; set; }
    ITexture? NormalTexture { get; set; }
    ITexture? EmissiveTexture { get; set; }
    ITexture? SpecularTexture { get; set; }

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


  public enum AlphaCompareType : byte {
    Never = 0,
    Less = 1,
    Equal = 2,
    LEqual = 3,
    Greater = 4,
    NEqual = 5,
    GEqual = 6,
    Always = 7
  }

  public enum AlphaOp : byte {
    And = 0,
    Or = 1,
    XOR = 2,
    XNOR = 3
  }

  public interface ILayer {
    IColorSource ColorSource { get; }

    byte TexCoordIndex { get; set; }
    BlendMode BlendMode { get; set; }
  }


  public enum FixedFunctionSource {
    TEXTURE_COLOR_0,
    TEXTURE_COLOR_1,
    TEXTURE_COLOR_2,
    TEXTURE_COLOR_3,
    TEXTURE_COLOR_4,
    TEXTURE_COLOR_5,
    TEXTURE_COLOR_6,
    TEXTURE_COLOR_7,

    TEXTURE_ALPHA_0,
    TEXTURE_ALPHA_1,
    TEXTURE_ALPHA_2,
    TEXTURE_ALPHA_3,
    TEXTURE_ALPHA_4,
    TEXTURE_ALPHA_5,
    TEXTURE_ALPHA_6,
    TEXTURE_ALPHA_7,

    CONST_COLOR_0,
    CONST_COLOR_1,
    CONST_COLOR_2,
    CONST_COLOR_3,
    CONST_COLOR_4,
    CONST_COLOR_5,
    CONST_COLOR_6,
    CONST_COLOR_7,
    CONST_COLOR_8,
    CONST_COLOR_9,
    CONST_COLOR_10,
    CONST_COLOR_11,
    CONST_COLOR_12,
    CONST_COLOR_13,
    CONST_COLOR_14,
    CONST_COLOR_15,

    CONST_ALPHA_0,
    CONST_ALPHA_1,
    CONST_ALPHA_2,

    VERTEX_COLOR_0,
    VERTEX_COLOR_1,

    VERTEX_ALPHA_1,
    VERTEX_ALPHA_0,

    OUTPUT_COLOR,
    OUTPUT_ALPHA,

    UNDEFINED,
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

    ITexture? CompiledTexture { get; set; }

    // TODO: Merge this into a single type
    IFixedFunctionMaterial SetBlending(
        BlendMode blendMode,
        BlendFactor srcFactor,
        BlendFactor dstFactor,
        LogicOp logicOp);

    BlendMode BlendMode { get; }
    BlendFactor SrcFactor { get; }
    BlendFactor DstFactor { get; }
    LogicOp LogicOp { get; }

    // TODO: Merge this into a single type
    IFixedFunctionMaterial SetAlphaCompare(
        AlphaOp alphaOp,
        AlphaCompareType alphaCompareType0,
        float reference0,
        AlphaCompareType alphaCompareType1,
        float reference1);

    AlphaOp AlphaOp { get; }
    AlphaCompareType AlphaCompareType0 { get; }
    float AlphaReference0 { get; }
    AlphaCompareType AlphaCompareType1 { get; }
    float AlphaReference1 { get; }
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

  public interface IColorShaderParam : IColorSource, IShaderParam<IColor> { }

  public enum UvType {
    NORMAL,
    SPHERICAL,
    LINEAR,
  }

  public enum WrapMode {
    CLAMP,
    REPEAT,
    MIRROR_REPEAT,
  }

  public enum ColorType {
    COLOR,
    INTENSITY,
  }

  public interface ITexture : IColorSource {
    string Name { get; set; }

    int UvIndex { get; set; }
    UvType UvType { get; set; }
    ColorType ColorType { get; set; }

    // TODO: Support mipmaps
    IImage Image { get; }
    Bitmap ImageData { get; }
    IFile SaveInDirectory(IDirectory directory);
    BitmapTransparencyType TransparencyType { get; }

    WrapMode WrapModeU { get; set; }
    WrapMode WrapModeV { get; set; }

    // TODO: UV Scaling
    // TODO: Support fixed # of repeats
    // TODO: Support animated textures
    // TODO: Support animated texture index param
  }
}