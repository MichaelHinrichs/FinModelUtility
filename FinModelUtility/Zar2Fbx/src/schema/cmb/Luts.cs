using schema;

namespace zar.schema.cmb {
  /// <summary>
  ///   "LUT" stands for "lookup table". (But where is this actually used...?)
  /// </summary>
  [Schema]
  public partial class Luts : IBiSerializable {
    public readonly string magic = "luts";
    public uint dataLength;
    public uint LutSetCount;
    public uint unk;

    
    [ArrayLengthSource(nameof(Luts.LutSetCount))]
    public uint[] Offset;

    [ArrayLengthSource(nameof(Luts.LutSetCount))]
    public LutSet[] luts;
  }

  [Schema]
  public partial class LutSet : IBiSerializable {
    public ushort BitFlags; //Not sure
    public ushort KeyCount; //Keyframes
    public short Start;
    public short End;
    [ArrayLengthSource(nameof(LutSet.KeyCount))]
    public LutKeyframe[] Frame;
    public float unk1;
    public float unk2;
  }

  [Schema]
  public partial class LutKeyframe : IBiSerializable {
    public float InSlope;
    public float OutSlope;
    public int Frame;
    public float Value;
  }
}