namespace zar.format.cmb {
  public enum CmbVersion {
    OCARINA_OF_TIME_3D = 6,
    MAJORAS_MASK_3D = 10,
    EVER_OASIS = 12,
    LUIGIS_MANSION_3D = 15
  }

  public enum GlTextureFormat : uint {
    RGB8 = 0x14016754,
    RGBA8 = 0x14016752,
    RGBA5551 = 0x80346752,
    RGB565 = 0x83636754,
    RGBA4444 = 0x80336752,
    LA8 = 0x14016758,
    Gas = 0x00006050,
    HiLo8 = 0x14016759,
    L8 = 0x14016757,
    A8 = 0x14016756,
    LA4 = 0x67606758,
    L4 = 0x67616757,
    A4 = 0x67616756,
    ETC1 = 0x0000675A,   // or 0x1401675A,
    ETC1a4 = 0x0000675B, // or 0x1401675B,
    Shadow = 0x00006040
  }

  public enum DataType : ushort {
    Byte = 0x1400,
    UByte = 0x1401,
    Short = 0x1402,
    UShort = 0x1403,
    Int = 0x1404,
    UInt = 0x1405,
    Float = 0x1406
  }

  public enum TestFunc {
    Invalid = 0,
    Never = 512,
    Less = 513,
    Equal = 514,
    Lequal = 515,
    Greater = 516,
    Notequal = 517,
    Gequal = 518,
    Always = 519
  }

  public enum CullMode {
    FrontAndBack = 0,
    Front = 1,
    BackFace = 2,
    Never = 3,
  }

  public enum BumpMode {
    NotUsed = 25288,
    AsBump = 25289,
    AsTangent = 25290 // Doesn't exist in OoT3D
  }

  public enum BumpTexture {
    Texture0 = 0x84C0,
    Texture1 = 0x84C0,
    Texture2 = 0x84C0
  }

  public enum BlendEquation {
    FuncAdd = 0x8006,
    FuncSubtract = 0x800A,
    FuncReverseSubtract = 0x800B,
    Min = 0x8007,
    Max = 0x8008
  }

  public enum BlendMode {
    BlendNone = 0,
    Blend = 1,
    BlendSeparate = 2,
    LogicalOp = 3
  }

  public enum BlendFactor {
    Zero = 0,
    One = 1,
    SourceColor = 768,
    OneMinusSourceColor = 769,
    DestinationColor = 774,
    OneMinusDestinationColor = 775,
    SourceAlpha = 770,
    OneMinusSourceAlpha = 771,
    DestinationAlpha = 772,
    OneMinusDestinationAlpha = 773,
    ConstantColor = 32769,
    OneMinusConstantColor = 32770,
    ConstantAlpha = 32771,
    OneMinusConstantAlpha = 32772,
    SourceAlphaSaturate = 776
  }

  public enum TexCombineMode {
    Replace = 0x1E01,
    Modulate = 0x2100,
    Add = 0x0104,
    AddSigned = 0x8574,
    Interpolate = 0x8575,
    Subtract = 0x84E7,
    DotProduct3Rgb = 0x86AE,
    DotProduct3Rgba = 0x86AF,
    MultAdd = 0x6401,
    AddMult = 0x6402
  }

  public enum TexCombineScale {
    One = 1,
    Two = 2,
    Four = 4
  }

  public enum TexCombinerSource {
    PrimaryColor = 0x8577,
    FragmentPrimaryColor = 0x6210,
    FragmentSecondaryColor = 0x6211,
    Texture0 = 0x84C0,
    Texture1 = 0x84C1,
    Texture2 = 0x84C2,
    Texture3 = 0x84C3,
    PreviousBuffer = 0x8579,
    Constant = 0x8576,
    Previous = 0x8578
  }

  public enum TexCombinerColorOp {
    Color = 0x0300,
    OneMinusColor = 0x0301,
    Alpha = 0x0302,
    OneMinusAlpha = 0x0303,
    Red = 0x8580,
    OneMinusRed = 0x8583,
    Green = 0x8581,
    OneMinusGreen = 0x8584,
    Blue = 0x8582,
    OneMinusBlue = 0x8585
  }

  public enum TexCombinerAlphaOp {
    Alpha = 0x0302,
    OneMinusAlpha = 0x0303,
    Red = 0x8580,
    OneMinusRed = 0x8583,
    Green = 0x8581,
    OneMinusGreen = 0x8584,
    Blue = 0x8582,
    OneMinusBlue = 0x8585
  }

  public enum TextureMinFilter {
    Nearest = 0x2600,
    Linear = 0x2601,
    NearestMipmapNearest = 0x2700,
    LinearMipmapNearest = 0x2701,
    NearestMipmapLinear = 0x2702,
    LinearMipmapLinear = 0x2703
  }

  public enum TextureMagFilter {
    Nearest = 0x2600,
    Linear = 0x2601,
  }

  public enum TextureWrapMode {
    ClampToBorder = 0x2900,
    Repeat = 0x2901,
    ClampToEdge = 0x812F,
    Mirror = 0x8370
  }

  public enum TextureMappingType {
    Empty = 0,
    UvCoordinateMap = 1,
    CameraCubeEnvMap = 2,
    CameraSphereEnvMap = 3,
    ProjectionMap = 4
  }

  public enum TextureMatrixMode {
    DccMaya = 0,
    DccSoftImage = 1,
    Dcc3dsMax = 2
  }

  public enum LutInput {
    CosNormalHalf = 25248,
    CosViewHalf = 25249,
    CosNormalView = 25250,
    CosLightNormal = 25251,
    CosLightSpot = 25252,
    CosPhi = 25253
  }

  public enum LayerConfig {
    LayerConfig0 = 25264,
    LayerConfig1 = 25265,
    LayerConfig2 = 25266,
    LayerConfig3 = 25267,
    LayerConfig4 = 25268,
    LayerConfig5 = 25269,
    LayerConfig6 = 25270,
    LayerConfig7 = 25271
  }

  public enum FresnelConfig {
    No = 25280,
    Pri = 25281,
    Sec = 25282,
    PriSec = 25283
  }

  public enum StencilTestOp {
    Keep = 7680,
    Zero = 0,
    Replace = 7681,
    Increment = 7682,
    Decrement = 7683,
    Invert = 5386,
    IncrementWrap = 34055,
    DecrementWrap = 34055
  }

  public enum VertexAttributeMode : ushort {
    Array = 0,
    Constant = 1,
  }

  public enum SkinningMode {
    Single = 0,
    Rigid = 1,
    Smooth = 2,
  }

  public enum PrimitiveMode {
    Triangles = 0,
    TriangleStrip = 1,
    TriangleFan = 2,
  }
}