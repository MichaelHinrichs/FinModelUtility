using System.Collections.Generic;
using System.Drawing;

using fin.color;
using fin.image;
using fin.io;
using fin.language.equations.fixedFunction;
using fin.util.image;

namespace fin.model {
  public interface IMaterialManager {
    IReadOnlyList<IMaterial> All { get; }
    IFixedFunctionRegisters? Registers { get; }

    // TODO: Name is actually required, should be required in the creation scripts?
    INullMaterial AddNullMaterial();
    ITextureMaterial AddTextureMaterial(ITexture texture);
    IColorMaterial AddColorMaterial(Color color);
    IStandardMaterial AddStandardMaterial();
    IFixedFunctionMaterial AddFixedFunctionMaterial();

    ITexture CreateTexture(IImage imageData);
    IReadOnlyList<ITexture> Textures { get; }
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


  public enum DepthMode {
    USE_DEPTH_BUFFER,
    IGNORE_DEPTH_BUFFER,
    SKIP_WRITE_TO_DEPTH_BUFFER
  }

  public enum DepthCompareType {
    LEqual,
    Less,
    Equal,
    Greater,
    NEqual,
    GEqual,
    Always,
    Never,
  }


  public interface IReadOnlyMaterial {
    string? Name { get; }

    IEnumerable<ITexture> Textures { get; }

    CullingMode CullingMode { get; }

    DepthMode DepthMode { get; }
    DepthCompareType DepthCompareType { get; }

    bool IgnoreLights { get; }
  }

  public interface IMaterial : IReadOnlyMaterial {
    new string? Name { get; set; }

    new CullingMode CullingMode { get; set; }

    new DepthMode DepthMode { get; set; }
    new DepthCompareType DepthCompareType { get; set; }

    new bool IgnoreLights { get; set; }
  }


  public interface INullMaterial : IMaterial { }

  public interface ITextureMaterial : IMaterial {
    ITexture Texture { get; }
  }

  public interface IColorMaterial : IMaterial {
    Color Color { get; set; }
  }

  public interface IStandardMaterial : IMaterial {
    ITexture? DiffuseTexture { get; set; }
    ITexture? AmbientOcclusionTexture { get; set; }
    ITexture? NormalTexture { get; set; }
    ITexture? EmissiveTexture { get; set; }
    ITexture? SpecularTexture { get; set; }
  }


  // TODO: Support empty white materials
  // TODO: Support basic diffuse materials
  // TODO: Support lit/unlit
  // TODO: Support merged diffuse/normal/etc. materials

  public enum BlendEquation {
    NONE,
    ADD,
    SUBTRACT,
    REVERSE_SUBTRACT,
    MIN,
    MAX
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
    CONST_COLOR,
    ONE_MINUS_CONST_COLOR,
    CONST_ALPHA,
    ONE_MINUS_CONST_ALPHA,
  }

  public enum LogicOp {
    UNDEFINED,
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

    VERTEX_ALPHA_0,
    VERTEX_ALPHA_1,

    OUTPUT_COLOR,
    OUTPUT_ALPHA,

    LIGHT_0_COLOR,
    LIGHT_1_COLOR,
    LIGHT_2_COLOR,
    LIGHT_3_COLOR,
    LIGHT_4_COLOR,
    LIGHT_5_COLOR,
    LIGHT_6_COLOR,
    LIGHT_7_COLOR,

    LIGHT_0_ALPHA,
    LIGHT_1_ALPHA,
    LIGHT_2_ALPHA,
    LIGHT_3_ALPHA,
    LIGHT_4_ALPHA,
    LIGHT_5_ALPHA,
    LIGHT_6_ALPHA,
    LIGHT_7_ALPHA,

    UNDEFINED,
  }


  public interface IReadOnlyFixedFunctionMaterial : IReadOnlyMaterial {
    IFixedFunctionEquations<FixedFunctionSource> Equations { get; }
    IFixedFunctionRegisters Registers { get; }

    IReadOnlyList<ITexture?> TextureSources { get; }

    ITexture? CompiledTexture { get; }

    BlendEquation ColorBlendEquation { get; }
    BlendFactor ColorSrcFactor { get; }
    BlendFactor ColorDstFactor { get; }
    BlendEquation AlphaBlendEquation { get; }
    BlendFactor AlphaSrcFactor { get; }
    BlendFactor AlphaDstFactor { get; }
    LogicOp LogicOp { get; }

    AlphaOp AlphaOp { get; }
    AlphaCompareType AlphaCompareType0 { get; }
    float AlphaReference0 { get; }
    AlphaCompareType AlphaCompareType1 { get; }
    float AlphaReference1 { get; }
  }

  public interface IFixedFunctionMaterial : IReadOnlyFixedFunctionMaterial,
                                            IMaterial {
    IFixedFunctionMaterial SetTextureSource(int textureIndex, ITexture texture);

    new ITexture? CompiledTexture { get; set; }

    // TODO: Merge this into a single type
    IFixedFunctionMaterial SetBlending(
        BlendEquation blendEquation,
        BlendFactor srcFactor,
        BlendFactor dstFactor,
        LogicOp logicOp);

    IFixedFunctionMaterial SetBlendingSeparate(
        BlendEquation colorBlendEquation,
        BlendFactor colorSrcFactor,
        BlendFactor colorDstFactor,
        BlendEquation alphaBlendEquation,
        BlendFactor alphaSrcFactor,
        BlendFactor alphaDstFactor,
        LogicOp logicOp);

    // TODO: Merge this into a single type
    IFixedFunctionMaterial SetAlphaCompare(
        AlphaOp alphaOp,
        AlphaCompareType alphaCompareType0,
        float reference0,
        AlphaCompareType alphaCompareType1,
        float reference1);
  }


  public enum UvType {
    STANDARD,
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

  public enum TextureMagFilter {
    NEAR,
    LINEAR,
  }

  public enum TextureMinFilter {
    NEAR,
    LINEAR,
    NEAR_MIPMAP_NEAR,
    NEAR_MIPMAP_LINEAR,
    LINEAR_MIPMAP_NEAR,
    LINEAR_MIPMAP_LINEAR,
  }

  public interface ITexture {
    string Name { get; set; }

    LocalImageFormat BestImageFormat { get; }
    string ValidFileName { get; }

    int UvIndex { get; set; }
    UvType UvType { get; set; }
    ColorType ColorType { get; set; }

    IImage Image { get; }
    Bitmap ImageData { get; }
    ISystemFile SaveInDirectory(ISystemDirectory directory);
    ImageTransparencyType TransparencyType { get; }

    WrapMode WrapModeU { get; set; }
    WrapMode WrapModeV { get; set; }

    IColor? BorderColor { get; set; }

    TextureMagFilter MagFilter { get; set; }
    TextureMinFilter MinFilter { get; set; }

    IReadOnlyVector2 Offset { get; }
    ITexture SetOffset(float x, float y);

    IReadOnlyVector2 Scale { get; }
    ITexture SetScale(float x, float y);

    float RotationDegrees { get; }
    ITexture SetRotationDegrees(float rotationDegrees);

    // TODO: UV Scaling
    // TODO: Support fixed # of repeats
    // TODO: Support animated textures
    // TODO: Support animated texture index param
  }
}