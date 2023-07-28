using schema.binary;
using schema.binary.attributes;

namespace cmb.schema.cmb {
  /// <summary>
  ///   "LUT" stands for "lookup table". (But where is this actually used...?)
  /// </summary>
  [BinarySchema]
  public partial class Luts : IBinaryConvertible {
    private uint LutSetCount;
    public uint unk;

    [RSequenceLengthSource(nameof(LutSetCount))]
    public uint[] Offset;

    [RSequenceLengthSource(nameof(LutSetCount))]
    public LutSet[] luts;
  }

  [BinarySchema]
  public partial class LutSet : IBinaryConvertible {
    public ushort BitFlags; //Not sure
    private ushort KeyCount; //Keyframes
    public short Start;
    public short End;
    [RSequenceLengthSource(nameof(KeyCount))]
    public LutKeyframe[] Frame;
    public float unk1;
    public float unk2;
  }

  [BinarySchema]
  public partial class LutKeyframe : IBinaryConvertible {
    public float InSlope;
    public float OutSlope;
    public int Frame;
    public float Value;
  }
}