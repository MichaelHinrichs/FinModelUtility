using fin.util.strings;

using schema;

namespace cmb.schema.cmb {
  [BinarySchema]
  public partial class Skl : IBiSerializable {
    private readonly string magic_ = "skl" + AsciiUtil.GetChar(0x20);

    public uint chunkSize;
    private uint boneCount_;

    // M-1: Only value found is "2", possibly "IsTranslateAnimationEnabled"
    // flag (I can't find a change in-game)
    public uint unkFlags;

    [ArrayLengthSource(nameof(boneCount_))]
    public Bone[] bones;
  }
}
