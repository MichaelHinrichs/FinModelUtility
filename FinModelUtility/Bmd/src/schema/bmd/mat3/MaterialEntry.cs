using gx;
using schema;


namespace bmd.schema.bmd.mat3 {
  /// <summary>
  ///   https://github.com/LordNed/WindEditor/wiki/BMD-and-BDL-Model-Format#material-entry
  /// </summary>
  [BinarySchema]
  public partial class MaterialEntry : IBiSerializable {
    public byte Flag { get; set; }
    public byte CullModeIndex { get; set; }
    public byte ColorChannelControlsCountIndex { get; set; }
    public byte TexGensCountIndex { get; set; }
    public byte TevStagesCountIndex { get; set; }
    public byte ZCompLocIndex { get; set; }
    public byte ZModeIndex { get; set; }
    public byte DitherIndex { get; set; }

    public short[] MaterialColorIndexes { get; } = new short[2];
    public ushort[] ColorChannelControlIndexes { get; } = new ushort[4];
    public ushort[] AmbientColorIndexes { get; } = new ushort[2];
    public ushort[] LightColorIndexes { get; } = new ushort[8];

    public ushort[] TexGenInfo { get; } = new ushort[8];

    public ushort[] TexGenInfo2 { get; } = new ushort[8];
    public ushort[] TexMatrices { get; } = new ushort[10];
    public ushort[] DttMatrices { get; } = new ushort[20];
    public short[] TextureIndexes { get; } = new short[8];
    public ushort[] TevKonstColorIndexes { get; } = new ushort[4];
    public GxKonstColorSel[] KonstColorSel { get; } = new GxKonstColorSel[16];
    public GxKonstAlphaSel[] KonstAlphaSel { get; } = new GxKonstAlphaSel[16];
    public short[] TevOrderInfoIndexes { get; } = new short[16];
    public ushort[] TevColorIndexes { get; } = new ushort[4];
    public short[] TevStageInfoIndexes { get; } = new short[16];
    public ushort[] TevSwapModeInfo { get; } = new ushort[16];
    public ushort[] TevSwapModeTable { get; } = new ushort[4];
    public ushort[] Unknown2 { get; } = new ushort[12];
    public short FogInfoIndex;
    public short AlphaCompareIndex;
    public short BlendModeIndex;
    public short UnknownIndex;
  }
}