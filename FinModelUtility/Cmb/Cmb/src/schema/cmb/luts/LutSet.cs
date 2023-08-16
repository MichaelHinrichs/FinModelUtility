using schema.binary;
using schema.binary.attributes;

namespace cmb.schema.cmb.luts {

  [BinarySchema]
  public partial class LutSet : IBinaryConvertible {
    public ushort BitFlags; //Not sure

    [WLengthOfSequence(nameof(Keyframes))]
    private ushort keyframeCount_;

    public short Start;
    public short End;

    [RSequenceLengthSource(nameof(keyframeCount_))]
    public LutKeyframe[] Keyframes;

    public float unk1;
    public float unk2;
  }
}
