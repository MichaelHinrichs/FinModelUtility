using fin.schema.color;

using gx;

using schema.binary;
using schema.binary.attributes;

namespace dat.schema {
  [Flags]
  public enum RenderMode : int {
    CONSTANT = (1 << 0),
    VERTEX = (1 << 1),
    BOTH = CONSTANT | VERTEX,
    DIFFUSE = (1 << 2),
    SPECULAR = (1 << 3),
    TEX0 = (1 << 4),
    TEX1 = (1 << 5),
    TEX2 = (1 << 6),
    TEX3 = (1 << 7),
    TEX4 = (1 << 8),
    TEX5 = (1 << 9),
    TEX6 = (1 << 10),
    TEX7 = (1 << 11),
    TOON = (1 << 12),

    //ALPHA_COMPAT = (0 << 13),
    ALPHA_MAT = (1 << 13),
    ALPHA_VTX = (2 << 13),
    ALPHA_BOTH = (3 << 13),
    ZOFST = (1 << 24),
    EFFECT = (1 << 25),
    SHADOW = (1 << 26),
    ZMODE_ALWAYS = (1 << 27),
    DF_ALL = (1 << 28),
    NO_ZUPDATE = (1 << 29),
    XLU = (1 << 30),
    USER = (1 << 31),
  }

  /// <summary>
  ///   Material object.
  ///
  ///   Shamelessly copied from:
  ///   https://github.com/jam1garner/Smash-Forge/blob/c0075bca364366bbea2d3803f5aeae45a4168640/Smash%20Forge/Filetypes/Melee/DAT.cs#L1256
  /// </summary>
  [BinarySchema]
  public partial class MObj : IBinaryDeserializable {
    public uint StringOffset { get; set; }
    public RenderMode RenderMode { get; set; }
    public uint FirstTObjOffset { get; set; }
    public uint MaterialOffset { get; private set; }
    public uint Unk3 { get; set; }
    public uint PeDescOffset { get; set; }


    [Ignore]
    public string? Name { get; set; }

    [RAtPositionOrNull(nameof(FirstTObjOffset))]
    public TObj? FirstTObj { get; set; }

    [RAtPositionOrNull(nameof(MaterialOffset))]
    public DatMaterial? Material { get; set; }

    [RAtPositionOrNull(nameof(PeDescOffset))]
    public PeDesc? PeDesc { get; set; }

    // TODO: Add https://github.com/Ploaj/HSDLib/blob/93a906444f34951c6eed4d8c6172bba43d4ada98/HSDRaw/Common/HSD_MOBJ.cs#L156

    [Ignore]
    public IEnumerable<(uint, TObj)> TObjsAndOffsets {
      get {
        var tObjOffset = this.FirstTObjOffset;
        var tObj = this.FirstTObj;

        while (tObj != null) {
          yield return (tObjOffset, tObj);

          tObjOffset = tObj.NextTObjOffset;
          tObj = tObj.NextTObj;
        }
      }
    }
  }

  /// <summary>
  ///   Shamelessly copied from:
  ///   https://github.com/Ploaj/HSDLib/blob/93a906444f34951c6eed4d8c6172bba43d4ada98/HSDRaw/Common/HSD_MOBJ.cs#L101
  /// </summary>
  [BinarySchema]
  public partial class DatMaterial : IBinaryDeserializable {
    public Rgba32 AmbientColor { get; set; }
    public Rgba32 DiffuseColor { get; set; }
    public Rgba32 SpecularColor { get; set; }
    public float Alpha { get; set; }
    public float Shininess { get; set; }
  }


  [Flags]
  public enum PIXEL_PROCESS_ENABLE : byte {
    COLOR_UPDATE = (1 << 0),
    ALPHA_UPDATE = (1 << 1),
    DST_ALPHA = (1 << 2),
    BEFORE_TEX = (1 << 3),
    COMPARE = (1 << 4),
    ZUPDATE = (1 << 5),
    DITHER = (1 << 6)
  }

  [BinarySchema]
  public partial class PeDesc : IBinaryDeserializable {
    public PIXEL_PROCESS_ENABLE Flags { get; set; }

    [NumberFormat(SchemaNumberType.UN8)]
    public float AlphaRef0 { get; set; }

    [NumberFormat(SchemaNumberType.UN8)]
    public float AlphaRef1 { get; set; }

    [NumberFormat(SchemaNumberType.UN8)]
    public float DestinationAlpha { get; set; }

    public GxBlendMode BlendMode { get; set; }
    public GxBlendFactor SrcFactor { get; set; }
    public GxBlendFactor DstFactor { get; set; }
    public GxLogicOp BlendOp { get; set; }
    public GxCompareType DepthFunction { get; set; }
    public GxCompareType AlphaComp0 { get; set; }
    public GxAlphaOp AlphaOp { get; set; }
    public GxCompareType AlphaComp1 { get; set; }
  }
}